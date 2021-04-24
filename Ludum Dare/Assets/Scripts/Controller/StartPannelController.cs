using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPannelController : MonoBehaviour
{
    public void StartPannelCallback()
    {
        SceneManager.LoadScene("GameScene");
    }
}
