using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (this.photonView.IsMine)
        {
            var followcam = FindObjectOfType<CinemachineVirtualCamera>();
            followcam.Follow = this.gameObject.transform;
            followcam.LookAt = this.gameObject.transform;

        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
