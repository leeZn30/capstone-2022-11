using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigVideoPanel : MonoBehaviour
{
    [SerializeField] private GameObject shutButton;
    [SerializeField] private SmallVideoPanel smallVideoPanel;
    [SerializeField] private GameObject soundSlider;

    // 없앨 오브젝트
    [SerializeField] private GameObject emoticonButton;
    [SerializeField] private GameObject soundButton;

    // 사용자에 따라 달라지는 버튼
    [SerializeField] private Button dynamicButton;
    [SerializeField] private Sprite[] buttonImages;

    // Start is called before the first frame update
    void Start()
    {
        shutButton.GetComponent<Button>().onClick.AddListener(smallVideoPanel.ScaleDownPanle);
    }

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = false;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = false;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().InteractiveButton.SetActive(false);
        emoticonButton.SetActive(false);
        soundButton.SetActive(false);

        if (AgoraChannelPlayer.Instance.role == "publisher")
        {
            dynamicButton.GetComponent<Image>().sprite = buttonImages[0];
            dynamicButton.onClick.AddListener(delegate { AgoraChannelPlayer.Instance.leaveChannel(); }); // 버스킹 나가기
        }
        else if (AgoraChannelPlayer.Instance.role == "audience")
        {
            dynamicButton.GetComponent<Image>().sprite = buttonImages[1];
            dynamicButton.onClick.AddListener(delegate { }); // 유저 정보 보는 판넬 띄우기
        }
        else
        {
            Debug.Log("Wrong role but big Panel is active");
        }

        soundSlider.SetActive(true);
    }

    private void OnDisable()
    {
        if (AgoraChannelPlayer.Instance.role != "publisher")
        {
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        }
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().InteractiveButton.SetActive(true);
        emoticonButton.SetActive(true);
        soundButton.SetActive(true);

        dynamicButton.onClick.RemoveAllListeners();

        soundSlider.SetActive(false);

    }
}
