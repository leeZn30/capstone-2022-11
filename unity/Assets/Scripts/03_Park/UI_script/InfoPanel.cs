using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] Button cofirmButton;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] string[] infoContents;

    private void Start()
    {
        cofirmButton.onClick.AddListener(delegate { gameObject.SetActive(false); });
    }

    public void showInfoPanel(int type)
    {
        text.text = infoContents[type];
        gameObject.SetActive(true);
    }

}
