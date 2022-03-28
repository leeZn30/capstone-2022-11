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
    public bool isUIActable = true;

    // 상호작용 버튼
    [SerializeField] bool isInteractiveAble = false;
    public GameObject InteractiveButton;
    [SerializeField] Sprite[] buttonImages;

    // 비디오
    public bool isVideoPanelShown = false;
    public GameObject videoPanel;

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

    // -------------- 이모지 동기화 관련 함수들 -------------
    public IEnumerator sendEmoji(int emojiNum)
    {
        GameObject bubble = transform.GetChild(0).gameObject;
        bubble.SetActive(true);
        bubble.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = "<sprite=" + emojiNum + ">";

        yield return new WaitForSeconds(1f);

        bubble.SetActive(false);
    }

    [PunRPC]
    public void callEmoji(int emojiNum)
    {
        StartCoroutine(sendEmoji(emojiNum));
    }

    public void rpctest(int emojiNum)
    {
        photonView.RPC("callEmoji", RpcTarget.AllBuffered, emojiNum);
    }
    //----------------------------------------------------------------
    

}
