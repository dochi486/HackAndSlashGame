using System;
using System.Collections;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    //Idle이 기본상태
    //Player가 다가오면 추격 , 추격할 때 플레이어와 닿는 거리면 공격 -> 아이들코루틴으로 만들 예정
    Animator animator;
    Coroutine fsmHandle;
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = Player.instance;

        CurrentFSM = IdleFSM;
        while (true) //FSM을 무한히 반복해서 실행하는 부분
        {
            fsmHandle = StartCoroutine(CurrentFSM());
            while (fsmHandle != null)
                yield return null;
        }
    }

    Func<IEnumerator> m_currentFSM;
    Func<IEnumerator> CurrentFSM
    {
        get { return m_currentFSM; }
        set
        {
            m_currentFSM = value;
            fsmHandle = null;
        }
    }
    //반환형이 IEnumerator로 있기 때문에 action을 사용할 수 없음.. Function을 써야한다.
    Player player;
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
        animator.Play("Idle");
        while (Vector3.Distance(transform.position, player.transform.position) > detectRange)
        {
            yield return null;
        }
        CurrentFSM = ChaseFSM;
    }

    public float speed = 34;
    IEnumerator ChaseFSM()
    {
        animator.Play("Run");
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
                CurrentFSM = AttackFSM;
                yield break; //실행하고 있는 코루틴을 빠져나가는 문장
                //나가면 다음 코루틴 지정한 곳(공격)으로 나간다?
            }
            yield return null;
        }
    }
    public float attackTime = 1;//공격하고 있는 시간
    public float attackApplyTime = 0.3f; //공격이 실제로 적용되는? 감지되는 시간!
    public int power = 10;
    IEnumerator AttackFSM()
    {
        animator.Play("Attack");
        yield return new WaitForSeconds(attackApplyTime);
        if (Vector3.Distance(player.transform.position, transform.position) < attackRange)
        {
            player.TakeHit(power);
        }
        yield return new WaitForSeconds(attackTime - attackApplyTime);
        CurrentFSM = ChaseFSM;
    }
    public float hp = 100;
    internal void TakeHit(float damage)
    {
        hp -= damage;
        StopCoroutine(fsmHandle);

        CurrentFSM = TakeHitFSM;
    }
    public float takeHitTime = 0.3f;
    private IEnumerator TakeHitFSM()
    {
        animator.Play("Hit");
        yield return new WaitForSeconds(takeHitTime);

        if (hp > 0)
            CurrentFSM = IdleFSM;
        else
            CurrentFSM = DeathFSM; //죽을 때 피격모션 1회 플레이 후 죽도록
    }
    public float deathTime = 0.5f;
    private IEnumerator DeathFSM()
    {
        animator.Play("Death");
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}

