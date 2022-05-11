using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SmallVideoPanel : MonoBehaviour
{
    public Vector3 initPosition;
    public Vector3 initSize;

    private void Start()
    {
        initPosition = transform.localPosition;
        initSize = transform.localScale;
    }


    public void ScaleUpPanel()
    {
        StartCoroutine(ScaleUp(this.transform, new Vector3(-2, -2, 0), 2f));
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
        this.gameObject.GetComponent<Button>().enabled = false;
    }

    public void ScaleDownPanle()
    {
        StartCoroutine(ScaleDown(this.transform, 2f));
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(false);
        this.gameObject.GetComponent<Button>().enabled = true;
    }

    IEnumerator ScaleUp(Transform transform, Vector3 upScale, float duration)
    {
        Vector3 initialScale = transform.localScale;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, upScale, duration);
            transform.localPosition = new Vector3(-230, 0, 0);
            yield return null;
        }
    }
    IEnumerator ScaleDown(Transform transform, float duration)
    {
        Vector3 initialScale = transform.localScale;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, initSize, duration);
            transform.localPosition = initPosition;
            yield return null;
        }
    }

}
