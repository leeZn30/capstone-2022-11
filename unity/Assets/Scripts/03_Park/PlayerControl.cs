using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviourPun
{
    // 플레이어 설정
    float moveSpeed = 10f;
    public bool isMoveAble = true;

    // 상호작용 버튼
    [SerializeField] bool isInteractiveAble = false;
    GameObject InteractiveButton;
    [SerializeField] Sprite[] buttonImages;

    // 비디오
    [SerializeField] bool isVideoPanelShown = false;
    GameObject videoPanel;

    // Start is called before the first frame update
    void Start()
    {
        if (this.photonView.IsMine)
        {
            InteractiveButton = FindObjectOfType<Canvas>().transform.Find("InteractiveButton").gameObject;
            videoPanel = FindObjectOfType<Canvas>().transform.Find("smallVideoPanel").gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveAble)
        {
            PlyerMove();
        }
    }

    private void PlyerMove()
    {
        if (this.photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                gameObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                gameObject.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }

        }
    }

    public void OnInteractiveButton(int type)
    {
        if (!isInteractiveAble)
        {
            InteractiveButton.GetComponent<Image>().sprite = buttonImages[type];
            InteractiveButton.SetActive(true);
            isInteractiveAble = true;
        }
    }

    public void OffInteractiveButton()
    {
        if (isInteractiveAble)
        {
            InteractiveButton.SetActive(false);
            isInteractiveAble = false;
        }

    }

    public void OnVideoPanel()
    {
        if (!isVideoPanelShown)
        {
            videoPanel.SetActive(true);
            isVideoPanelShown = true;
        }
    }

    public void OffVideoPanel()
    {
        if (isVideoPanelShown)
        {
            videoPanel.SetActive(false);
            isVideoPanelShown = false;
        }
    }

    public void sendEmoji(int emojiNum)
    {
        GameObject bubble = transform.GetChild(0).gameObject;
        bubble.SetActive(true);
        bubble.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = "<sprite=" + emojiNum + ">";
    }

    public void startBusking()
    {

    }

    /**
     * 이럴 경우 ontrigger가 두번씩 처리됨 why
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("enter");
        switch (collision.tag)
        {
            case "BuskingSpot":
                OnVideoPanel();
                break;

            case "InteractiveObject":
                OnInteractiveButton();
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "BuskingSpot":
                OffVideoPanel();
                break;

            case "InteractiveObject":
                OffInteractiveButton();
                break;

            default:
                break;
        }

    }
    **/
    

}
