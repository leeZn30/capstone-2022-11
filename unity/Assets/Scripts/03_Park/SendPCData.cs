using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Unity.WebRTC;

public class SendPCData
{
    //int roomNum;
    //int userId; // server의 socketId와 다름
    //int option;
    //JsonData candidate;
    //JsonData offer;
    //RTCIceCandidate candidate;
    RTCPeerConnection pc;

    public void setSendPC(RTCPeerConnection peerconnection)
    {
        pc = peerconnection;
    }
}
