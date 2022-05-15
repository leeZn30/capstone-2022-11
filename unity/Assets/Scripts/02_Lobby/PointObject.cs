using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointObject : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{


    public delegate void PointHandler();
    public event PointHandler OnPointDown;
    public event PointHandler OnPointExit;
    public event PointHandler OnPointEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointEnter?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointExit?.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointDown?.Invoke();
    }

}
