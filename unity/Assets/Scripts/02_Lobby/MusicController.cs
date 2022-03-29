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
                pauseplayBtns[i].onClick.AddListener(ChangeState);
                //prevBtns[i].onClick.AddListener();
                //nextBtns[i].onClick.AddListener();

            }
            slider.onValueChanged.AddListener(OnValueChange);
            openBtn.onClick.AddListener(OpenCloseInfo);
            MusicWebRequest.Instance.OnGetClip += SetAudioClip;
            MusicWebRequest.Instance.OnGetMusicList += SetMusicList;
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
    {
        audioSource.Stop();
        audioClip = ac;
        audioSource.clip = audioClip;
    }

    void OnValueChange(float value)
    {
        audioSource.time = Mathf.Max(Mathf.Min(audioClip.length * value, audioClip.length), 0);
        if (audioSource.time == audioClip.length)
        {
            if (audioSource.isPlaying == true)
            {
                ChangeState();
                audioSource.time = 0;
                slider.value = 0;
            }
        }
    }
    void ChangeState()
    {
        if (audioClip != null)
        {
            StopCoroutine("MoveSlider");
            if (audioSource.isPlaying)
            {
                audioSource.Pause();

            }
            else
            {
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
        Debug.Log(audioSource.time);

    }
}
