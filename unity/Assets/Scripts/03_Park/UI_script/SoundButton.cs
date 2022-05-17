using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    [SerializeField] private GameObject soundSlide;
    public bool isSoundSlideShown = false;

    public void showSlide()
    {
        if (GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable)
        {
            if (!isSoundSlideShown)
            {
                soundSlide.gameObject.SetActive(true);
                isSoundSlideShown = true;
            }
            else
            {
                soundSlide.gameObject.SetActive(false);
                isSoundSlideShown = false;
            }
        }
    }
}
