using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
public class MusicController : MusicWebRequest
{
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
    public SubMusicController SubMusicController;



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
    CancellationTokenSource cts;
    //private IEnumerator audioLoadIEnum; 
    private enum PlayState
    {
        Play,Pause
    }
    enum RepeatMode
    {
        None,OneRepeat,AllRepeat
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
;

        /*
        Task.Factory.StartNew(async () =>
        {
            Task<string> s = Task<string>.Factory.StartNew(() => ssss(1));
            await s;
            Debug.Log("테스크 끝"+s.Result.ToString());

        });*/

    }


    public void Stop()
    {
        if (audioSource.clip != null)
            audioSource.Pause();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(testLoadUpload());
        }
        if (audioSource.clip != null)
        {
            
            if (audioSource.isPlaying == true && isCurrentSongFinish == false)
            {//재생상태일 때
                if ((int)audioSource.time == (int)audioClip.length)
                {//재생이 끝나면
                    
                    
                    if (RepeatMode.OneRepeat != repeatMode)
                    {
                        Debug.Log("자연 재생 끝");
                        isCurrentSongFinish = true;
                        AutoPlayNextMusic();
                    }

                }
            }

            if (audioSource.isPlaying == true && playState == PlayState.Pause)
            {
                //Debug.Log("Play");
                playState = PlayState.Play;

                StartCoroutine(enumerator);

                for (int i = 0; i < 2; i++)
                {
                    pauseplayBtnImage[i].sprite = Resources.Load<Sprite>("Image/UI/pause");
                }

            }
            else if(audioSource.isPlaying ==false && playState == PlayState.Play)
            {
                //Debug.Log("Pause");
                playState = PlayState.Pause;
                StopCoroutine(enumerator);
                for (int i = 0; i < 2; i++)
                {
                    pauseplayBtnImage[i].sprite = Resources.Load<Sprite>("Image/UI/play");
                }
            }

        }

        
    }
    private void LoadInfo(string type)
    {
        if (type == "lyrics")
        {
            animator.SetBool("isContentOpen", true);
            animator.SetTrigger("OpenContent");
            contentText.text = currentSongSlotList[currentSongIndex].GetMusic().lyrics;
        }
        else if(type == "info")
        {
            animator.SetBool("isContentOpen", true);
            animator.SetTrigger("OpenContent");
            contentText.text = currentSongSlotList[currentSongIndex].GetMusic().info;
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
            
            StartGetAudioCoroution(tmpSongIndex, play);
           
        }
    }
    void SongClickHandler(SongSlot ss)
    {
        int newIndex = currentSongSlotList.IndexOf(ss);

        if (newIndex == currentSongIndex) return;//현재 재생중인 음원이면 패스


        StartGetAudioCoroution(newIndex, true);

    }
    void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;

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
                    if (audioSource.clip != null) {
                        ChangeState(!audioSource.isPlaying);
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
            listBtn.onClick.AddListener(delegate { animator.SetBool("isContentOpen",false); });

            playState = PlayState.Pause;
            enumerator = MoveSlider();

            //리스너
            OnGetClip += SetAudioClip;
            OnGetSongList += SetSongList;

            StartGetListCoroution("myList", 0, false);
            
            cts = new CancellationTokenSource();
        }
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

        currentSongSlotList[currentSongIndex].SetImage(new Color(1f, 1f, 1f));
        currentSongSlotList[newIdx].SetImage(new Color(0.8f, 0.8f, 0.8f));
        currentSongIndex = newIdx;

        //음원 받아오기
        GetAudioAsync(currentSongSlotList[currentSongIndex].GetMusic().locate,play);
        /*
        if (audioLoadIEnum != null)
        {
            getAudioWWW.Dispose();
            //StopCoroutine(audioLoadIEnum);
        }*/
        //audioLoadIEnum = GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, play);
        //StartCoroutine(audioLoadIEnum);

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
        AudioClipPlay a = await GetAudioClicpAsync(path, play);
        if(a!=null)
            SetAudioClip(a.audioClip, a.play);
    }
    public void StartGetListCoroution(string name, int idx, bool play)
    {     

        if (name != currentListName)
        {
            tmpSongIndex = idx;
            currentListName = name;
            StartCoroutine(GET_MusicList(currentListName, UserData.Instance.id, play));
        }
        else
        {
            StartGetAudioCoroution(idx, play);
        }
    }
    void ClickPrevButton()
    {
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

   
        Debug.Log("Autoplay" + nextIdx);
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


        ChangeState(play);

        Music music = currentSongSlotList[currentSongIndex].GetMusic();


    }

    void OnValueChange(float value)
    {
        if (audioSource == null) return;

        audioSource.time = Mathf.Max(Mathf.Min(audioClip.length *value, audioClip.length), 0);

        if (audioSource.isPlaying == true)
            StartCoroutine(enumerator);
    }
    void StopSlider(float value)
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying == true)
        {
            Debug.Log("stop!!!!!");
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

    
}
