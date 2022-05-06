using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaySongSlot : SongSlot, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{ 
    public GameObject imageBackObject;
    public GameObject playBtnObject;
    public CustomSlider slider;

    private Image playImage;
    private Button playBtn;
    private bool isPlaying;
    private bool isSet;
    private IEnumerator enumerator;


    new public delegate void ClickHandler(PlaySongSlot ss);
    new public event ClickHandler OnClickSlot;

    new public void OnPointerDown(PointerEventData eventData)
    {
        OnClickSlot(this);
    }
    private void Awake()
    {
        backImage = GetComponent<Image>();
        playBtn = playBtnObject.GetComponent<Button>();
        playImage= playBtnObject.GetComponent<Image>();
        playBtn.onClick.AddListener(PlayThisMusic);

        imageBackObject.SetActive(false);
        if (slider!= null)
        {
            slider.gameObject.SetActive(false);
            enumerator = MoveSlider();
            slider.OnPointUp += OnValueChange;
            slider.OnPointDown += StopSlider;
            
        }

        MusicController.Instance.SubMusicController.OnChanged += Off;


    }

    void OnValueChange(float value)
    {

        if (isSet == false) return;
        float tmp=Mathf.Max(Mathf.Min(MusicController.Instance.SubMusicController.GetLength()* value, MusicController.Instance.SubMusicController.GetLength()), 0);
        MusicController.Instance.SubMusicController.SetTime(tmp);

        if (isPlaying == true)
            StartCoroutine(enumerator);
    }
    void StopSlider(float value)
    {
        if (isSet == false) return;

        if (isPlaying == true)
        {
            StopCoroutine(enumerator);
        }
    }
    IEnumerator MoveSlider()
    {
        while (isSet)
        {
            slider.value = Mathf.Max(MusicController.Instance.SubMusicController.GetTime() / MusicController.Instance.SubMusicController.GetLength(),0);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Off(string path)
    {

        if (path == music.locate) {
            isSet = true;
            isPlaying = true;
            ChangeBtnImage();
            return; 
        }
        isSet = false;
        isPlaying = false;
        if(slider!=null)
            slider.gameObject.SetActive(false);
        if (imageBackObject!=null)
            imageBackObject.SetActive(false);

        ChangeBtnImage();

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        imageBackObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSet == true) return;
        imageBackObject.SetActive(false);
    }
    private void PlayThisMusic()
    {
        if (isSet==false)
        {
            if (slider != null)
            {
                StopCoroutine(enumerator);
                slider.value = 0;
                slider.gameObject.SetActive(true);
            }

            isSet = true;
            isPlaying = true;
            MusicController.Instance.SubMusicController.Pause();
            MusicController.Instance.SubMusicController.SetTime(0);

            MusicController.Instance.SubMusicController.SetAudioPath(music.locate);
            
            
        }
        else
        {
            if (isPlaying == false)
            {
                isPlaying = true;
                MusicController.Instance.SubMusicController.Play();
            }
            else
            {
                isPlaying = false;
                MusicController.Instance.SubMusicController.Pause();
            }
            ChangeBtnImage();
        }
        
    }
    private void ChangeBtnImage()
    {
        if (playImage == null) return;
        if (isPlaying)
        {
            playImage.sprite = Resources.Load<Sprite>("Image/UI/pause");
            if(slider!=null)
                StartCoroutine(enumerator);

        }
        else
        {
            playImage.sprite = Resources.Load<Sprite>("Image/UI/play");
            if (slider != null)
                StopCoroutine(enumerator);
        }
    }
    private void OnDestroy()
    {
        MusicController.Instance.SubMusicController.OnChanged -= Off;
        if (slider != null)
        {
            slider.OnPointUp -= OnValueChange;
            slider.OnPointDown -= StopSlider;
        }

    }
}
