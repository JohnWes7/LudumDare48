using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCPanelController : MonoBehaviour
{
    public void ESCYes()
    {
        GameManager.Instance.ESCYesCallBack();
    }

    public void ESCNo()
    {
        Destroy(gameObject);
    }
}
