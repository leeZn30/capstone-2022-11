using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;

    public Button openBtn;
    public Image[] images;
    public Button[] pauseplayBtns;
    public Button[] prevBtns;
    public Button[] nextBtns;

    public TextMeshProUGUI[] titleTexts;
    public TextMeshProUGUI[] artistTexts;

    public Slider slider;

    public Toggle randomBtns;
    public Button repeatBtns;
    public TextMeshProUGUI contentText;

    private Animator animator;
    private AudioClip audioClip;

    private bool isAlreadyInit = false;
    private bool isRandomMode = false;
    private RepeatMode repeatMode;
    
    public  List<Music> musicList;

    private int currentMusicList;
    private int currentMusicIndex;

    enum RepeatMode
    {
        None,OneRepeat,AllRepeat
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MusicWebRequest.Instance.GetMusicList("테스트검색");
        }

        if (audioSource.clip != null)
        {
            if (audioSource.isPlaying == true)
            {//재생상태일 때
                
                if (audioSource.time == audioClip.length)
                {//재생이 끝나면
                    Debug.Log("자연 재생 끝");
                    audioSource.time = 0;
                    slider.value = 0;

                    AutoPlayNextMusic();

                }
            }
        }
        
    }
    void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            int[] num = { 0, 1 };
            for (int i = 0; i < num.Length; i++)
            {
                pauseplayBtns[i].onClick.AddListener(delegate {
                    if (audioSource.clip != null) {
                        ChangeState(!audioSource.isPlaying);
                    }
                });

                prevBtns[i].onClick.AddListener(ClickPrevButton);
                nextBtns[i].onClick.AddListener(ClickNextButton);

            }

            slider.onValueChanged.AddListener(OnValueChange);//추후 버튼 뗄때만으로 수정
            openBtn.onClick.AddListener(OpenCloseInfo);
            MusicWebRequest.Instance.OnGetClip += SetAudioClip;
            MusicWebRequest.Instance.OnGetMusicList += SetMusicList;
        }
    }
    void ClickPrevButton()
    {

        currentMusicIndex = (currentMusicIndex - 1) % musicList.Count;
    }
    void ClickNextButton()
    {
        if (isRandomMode == true)
        {
            //랜덤 뽑기
        }
        else
        {
            currentMusicIndex = (currentMusicIndex + 1) % musicList.Count;
        }
        
        //재생
        MusicWebRequest.Instance.GetAudioClip(musicList[currentMusicIndex].locate);
        
    }
    void AutoPlayNextMusic()
    {

        int randomIdx = currentMusicIndex;

        while (randomIdx == currentMusicIndex)
        {
            System.Random rand = new System.Random();
            randomIdx = rand.Next(0, musicList.Count);

        }


        int nextIdx = randomIdx;

        if (isRandomMode ==false)
        {//랜덤모드가 아니라면
            nextIdx = (currentMusicIndex + 1) % musicList.Count;
        }

        if (repeatMode == RepeatMode.None)
        {
            if (nextIdx == 0)
            {
                //재생목록의 끝에 도달하여 재생 종료하고 맨앞 음원으로 이동
                if (musicList != null)
                {
                    MusicWebRequest.Instance.GetAudioClip(musicList[0].locate);
                }
            }
        }
        else if (repeatMode == RepeatMode.OneRepeat)
        {
            audioSource.time = 0;
            audioSource.Play();
        }
        else if (repeatMode == RepeatMode.AllRepeat)
        {
            MusicWebRequest.Instance.GetAudioClip(musicList[currentMusicIndex].locate);
        }
    }
    void OpenCloseInfo()
    {

        animator.SetBool("isOpen", !animator.GetBool("isOpen"));
    }
    public void SetMusicList(List<Music> _musics = null)
    {
        musicList = _musics;
        if (musicList != null)
        {
            MusicWebRequest.Instance.GetAudioClip(musicList[0].locate);
        }
    }

    public void SetAudioClip(AudioClip ac)
    {//OnGetClip 리스너가 호출되면 함수 실행
        audioSource.Stop();
        audioSource.time = 0;
        audioClip = ac;
        audioSource.clip = audioClip;
        ChangeState(true);
        for(int i=0; i<2; i++)
        {
            titleTexts[i].text = musicList[currentMusicIndex].title;
            artistTexts[i].text = musicList[currentMusicIndex].userID;
        }
    }

    void OnValueChange(float value)
    {
        audioSource.time = Mathf.Max(Mathf.Min(audioClip.length *value, audioClip.length), 0);
    }
    void ChangeState(bool isPlay)
    {
        if (audioClip != null)
        {           
            if (isPlay==false && audioSource.isPlaying ==true)
            {
                Debug.Log("Pause");
                StopCoroutine("MoveSlider");
                audioSource.Pause();

            }
            else if(isPlay==true && audioSource.isPlaying==false)
            {
                Debug.Log("Play");
                audioSource.Play();
                StartCoroutine("MoveSlider");
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
}
