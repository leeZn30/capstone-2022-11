using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ParkFollow: MusicWebRequest
{
    public GameObject panel;
    public Button exitBtn;

    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI followerText;
    public TextMeshProUGUI followText;
    public TextMeshProUGUI musicCntText;
    public Character character;

    public Button infoFollowBtn;


    public GameObject uploadedSongScrollViewObject;
    private List<SongSlot> uploadedSlots;

    public GameObject noListObjects;

    private TextMeshProUGUI infoFollowText;//팔로우 취소, 팔로우 하기 text
    private bool isAlreadyInit=false;
    private string currentUserId;
    private string currentUserNickname;

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(wp, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10f, LayerMask.GetMask("Player"));
            if (hit.collider != null)
            {
                ShowUserInfoBlock(hit.collider,wp);
            }
            else
            {
                UserInfoMini info = (UserInfoMini)FindObjectOfType(typeof(UserInfoMini));

                if (info != null)
                    Destroy(info.gameObject);

            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {//UI위에 커서가 없을 때만 정보 삭제
                UserInfoMini info = (UserInfoMini)FindObjectOfType(typeof(UserInfoMini));

                if (info != null)
                {
                    Destroy(info.gameObject);

                }
            }

        }
    }

    void ShowUserInfoBlock(Collider2D collider,Vector2 wp)
    {

            PlayerManager pm;
            if (collider.TryGetComponent<PlayerManager>(out pm))
            {
                if (pm.GetId() == UserData.Instance.id)
                {
                return;
                }
                
                UserInfoMini info = (UserInfoMini)FindObjectOfType(typeof(UserInfoMini));

                if (info == null)
                {
                    GameObject obj = Instantiate((Resources.Load("Prefabs/03_Park/UserInfo")) as GameObject);

                    info = obj.GetComponent<UserInfoMini>();

                }

                info.gameObject.transform.position = new Vector3(wp.x, wp.y, 0);

                info.SetUserNickName(pm.GetNickName(), pm.GetId());
                info.SetFollowPage(this);


            }

        
    }
    public void Open(string id)
    {
        panel.SetActive(true);
        LoadUserProfile(id);//내 프로필 로드
    }
     public void Init()
    {
        if (isAlreadyInit == false)
        {

            isAlreadyInit = true;

            uploadedSlots = new List<SongSlot>(uploadedSongScrollViewObject.GetComponentsInChildren<SongSlot>());
            infoFollowText = infoFollowBtn.transform.GetComponentInChildren<TextMeshProUGUI>();

            exitBtn.onClick.AddListener(delegate { panel.SetActive(false); });
            infoFollowBtn.onClick.AddListener(delegate{
                if (infoFollowText.text == "팔로우")
                {//팔로우
                    FollowUser(currentUserId, currentUserNickname);
                    infoFollowText.text = "팔로우 취소";
                }
                else
                {//팔로우 취소
                    FollowCancelUser(currentUserId, currentUserNickname);
                    infoFollowText.text = "팔로우";
                }
            });




        }
    }

    void FollowUser(string id, string nickname)
    {
        //팔로우 버튼 클릭시
        CallFollowApi(id, nickname);
        UserData.Instance.AddFollow(id);

    }
    void FollowCancelUser(string id, string nickname)
    {
        // 팔로우 취소 버튼 클릭시
        CallFollowApi(id, nickname, true);
        UserData.Instance.DelFollow(id);
    }

    //유저 프로필을 로드하는 함수
    async void LoadUserProfile(string _id)
    {
        if (UserData.Instance.user.follow.Contains(_id))
        {
            infoFollowText.text = "팔로우 취소";
            infoFollowBtn.gameObject.SetActive(true);

        }
        else
        {
            infoFollowText.text = "팔로우";
            infoFollowBtn.gameObject.SetActive(true);
        }
        
        if(_id==UserData.Instance.id)
            infoFollowBtn.gameObject.SetActive(false);


        User user = await GET_UserInfoAsync(_id);

        currentUserId = user.id;
        currentUserNickname = user.nickname;
        userNameText.text = user.GetName();
        followText.text = user.followNum + "\n팔로우";
        followerText.text = user.followerNum + "\n팔로워";
        musicCntText.text = "\n올린 음원";

        character.ChangeSprite(user.character);
        //GetuploadedMusicListAsync(user == UserData.Instance.user?null : user.id);//본인이면 null, 본인이 아니면 id

    }

    async void GetuploadedMusicListAsync(string userid)
    {
        MusicList ML = await GET_MusicListAsync("uploadList",false,userid);
        if (ML != null)
        {
            LoadUploadedSlots( ML.musicList);
        }
    }

    //업로드한 음원리스트를 표시하고 팔로우, 팔로워, 올린음원 수를 업데이트하는 함수
    void LoadUploadedSlots(List<Music> _musics)
    {

        if (_musics != null)
        {

            musicCntText.text = _musics.Count + "\n올린 음원";

            RemoveSlots();
            if (_musics.Count == 0)
            {//올린 음원이 없습니다.
                
                SetNoListObject( true);
            }
            else
            {
                SetNoListObject( false);
            }
            GameObject _obj = null;
            SongSlot slot;
            for (int i = 0; i <_musics.Count; i++)
            { 
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotNemoNonePlay") as GameObject,uploadedSongScrollViewObject.transform);

                slot = _obj.GetComponent<SongSlot>();
                slot.SetMusic(_musics[i]);


                uploadedSlots.Add(slot);


            }

            uploadedSongScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();

        }
    }

    async void CallFollowApi(string userID, string userName, bool isDelete = false)
    {
        await POST_FollowUserAsync(userID, userName, isDelete);

    }
    void RemoveSlots()
    {
        for (int i = 0; i < uploadedSlots.Count; i++)
        {
            Destroy(uploadedSlots[i].gameObject);
        }
        uploadedSlots.Clear();
       

    }
    void SetNoListObject( bool on)
    {       
        noListObjects.SetActive(on);
    }/*
    override public void Reset()
    {
        currentUserId[0] = "";
        currentUserId[1] = "";
        RemoveSlots();

    }*/
    private void OnDestroy()
    {
        //UserData.Instance.OnDeleteFollow -= SetInfoFollowTextOnDelete;
        //UserData.Instance.OnAddFollow -= SetInfoFollowTextOnAdd;
    }
}
