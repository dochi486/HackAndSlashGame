using System;
using System.Collections;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    //Idle이 기본상태
    //Player가 다가오면 추격 , 추격할 때 플레이어와 닿는 거리면 공격 -> 아이들코루틴으로 만들 예정
    Animator animator;

    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = Player.instance;

        currentFSM = IdleFSM;
        while(true) //FSM을 무한히 반복해서 실행하는 부분
        {
            yield return StartCoroutine(currentFSM());
        }
    }

    Func<IEnumerator> currentFSM; //반환형이 IEnumerator로 있기 때문에 action을 사용할 수 없음.. Functin을 써야한다.
    Player player;
    public float detectRange = 40;
    IEnumerator IdleFSM()
    {
        animator.Play("Goblin_Idle");
        while (Vector3.Distance(transform.position, player.transform.position) > detectRange)
        {
            yield return null;
        }
        currentFSM = ChaseFSM;
    }

    public float speed = 34;
    IEnumerator ChaseFSM()
    {
        animator.Play("Goblin_Run");
        while(true)
        {
            Vector3 toPlayerDriection = player.transform.position - transform.position;
            toPlayerDriection.Normalize();
            transform.Translate(toPlayerDriection * speed * Time.deltaTime, Space.World);


            bool isRightSide = toPlayerDriection.x > 0;
            if (isRightSide)
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
                //spriteTr.rotation = Quaternion.Euler(45, 0, 0); 이제 부모가 회전한대로 그대로 따라가며 ㄴ돼서 필요없다!
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                //spriteTr.rotation = Quaternion.Euler(-45, 180, 0); //부모의 로테이션이 변경되어서 로컬 y축 값도 180으로 변경해야되더라..?
            }

            yield return null; ;
        }
    }
}

