using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ListPageInSongPage : Page
{

    public TextMeshProUGUI contentText;

    public Button editBtn;
    public Button delBtn;
    public Button putBtn;
    public GameObject scrollViewObject;

    
    private List<SongSlot> songSlots;
    [SerializeField]
    public List<Music> musicList;

    private ScrollViewRect scrollViewRect;

    private bool isEditMode=false;
    private GraphicRaycaster gr;

    private TextMeshProUGUI editText;
    private GameObject editObject;


    private void Update()
    {
        if (isEditMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ped = new PointerEventData(null);
                ped.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                gr.Raycast(ped, results);

                if (results.Count <= 0) return;
                // 이벤트 처리부분
                if (results[0].gameObject.name != "SongSlot(Clone)") return;

                SongSlot ss = results[0].gameObject.GetComponent<SongSlot>();
                ss.isSelected = !ss.isSelected;
            }
            
        }

    }
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;

            gr = GetComponent<GraphicRaycaster>();
            isEditMode = false;
            scrollViewRect = scrollViewObject.GetComponent<ScrollViewRect>();
            //musicList = new List<Music>();
            songSlots = new List<SongSlot>();
            editBtn.onClick.AddListener(OnEditMode);
            delBtn.onClick.AddListener(DeleteMusic);
            editText = editBtn.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            editObject = editBtn.gameObject.transform.GetChild(1).gameObject;

           OnGetSongList += LoadSongList;
        }

    }

    void DeleteMusic()
    {
        if (isEditMode == true)
        {
            isEditMode = false;
            for (int i=0; i< songSlots.Count; i++)
            {
                if (songSlots[i].isSelected == true)
                {

                }
            }
        }
    }
    void OnEditMode()
    {
        if (isEditMode==true)
        {
            isEditMode = false;
            editText.text = "편집";
            for(int i=0; i<songSlots.Count; i++)
            {
                if (songSlots[i].isSelected == true)
                {
                    songSlots[i].isSelected = false;
                }
            }
        }
        else
        {
            isEditMode = true;
            editText.text = "취소";
        }
        editObject.SetActive(isEditMode);

    }
    public void Open(string listName, string content)
    {//오버로딩
        Init();
        gameObject.SetActive(true);
        contentText.text = content;


        GetSongList(listName);

    }

    void GetSongList(string _listName)
    {//id에 따라 알맞은 재생목록을 불러오는 함수

        if (musicList != null)//테스트용 코드
        {
            GetMusicList(_listName, UserData.Instance.id);
        }
        
    }


    void LoadSongList(List<Music> _musicList=null,bool play=false)
    {
        if (_musicList != null || _musicList.Count==0)
        {
            musicList = _musicList;


            GameObject _obj = null;
            SongSlot songSlot;
            for (int i = 0; i < musicList.Count; i++)
            {
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot") as GameObject, scrollViewObject.transform);
                songSlot = _obj.GetComponent<SongSlot>();
                songSlot.SetMusic(musicList[i]);
                songSlots.Add(songSlot);

            }
            scrollViewRect.SetContentSize(100);
        }

    }

    override public void Reset()
    {
        Debug.Log("list reset");
        //재생목록 초기화
        Init();

        //musicList.Clear();
        songSlots.Clear();

        Transform[] childList = scrollViewObject.GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                {
                    Destroy(childList[i].gameObject);
                }
            }
        }
        //초기화
        contentText.text = "";
        editText.text = "편집";
        editObject.SetActive(false);
        isEditMode = false;
    }
}
