using UnityEngine;
using UnityEngine.UI;

public class StageResultUI : BaseUI<StageResultUI>
{
    Text gradeText;
    Text eneiesKilledText;
    Text damageTakenText;
    Button continueButton;

    void Start()
    {

        gradeText = transform.Find("GradeText").GetComponent<Text>();
        eneiesKilledText = transform.Find("EneiesKilledText").GetComponent<Text>();
        damageTakenText = transform.Find("DamageTakenText").GetComponent<Text>();
        continueButton = transform.Find("ContinueButton").GetComponent<Button>();
        continueButton.AddListener(this, LoadNextStage);
    }

    private void LoadNextStage()
    {
        Debug.LogWarning("LoadNextStage");
    }

    protected override void OnShow()
    {
        eneiesKilledText.text = ($"{StageManager.Instance.enemiesKilledCount.ToString()} / {StageManager.Instance.sumMonsterCount}");
        damageTakenText.text = StageManager.Instance.damageTaken.ToString();
        gradeText.text = "A";
    }

}
