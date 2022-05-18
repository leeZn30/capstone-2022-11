using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviourPunCallbacks
{

    // 플레이어 설정
    float moveSpeed = 6f;
    public bool isMoveAble = true;
    public bool isUIActable = true;

    // 상호작용 버튼
    [SerializeField] bool isInteractiveAble = false;
    public GameObject InteractiveButton;
    [SerializeField] Sprite[] buttonImages;
    [SerializeField] int nowInteractiveType = -1;

    // 비디오
    public bool isVideoPanelShown = false;
    public GameObject videoPanel;
    public GameObject bigVideoPanel;

    // 이모티콘 코루틴 실행여부
    private bool isEmojiRunning = false;
    private Coroutine runningEmojiCorutine;

    // 채팅
    [SerializeField] GameObject ChatPanel;

    // BusketPanel
    [SerializeField] private GameObject buskerPanel;

    // 줌 인/줌 아웃
    private float wheelSpeed = 10;
    [SerializeField] private float cameraDistance = 10;

    // 팔로우
    [SerializeField] private ParkFollow parkFollow;

    private Vector2 mapSize;
    private int isMoving=0;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (this.photonView.IsMine)
        {
            InteractiveButton = FindObjectOfType<Canvas>().transform.Find("InteractiveButton").gameObject;
            videoPanel = FindObjectOfType<Canvas>().transform.Find("smallVideoPanel").gameObject;
            bigVideoPanel = FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject;
            ChatPanel = FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.transform.Find("ChatView").gameObject;
            buskerPanel = FindObjectOfType<Canvas>().transform.Find("BuskerVideoPanel").gameObject;
            animator = GetComponent<Animator>();
            parkFollow = FindObjectOfType<ParkFollow>();

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
            isMoving = 0;

            if (Input.GetKey(KeyCode.W))
            {
                isMoving = 1;
                Debug.DrawRay(gameObject.transform.position, Vector3.up * 0.3f, Color.green);
                RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector3.up, 0.3f, LayerMask.GetMask("CantPassObj"));

                if (hit.collider == null)
                {
                    gameObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                }

            }

            if (Input.GetKey(KeyCode.S))
            {
                isMoving = 1;
                Debug.DrawRay(gameObject.transform.position, Vector3.down * 0.3f, Color.green);
                RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector3.down, 0.3f, LayerMask.GetMask("CantPassObj"));
                if (hit.collider == null)
                {
                    gameObject.transform.Translate(Vector3.down  * moveSpeed * Time.deltaTime);
                }
                


               
            }

            if (Input.GetKey(KeyCode.D))
            {
                isMoving = 1;
                Debug.DrawRay(gameObject.transform.position, Vector3.right * 0.3f, Color.green);
                RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector3.right, 0.3f,LayerMask.GetMask("CantPassObj"));
                if (hit.collider == null)
                {
                    gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                isMoving = -1;
                Debug.DrawRay(gameObject.transform.position, Vector3.left * 0.3f, Color.green);
                RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector3.left, 0.3f, LayerMask.GetMask("CantPassObj"));
                if (hit.collider == null)
                {
                    gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }
                

                
            }

            animator.SetBool("isMoving", isMoving != 0);
            //cameraDistance = Input.GetAxis("Mouse ScrollWheel") * wheelSpeed * Time.deltaTime;
            //Camera.main.orthographicSize = cameraDistance;

        }
    }

    public void OnInteractiveButton(int type)
    {
        /**
         * 0) 버스킹
         * 1) 버스킹 그만두기
         * 2) 버스커 팔로우
         * 3) 순간이동기
         * 4) 음원존 등록
         * **/

        if (!isInteractiveAble)
        {
            InteractiveButton.GetComponent<Image>().sprite = buttonImages[type];
            InteractiveButton.SetActive(true);
            isInteractiveAble = true;

            switch (type)
            {
                case 0:
                    InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { OnVideoPanel(1); });
                    break;
                case 1:
                    InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { AgoraChannelPlayer.Instance.leaveChannel(); });
                    break;
                case 2:
                    InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { parkFollow.Open(AgoraChannelPlayer.Instance.nowBuskingSpot.buskerID); });
                    break;
                case 3:
                    break;
                case 4:
                    //musicSpot를 받아야해서 일단 다른 방식으로 함
                    break;

            }
            nowInteractiveType = type;
        }
    }

    public void OffInteractiveButton(int type)
    {
        if (isInteractiveAble)
        {
            if (type == nowInteractiveType)
            {
                InteractiveButton.GetComponent<Button>().onClick.RemoveAllListeners();
                InteractiveButton.SetActive(false);
                isInteractiveAble = false;

                nowInteractiveType = -1;
            }
        }

    }

    public void changeInteractiveButton(int type)
    {
        if (isInteractiveAble)
        {
            InteractiveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            InteractiveButton.GetComponent<Image>().sprite = buttonImages[type];

            switch (type)
            {
                case 0:
                    InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { OnVideoPanel(1); }) ;
                    break;
                case 1:
                    InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { AgoraChannelPlayer.Instance.leaveChannel(); });
                    break;
                case 2:
                    InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { parkFollow.Open(AgoraChannelPlayer.Instance.nowBuskingSpot.buskerID); });
                    break;
                case 3:
                    break;
            }
            nowInteractiveType = type;
        }
    }

    public void OnVideoPanel(int mode)
    {
        if (!isVideoPanelShown)
        {
            switch (mode)
            {
                case 0: // small
                    videoPanel.GetComponent<SmallVideoPanel>();
                    videoPanel.SetActive(true);
                    isVideoPanelShown = true;
                    break;

                case 1: // busker 준비
                    buskerPanel.SetActive(true);
                    GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = false;
                    GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = false;
                    buskerPanel.GetComponent<BuskerVideoPanel>().setDevice();
                    isVideoPanelShown = true;
                    break;

                default:
                    break;
            }
        }
    }

    public void OffVideoPanel()
    {
        if (isVideoPanelShown)
        {
            // child 에는 부모와 자식이 함께 설정 된다.
            var child = ChatPanel.GetComponent<Chat>().msgList.GetComponentsInChildren<Transform>();

            foreach (var iter in child)
            {
                // 부모(this.gameObject)는 삭제 하지 않기 위한 처리
                if (iter != ChatPanel.GetComponent<Chat>().msgList.transform)
                {
                    Destroy(iter.gameObject);
                }
            }
            ChatPanel.GetComponent<Chat>().emojimsg = "";
            // 걍 체크하지 말고 다 false
            videoPanel.SetActive(false);
            buskerPanel.SetActive(false);
            bigVideoPanel.SetActive(false);

            isVideoPanelShown = false;
        }
    }

    // 방송 초기설정
    public void initBusking()
    {
        // 캐릭터 위치 등
    }

    // -------------- 이모지 동기화 관련 함수들 -------------
    public IEnumerator sendEmoji(int emojiNum)
    {
        GameObject bubble = transform.GetChild(1).gameObject;
        bubble.SetActive(true);
        bubble.GetComponent<TextMeshPro>().text = "<sprite=" + emojiNum + ">";
        isEmojiRunning = true;

        yield return new WaitForSeconds(1f);

        bubble.SetActive(false);
        isEmojiRunning = false;

    }

    [PunRPC]
    public void callEmoji(int emojiNum)
    {
        if (isEmojiRunning)
        {
            StopCoroutine(runningEmojiCorutine);
            isEmojiRunning = false;
        }

        runningEmojiCorutine = StartCoroutine(sendEmoji(emojiNum));
    }

    public void rpcEmoji(int emojiNum)
    {
        photonView.RPC("callEmoji", RpcTarget.AllBuffered, emojiNum);
    }
    //----------------------------------------------------------------

}
