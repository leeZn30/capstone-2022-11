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
    private Button btn;

    public delegate void LoadSongListHandler(string listName,string str);
    public event LoadSongListHandler OnClickButton_;
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickButton);
        name_text.text = folderName;
        
    }
    void OnClickButton()
    {
        OnClickButton_(listName, content);
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
