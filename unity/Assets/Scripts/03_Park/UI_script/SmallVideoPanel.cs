using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SmallVideoPanel : MonoBehaviour
{

    public void ScaleUpPanel()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
    }
    
}
