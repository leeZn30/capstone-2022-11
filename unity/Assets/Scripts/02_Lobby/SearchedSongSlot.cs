using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
public class SearchedSongSlot : MonoBehaviour
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
 
        artistText.text = music.nickname + "(" + music.userID + ")";
        LoadImage(music.imagelocate);
        toggle.isOn = false;
    }
    private void LoadImage(string filePath)
    {
        if (filePath == "")
        {
            Debug.Log("ddd");
            image.sprite = Resources.Load<Sprite>("Image/none"); 
        }
        else
        {
            StartCoroutine(GetTexture("http://localhost:8080" + filePath));
        }
    }

    IEnumerator GetTexture(string path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            image.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
