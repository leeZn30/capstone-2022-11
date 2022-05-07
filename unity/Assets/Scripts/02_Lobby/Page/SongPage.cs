using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongPage : Page
{

    public ListPageInSongPage listPage;
    public SearchPageInSongPage searchPage;
    public AddPageInSongPage addPage;
    public Button searchBtn;

    public Button addBtn;

    public GameObject popularScrollViewObject;
    public GameObject genreScrollViewObject;
    public GameObject recentScrollViewObject;

    private List<PlaySongSlot> popularSlots;
    private List<PlaySongSlot> genreSlots;
    private List<PlaySongSlot> recentSlots;

    public List<SongFolder> songFolderList;
    void Start()
    {
        Init();
    }
    override public void Init()
    {
        if (isAlreadyInit == false)
        {
            songFolderList = new List<SongFolder>(GetComponentsInChildren<SongFolder>());
            for (int i = 0; i < songFolderList.Count; i++)
            {
                songFolderList[i].OnClickButton_ += listPage.Open;
            }
            searchBtn.onClick.AddListener(searchPage.OpenSearchObject);
       
            addBtn.onClick.AddListener(addPage.Open);
            Debug.Log(gameObject.name + "open");
            isAlreadyInit = true;

            popularSlots = new List<PlaySongSlot>();
            genreSlots = new List<PlaySongSlot>();
            recentSlots = new List<PlaySongSlot>();

            
        }
    }
    
    void MusicListLoad()
    {
        GetSpecificMusicListAsync(SpecificMusic.popular);
        GetSpecificMusicListAsync(SpecificMusic.personalGenre);
        GetSpecificMusicListAsync(SpecificMusic.recent);
    }
    async void GetSpecificMusicListAsync(SpecificMusic type)
    {
        MusicList ML = await GET_SpecificMusicListAsync(type);
        if (ML != null)
        {
            LoadSlots(type, ML.musicList);
        }
    }
    void LoadSlots(SpecificMusic type, List<Music> _musics)
    {
        if (_musics != null)
        {
            RemoveSlots(type);
           
            GameObject _obj = null;
            PlaySongSlot slot;
            for (int i = 0; i <_musics.Count; i++)
            {
                if(type==SpecificMusic.popular)
                    _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotBasicPlayable") as GameObject,  popularScrollViewObject.transform);
                if (type == SpecificMusic.personalGenre)
                    _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotNemoPlayable") as GameObject,  genreScrollViewObject.transform);
                else if (type == SpecificMusic.recent)
                    _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotNemoPlayable") as GameObject, recentScrollViewObject.transform);

                slot = _obj.GetComponent<PlaySongSlot>();
                slot.SetMusic(_musics[i]);
                slot.OnClickSlot += SongClickHandler;


                if (type == SpecificMusic.popular)
                    popularSlots.Add(slot);
                if (type == SpecificMusic.personalGenre)
                    genreSlots.Add(slot);
                else if (type == SpecificMusic.recent)
                    recentSlots.Add(slot);


            }
            if (type == SpecificMusic.popular)
                popularScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();
            if (type == SpecificMusic.personalGenre)
                genreScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();
            else if (type == SpecificMusic.recent)
                recentScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();

        }
    }
    void RemoveSlots(SpecificMusic type)
    {
        if (type == SpecificMusic.popular)
        {
            for(int i= 0; i< popularSlots.Count; i++)
            {
                Destroy(popularSlots[i].gameObject);
            }
            popularSlots.Clear();
        }
        else if(type == SpecificMusic.personalGenre)
        {
            for (int i = 0; i < genreSlots.Count; i++)
            {
                Destroy(genreSlots[i].gameObject);
            }
            genreSlots.Clear();
        }
        else
        {
            for (int i = 0; i < recentSlots.Count; i++)
            {
                Destroy(recentSlots[i].gameObject);
            }
            recentSlots.Clear();
        }
        
    }
    void SongClickHandler(PlaySongSlot ss)
    {

    }
    override public void Load()
    {
        MusicListLoad();
    }

    override public void Reset()
    {
        listPage.Close();
        searchPage.Close();
        addPage.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
