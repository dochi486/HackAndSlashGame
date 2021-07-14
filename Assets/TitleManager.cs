using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    CanvasGroup blackScreen;
    // Start is called before the first frame update
    void Start()
    {
      
        blackScreen = GameObject.Find("PersistCanvas").transform.Find("BlackScreen").GetComponent<CanvasGroup>();
        //검은 화면에서 페이드인으로 시작

        blackScreen.DOFade(0, 0.7f);

        //뉴게임 버튼 누르면 스테이지 1로드
        //검은화면에서 페이드아웃되면서 스테이지1 페이드인 
        Button button = GameObject.Find("TitleCanvas").transform.Find("Button").GetComponent<Button>();

        button.AddListener(this, OnClickNewGame);
    }
public void OnClickNewGame()
    {
        blackScreen.DOFade(1, 0.7f).OnComplete(()=> 
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1");
        
        });
        //알파의 최대값은 1이고 어두운 화면을 보이게 한다...

    }
}
