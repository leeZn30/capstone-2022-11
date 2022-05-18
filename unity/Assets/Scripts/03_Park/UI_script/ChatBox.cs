using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBox : MonoBehaviour
{
    [SerializeField] RectTransform imageBG;
    [SerializeField] TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        setChatBoxSize();
    }

    public void setChatBoxSize()
    {
        var rectSize = imageBG.sizeDelta;

        rectSize.y = text.preferredHeight + 30;

        imageBG.sizeDelta = rectSize;
    }
}
