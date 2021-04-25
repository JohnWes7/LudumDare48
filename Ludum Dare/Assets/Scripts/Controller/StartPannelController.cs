using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPannelController : MonoBehaviour
{
    public GameObject GameManager;

    public void StartPannelCallback()
    {
        Instantiate<GameObject>(GameManager, Vector3.zero, Quaternion.identity);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
