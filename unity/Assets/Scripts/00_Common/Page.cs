using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Page : MusicWebRequest
{//상속용 클래스
    public Button exitBtn; 
    protected bool isAlreadyInit = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(exitBtn!=null)
            exitBtn.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Open()
    {
        if (gameObject.activeSelf == true) return;
        Init();

        gameObject.SetActive(true);

        Load();
        MusicController.Instance.subMusicController.Reset();
    }

    virtual public void Init()
    {//해당 페이지 이벤트리스너, 객체 등록

    }
    virtual public void Load()
    {//해당 페이지 초기화하기

    }
    public void Close()
    {
        Init();

        //초기화
        Reset();

        //닫기
        MusicController.Instance.subMusicController.Reset();
        gameObject.SetActive(false);
    }
    virtual public void Reset()
    {//해당 페이지 초기화하기

    }
}
