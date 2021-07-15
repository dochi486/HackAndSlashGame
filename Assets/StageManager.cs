using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 스테이지에서 발생하는 모든 이벤트를 관리하고 스테이지가 끝나면 같이 파괴된다.
/// 1. 플레이어 로드 게임 시작할 때까지 플레이어를 대기시킴
/// 2. 스테이지 시작할 때 화면이 밝아지도록
/// 3. 몬스터 로드
/// </summary>
public class StageManager : MonoBehaviour
{
    IEnumerator Start()
    {
        //화면 어두운 상태에서 2초동안 밝아지도록
        CanvasGroup blackScreen = PersistCanvas.instance.blackScreen;
        blackScreen.gameObject.SetActive(true);
        blackScreen.alpha = 1;
        blackScreen.DOFade(1, 1.7f);

        yield return new WaitForSeconds(1.7f); 
        //OnComplete람다식으로 쉬지 않는 이유는 쉬어야하는 부분이 많기 때문에 람다식보다 코루틴으로 쉬는 게 더 직관적이다. 

        //스테이지 이름 표시 "Stage 1"


        //2초 쉬고 플레이어를 움직일 수 있게 


    }

    void Update()
    {
        
    }
}
