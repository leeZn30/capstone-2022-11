using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicControllerMini : MonoBehaviour
{


    private AudioSource audioSource;

    public Button pauseplayBtn;
    public Scrollbar scrollbar;

    public AudioClip audioClip;


    public void SetAudioClip(AudioClip ac)
    {
        audioSource.Stop();
        audioClip = ac;
        audioSource.clip = audioClip;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pauseplayBtn.onClick.AddListener(ChangeState);
        scrollbar.onValueChanged.AddListener(OnValueChange);
    }
    void OnValueChange(float value)
    {
        if (audioSource.clip != null)
        {
            audioSource.time = Mathf.Max(Mathf.Min(audioClip.length * value, audioClip.length), 0);
            if (audioSource.time == audioClip.length)
            {
                if (audioSource.isPlaying == true)
                {
                    ChangeState();
                    audioSource.time = 0;
                    scrollbar.value = 0;
                }
            }
        }

    }
    void ChangeState()
    {
        if (audioClip != null)
        {
            StopCoroutine("MoveScrollBar");
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                
            }
            else
            {
                audioSource.Play();
                StartCoroutine("MoveScrollBar");
            }
        }
    }
    IEnumerator MoveScrollBar()
    {
        while (audioSource.isPlaying)
        {
            scrollbar.value = audioSource.time / audioClip.length;
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log(audioSource.time);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
