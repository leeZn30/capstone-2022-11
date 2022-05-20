using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using Unity.Jobs;
using Unity.Collections;

public class MusicController : MusicWebRequest
{
    
    //싱글톤 패턴 적용
    private static MusicController instance;
    public static MusicController Instance
    {
        get
        {
            return GetInstance();
        }
    }
    public static MusicController GetInstance()
    {
        if (instance == null)
        {
            instance = (MusicController)FindObjectOfType(typeof(MusicController));

        }
        return instance;
    }

    private AudioSource audioSource;
    public SubMusicController subMusicController;

    public PointObject mainControllBar;
    public PointObject volumeToggleObj;
    private Toggle volumeToggle;
    public Slider volumeSlider;

    public TMP_Dropdown dropdown;
    public Button openBtn;
    public Image[] images;
    public Button[] pauseplayBtns;
    public Button[] prevBtns;
    public Button[] nextBtns;

    public TextMeshProUGUI[] titleTexts;
    public TextMeshProUGUI[] artistTexts;

    public CustomSlider slider;

    public Toggle randomToggle;
    public Button repeatBtn;
    public TextMeshProUGUI contentText;

    public GameObject[] noListObj;
    public GameObject[] yesListObj;

    private Animator animator;
    private AudioClip audioClip;

    private bool isAlreadyInit = false;
    private bool isRandomMode = false;
    private RepeatMode repeatMode;

    //public  List<Music> musicList;

    public string currentListName;//현재 선택된 재생목록
    public int currentSongIndex = 0;
    public int tmpSongIndex = 0;

    private Image[] pauseplayBtnImage;
    private Image repeatBtnImage;

    private IEnumerator enumerator;
    private bool isCurrentSongFinish=false;
    private bool isCleanListen=false;
    PlayState playState;

    //
    //
    //
    //
    //
    //

    public Button listBtn;
    public Button lyricsBtn;
    public Button contentBtn;

    [SerializeField]
    private List<SongSlot> currentSongSlotList;
    public GameObject scrollViewObject;
    private ScrollViewRect scrollViewRect;
    private IEnumerator audioLoadIEnum;

    private float lastVolume;
    private enum PlayState
    {
        Play,Pause
    }
    enum RepeatMode
    {
        None,OneRepeat,AllRepeat
    }
    // Start is called before the first frame update
    void Awake()
    {
        Init();
       

    }

    void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;

            volumeSlider.gameObject.SetActive(false);
            //볼륨제어 포인터 이벤트 등록
            volumeToggle = volumeToggleObj.GetComponent<Toggle>();
            volumeSlider.onValueChanged.AddListener(delegate {
                AudioListener.volume = volumeSlider.value; 
            });
            volumeToggle.onValueChanged.AddListener(delegate {
                if (volumeToggle.isOn == true)
                {
                    
                    lastVolume = AudioListener.volume;
                    volumeSlider.value = 0;
                }
                else
                {
                    AudioListener.volume = lastVolume;
                    volumeSlider.value = lastVolume;
                }
            });
            //볼륨 토글에 포인터 올리면 세부조절활성화
            volumeToggleObj.OnPointEnter += delegate
            {
                volumeSlider.gameObject.SetActive(true);
                volumeSlider.value = AudioListener.volume;
            };
            //컨트롤바에서 포인터 나가면 세부조절 비활성화
            mainControllBar.OnPointExit += delegate
            {

                volumeSlider.gameObject.SetActive(false);
            };




            //info 오른쪽 오브젝트
            scrollViewRect = scrollViewObject.GetComponent<ScrollViewRect>();
            currentSongSlotList = new List<SongSlot>();

            //겉
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;

            pauseplayBtnImage = new Image[2];

            int[] num = { 0, 1 };
            for (int i = 0; i < num.Length; i++)
            {
                pauseplayBtns[i].onClick.AddListener(delegate {
                    if (audioSource.clip != null)
                    {
                        //완전히 들었는지 false
                        isCleanListen = false;

                        ChangeState(!audioSource.isPlaying);
                    }
                    else
                    {
                        if (audioSource.isPlaying ==false && currentSongSlotList.Count > currentSongIndex)
                        {
                            StartGetAudioCoroution(currentSongIndex, true);
                        }
                    }
                });
                pauseplayBtnImage[i] = (pauseplayBtns[i].gameObject).GetComponent<Image>();


                prevBtns[i].onClick.AddListener(ClickPrevButton);
                nextBtns[i].onClick.AddListener(ClickNextButton);

            }
            repeatBtnImage = (repeatBtn.gameObject).GetComponent<Image>();
            repeatBtn.onClick.AddListener(SetRepeatMode);


            slider.OnPointUp += OnValueChange;
            slider.OnPointDown += StopSlider;

            openBtn.onClick.AddListener(OpenCloseInfo);

