using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchPageInSongPage : Page
{
    public Button searchBtn;
    public TMP_InputField searchField;
    public GameObject scrollViewObject;
    private List<SearchedSongSlot> searchedSlots;
    private List<Music> currentMusics;

    private ScrollViewRect scrollViewRect;
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
            scrollViewRect = scrollViewObject.GetComponent<ScrollViewRect>();
            currentMusics = new List<Music>();
            searchedSlots = new List<SearchedSongSlot>();
            searchBtn.onClick.AddListener(Search);
            MusicWebRequest.Instance.OnSearched += LoadSongs;
        }
    }
    override public void Load()
    {//오버라이딩
        LoadSongs();
        Debug.Log("search Page Load");
    }
    void LoadSongs(List<Music> _musics = null)
    {
        if (_musics != null)
        {
            currentMusics = _musics;
            

            GameObject _obj = null;
            SearchedSongSlot _searchedSlot;
            for (int i=0; i < currentMusics.Count; i++)
            {
                Debug.Log(currentMusics[i].id);
                _obj = Instantiate(Resources.Load("Prefabs/searchedSlot") as GameObject,scrollViewObject.transform);
                _searchedSlot = _obj.GetComponent<SearchedSongSlot>();
                _searchedSlot.SetMusic(currentMusics[i]);
                searchedSlots.Add(_searchedSlot);
                
            }
            scrollViewRect.SetContentSize(100);
        }
    }
    public void Search()
    {
        string searchText = searchField.text;
        Reset();
        MusicWebRequest.Instance.SearchTitle(searchText);
    }
    override public  void Reset()
    {//오버라이딩
        Debug.Log("search Page Reset");

        Init();

        currentMusics.Clear();
        searchedSlots.Clear();
        searchField.text = "";

        Transform[] childList = scrollViewObject.GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for(int i=1; i<childList.Length; i++)
            {
               if(childList[i] != transform)
                {
                    Destroy(childList[i].gameObject);
                }
            }
        }
    }
}
