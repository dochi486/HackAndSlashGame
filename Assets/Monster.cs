using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public static List<Monster> Items = new List<Monster>();
    //퍼블릭이기 때문에 고블린, 스켈레톤 스크립트에서도 참조가능
    private void Awake()
    {
        Items.Add(this);
        //Monster를 사용하고 있는 오브젝트를 모두 List에 담는다.
    }
    //Idle이 기본상태
    //Player가 다가오면 추격 , 추격할 때 플레이어와 닿는 거리면 공격 -> 아이들코루틴으로 만들 예정
    Animator animator;
    Coroutine fsmHandle;
    SpriteRenderer spriteRenderer;
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = Player.instance;
        CurrentFSM = IdleFSM;

        while (true) //FSM을 무한히 반복해서 실행하는 부분
        {
            //try 
            //에러가 예상되는 구간에 try~catch로 에러를 잡을 수 있으나 이 경우에는 이 부분에서 루프 도는 게 아니기 때문에 try~catch로 확인 불가
            {
                var previousFSM = CurrentFSM;

                fsmHandle = StartCoroutine(CurrentFSM());
                //}
                //catch (Exception e)
                //{
                //    Debug.LogError(e);
                //}

                if (fsmHandle == null && previousFSM == CurrentFSM) //바로 전의 FSM과 현재의 FSM이 같으면 한 프레임 쉬고 에러 로그를 찍도록
                    //기존의 무한루프 도는 에러를 보완한 부분
                    yield return null;

                while (fsmHandle != null)
                    yield return null;
            }
        }
    }

    Func<IEnumerator> m_currentFSM;
    protected Func<IEnumerator> CurrentFSM
    {
        get { return m_currentFSM; }
        set
        {
            m_currentFSM = value;
            fsmHandle = null;
        }
    }
    //반환형이 IEnumerator로 있기 때문에 action을 사용할 수 없음.. Function을 써야한다.
    protected Player player;
    public float detectRange = 20;
    public float attackRange = 10;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    IEnumerator IdleFSM()
    {
        PlayAnimation("Idle");
        while (Vector3.Distance(transform.position, player.transform.position) > detectRange)
        //무한루프 도는 구간! 
        //한 프레임이라도 쉬어야 에러 로그가 뜬다..
        {
            yield return null;
        }
        CurrentFSM = ChaseFSM;
    }

    public float speed = 34;
    protected IEnumerator ChaseFSM()
    {
        PlayAnimation("Run");
        //yield return null;
        while (true)
        {
            Vector3 toPlayerDriection = player.transform.position - transform.position;
            toPlayerDriection.Normalize();
            transform.Translate(toPlayerDriection * speed * Time.deltaTime, Space.World);

            bool isRightSide = toPlayerDriection.x > 0;
            if (isRightSide)
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            if ((Vector3.Distance(transform.position, player.transform.position) < attackRange))
            {
                SelectAttackType();

                yield break;
                //실행하고 있는 코루틴을 빠져나가는 문장
                //나가면 다음 코루틴 지정한 곳(공격)으로 나간다?
            }
            yield return null;
        }
    }

    virtual protected void SelectAttackType()
    {
        CurrentFSM = AttackFSM;
        //공격 타입을 여러가지로 지정해서 스켈레톤이 방패모션을 랜덤하게 할 예정
        //virtual함수로 구현 -> virtual함수는 private이 안되고 protected로 자식이 사용가능하게 접근한정자
    }

    public float attackTime = 1;//공격하고 있는 시간
    public float attackApplyTime = 0.3f; //공격이 실제로 적용되는? 감지되는 시간!
    public int power = 10;
    protected IEnumerator AttackFSM()
    {
        PlayAnimation("Attack");
        yield return new WaitForSeconds(attackApplyTime);
        if (Vector3.Distance(player.transform.position, transform.position) < attackRange)
        {
            player.TakeHit(power);
        }
        yield return new WaitForSeconds(attackTime - attackApplyTime);
        CurrentFSM = ChaseFSM;
    }
    public float hp = 100;
    virtual public void TakeHit(float damage)
    {
        if (hp < 0)
            return;

        //if (CurrentFSM == DeathFSM)
        //return; //죽기전에 항상 TakeHit를 하는 중이기 때문에 안된다?
        //일반적으로는 FSM을 사용해서 상태를 판정하지만 지금 이 구조에서는 FSM으로 할 수 없다.

        hp -= damage;
        StopCoroutine(fsmHandle);
        CurrentFSM = TakeHitFSM;
    }

    public float takeHitTime = 0.3f;

    private IEnumerator TakeHitFSM()
    {
        PlayAnimation("Hit");
        yield return new WaitForSeconds(takeHitTime);

        if (hp > 0)
            CurrentFSM = IdleFSM;
        else
            CurrentFSM = DeathFSM; //죽을 때 피격모션 1회 플레이 후 죽도록
    }
    public float deathTime = 0.5f;
    private IEnumerator DeathFSM()
    {
        PlayAnimation("Death");
        Items.Remove(this);
        Debug.Log($"남은 몬스터 수 : {Items.Count}");
        if (Items.Count == 0)
        {
            //StageResultUI.instance.Show();
            //몬스터가 0마리이면 스테이지 결과를 표시하는 함수
        }
        //몬스터가 죽을 때 리스트에서 빠지도록
        yield return new WaitForSeconds(deathTime);

        spriteRenderer.DOFade(0, 1).OnComplete(() =>
        {

            Destroy(gameObject);
        });
        //반환도 없고 파라미터도 없는 함수라는 의미
    }

    protected void PlayAnimation(string clipName)
    {
        //Debug.Log(clipName);
        animator.Play(clipName, 0, 0);
        //애니메이터의 노드의 이름(클립의 이름과는 다름), 애니메이터 레이어의 인덱스(인덱스는 0부터 시작),  시작위치? 노멀라이즈드타임
    }
}


