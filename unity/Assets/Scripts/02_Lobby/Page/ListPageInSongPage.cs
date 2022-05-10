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
    public GameObject delCheckPanel;//이중 삭제 체크 판넬
    public Button delCancelBtn;
    public Button deldelBtn;

    [SerializeField]
    private List<SongSlot> songSlots;
    private ScrollViewRect scrollViewRect;

    private bool isEditMode=false;


    private TextMeshProUGUI editText;
    private GameObject editObject;

    private string listName;
    private SongSlot deleteCheckSlot;
    private void Update()
    {

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


            isEditMode = false;
            scrollViewRect = scrollViewObject.GetComponent<ScrollViewRect>();
            //musicList = new List<Music>();
            songSlots = new List<SongSlot>();
            editBtn.onClick.AddListener(OnEditMode);

            editText = editBtn.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            editObject = editBtn.gameObject.transform.GetChild(1).gameObject;

            delCancelBtn.onClick.AddListener(delegate { delCheckPanel.SetActive(false); });
            deldelBtn.onClick.AddListener(delegate { Delete(deleteCheckSlot); });
        }

    }

    void Delete(SongSlot ss)
    {
        if (ss == null) return;

        if (isEditMode == true)
        {
            if (listName == "uploadList")
            {
                if (delCheckPanel.activeSelf == false)
                {
                    deleteCheckSlot = ss;
                    delCheckPanel.SetActive(true);
                    return;
                }
                delCheckPanel.SetActive(false);

            }

            MusicID id = new MusicID();
            id.musicId = ss.GetMusic().id;
            MusicController.Instance.DelNewMusic(listName, songSlots.IndexOf(ss), ss.GetMusic());
            songSlots.Remove(ss);
            Destroy(ss.gameObject);
            
            StartCoroutine(POST_Delete(id, listName));
            
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
                songSlots[i].delBtn.gameObject.SetActive(false);
                /*
                if (songSlots[i].isSelected == true)
                {
                    songSlots[i].isSelected = false;
                }*/
            }
        }
        else
        {
            isEditMode = true;
            editText.text = "취소";
            for (int i = 0; i < songSlots.Count; i++)
            {
                songSlots[i].delBtn.gameObject.SetActive(true);
                /*
                if (songSlots[i].isSelected == true)
                {
                    songSlots[i].isSelected = false;
                }*/
            }
        }
        editObject.SetActive(isEditMode);

    }
    public void Open(string listName, string content)
    {//오버로딩
        Init();
        gameObject.SetActive(true);
        contentText.text = content;


        GetSongList(listName);
        MusicController.Instance.subMusicController.Reset();
    }

    async void GetSongList(string _listName)
    {//id에 따라 알맞은 재생목록을 불러오는 함수

        if (songSlots != null)//테스트용 코드
        {
            listName = _listName;
            MusicList ml= await GET_MusicListAsync(_listName);
            if (ml != null)
            {
                LoadSongList(ml.musicList, ml.play);
            }
        }
        
    }


    void LoadSongList(List<Music> _musicList=null,bool play=false)
    {
        if (_musicList != null || _musicList.Count==0)
        {

            songSlots.Clear();

            GameObject _obj = null;
            SongSlot songSlot;
            for (int i = 0; i < _musicList.Count; i++)
            {
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlot") as GameObject, scrollViewObject.transform);
                songSlot = _obj.GetComponent<SongSlot>();
                songSlot.SetMusic(_musicList[i]);
                songSlot.OnDeleteButtonClick += Delete;
                songSlot.OnClickSlot += SongClickHandler;
                songSlots.Add(songSlot);
                

            }
            scrollViewRect.SetContentSize(100);
        }

    }
    private void SongClickHandler(SongSlot ss)
    {
        MusicController.Instance.StartGetListCoroution(listName, songSlots.IndexOf(ss), true);
    }
    override public void Reset()
    {
        Debug.Log("list reset");
        //재생목록 초기화
        Init();

        //musicList.Clear();
        songSlots.Clear();

        SongSlot[] childList = scrollViewObject.GetComponentsInChildren<SongSlot>();
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
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
