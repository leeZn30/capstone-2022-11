using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FollowPage : Page
{
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI followerText;
    public TextMeshProUGUI followText;
    public TextMeshProUGUI musicCntText;
    public TMP_InputField searchField;
    public Character character;

    public Button myInfoBtn;
    public Button infoFollowBtn;

    public Button[] followBtns;

    public GameObject uploadedSongScrollViewObject;
    private List<PlaySongSlot> uploadedSlots;

    public GameObject followUserScrollViewObject;
    public GameObject searchedUserScrollViewObject;


    private List<UserSlot> followUserSlots;
    private List<UserSlot> searchedUserSlots;
    private string[] currentUserId;
    private TextMeshProUGUI infoFollowText;//팔로우 취소, 팔로우 하기 text

    private FollowSystemType FL;//follow를 보여줄건지, follower보여줄건지
    public enum FollowSystemType
    {
        uploadList,follow,follower,searched
    }
    private void Start()
    {
        Init();
    }
    override public void Init()
    {
        if (isAlreadyInit == false)
        {

            isAlreadyInit = true;
            currentUserId = new string[2];

            uploadedSlots = new List<PlaySongSlot>(uploadedSongScrollViewObject.GetComponentsInChildren<PlaySongSlot>());
            followUserSlots = new List<UserSlot>(followUserScrollViewObject.GetComponentsInChildren<UserSlot>());
            searchedUserSlots = new List<UserSlot>(searchedUserScrollViewObject.GetComponentsInChildren<UserSlot>());

            infoFollowText = infoFollowBtn.transform.GetComponentInChildren<TextMeshProUGUI>();

            searchField.onSubmit.AddListener(delegate {
                
                GetUserListAsync(FollowSystemType.searched);
            
            });
            //내 정보 보기 버튼
            myInfoBtn.onClick.AddListener(delegate {
                LoadUserProfile();
            });
            //내 팔로워 버튼
            followBtns[0].onClick.AddListener(delegate {
                followBtns[0].image.color = new Color(1f, 1f, 1f, 1f);
                followBtns[1].image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                GetUserListAsync(FollowSystemType.follower);
  
            });
            //내 팔로우 버튼
            followBtns[1].onClick.AddListener(delegate {
                followBtns[1].image.color = new Color(1f, 1f, 1f, 1f);
                followBtns[0].image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                GetUserListAsync(FollowSystemType.follow);
            });

            infoFollowBtn.onClick.AddListener(delegate{
                if (infoFollowText.text == "팔로우")
                {//팔로우
                    FollowUser(currentUserId[0], currentUserId[1]);
                }
                else
                {//팔로우 취소
                    FollowCancelUser(currentUserId[0], currentUserId[1]);
                }
            });
            //팔로우가 취소되었을때
            UserData.Instance.OnDeleteFollow += SetInfoFollowTextOnDelete;

            //팔로우가 추가되었을 때
            UserData.Instance.OnAddFollow += SetInfoFollowTextOnAdd;



        }
    }
    void SetInfoFollowTextOnDelete(string str)
    {
        if (str == currentUserId[0])
            infoFollowText.text = "팔로우";

    }
    void SetInfoFollowTextOnAdd(string str)
    {
        if (str == currentUserId[0])
            infoFollowText.text = "팔로우 취소";
    }
    //유저 프로필을 로드하는 함수
    async void LoadUserProfile(User user=null, bool isFollow=false)
    {
        if (user != null)
        {
            if (currentUserId[0] == user.id)
                return;
            //현재 보여지는 유저의 id를 새로 설정
            currentUserId[0] = user.id;
            currentUserId[1] = user.nickname;//팔로우 삭제용

            if (isFollow == true)
            {//내가 이미 팔로우한 유저라면
                infoFollowText.text = "팔로우 취소";
                infoFollowBtn.gameObject.SetActive(true);
                
            }
            else
            {//내가 팔로우하지않은 유저라면
                infoFollowText.text = "팔로우";
                infoFollowBtn.gameObject.SetActive(true);
            }
        }
        else if (user == null)
        {   //null일때 본인의 프로필을 표시.
            //현재 유저 아이디를 본인의 아이디로 바꿈.
            currentUserId[0] = UserData.Instance.user.id;
            currentUserId[1] = UserData.Instance.user.nickname;
            user = UserData.Instance.user;
            //팔로우 버튼을 비활성화
            infoFollowBtn.gameObject.SetActive(false);
        }

        if (user.preferredGenres.Count == 0)
        {//정보를 받아오지 않은 유저의 정보를 로드할 때
            user = await GET_UserInfoAsync(user.id);
            if (user == null) return;
        }


        userNameText.text = user.GetName();
        followText.text = user.followNum + "\n팔로우";
        followerText.text = user.followerNum + "\n팔로워";
        musicCntText.text = "\n올린 음원";

        character.ChangeSprite(user.character);
        GetuploadedMusicListAsync(user == UserData.Instance.user?null : user.id);//본인이면 null, 본인이 아니면 id

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

            RemoveSlots(FollowSystemType.uploadList);
            if (_musics.Count == 0)
            {//올린 음원이 없습니다.
              
            }
            GameObject _obj = null;
            PlaySongSlot slot;
            for (int i = 0; i <_musics.Count; i++)
            { 
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotNemoPlayable") as GameObject,uploadedSongScrollViewObject.transform);

                slot = _obj.GetComponent<PlaySongSlot>();
                slot.SetMusic(_musics[i]);


                uploadedSlots.Add(slot);


            }

            uploadedSongScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();

        }
    }
    async void GetUserListAsync(FollowSystemType type)
    {
        UserList UL;
        if (type == FollowSystemType.searched)
        {
            UL = await GET_SearchUserAsync(searchField.text);
        }
        else
        {
            UL = await GET_FollowSystemUserListAsync(type);
        }

        if (UL != null)
        {
            LoadUserSlots(type,UL.userList);
        }
    }

    //유저 슬롯의 타입별로 유저리스트를 알맞은 슬롯에 로드하는 함수
    void LoadUserSlots(FollowSystemType type, List<User> _users)
    {
        if (_users != null)
        {
            //기존 슬롯을 지움
            RemoveSlots(type);

            GameObject _obj = null;
            UserSlot slot;

            for (int i = 0; i < _users.Count; i++)
            {
                //유저 본인 정보는 로드하지 않음
                if (_users[i].id == UserData.Instance.id) continue;

                if(type==FollowSystemType.searched)
                    _obj = Instantiate(Resources.Load("Prefabs/SearchedUserSlot") as GameObject, searchedUserScrollViewObject.transform);
                else
                    _obj = Instantiate(Resources.Load("Prefabs/UserSlot") as GameObject, followUserScrollViewObject.transform);
            
                
                slot = _obj.GetComponent<UserSlot>();
                if(slot==null) Debug.Log(_users[i].id);
         
                slot.SetUser(_users[i]);
                slot.SetType(type);

                //팔로우하고 있는 유저라면 슬롯에서 팔로우 처리하고 팔로우 버튼을 비활성화 시킴
                if (UserData.Instance.user.follow.Contains(_users[i].id))
                {
                    slot.Follow = true;
                }

                slot.OnClickAddButton += FollowUser;
                slot.OnClickDelButton += FollowCancelUser;
                slot.OnClickSlot += UserSlotClickHandler;


                if (type == FollowSystemType.searched)
                {
                    searchedUserSlots.Add(slot);
                }
                else
                {
                    followUserSlots.Add(slot);
                }

            }
            if (type == FollowSystemType.searched)
                searchedUserScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();
            else
                followUserScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();
        }
    }
    void FollowUser(UserSlot us)
    {
        //유저슬롯 팔로우 버튼 클릭시
        CallFollowApi(us.user.id, us.user.nickname);
        UserData.Instance.AddFollow(us.user.id);

    }
    void FollowUser(string id, string nickname)
    {
        //유저슬롯 팔로우 버튼 클릭시
        CallFollowApi(id, nickname);
        UserData.Instance.AddFollow(id);

    }
    void FollowCancelUser(UserSlot us)
    {
        //유저슬롯 팔로우 취소 버튼 클릭시
        CallFollowApi(us.user.id, us.user.nickname, true);
        UserData.Instance.DelFollow(us.user.id);
    }
    void FollowCancelUser(string id, string nickname)
    {
        //유저슬롯 팔로우 취소 버튼 클릭시
        CallFollowApi(id, nickname, true);
        UserData.Instance.DelFollow(id);
    }
    async void CallFollowApi(string userID, string userName, bool isDelete = false)
    {
        await POST_FollowUserAsync(userID, userName, isDelete);
        if (followBtns[1].image.color.r == 1.0f)
        {   //팔로우 리스트가 보이고 있을 때
            //팔로우 리스트 업데이트
            GetUserListAsync(FollowSystemType.follow);
        }

        if(currentUserId[0]==UserData.Instance.user.id)
            followText.text = UserData.Instance.user.follow.Count + "\n팔로우";

    }
    void UserSlotClickHandler(UserSlot us)
    {
        //유저 슬롯 클릭시
        LoadUserProfile(us.user,us.Follow);
    }
    void RemoveSlots(FollowSystemType type)
    {
        if (type == FollowSystemType.uploadList)
        {//업로드 리스트
            for (int i = 0; i < uploadedSlots.Count; i++)
            {
                Destroy(uploadedSlots[i].gameObject);
            }
            uploadedSlots.Clear();
        }
        else if (type == FollowSystemType.searched)
        {//검색 리스트
            for (int i = 0; i < searchedUserSlots.Count; i++)
            {
                Destroy(searchedUserSlots[i].gameObject);
            }
            searchedUserSlots.Clear();
        }
        else
        {
            //팔로우,팔로워 리스트
            for (int i = 0; i < followUserSlots.Count; i++)
            {
                Destroy(followUserSlots[i].gameObject);
            }
            followUserSlots.Clear();
        }

    }
    override public void Load()
    {
        LoadUserProfile();//내 프로필 로드
        followBtns[0].image.color = new Color(1f, 1f, 1f, 1f);
        followBtns[1].image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        GetUserListAsync(FollowSystemType.follower);
    }
    override public void Reset()
    {
        currentUserId[0] = "";
        currentUserId[1] = "";
        RemoveSlots(FollowSystemType.searched);
        RemoveSlots(FollowSystemType.follow);
        RemoveSlots(FollowSystemType.uploadList);
    }
    private void OnDestroy()
    {

        //UserData.Instance.OnDeleteFollow -= SetInfoFollowTextOnDelete;
        //UserData.Instance.OnAddFollow -= SetInfoFollowTextOnAdd;
    }
}
