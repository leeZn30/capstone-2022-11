using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SongSlot : SearchedSongSlot
{
    private bool select = false;
    public bool isSelected
    {
        get { return select; }
        set 
        {
            select = value;
            if (select)
            {
                SetImage( new Color(1.0f,0.7f, 0.7f));
            }
            else
            {
                SetImage(new Color(1,1, 1));
            }
            
            
        }
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
