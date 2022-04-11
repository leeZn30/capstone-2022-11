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

    public Button clearBtn;
    public TMP_InputField searchField;
    public GameObject scrollViewObject;
    private List<SongSlot> searchedSlots;
    private List<Music> currentMusics;

    private ScrollViewRect scrollViewRect;
    private GraphicRaycaster gr;
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
            searchedSlots = new List<SongSlot>();
            clearBtn.onClick.AddListener(delegate { searchField.text = ""; });
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
            SongSlot _searchedSlot;
            for (int i=0; i < currentMusics.Count; i++)
            {
                Debug.Log(currentMusics[i].id);
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SearchedSlot") as GameObject,scrollViewObject.transform);
                _searchedSlot = _obj.GetComponent<SongSlot>();
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
