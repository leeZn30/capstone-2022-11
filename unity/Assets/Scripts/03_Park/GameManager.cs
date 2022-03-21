using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject character;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(720, 480, false);

        PhotonNetwork.AutomaticallySyncScene = true;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10f);
        PhotonNetwork.Instantiate(character.name, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
