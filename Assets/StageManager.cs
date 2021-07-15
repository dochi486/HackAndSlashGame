using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 스테이지에서 발생하는 모든 이벤트를 관리하고 스테이지가 끝나면 같이 파괴된다.
/// 1. 플레이어 로드 게임 시작할 때까지 플레이어를 대기시킴
/// 2. 스테이지 시작할 때 화면이 밝아지도록
/// 3. 몬스터 로드
/// </summary>
public class StageManager : SingletonMonoBehavior<StageManager>
{
    public GameStateType gameState = GameStateType.Ready;

    public int sumMonsterCount;
    public int enemiesKilledCount;
    public int damageTaken;

    new private void Awake()
    {
        base.Awake(); //부모의 Awake에서 instance생성
        gameState = GameStateType.Ready;

        List<SpawnPoint> allSpawnPoints = new List<SpawnPoint>(FindObjectsOfType<SpawnPoint>(true));
        //FindObjects의 결과 배열을 그대로 리스트에 담는 내용
        sumMonsterCount = allSpawnPoints.Where(x => x.spawnType != SpawnType.Player).Count();
        //리스트로 받았기 때문에 타입에 따른 분류를 Linq로 쉽게 할 수 있다!
    }

    public Ease inEaseType = Ease.InBounce;
    public Ease outEaseType = Ease.OutBounce;

    [Button] //NaughtyAttributes애셋으로 Start함수만 다시 실행할 수 있는 버튼 인스펙터에 생성
    IEnumerator Start()
    {
        CanvasGroup blackScreen = PersistCanvas.instance.blackScreen;
        blackScreen.gameObject.SetActive(true);
        blackScreen.alpha = 1;
        blackScreen.DOFade(0, 1.7f); //화면 어두운 상태에서 1.7초동안 밝아지도록

        yield return new WaitForSeconds(1.7f);
        //OnComplete람다식으로 쉬지 않는 이유는 쉬어야하는 부분이 많기 때문에 람다식보다 코루틴으로 쉬는 게 더 직관적이다. 

        //스테이지 이름 표시
        StageInfo stageInfo = GameData.stageInfoMap[SceneProperty.instance.StageID];
        //GameData.instance.stageInfoMap[SceneProperty.instance.StageID];
        string stageName = stageInfo.titleString;
        /*"Stage" + SceneProperty.instance.StageID;*/


        StageCanvas.instance.stageNameText.transform.localPosition = new Vector3(-1000, 0, 0);
        StageCanvas.instance.stageNameText.transform.DOLocalMoveX(0, 0.5f).SetEase(inEaseType); //스테이지 이름 나타나는 효과

        StageCanvas.instance.stageNameText.text = stageName;
        //2초 쉬고 플레이어를 움직일 수 있게 
        yield return new WaitForSeconds(2f);

        StageCanvas.instance.stageNameText.transform.DOLocalMoveX(-1000f, 0.5f).SetEase(outEaseType); //스테이지 이름 사라지는 효과

        gameState = GameStateType.Playing;//움직일 수 있는 상태
    }
}
public enum GameStateType
{
    Playing,
    Ready,
    StageEnd
}