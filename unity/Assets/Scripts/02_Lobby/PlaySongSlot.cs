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

    public bool isSearchSlot;

    private bool isPlaying;
    private bool isSet;
    private IEnumerator enumerator;


    new public delegate void ClickHandler(PlaySongSlot ss);
    new public event ClickHandler OnClickSlot;


    new public void OnPointerDown(PointerEventData eventData)
    {
        
        if (isSearchSlot)
            OnClickSlot(this);
        else
            MusicController.Instance.subMusicController.SetAudioPath(this, false);


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

        MusicController.Instance.subMusicController.OnChanged += Off;


    }

    void OnValueChange(float value)
    {

        if (isSet == false) return;
        float tmp=Mathf.Max(Mathf.Min(MusicController.Instance.subMusicController.GetLength()* value, MusicController.Instance.subMusicController.GetLength()), 0);
        MusicController.Instance.subMusicController.SetTime(tmp);

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
            slider.value = Mathf.Max(MusicController.Instance.subMusicController.GetTime() / MusicController.Instance.subMusicController.GetLength(),0);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Off(PlaySongSlot ss)
    {

        if (ss == this) {
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
            MusicController.Instance.subMusicController.Pause();
            MusicController.Instance.subMusicController.SetTime(0);

            MusicController.Instance.subMusicController.SetAudioPath(this);

           
        }
        else
        {
            if (isPlaying == false)
            {
                isPlaying = true;
                MusicController.Instance.subMusicController.Play();

            }
            else
            {
                isPlaying = false;
                MusicController.Instance.subMusicController.Pause();
            }
            ChangeBtnImage();
        }
        
    }
    public void ChangeBtnImage()
    {
        if (playImage == null) return;
        if (isPlaying )
        {       
            if (slider!=null)
                StartCoroutine(enumerator);
            playImage.sprite = Resources.Load<Sprite>("Image/UI/pause");
        }
        else
        {
            
            if (slider != null)
                StopCoroutine(enumerator);
            playImage.sprite = Resources.Load<Sprite>("Image/UI/play");
        }
    }
    private void OnDestroy()
    {
        MusicController.Instance.subMusicController.OnChanged -= Off;
        if (slider != null)
        {
            slider.OnPointUp -= OnValueChange;
            slider.OnPointDown -= StopSlider;
        }

    }
}
