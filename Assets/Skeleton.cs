using System.Collections;
using UnityEngine;


public class Skeleton : Monster
{
    //방패 막기 추가 + 공격 타이밍에 랜덤하게 방패 모션
    //방패 모션 중에는 대미지 X 
    //막았다는 이펙트 생성
    public Transform blockEffectPosition;
    override protected void SelectAttackType()
    {
        if (Random.Range(0, 1f) > 0.5f)
            //Random.Range(0,1)은 결과가 항상 0이 나오지만 1f로 하면 0~1까지 소수점 범위 사용 가능
            CurrentFSM = AttackFSM;
        else
            CurrentFSM = ShieldFSM;
    }
    //override는 ToString으로 많이 사용했었다! 

    bool isDefending = false;
    public float activeDefendingTime = 2;
    protected IEnumerator ShieldFSM()
    {
        PlayAnimation("Shield");
        isDefending = true;

        yield return new WaitForSeconds(activeDefendingTime);

        isDefending = false;
        CurrentFSM = ChaseFSM;
    }

    public GameObject blockEffect;
    enum Direction
    {
        Right,
        Left
    }
    public override void TakeHit(float damage)
    {
        bool succeedBlock = SucceedBlock();
        if (succeedBlock)
        {
            Instantiate(blockEffect, blockEffectPosition.position, Quaternion.identity);
            //자기 위치에 로테이션 0인 값으로 생성하는 일반적인 코드
        }
        else
        {
            base.TakeHit(damage);
        }
    }

    private bool SucceedBlock()
    {
        if (isDefending == false)
            return false;

        //bool isInFront = false; //플레이어가 앞에 있는지 체크하는 변수

        Direction myDirection = transform.rotation.eulerAngles.y == 180 ? Direction.Left : Direction.Right;
        //180도이면 스켈레톤이 왼쪽을 보고 있다. 0도일때는 오른쪽!
        if (myDirection == Direction.Right)
        {
            //스켈레톤이 오른쪽을 보고 있을 때 플레이어가 왼쪽에서 공격한다면 block실패
            if (player.transform.position.x - transform.position.x < 0)
                //인스펙터에서 포지션의 x값을 확인하면서 뺀 값이 음수이면 어떻게 되는지 확인할 수 있다.
                return false;
        }
        else
        {
            if (transform.position.x - player.transform.position.x < 0)
                return false;
        }

        return true;
    }
}
