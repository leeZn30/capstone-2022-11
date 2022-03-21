using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI와 관련된 스크립트 작업을 위해서 추가해 주어야 한다. 


public class ScrollViewRect : MonoBehaviour
{
    // 스크롤 뷰와 관련된 수정을 하기 위해 가지고 있는 변수 
    RectTransform rect;

    void Start () {
        rect = GetComponent<RectTransform>();
        SetContentSize();
    } 
    void SetContentSize() {
        float height = 100;
        Debug.Log(height);
        int cnt = transform.childCount;
        for(int i=0; i<cnt; i++)
        {
            height += transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta.y;
        }


        Debug.Log(height);
        // scrollRect.content를 통해서 Hierachy 뷰에서 봤던 Viewport 밑의 Content 게임 오브젝트에 접근할 수 있다. 
        // 그리고 sizeDelta 값을 통해서 Content의 높이와 넓이를 수정할 수 있다. 

        rect.sizeDelta = new Vector2(rect.sizeDelta.x,height);
    }


}
