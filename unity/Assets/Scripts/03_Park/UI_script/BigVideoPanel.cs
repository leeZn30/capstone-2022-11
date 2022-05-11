using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigVideoPanel : MonoBehaviour
{
    [SerializeField] private GameObject shutButton;
    [SerializeField] private SmallVideoPanel smallVideoPanel;

    // Start is called before the first frame update
    void Start()
    {
        shutButton.GetComponent<Button>().onClick.AddListener(smallVideoPanel.ScaleDownPanle);
    }

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = false;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = false;
    }

    private void OnDisable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
    }


    public void ScaleDownPanel()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<Canvas>().transform.Find("smallVideoPanel").gameObject.SetActive(true);
    }
}
