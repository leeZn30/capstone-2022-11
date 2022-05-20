using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CustomSlider : Slider, IPointerUpHandler,IPointerDownHandler
{

    public delegate void PointerUpHandler(float value);
    public event PointerUpHandler OnPointUp;
    public event PointerUpHandler OnPointDown;
    override public void OnPointerUp(PointerEventData eventData)
    {
        OnPointUp(value);
    }
    override public  void OnPointerDown(PointerEventData eventData)
    {
        OnPointDown(value);
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
