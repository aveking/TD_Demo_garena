using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyHandCard : MonoBehaviour, IPointerDownHandler
{
    public int card_id;
    public bool draging = false;
    public static Transform tf_dragone;
    public void OnPointerDown(PointerEventData e)
    {
        tf_dragone = transform;
    }
}