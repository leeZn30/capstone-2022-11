using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioListener mainAudioSource;
    [SerializeField] private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        mainAudioSource = FindObjectOfType<AudioListener>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVolume();
    }

    void ChangeVolume()
    {
        AudioListener.volume = slider.value;
    }
}
