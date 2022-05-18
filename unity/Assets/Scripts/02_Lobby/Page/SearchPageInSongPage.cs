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

    public Button typeBtn;
    public Button clearBtn;

    public TextMeshProUGUI searchTypeText;
    public TMP_InputField searchField;
    public GameObject scrollViewObject;

    public TMP_Dropdown listNameDropdown;

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
            typeBtn.onClick.AddListener(delegate
            {
               
                if (searchTypeText.text=="제목 검색")
                {
                    searchTypeText.text = "가수 검색";
                }
                else if(searchTypeText.text == "가수 검색")
                {
                    searchTypeText.text = "장르 검색";
                }
                else if (searchTypeText.text == "장르 검색")
                {
                    searchTypeText.text = "제목 검색";
                }
            });
            searchField.onSubmit.AddListener(delegate {
               
                SearchAsync(searchTypeText.text=="제목 검색"?"title":(searchTypeText.text == "가수 검색"?"artist":"category")); 
            });

        }
    }
    private void OnOffBtnObject()
    {
        if (selectedSlots == null) return;
        SetOptions();
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
        if (listNameDropdown.options.Count == 0) return;
        MusicIDList iDList = new MusicIDList();
        iDList.musicList = new List<string>();

        List<Music> ms = new List<Music>();
        for (int i = 0; i < selectedSlots.Count; i++)
        {

            selectedSlots[i].isSelected = false;
            iDList.musicList.Add(selectedSlots[i].GetMusic().id);
            ms.Add(selectedSlots[i].GetMusic());
        }
        selectedSlots.Clear();
        OnOffBtnObject();

        StartCoroutine(POST_AddMusic(iDList,listNameDropdown.options[listNameDropdown.value].text));
        MusicController.Instance.AddNewMusics(listNameDropdown.options[listNameDropdown.value].text, ms);

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
            Remove();
            currentMusics = _musics;
            

            GameObject _obj = null;
            PlaySongSlot _searchedSlot;
            for (int i=0; i < currentMusics.Count; i++)
            {
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SearchedSlot") as GameObject,scrollViewObject.transform);
                _searchedSlot = _obj.GetComponent<PlaySongSlot>();
                _searchedSlot.SetMusic(currentMusics[i]);
                _searchedSlot.OnClickSlot += SongClickHandler;
                searchedSlots.Add(_searchedSlot);
                
            }
            OnOffBtnObject();
            scrollViewRect.SetContentSize(100);
        }
    }
    private void SongClickHandler(PlaySongSlot ss)
    {
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
    async void SearchAsync(string type)
    {

        //Remove();
        searchedObj.SetActive(true);
        MusicList m = await GET_SearchMusicTitleAsync(type, searchField.text);
        if (m != null)
            LoadSongs(m.musicList);
        //StartCoroutine(GET_SearchMusicTitle(searchField.text));
    }
    public void SetOptions()
    {

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>(MusicController.Instance.dropdown.options);
        options.RemoveAt(0);
        listNameDropdown.options = options;
        if (listNameDropdown.options.Count != 0)
        {

            listNameDropdown.value = MusicController.Instance.dropdown.value;

        }

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
