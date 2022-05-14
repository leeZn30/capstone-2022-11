using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmallVideoPanel : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.localPosition.Set(0, 290, 0);
        gameObject.transform.localScale.Set(-1, -1, 1);

        GetComponent<Button>().onClick.AddListener(ScaleUpPanel);
    }


    public void ScaleUpPanel()
    {
        //StartCoroutine(ScaleUp(this.transform, new Vector3(-2, -2, 0), new Vector3(-330, 0, 0), 2f));
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
        transform.localScale.Set(-2, -2, 1);
        transform.localPosition.Set(-350, 0, 0);
        this.gameObject.GetComponent<Button>().enabled = false;
    }

    public void ScaleDownPanle()
    {
        //StartCoroutine(ScaleDown(this.transform, 2f));
        FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(false);
        transform.localScale.Set(-1, -1, 1);
        transform.localPosition.Set(0, 290, 0);
        this.gameObject.GetComponent<Button>().enabled = true;
    }

    IEnumerator ScaleUp(Transform transform, Vector3 upScale, Vector3 upPosition, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 initialPosition = transform.localPosition;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, upScale, duration);
            transform.localPosition = Vector3.Lerp(initialPosition, upPosition, duration);
            yield return null;
        }
    }
    IEnumerator ScaleDown(Transform transform, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 initialPosition = transform.localPosition;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, new Vector3(-1, -1, 1), duration);
            transform.localPosition = Vector3.Lerp(initialPosition, new Vector3(0, 250, 0), duration);
            yield return null;
        }
    }

}
