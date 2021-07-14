using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

public class EditorSceneLoad 
{
    [MenuItem("Window/1. Title Scene Load")]
    private static void TitleSceneLoad()
    {
        LoadScene("Title");
    }

    [MenuItem("Window/2. Stage1 Scene Load")]
    private static void Stage1SceneLoad()
    {
        LoadScene("Stage1");
    }
    //알트 W 누르고 윈도우 메뉴 열고, 1누르면 Title씬 로드, 2누르면 스테이지1

    private static void LoadScene(string loadSceneName)
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{loadSceneName}.unity");
        //Scene Path를 Assets부터 다 써서 확장자까지 입력!!


        //UnityEditor.SceneManagement.EditorSceneManager.LoadScene(loadSceneName); 
        //플레이중일 때만 로드 가능한 방법
        //에디터에서 씬을 로드할 때는 일반 유니티엔진에서 로드하는 방식과 다름
    }
}
