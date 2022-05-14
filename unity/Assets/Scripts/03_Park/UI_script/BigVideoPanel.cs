using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigVideoPanel : MonoBehaviour
{
    [SerializeField] private GameObject shutButton;
    [SerializeField] private SmallVideoPanel smallVideoPanel;
    [SerializeField] private GameObject emoticonPanel;

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
        emoticonPanel.SetActive(false);

        if (AgoraChannelPlayer.Instance.role != "publisher")
        {
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
        }
    }
}
