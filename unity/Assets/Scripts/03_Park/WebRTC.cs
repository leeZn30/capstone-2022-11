using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KyleDulce.SocketIo;

public class WebRTC : MonoBehaviour
{

    //Socket io
    Socket socket;
    
    //Servers
    string mainServer = "http://localhost:8080";
    /**
    Dictionary<string, Dictionary<string, string>[]> iceServers;
    **/

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
        
        iceServers.Add("iceServers", tmp);
        **/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            joinRoom();
        }
    }

    public void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection(mainServer);
            socket.connect();

            Debug.Log("Connect Success!");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }

    void joinRoom()
    {
        int roomNum = 1; // 일단 통일

        if (socket != null)
        {
            //let userOption = { roomNum:roomNum, userId: socket.id};
            Dictionary<string, dynamic> userOption = new Dictionary<string, dynamic>();
            userOption.Add("roomNum", roomNum);
            userOption.Add("userId", socket);

            Debug.Log(socket);

            socket.emit("joinRoom", (string)(roomNum + " " + socket));

        }
    }
}
