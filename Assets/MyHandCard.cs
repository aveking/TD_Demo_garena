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

    public void RefreshUI()
    {
        if (card_id < 0 || card_id >= card_setting.CARD_NUM) return;
        int lv = card_setting.cards_lv[card_id];
        int ql = card_setting.cards_ql[card_id];
        string des = card_setting.cards_des[card_id];

        for (int i = 0; i < transform.GetChild(0).childCount; ++i) transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        for (int i = 0; i < transform.GetChild(1).childCount; ++i) transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        if (ql >= 0 && ql < transform.GetChild(0).childCount) transform.GetChild(0).GetChild(ql).gameObject.SetActive(true);
        if (lv >= 0 && lv < transform.GetChild(1).childCount) transform.GetChild(1).GetChild(lv).gameObject.SetActive(true);

        transform.GetChild(3).GetComponent<Text>().text = des;
    }
}