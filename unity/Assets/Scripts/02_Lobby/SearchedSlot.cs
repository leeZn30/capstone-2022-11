using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SearchedSlot : MonoBehaviour
{
    [SerializeField]
    private Music music;
    public Image image;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI artistText;
    public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = false;
    }
    public void SetMusic(Music _music)
    {
        music = _music;
        titleText.text = music.title;
        artistText.text = music.userID;
        toggle.isOn = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
