using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Page : MonoBehaviour
{//상속용 클래스
    public Button exitBtn;
    // Start is called before the first frame update
    void Awake()
    {
        exitBtn.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Open()
    {
        gameObject.SetActive(true);

    }
    public void Close()
    {
        //초기화
        Reset();
        //닫기
        gameObject.SetActive(false);
    }
    virtual public void Reset()
    {//해당 페이지 초기화하기

    }
}
