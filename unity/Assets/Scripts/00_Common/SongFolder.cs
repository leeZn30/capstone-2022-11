using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SongFolder : MonoBehaviour
{
    public string folderName;
    public string content;
    public string listName;
    public Image img;
    public TextMeshProUGUI name_text;
    public Button delBtn;

    private Button btn;

    public delegate void LoadSongListHandler(string listName,string str=null);
    public event LoadSongListHandler OnClickButton_;

    public delegate void DeleteHandler(SongFolder sf);
    public event DeleteHandler OnDelete;
    void Start()
    {

        
    }
    public void SetData(string value)
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickButton);
        if (delBtn != null)
        {
            delBtn.onClick.AddListener(delegate { OnDelete?.Invoke(this); });
            SetActiveDelButton(false);
        } 
        
        name_text.text = value;
        folderName = value;
        content = value;
        listName = value;
        
    }
    void OnClickButton()
    {
        OnClickButton_(listName, content);
    }

    public void SetActiveDelButton(bool isOn)
    {
        delBtn.gameObject.SetActive(isOn);
    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
