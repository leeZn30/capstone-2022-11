using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallVideoPanel : MonoBehaviour
{

    public void ScaleUpPanel()
    {
        /**
        mode = 1;
        gameObject.transform.localScale = new Vector3(3.5f, 3.5f, 1);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -100, 1);
        **/
        this.gameObject.SetActive(false);
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);

        
    }
    
}
