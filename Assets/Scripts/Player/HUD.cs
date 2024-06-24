using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = PlayerStats.instance.exp;
                float maxExp = PlayerStats.instance.nextExp[Mathf.Min(PlayerStats.instance.level, PlayerStats.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", PlayerStats.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", PlayerStats.instance.kill);
                break;
            case InfoType.Time:
                break;
            case InfoType.Health:
                break;
        }
    }
}
