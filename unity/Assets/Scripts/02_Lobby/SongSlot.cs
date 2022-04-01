using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SongSlot : SearchedSongSlot
{
    private Image backImage;
    private bool select = false;
    public bool isSelected
    {
        get { return select; }
        set 
        {
            select = value;
            if (select)
            {
                backImage.color = new Color(1.0f,0.7f, 0.7f);
            }
            else
            {
                backImage.color = new Color(1,1, 1);
            }
            
            
        }
    }
    private void Awake()
    {
        backImage = GetComponent<Image>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
