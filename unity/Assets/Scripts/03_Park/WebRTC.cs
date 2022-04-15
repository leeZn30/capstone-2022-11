using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KyleDulce.SocketIo;
using UnityEngine.UI;
using Unity.WebRTC;

public class WebRTC : MonoBehaviour
{
    bool isJoin = false;

    //Socket io
    Socket socket;
    
    //Servers
    string mainServer = "http://localhost:8080";

    //Dictionary<string, Dictionary<string, string>[]> iceServers;
    RTCIceServer[] iceServers = new RTCIceServer[2];

    // 카메라, 마이크
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    // sourceStream
    private MediaStream sourceStream;

    private RTCConfiguration config = new RTCConfiguration
    {
        iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.services.mozilla.com", "stun:stun.l.google.com:19302" } } }
    };

    // Start is called before the first frame update
    void Start()
    {

        /**
        Dictionary<string, string>[] tmp = new Dictionary<string, string>[2];

        Dictionary<string, string> url1 = new Dictionary<string, string>();
        url1.Add("urls", "stun:stun.services.mozilla.com");

        Dictionary<string, string> url2 = new Dictionary<string, string>();
        url2.Add("urls", "stun:stun.l.google.com:19302");

        tmp[0] = url1;
        tmp[1] = url2;
        **/

        //iceServers.Add("iceServers", tmp);

        //cameraImage = this.GetComponent<BuskerVideoPanel>().objectTarget.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        //MicSource = this.GetComponent<BuskerVideoPanel>().micAudioSource;

    }

    // Update is called once per frame
    void Update()
    {

    }

}
