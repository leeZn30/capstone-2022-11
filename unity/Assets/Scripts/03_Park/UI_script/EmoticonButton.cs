using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoticonButton : MonoBehaviour
{
    // 0 - 메인 / 1 - 서브
    [SerializeField] private int mode = 0;

    [SerializeField] GameObject emojiPanel;

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(showEmoticons);
    }

    public void showEmoticons()
    {
        if ((GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable && mode == 0) || mode == 1)
        {
            if (!emojiPanel.active && gameObject.GetComponent<EmoticonButton>() != null)
            {
                emojiPanel.SetActive(true);
            }

            else if (emojiPanel.active && gameObject.GetComponent<EmoticonButton>() != null)
            {
                emojiPanel.gameObject.SetActive(false);
            }
        }
    }

}
