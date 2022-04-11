using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioSource mainAudioSource;
    [SerializeField] private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        mainAudioSource.volume = slider.value;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVolume();
    }

    void ChangeVolume()
    {
        mainAudioSource.volume = slider.value;
    }
}
