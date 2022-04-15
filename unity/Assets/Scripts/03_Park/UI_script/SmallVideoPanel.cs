using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SmallVideoPanel : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

    }


    public void ScaleUpPanel()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
    }
    
}
