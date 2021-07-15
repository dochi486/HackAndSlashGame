using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneProperty : MonoBehaviour
{
    public static SceneProperty instance;

    public int StageID = -1;
    public enum SceneType
    {
        Title,
        Stage,

    }
    public SceneType sceneType = SceneType.Stage;
    private void Awake()
    {
        instance = this;
    }
    private void OnDestroy()
    {
        instance = null;  //꼭 해줘야하는 것? 의도치 않은 결과를 막위 해서 싱글턴을 사용할 때 디스트로이에서 null로 초기화 해줘야한다. 
        //null 초기화를 안하게 되면: 
    }
}
