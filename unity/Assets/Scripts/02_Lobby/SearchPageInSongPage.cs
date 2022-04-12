using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SearchPageInSongPage : Page
{

    public GameObject searchObj;
    public GameObject searchedObj;
    public GameObject btnsObj;

    public Button putBtn;
    public Button cancelBtn;

    public Button clearBtn;
    public TMP_InputField searchField;
    public GameObject scrollViewObject;
    private List<PlaySongSlot> searchedSlots;
    private List<Music> currentMusics;

    private ScrollViewRect scrollViewRect;
    private GraphicRaycaster gr;

    private List<PlaySongSlot> selectedSlots;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);

            if (results.Count <= 0) return;
            // 이벤트 처리부분
            if (results[0].gameObject.name != "SearchedSlot(Clone)") return;

            PlaySongSlot ss = results[0].gameObject.GetComponent<PlaySongSlot>();
            ss.isSelected = !ss.isSelected;

            if (ss.isSelected == true)
            {
                selectedSlots.Add(ss);
            }
            else
            {
                selectedSlots.Remove(ss);
            }
            OnOffBtnObject();

        }
    }


    override public void Init()
    {//오버라이딩
        if(isAlreadyInit==false)
        {
            isAlreadyInit = true;

            gr = GetComponent<GraphicRaycaster>();
            selectedSlots = new List<PlaySongSlot>();
            scrollViewRect = scrollViewObject.GetComponent<ScrollViewRect>();
            currentMusics = new List<Music>();
            searchedSlots = new List<PlaySongSlot>();
            clearBtn.onClick.AddListener(delegate { searchField.text = ""; });
            putBtn.onClick.AddListener(PutSelect);
            cancelBtn.onClick.AddListener(CancelSelect);

            OnOffBtnObject();

            clearBtn.gameObject.SetActive(false);
            searchField.text = "";
            searchField.onValueChanged.AddListener(delegate {
                if (searchField.text.Length <= 0)
                {
                    clearBtn.gameObject.SetActive(false);
                }
                else
                {
                    clearBtn.gameObject.SetActive(true);
                }
                    });
            searchField.onSubmit.AddListener(delegate { Search(); });
            OnGetSongList += LoadSongs;
        }
    }
    private void OnOffBtnObject()
    {
        if (selectedSlots.Count > 0)
        {
            btnsObj.SetActive(true);
        }
        else
        {
            btnsObj.SetActive(false);
        }
    
    }
    private void PutSelect()
    {
        MusicIDList iDList = new MusicIDList();
        iDList.musicList = new List<string>();
        for (int i = 0; i < selectedSlots.Count; i++)
        {
            selectedSlots[i].isSelected = false;
            iDList.musicList.Add(selectedSlots[i].GetMusic().id);
        }
        selectedSlots.Clear();
        OnOffBtnObject();
        StartCoroutine(POST_AddMyList(iDList));
    }
    private void CancelSelect()
    {
        for(int i=0; i<selectedSlots.Count; i++)
        {
            selectedSlots[i].isSelected=false;
        }
        selectedSlots.Clear();
        OnOffBtnObject();
    }
    public void OpenSearchObject()
    {
        Open();
        searchObj.SetActive(true);
    }
    override public void Load()
    {//오버라이딩
        searchField.text = "";
        LoadSongs();
        Debug.Log("search Page Load");
    }
    void LoadSongs(List<Music> _musics = null,bool play=false)
    {
        if (_musics != null)
        {
            currentMusics = _musics;
            

            GameObject _obj = null;
            PlaySongSlot _searchedSlot;
            for (int i=0; i < currentMusics.Count; i++)
            {
                Debug.Log(currentMusics[i].id);
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SearchedSlot") as GameObject,scrollViewObject.transform);
                _searchedSlot = _obj.GetComponent<PlaySongSlot>();
                _searchedSlot.SetMusic(currentMusics[i]);
                searchedSlots.Add(_searchedSlot);
                
            }
            scrollViewRect.SetContentSize(100);
        }
    }
    public void Search()
    {

        Remove();
        searchedObj.SetActive(true);
        StartCoroutine(GET_SearchMusicTitle(searchField.text));
    }
  
    void Remove()
    {
        currentMusics.Clear();
        searchedSlots.Clear();
        selectedSlots.Clear();

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
    }
    override public  void Reset()
    {//오버라이딩
        Debug.Log("search Page Reset");

        Init();
        Remove();
        searchedObj.SetActive(false);
        searchObj.SetActive(false);


    }

}
