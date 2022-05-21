using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public int mode = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == 0)
            ChangeVolume();
        else if (mode == 1)
            buskerVolume();
    }

    void ChangeVolume()
    {
        AudioListener.volume = slider.value;
    }

    void buskerVolume()
    {
        int ret = AgoraEngine.mRtcEngine.AdjustPlaybackSignalVolume((int)(slider.value * 400));

    }
}
