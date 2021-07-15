using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;
    [SerializeField]private List<StageInfo> stageInfos; 
    //다른 스크립트에서 수정 안되도록 프라이빗으로 바꾸고 인스펙터에서 디버깅은 가능하도록 시리얼라이즈드 해준다.
    public static Dictionary<int, StageInfo> stageInfoMap = new Dictionary<int, StageInfo>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        stageInfoMap = stageInfos.ToDictionary(x => x.stageID); //리스트를 딕셔너리로 바꾸는 과정
    }
}

[System.Serializable]
public class StageInfo //글로벌로 쓰는 게 좋지는 않지만 사용하기 편리해서 아직 배우는 단계에선 쓰면 편하다. 
{
    public int stageID;
    public string titleString;
    public int rewardXP; //해당 스테이지에 있는 총 경험치 (인스펙터에서 조절가능하게)
}
