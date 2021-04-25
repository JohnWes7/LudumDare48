using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果player到达终点就下一关
        if (collision.tag == "Player")
        {
            GameManager.Instance.StartNextLevel();
        }
        Debug.Log(collision.tag);
    }

}