            lyricsBtn.onClick.AddListener(delegate { LoadInfo("lyrics"); });
            contentBtn.onClick.AddListener(delegate { LoadInfo("info"); });
            listBtn.onClick.AddListener(delegate { animator.SetBool("isContentOpen", false); });

            playState = PlayState.Pause;
            enumerator = MoveSlider();

            //리스너
            OnGetClip += SetAudioClip;

            dropdown.onValueChanged.AddListener(OnChangeDropDown);
            
        }
    }
    public void Stop()
    {
        if (audioSource.clip != null)
            audioSource.Pause();
        isCleanListen = false;
    }
    private void Update()
    {
        if (audioSource.clip != null)
        {
            
            if (audioSource.isPlaying == true && isCurrentSongFinish == false)
            {//재생상태일 때
                if ((int)audioSource.time == (int)audioSource.clip.length)
                {//재생이 끝나면
                    
                    
                    if (RepeatMode.OneRepeat != repeatMode)
                    {
                        Debug.Log("자연 재생 끝");
                        if (isCleanListen == true)
                        {
                            PostPlayCountPlus(currentSongSlotList[currentSongIndex].GetMusic().id);
                        }
                        isCleanListen = false;
                        isCurrentSongFinish = true;
                        AutoPlayNextMusic();
                    }
                    else
                    {//한곡 재생 모드라면
                        if (isCleanListen == true)
                        {
                            PostPlayCountPlus(currentSongSlotList[currentSongIndex].GetMusic().id);
                        }
                        isCleanListen = true;
                    }

                }
            }

            if (audioSource.isPlaying == true && playState == PlayState.Pause)
            {//재생시키기
                subMusicController.Pause();
                //Debug.Log("Play");
                playState = PlayState.Play;

                StartCoroutine(enumerator);
                ChangeControllImage(true);

            }
            else if(audioSource.isPlaying ==false && playState == PlayState.Play)
            {//중지시키기
                //Debug.Log("Pause");
                playState = PlayState.Pause;
                StopCoroutine(enumerator);
                ChangeControllImage(false);

            }

        }

        
    }
    private void ChangeControllImage(bool isPlay)
    {
        for (int i = 0; i < 2; i++)
        {
            pauseplayBtnImage[i].sprite = Resources.Load<Sprite>(isPlay? "Image/UI/pause": "Image/UI/play");
        }
    }
    private void LoadInfo(string type)
    {
        if (audioSource.clip == null) return;
        if (type == "lyrics")
        {
            contentText.text = currentSongSlotList[currentSongIndex].GetMusic().lyrics;
            contentText.transform.parent.GetComponent<ScrollViewRect>().SetContentSize();
    
            animator.SetBool("isContentOpen", true);
            animator.SetTrigger("OpenContent");
            
        }
        else if(type == "info")
        {
            contentText.text = currentSongSlotList[currentSongIndex].GetMusic().info;
            contentText.transform.parent.GetComponent<ScrollViewRect>().SetContentSize();
            animator.SetBool("isContentOpen", true);
            animator.SetTrigger("OpenContent");
            
        }
   
    }
    public void SetSongList(List<Music> _musics = null,bool play=false)
    {

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
        currentSongSlotList.Clear();


        if (_musics != null)
        {
            SongSlot ss;
            GameObject _obj = null;
            for (int i=0; i< _musics.Count; i++)
            { 
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlot2") as GameObject, scrollViewObject.transform);
                ss = _obj.GetComponent<SongSlot>();
                ss.SetMusic(_musics[i]);
                ss.OnClickSlot += SongClickHandler;

                currentSongSlotList.Add(ss);
            }
            scrollViewRect.SetContentSize(100);

            if (_musics.Count != 0) {
                StartGetAudioCoroution(tmpSongIndex, play);
                SetActiveNoList(false);
            }
            else
            {
                for (int i=0; i<images.Length; i++)
                {
                    images[i].sprite= Resources.Load<Sprite>("Image/none");
                }

                currentSongIndex = 0;
                audioSource.Stop();
                audioSource.clip = null;
                SetActiveNoList(true);
            }
            
        }
    }
    void SetActiveNoList(bool noList)
    {
        if (noListObj != null)
        {
            for(int i=0; i<noListObj.Length; i++)
            {
                noListObj[i].SetActive(noList);
            }

        }
        if (yesListObj != null)
        {
            for (int i=0; i<yesListObj.Length; i++)
            {
                yesListObj[i].SetActive(!noList);
            }
        }

        if (noList)
        {
            titleTexts[1].text = "None";
            artistTexts[1].text = "None";
        } 
    }
    void SongClickHandler(SongSlot ss)
    {
        int newIndex = currentSongSlotList.IndexOf(ss);

        if (newIndex == currentSongIndex) return;//현재 재생중인 음원이면 패스


        StartGetAudioCoroution(newIndex, true);

    }
    

    void SetRepeatMode()
    {
        repeatMode = (RepeatMode)(((int)repeatMode + 1) % 3);
        switch (repeatMode)
        {
            case RepeatMode.None:

                break;
            case RepeatMode.OneRepeat:
                audioSource.loop = true;
                break;
            case RepeatMode.AllRepeat:
                audioSource.loop = false;
                break;
        }
        repeatBtnImage.sprite = Resources.Load<Sprite>("Image/UI/repeat" + (int)repeatMode);

    }
    void StartGetAudioCoroution(int newIdx, bool play)
    {
        if (currentSongSlotList.Count > currentSongIndex)
        {
            currentSongSlotList[currentSongIndex].SetImage(new Color(1f, 1f, 1f));
        }
        if (currentSongSlotList.Count <= newIdx)
        {
            Debug.Log("오류!!!! 없는 음원 참조");
            return;
        }
        currentSongSlotList[newIdx].SetImage(new Color(0.8f, 0.8f, 0.8f));
        currentSongIndex = newIdx;

        //음원 받아오기
        GetAudioAsync(currentSongSlotList[currentSongIndex].GetMusic().locate,play);
        /*
        if (audioLoadIEnum != null)
        {
            flag = true;
            getAudioWWW.Dispose();
            StopCoroutine(audioLoadIEnum);GetAudioAsync
        }
        audioLoadIEnum = GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, play);
        StartCoroutine(audioLoadIEnum);
        */
        //끝

        audioSource.Stop();
        audioSource.time = 0;

        slider.value = 0;
        isCurrentSongFinish = true;


        Music music = currentSongSlotList[currentSongIndex].GetMusic();

        LoadImage(music.imageLocate);

        for (int i = 0; i < 2; i++)
        {
            titleTexts[i].text = music.title;
            artistTexts[i].text = music.GetArtistName();
        }
    }
    async void GetAudioAsync(string path, bool play)
    {
        if (getAudioWWW!=null)
        {
            getAudioWWW.Dispose();
            //StopCoroutine(audioLoadIEnum);
        }
        AudioClipPlay a = await GetAudioClipAsync(path, play);

        if(a!=null)
            SetAudioClip(a.audioClip, a.play);
        else
        {
            ChangeControllImage(false);
        }
    }
    async void PostPlayCountPlus(string msID)
    {
        await POST_AddPlayCount(msID);
    }
    public async void StartGetListCoroution(string name, int idx, bool play)
    {     

        if (name != currentListName)
        {
            tmpSongIndex = idx;
            currentListName = name;

            dropdown.value = UserData.Instance.user.listName.IndexOf(name);
            MusicList ml= await GET_MusicListAsync(currentListName, play);
            if (ml != null)
            {
                SetSongList(ml.musicList, ml.play);
            }
        }
        else
        {
            StartGetAudioCoroution(idx, play);
        }
    }
    void ClickPrevButton()
    {   //완전히 들었는지 false
        isCleanListen = false;

        if (currentSongSlotList != null && currentSongSlotList.Count == 0) return;
        int newIdx=0;
        if (randomToggle.isOn == true)
        {
            //랜덤 뽑기
            newIdx = PickRandomIndex();
        }
        else
        {
            newIdx = (currentSongIndex - 1+ currentSongSlotList.Count) % currentSongSlotList.Count;
        }

        StartGetAudioCoroution(newIdx, true);
    }
    void ClickNextButton()
    {
        //완전히 들었는지 false
        isCleanListen = false;

        if (currentSongSlotList!=null && currentSongSlotList.Count == 0) return;
        int newIdx = 0;
        if (randomToggle.isOn == true)
        {
            //랜덤 뽑기
            newIdx = PickRandomIndex();
        }
        else
        {
            newIdx = (currentSongIndex + 1) % currentSongSlotList.Count;
        }
        //재생
        StartGetAudioCoroution(newIdx,true);

    }
    int PickRandomIndex()
    {
        if (currentSongSlotList.Count <= 1) return 0;
        int randomIdx = currentSongIndex;
        

        while (randomIdx == currentSongIndex)
        {
            System.Random rand = new System.Random();
            randomIdx = rand.Next(0, currentSongSlotList.Count);

        }
        return randomIdx;
    }
    void AutoPlayNextMusic()
    {
        int nextIdx = PickRandomIndex();

        if (randomToggle.isOn==false)
        {//랜덤모드가 아니라면
            nextIdx = (currentSongIndex + 1) % currentSongSlotList.Count;
        }

        if (repeatMode == RepeatMode.None)
        {
            if (nextIdx == 0)
            {
                //재생목록의 끝에 도달하여 재생 종료하고 맨앞 음원으로 이동
                if (currentSongSlotList != null)
                {
                    StartGetAudioCoroution(0, false);
                    return;
                }
            }
        }
        StartGetAudioCoroution(nextIdx, true);

    }
    void OpenCloseInfo()
    {
        Debug.Log("Open");
        animator.SetBool("isOpen", !animator.GetBool("isOpen"));
        if (animator.GetBool("isOpen")==false)
        {
            animator.SetBool("isContentOpen", false);
        }

    }

    public void SetAudioClip(AudioClip ac, bool play)
    {//OnGetClip 리스너가 호출되면 함수 실행
        Debug.Log("오디오 교체");

        audioClip = ac;
        audioSource.clip = audioClip;

        isCurrentSongFinish = false;

        //완전히 건들지 않고 들었는지 초기화
        isCleanListen = true;

        ChangeState(play);

        Music music = currentSongSlotList[currentSongIndex].GetMusic();


    }

    void OnValueChange(float value)
    {
        
        if (audioSource == null) return;
        if (audioSource.clip == null) return;

        isCleanListen = false;
        audioSource.time = Mathf.Max(Mathf.Min(audioClip.length *value, audioClip.length), 0);

        if (audioSource.isPlaying == true)
            StartCoroutine(enumerator);
    }
    void StopSlider(float value)
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying == true)
        {
            StopCoroutine(enumerator);
        }
    }
    void ChangeState(bool isPlay)
    {
        if (audioSource.clip != null)
        {           
            if (isPlay==false && audioSource.isPlaying ==true)
            {
                audioSource.Pause();
            }
            else if(isPlay==true && audioSource.isPlaying==false)
            {
                audioSource.Play();
            }
        }
    }
    IEnumerator MoveSlider()
    {
        while (audioSource.isPlaying)
        {
            slider.value = audioSource.time / audioClip.length;
            yield return new WaitForSeconds(0.1f);
        }

    }
    private void LoadImage( string filePath)
    {
        if (filePath == null || filePath == "")
        {
            for(int i=0; i<images.Length; i++)
                images[i].sprite = Resources.Load<Sprite>("Image/none");
        }
        else
        {
            StartCoroutine(GetTexture(filePath));
        }
    }

    IEnumerator GetTexture(string _path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://"+_path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            for (int i = 0; i < images.Length; i++)
                images[i].sprite = Resources.Load<Sprite>("Image/none");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            for (int i = 0; i < images.Length; i++)
                images[i].sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
    }
    public void AddNewMusics(string listName,  List<Music> _musics)
    {//현재 재생목록에 음원을 추가했을 때 실시간 반영되도록
        if (_musics != null)
        {
            if (currentListName == listName)
            {
                bool wasZero = false;
                if (currentSongSlotList.Count == 0)
                {
                    wasZero = true;

                }

                SongSlot ss;
                GameObject _obj = null;
                for (int i = 0; i < _musics.Count; i++)
                {
                    _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlot2") as GameObject, scrollViewObject.transform);
                    ss = _obj.GetComponent<SongSlot>();
                    ss.SetMusic(_musics[i]);
                    ss.OnClickSlot += SongClickHandler;

                    currentSongSlotList.Add(ss);
                }
                scrollViewRect.SetContentSize(100);

                if (wasZero == true)
                {
                    if (_musics.Count != 0)
                    {
                        SetActiveNoList(false);
                        StartGetAudioCoroution(0, false);

                    }

                }
            }
        }
    }
    public void DelNewMusic(string listName,int idx ,Music _music)
    {//현재 재생목록에 음원을 추가했을 때 실시간 반영되도록
        if (_music != null)
        {
            if (currentListName == listName)
            {
                Debug.Log(idx+" "+currentSongIndex);
                if (currentSongIndex == idx)
                {
                    Stop();
                    audioSource.clip = null;

                    Destroy(currentSongSlotList[idx].gameObject);
                    currentSongSlotList.RemoveAt(idx);
                    if (currentSongSlotList.Count == 0)
                    {
                        SetActiveNoList(true);

                    }
                    else
                    {
                        StartGetAudioCoroution(idx%currentSongSlotList.Count, false);
                        Debug.Log(idx % currentSongSlotList.Count);
                    }
                }
                else
                {
                    if(currentSongIndex > idx)
                    {
                        currentSongIndex--;
                    }
                    Destroy(currentSongSlotList[idx].gameObject);
                    currentSongSlotList.RemoveAt(idx);
                }

                
                scrollViewRect.SetContentSize(100);

            }
        }
    }
    public void SetOptions(List<string> listNames)
    {
       
        if (listNames == null) return;

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < listNames.Count; ++i)
            options.Add(new TMP_Dropdown.OptionData(listNames[i].ToString()));

        dropdown.options = options;
    }

    void OnChangeDropDown(int value)
    {
        StartGetListCoroution(dropdown.options[value].text, 0, true);
    }

}
