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
    public FolderAddPage folderAddPage;

    public Button searchBtn;
    public Button addBtn;

    public Button folderAddBtn;
    public Button folderDelBtn;

    public GameObject popularScrollViewObject;
    public GameObject genreScrollViewObject;
    public GameObject recentScrollViewObject;

    private List<PlaySongSlot> popularSlots;
    private List<PlaySongSlot> genreSlots;
    private List<PlaySongSlot> recentSlots;

    public List<SongFolder> songFolderList;

    public GameObject songFolderParent;
    private bool deleteMode = false;

    void Start()
    {
        Init();
    }
    override public void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;

            deleteMode = false;
            songFolderList = new List<SongFolder>(GetComponentsInChildren<SongFolder>());
            searchBtn.onClick.AddListener(searchPage.OpenSearchObject);
       
            addBtn.onClick.AddListener(addPage.Open);
            folderAddBtn.onClick.AddListener(folderAddPage.Open);
            folderDelBtn.onClick.AddListener(delegate { ChangeModeDeleteFolder(!deleteMode); });


            folderAddPage.OnMakeFolder += AddMusicFolder;

            popularSlots = new List<PlaySongSlot>(popularScrollViewObject.GetComponentsInChildren<PlaySongSlot>());  
            genreSlots = new List<PlaySongSlot>(genreScrollViewObject.GetComponentsInChildren<PlaySongSlot>());
            recentSlots = new List<PlaySongSlot>(recentScrollViewObject.GetComponentsInChildren<PlaySongSlot>());

            
        }
    }
    void LoadMusicFolder()
    {
        if (songFolderList == null) return;

        for (int i = 0; i < songFolderList.Count; i++)
        {
            songFolderList[i].OnClickButton_ -= listPage.Open;
            songFolderList[i].OnDelete -= DelMusicFolder;
            Destroy(songFolderList[i].gameObject);
        }
        songFolderList.Clear();
        //오류 생길 수 있음.
        //이전 단계에서 dropdown이 로드되지않았을 경우. 출력안됨.
        foreach (TMP_Dropdown.OptionData optionData in MusicController.Instance.dropdown.options)
        {
            AddMusicFolder(optionData.text);
        }
    }
    void AddMusicFolder(string value)
    {
        if (songFolderList == null) return;

        SongFolder sf;
        GameObject obj = Instantiate(Resources.Load("Prefabs/SongFolder") as GameObject, songFolderParent.transform);
        sf = obj.GetComponent<SongFolder>();
        sf.SetData(value);
        sf.OnClickButton_ += listPage.Open;
        sf.OnDelete += DelMusicFolder;

        songFolderList.Add(sf);
        Debug.Log(value);
    }
    void DelMusicFolder(SongFolder sf)
    {
        if (songFolderList == null) return;
        if (sf == null) return;

        CallPostDeleteFolder(sf.listName);
        if (MusicController.Instance.currentListName == sf.listName)
        {
            MusicController.Instance.StartGetListCoroution("uploadList", 0, false);
        }

        songFolderList.Remove(sf);
        sf.OnClickButton_-= listPage.Open;
        sf.OnDelete -= DelMusicFolder;
        Destroy(sf.gameObject);


    }
    async void CallPostDeleteFolder(string name)
    {
        StringList sl= await POST_DeleteListAsync(name);
        if (sl != null)
        {
            MusicController.Instance.SetOptions(sl.stringList);
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
                {
                    slot.SetRank(i + 1);
                    popularSlots.Add(slot);
                }
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
    void ChangeModeDeleteFolder(bool isOn)
    {
        deleteMode = isOn;
        folderDelBtn.GetComponentInChildren<TextMeshProUGUI>().text = isOn ? "삭제 취소" : "목록 삭제";    
        for(int i=0; i<songFolderList.Count; i++)
        {
            songFolderList[i].SetActiveDelButton(isOn);
        }
    }
    void SongClickHandler(PlaySongSlot ss)
    {

    }

    override public void Load()
    {
        LoadMusicFolder();
        MusicListLoad();
    }

    override public void Reset()
    {
        listPage.Close();
        searchPage.Close();
        addPage.Close();
        folderAddPage.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
