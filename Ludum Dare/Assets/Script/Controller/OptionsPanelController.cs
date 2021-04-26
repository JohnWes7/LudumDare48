using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelController : MonoBehaviour
{
    public Slider VolumeSlider;
    public Text VolumeValueText;
    public Dropdown ResolutionDropdown;
    public Dropdown LanguageDropdown;

    private void Start()
    {
        int volumeValue = (int)(PlayerPrefs.GetFloat("Volume", 1) * 100);
        VolumeSlider.value = volumeValue;
        VolumeValueText.text = volumeValue.ToString();

        int resolutionValue = PlayerPrefs.GetInt("Resolution", 0);
        ResolutionDropdown.value = resolutionValue;

        int languageValue = PlayerPrefs.GetInt("Language", (int)Config.Language.CH);
        LanguageDropdown.value = languageValue;
    }

    public void Back()
    {
        transform.parent.DOLocalMoveX(0, 0.2f).OnComplete(() => { Destroy(gameObject); });
    }

    //调整声音的回调函数
    public void VolumeSliderChange(float value)
    {
        VolumeValueText.text = value.ToString();
        PlayerPrefs.SetFloat("Volume", value / 100);
    }

    //调整分辨率的回调函数
    public void ScreenResolutionChange(int value)
    {
        switch (value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            default:
                break;
        }

        PlayerPrefs.SetInt("Resolution", value);
    }

    public void LanguageChange(int value)
    {
        Config.Language language = (Config.Language)value;

        PlayerPrefs.SetInt("Language", (int)language);
    }
}
