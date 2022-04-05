using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SmallVideoPanel : MonoBehaviour
{
    // 비디오 동기화 시도해볼까

    // mode = 0 시청자 mode = 1 버스커
    public int mode;

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

        if (mode == 0)
        {
        }
    }


    public void ScaleUpPanel()
    {
        if (mode == 0)
        {
            this.gameObject.SetActive(false);
            FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
        }
    }
    
}
