using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    // SingleTon
    public static GameManager instance = null;

    [SerializeField] private GameObject character;
    public GameObject myPlayer = null;

    void Awake()
    {
        Screen.SetResolution(720, 480, false);

        PhotonNetwork.AutomaticallySyncScene = true;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10f);
        myPlayer = PhotonNetwork.Instantiate(character.name, new Vector3(0, 0, 0), Quaternion.identity);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != null)
            {
                Destroy(this);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
