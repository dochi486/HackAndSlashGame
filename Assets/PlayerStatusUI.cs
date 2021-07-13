using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
   
    Image hpTemp;
    Text hpText;
    float hpMax = 100;
    //Player.StateType state;
    void Start()
    {
        hpTemp = GetComponent<Image>();
        hpText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        hpTemp.fillAmount = Player.instance.hp / hpMax;
        hpText.text = ($"{Player.instance.hp}/100");

    }
}
