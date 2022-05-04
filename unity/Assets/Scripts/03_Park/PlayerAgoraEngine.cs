using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

public class PlayerAgoraEngine : MonoBehaviour
{
    [SerializeField] private string appID;
    [SerializeField] private string token;
    [SerializeField] private string channelName;

    private IRtcEngine rtcEngine;

    // Start is called before the first frame update
    void Start()
    {
        loadEngine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadEngine()
    {
        if (rtcEngine != null)
        {
            rtcEngine = null;
            IRtcEngine.Destroy();
            return;
        }

        rtcEngine = IRtcEngine.GetEngine(appID);
    }

    public void join()
    {
        rtcEngine.JoinChannelByKey(channelKey: token, channelName: channelName);

    }

}
