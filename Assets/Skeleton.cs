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
    public override void TakeHit(float damage)
    {
        if (isDefending)
        {
            Instantiate(blockEffect, blockEffectPosition.position, Quaternion.identity); 
            //자기 위치에 로테이션 0인 값으로 생성하는 일반적인 코드
        }
        else
        {
            base.TakeHit(damage);
        }
    }
}
