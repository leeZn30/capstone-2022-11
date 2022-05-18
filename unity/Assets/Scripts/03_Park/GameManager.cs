using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    // SingleTon
    public static GameManager instance = null;

    [SerializeField] private GameObject character;
    public GameObject myPlayer = null;

    void Awake()
    {

        GameObject[] pms =  GameObject.FindGameObjectsWithTag("Character");
        Debug.Log(pms.Length);
        /*
        foreach (PlayerManager pm in pms)
        {
            Debug.Log("dsadsa");
            if (pm.GetId() == UserData.Instance.id)
            {
                Debug.Log("dsadsa");
                Popup.Instance.Open(2);
               
            }
        }*/


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
}
