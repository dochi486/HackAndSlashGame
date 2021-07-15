using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

/// <summary>
/// 스테이지에서 발생하는 모든 이벤트를 관리하고 스테이지가 끝나면 같이 파괴된다.
/// 1. 플레이어 로드 게임 시작할 때까지 플레이어를 대기시킴
/// 2. 스테이지 시작할 때 화면이 밝아지도록
/// 3. 몬스터 로드
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager instance;
    public GameStateType gameState = GameStateType.Ready;

    private void Awake()
    {
        instance = this;
        gameState = GameStateType.Ready;
    }
    private void OnDestroy()
    {
        instance = null;
    }
    public Ease inEaseType = Ease.InBounce;
    public Ease outEaseType = Ease.OutBounce;
    [Button]
    IEnumerator Start()
    {
        //화면 어두운 상태에서 2초동안 밝아지도록
        CanvasGroup blackScreen = PersistCanvas.instance.blackScreen;
        blackScreen.gameObject.SetActive(true);
        blackScreen.alpha = 1;
        blackScreen.DOFade(0, 1.7f);

        yield return new WaitForSeconds(1.7f);
        //OnComplete람다식으로 쉬지 않는 이유는 쉬어야하는 부분이 많기 때문에 람다식보다 코루틴으로 쉬는 게 더 직관적이다. 

        //스테이지 이름 표시 "Stage 1"
     
        StageInfo stageInfo = GameData.stageInfoMap[SceneProperty.instance.StageID];
        //GameData.instance.stageInfoMap[SceneProperty.instance.StageID];
        string stageName = stageInfo.titleString;


        /*"Stage" + SceneProperty.instance.StageID;*/
        StageCanvas.instance.stageNameText.transform.localPosition = new Vector3(-1000, 0, 0);
        StageCanvas.instance.stageNameText.transform.DOLocalMoveX(0, 0.5f).SetEase(inEaseType);


        StageCanvas.instance.stageNameText.text = stageName;
        //2초 쉬고 플레이어를 움직일 수 있게 
        yield return new WaitForSeconds(2f);

        StageCanvas.instance.stageNameText.transform.DOLocalMoveX(-1000f, 0.5f).SetEase(outEaseType);

        gameState = GameStateType.Playing;//움직일 수 있는 상태
    }
}
public enum GameStateType
{
    Playing,
    Ready,
    StageEnd
}