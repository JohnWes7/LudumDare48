using FE_EventInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButtonController : MonoBehaviour
{
    public Text label;
    public Button button;
    public Option option;

    public void InIt(Option option)
    {
        this.label.text = option.label;
    }
}
