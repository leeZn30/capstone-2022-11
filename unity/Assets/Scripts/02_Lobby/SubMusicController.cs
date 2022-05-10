using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubMusicController : MusicWebRequest
{
    public GameObject subController;
    public GameObject messageObj;

    public TextMeshProUGUI titleText;

    public Button putBtn;
    public Button cancelBtn;
    public Button playBtn;

    private Image playBtnImage;

    private AudioSource audioSource;

    public delegate void SongHandler(PlaySongSlot ss);
    public event SongHandler OnChanged;

    private Music music;
    private IEnumerator audioLoadIEnum;

    // Start is called before the first frame update
    void Start()
    {
        playBtnImage = playBtn.gameObject.GetComponent<Image>();
        subController.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        OnGetClip += SetAudioClip;
        putBtn.onClick.AddListener(delegate
        {
            if (music != null)
            {
                MusicIDList iDList = new MusicIDList();
                iDList.musicList = new List<string>();
                iDList.musicList.Add(music.id);

                StartCoroutine(POST_AddMyList(iDList));
                StartCoroutine(messageOn());
            }
        });       
        playBtn.onClick.AddListener(delegate {
            if (audioSource.isPlaying == true)
            {
                Pause();
            }
            else
            {
                Play();
            }
        });
        cancelBtn.onClick.AddListener(Reset);
    }
    public void SetAudioPath(PlaySongSlot slot, bool play=true)
    {

        OnChanged(slot);
        //if (audioLoadIEnum != null)
        //    StopCoroutine(audioLoadIEnum);
        //audioLoadIEnum = GetAudioCilpUsingWebRequest(path, true);
        //StartCoroutine(audioLoadIEnum);
        music = slot.GetMusic();
        if (play == false)
        {
            Pause();
            audioSource.clip = null;

        }
        else
        {
            GetAudioAsync(music.locate);
        }
        if (slot.isSearchSlot) return;

        subController.SetActive(true);
        titleText.text = music.title+ " - " + music.GetArtistName();

    }
    async void GetAudioAsync(string path)
    {
        if (getAudioWWW != null)
        {
            getAudioWWW.Dispose();
            //StopCoroutine(audioLoadIEnum);
        }
        AudioClipPlay a = await GetAudioClipAsync(path, true);
        SetAudioClip(a.audioClip, a.play);
        Debug.Log("d");
    }
    public void SetAudioClip(AudioClip ac, bool play)
    {
        audioSource.Stop();
        audioSource.time = 0;

        audioSource.clip =ac;
        if (play)
            Play();

    }

    public void Play()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
            playBtnImage.sprite = Resources.Load<Sprite>("Image/UI/pause");
        }
        else
        {
            if (music != null)
                GetAudioAsync(music.locate);
        }
        MusicController.Instance.Stop();
    }
    public void Pause()
    {
        if (audioSource.clip != null)
        {
            audioSource.Pause();
            playBtnImage.sprite = Resources.Load<Sprite>("Image/UI/play");
        }
    }
    public void Reset()
    {
        if (audioSource.clip != null)
        {
            messageObj.SetActive(false);
            subController.SetActive(false);
            music = null;

            audioSource.Stop();
            audioSource.clip = null;
            audioSource.time = 0;
            OnChanged(null);

        }

    }
    public void SetTime(float value)
    {
        if (audioSource.clip != null)
        {
            audioSource.time = value;
        }

    }
    public float GetTime()
    {
        if (audioSource.clip != null)
        {
            return audioSource.time;
        }
        return 0;

    }
    public float GetLength()
    {
        if (audioSource.clip != null)
        {
            return audioSource.clip.length;
        }
        return 0;

    }
    public Sprite GetPlayBtnImg()
    {
        return playBtnImage.sprite;
    }
    IEnumerator messageOn()
    {
        messageObj.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        messageObj.SetActive(false);
    }
    private void OnDestroy()
    {
        OnGetClip -= SetAudioClip;
    }
}
