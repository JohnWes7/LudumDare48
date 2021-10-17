using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoursePanelController : MonoBehaviour
{
    public void Back()
    {
        //动画
        transform.parent.DOLocalMoveX(0, 0.2f).OnComplete(() => { Destroy(gameObject); });
    }
}
