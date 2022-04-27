using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMusicController : MusicWebRequest
{
    private AudioSource audioSource;

    public delegate void SongHandler(string currentPath);
    public event SongHandler OnChanged;

    private IEnumerator audioLoadIEnum;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OnGetClip += SetAudioClip;
    }
    public void SetAudioPath(string path)
    {

        OnChanged(path);
        if (audioLoadIEnum != null)
            StopCoroutine(audioLoadIEnum);
        audioLoadIEnum = GetAudioCilpUsingWebRequest(path, true);
        StartCoroutine(audioLoadIEnum);

    }
    public void SetAudioClip(AudioClip ac, bool play)
    {
        audioSource.Stop();
        audioSource.time = 0;

        audioSource.clip =ac;
        if(play)
            audioSource.Play();
        
    }

    public void Play()
    {
        if (audioSource.clip != null)
            audioSource.Play();
    }
    public void Pause()
    {
        if (audioSource.clip != null)
            audioSource.Pause();
    }
    public void Reset()
    {
        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.time = 0;
            OnChanged("");
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
}
