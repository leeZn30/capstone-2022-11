using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicZoneUI : MusicWebRequest
{
    private AudioSource audioSource;

    private MusicSpot currentMusicSpot;

    public MusicZoneInputData musicZoneData;
    public MusicZoneOutputData musicZoneOutputData;
    private int currentMusicIndex=0;

    //듣기 창
    public GameObject listeningUI;
    public TextMeshProUGUI currentMusicTitleText;


    //설정 창
    public GameObject setUI;
    public TextMeshProUGUI selectedListNameText;
    public GameObject songFolderParent;
    public GameObject songSlotParent;

    private List<SongFolder> songFolderList;
    private List<SongSlot> songSlotList;


    public TextMeshProUGUI totalTimeText;

    public Button setBtn;
    public Button exitBtn;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        songSlotList = new List<SongSlot>(songSlotParent.GetComponentsInChildren<SongSlot>());
        songFolderList = new List<SongFolder>(songFolderParent.GetComponentsInChildren<SongFolder>());
        
        RemoveSongSlot();
        RemoveSongFolder();

        LoadSongFolder();

        exitBtn.onClick.AddListener(CloseSetUI);
        setBtn.onClick.AddListener(CallCreateZone);
        musicZoneData = new MusicZoneInputData();
        musicZoneOutputData = new MusicZoneOutputData();
    }

    public async void CallJoinZone(MusicSpot _musicSpot)
    {
        if (setUI.activeSelf)
        {//등록창 켜져있다면 close;
            CloseSetUI();
        }
        currentMusicSpot = _musicSpot;
        MusicZoneNumber zoneNumber = new MusicZoneNumber();
        zoneNumber.zoneNumber = _musicSpot.spotNum;
        MusicZoneOutputData mz = await GET_JoinZone(zoneNumber);

        if (mz != null)
        {
            
            OpenListeningUI( mz);
            currentMusicSpot.state = MusicSpot.State.Listening;
        }
        else
        {
            currentMusicSpot.state = MusicSpot.State.None;
        }
    }
    async void CallCreateZone()
    {//음원존 음원 등록
        if (musicZoneData == null) return;
        if (musicZoneData.titleList.Count == 0) return;

        bool isDone = await POST_CreateZone(musicZoneData);
        if (isDone)
        {
            currentMusicSpot.state = MusicSpot.State.Listening;
            currentMusicSpot.RPCJoin();

        }
        else
        {
            
        }
    }
    public void OpenListeningUI(MusicZoneOutputData _musicZone)
    {
        currentMusicIndex = 0;

        musicZoneOutputData = _musicZone;
        currentMusicTitleText.text= "♬ 재생 준비 중 ♪";

        GetAudioAsync(currentMusicIndex, musicZoneOutputData.time);
        listeningUI.SetActive(true);

    }

    public void OpenSetUI(MusicSpot _musicSpot)
    {
        currentMusicSpot = _musicSpot;
        musicZoneData.zoneNumber = _musicSpot.spotNum;
        musicZoneData.titleList.Clear();
        musicZoneData.timeList.Clear();
        musicZoneData.locateList.Clear();

        RemoveSongSlot();
        selectedListNameText.text = "";
        totalTimeText.text = "재생 시간 :\n";

        setUI.SetActive(true);
    }

    void LoadSongFolder()
    {
        foreach (string name in UserData.Instance.user.listName)
        {
            SongFolder sf;
            GameObject obj = Instantiate(Resources.Load("Prefabs/SongFolderNonDel") as GameObject, songFolderParent.transform);
            sf = obj.GetComponent<SongFolder>();
            sf.SetData(name);
            sf.OnClickButton_ += LoadSongSlot;

            songFolderList.Add(sf);
        }
    }
    async void LoadSongSlot(string _listName,string str)
    {
        if (songSlotList != null)
        {
            selectedListNameText.text = _listName;
            MusicList ml = await GET_MusicListAsync(_listName);
            if (ml != null)
            {
                if (ml.musicList != null)
                {

                    RemoveSongSlot();


                    GameObject _obj = null;
                    SongSlot songSlot;
                    float totalTime = 0;
                    for (int i = 0; i < ml.musicList.Count; i++)
                    {
                        _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotBasic") as GameObject, songSlotParent.transform);
                        songSlot = _obj.GetComponent<SongSlot>();
                        songSlot.SetMusic(ml.musicList[i]);
                        songSlotList.Add(songSlot);

                        totalTime += ml.musicList[i].time;

                        musicZoneData.timeList.Add(ml.musicList[i].time);
                        musicZoneData.locateList.Add(ml.musicList[i].locate);
                        musicZoneData.titleList.Add(ml.musicList[i].title);
                    }
                    songSlotParent.GetComponent<ScrollViewRect>().SetContentSize(100);
                    totalTimeText.text="재생 시간 :\n"+((int)totalTime)/60+":"+(int)totalTime%60;
                }
            }
        }
    }
    void RemoveSongSlot()
    {
        musicZoneData.timeList.Clear();
        musicZoneData.locateList.Clear();
        musicZoneData.titleList.Clear();

        for (int i = 0; i < songSlotList.Count; i++)
        {
            Destroy(songSlotList[i].gameObject);
        }
        songSlotList.Clear();
    }
    void RemoveSongFolder()
    {
        for (int i = 0; i < songFolderList.Count; i++)
        {
            Destroy(songFolderList[i].gameObject);
        }
        songFolderList.Clear();
    }
    public void CloseSetUI()
    {
        setUI.SetActive(false);
        ClearData();
    }
    public void CloseListeningUI()
    {
        listeningUI.SetActive(false);
        StopAllCoroutines();
        ClearData();

    }
    async void GetAudioAsync(int index, float time = 0)
    {

        if (getAudioWWW != null)
        {
            getAudioWWW.Dispose();
            //StopCoroutine(audioLoadIEnum);
        }
        AudioClipPlay ac = await GetAudioClipAsync(musicZoneOutputData.locateList[index], true);

        if (ac != null)
        { 
            audioSource.clip = ac.audioClip;
            currentMusicTitleText.text = "♬ 현재 재생 중인 음악 - " + musicZoneOutputData.titleList[index] + "  ♪";
            audioSource.time = time;
            audioSource.Play();
            StartCoroutine(StartListening());
        }

    }
    public void ClearData()
    {
        audioSource.Stop();
        audioSource.clip=null;
        musicZoneData.locateList.Clear();
        musicZoneData.timeList.Clear();
        musicZoneData.titleList.Clear();
        musicZoneData.zoneNumber = 0;


        musicZoneOutputData.locateList.Clear();
        musicZoneOutputData.titleList.Clear();
        musicZoneOutputData.time = 0;
        currentMusicIndex = 0;
    }
    public void CleatSpotData()
    {
        currentMusicSpot = null;
    }
    IEnumerator StartListening()
    {
        if (musicZoneOutputData.titleList == null) yield break;

        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(1.0f);
        }

        if(++currentMusicIndex >= musicZoneOutputData.titleList.Count)
        {//재생이 끝났을 때
            CloseListeningUI();
            currentMusicSpot.state = MusicSpot.State.None;
        }
        else
        {
            GetAudioAsync(currentMusicIndex);
        }


    }
}
