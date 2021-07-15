using System;
using UnityEngine.UI;

public class StageResultUI : BaseUI<StageResultUI>
{
    Text gradeText;
    Text eneiesKilledText;
    Text damageTakenText;
    Button continueButton;

    void Start()
    {

        gradeText = transform.Find("StageResultUI/GradeText").GetComponent<Text>();
        eneiesKilledText = transform.Find("StageResultUI/EneiesKilledText").GetComponent<Text>();
        damageTakenText = transform.Find("StageResultUI/DamageTakenText").GetComponent<Text>();
        continueButton = transform.Find("StageResultUI/ContinueButton").GetComponent<Button>();
        continueButton.AddListener(this, LoadNextStage);
    }

    private void LoadNextStage()
    {
        throw new NotImplementedException();
    }


}
