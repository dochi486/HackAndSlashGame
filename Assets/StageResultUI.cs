using System;
using UnityEngine;
using UnityEngine.UI;

public class StageResultUI : BaseUI<StageResultUI>
{
    public override string HierarchyPath => "StageCanvas/StageResultUI"; //하이어라키에 해당 오브젝트가 없으면 생성하게 하는 로직

    Text gradeText;
    Text eneiesKilledText;
    Text damageTakenText;
    Button continueButton;



    override protected void OnShow()
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

    internal void Show(int enemiesKilledCount, int sumMonsterCount, int damageTaken)
    {
        base.Show();

        eneiesKilledText.text = ($"{enemiesKilledCount} / {sumMonsterCount}");
        damageTakenText.text = damageTaken.ToString();
        gradeText.text = "A";
    }
}
