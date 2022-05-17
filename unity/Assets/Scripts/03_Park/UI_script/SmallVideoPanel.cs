using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmallVideoPanel : MonoBehaviour
{

    Vector3 initPosition;
    Vector3 initScale;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ScaleUpPanel);
    }

    private void OnEnable()
    {
        if (AgoraChannelPlayer.Instance.role == "publisher")
        {
            gameObject.transform.localPosition = new Vector3(-700, 290, 0);
            gameObject.transform.localScale = new Vector3(-1, -1, 1);

            initPosition = new Vector3(-700, 290, 0);

        }
        else
        {
            gameObject.transform.localPosition = new Vector3(0, 290, 0);
            gameObject.transform.localScale = new Vector3(-1, -1, 1);

            initPosition = new Vector3(0, 290, 0);
        }

        this.gameObject.GetComponent<Button>().enabled = true;

    }


    public void ScaleUpPanel()
    {
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
        transform.localScale = new Vector3(-2, -2, 1);
        transform.localPosition = new Vector3(-350, 0, 0);
        this.gameObject.GetComponent<Button>().enabled = false;
    }

    public void ScaleDownPanle()
    {
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(false);
        transform.localScale = new Vector3(-1, -1, 1);
        //transform.localPosition = new Vector3(0, 290, 0);
        transform.localPosition = initPosition;
        this.gameObject.GetComponent<Button>().enabled = true;
    }

}
