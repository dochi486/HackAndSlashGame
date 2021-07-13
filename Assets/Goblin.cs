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
            transform.Translate(toPlayerDriection * speed * Time.deltaTime);
            yield return null; ;
        }
    }
}

