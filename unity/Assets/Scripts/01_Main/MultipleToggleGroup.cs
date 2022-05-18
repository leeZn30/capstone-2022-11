using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleToggleGroup : MonoBehaviour
{
    private Toggle[] toggles;
    private bool[] tempToggles;
    private int cnt;
    // Start is called before the first frame update
    void Start()
    {
        toggles = GetComponentsInChildren<Toggle>();
        tempToggles = new bool[toggles.Length];
        tempToggles[0] = true;
        for(int i=0; i<toggles.Length; i++)
        {

            toggles[i].onValueChanged.AddListener(CheckToggle);
            if (i == 0)
                toggles[i].isOn = true;
            else
                toggles[i].isOn = false;
        }
        cnt = 1;
    }
    void CheckToggle(bool isOn)
    {
        if (isOn==false)
        {
            cnt -= 1;
            if (cnt == 1)
            {//cnt이 1가 되면(남은 토글이 1면)
                for(int i=0; i < toggles.Length; i++)
                {
                    tempToggles[i] = toggles[i].isOn;
                }
            }
            else if (cnt == 0)
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    toggles[i].isOn = tempToggles[i];
                }
            }
        }
        else {
            cnt++;
        }
    }
    public Toggle[] ActiveToggles()
    {
        return toggles;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
