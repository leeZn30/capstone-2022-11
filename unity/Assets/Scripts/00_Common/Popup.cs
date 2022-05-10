using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Popup : Singleton<Popup>
{
    public GameObject tokenPanel;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {

        button.onClick.AddListener(delegate {
            UserData.Instance.Clear();
            tokenPanel.SetActive(false);
            SceneManager.LoadScene("01_Main");
        });

    }
    public void Open()
    {
        tokenPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
