using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartPannelController : MonoBehaviour
{
    [Header("Manager")]
    public GameObject GameManager;
    [Header("Panel相关预制体")]
    public GameObject CoursePanelPrefab;
    public GameObject OptionsPanelPrefab;

    public void StartPannelCallback()
    {
        Instantiate<GameObject>(GameManager, Vector3.zero, Quaternion.identity);
    }

    public void Options()
    {
        //实例化
        GameObject tutorialsPanel = Instantiate<GameObject>(OptionsPanelPrefab, new Vector3((float)Screen.width * 1.5f, (float)Screen.height / 2, 0), Quaternion.identity, transform);
        //改名字
        tutorialsPanel.name = CoursePanelPrefab.name;

        //动画
        transform.DOLocalMoveX(-1920, 0.2f);
    }

    public void Course()
    {
        //实例化
        GameObject tutorialsPanel = Instantiate<GameObject>(CoursePanelPrefab, new Vector3((float)Screen.width * 1.5f, (float)Screen.height / 2, 0), Quaternion.identity, transform);
        //改名字
        tutorialsPanel.name = CoursePanelPrefab.name;

        //动画
        transform.DOLocalMoveX(-1920, 0.2f);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
