using UnityEngine;
using UnityEngine.UI;

public class StageResultUI : BaseUI<StageResultUI>
{
    public override string HierarchyPath => "StageCanvas/StageResultUI";

    Text gradeText;
    Text eneiesKilledText;
    Text damageTakenText;
    Button continueButton;

    void Init()
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
        Init();
        eneiesKilledText.text = ($"{StageManager.Instance.enemiesKilledCount} / {StageManager.Instance.sumMonsterCount}");
        damageTakenText.text = StageManager.Instance.damageTaken.ToString();
        gradeText.text = "A";
    }
}
