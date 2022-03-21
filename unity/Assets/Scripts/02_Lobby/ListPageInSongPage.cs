using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListPageInSongPage : Page
{

    public TextMeshProUGUI contentText;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Open(int id, string content)
    {//오버로딩
        gameObject.SetActive(true);
        contentText.text = content;
        Load(id);
    }
    void Load(int id)
    {//id에 따라 알맞은 재생목록을 불러오는 함수

    }

    override public void Reset()
    {   
        //재생목록 초기화

        //초기화
        contentText.text = "";
    }
}
