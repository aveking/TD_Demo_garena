using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class card_setting
{
    public const int CARD_NUM = 6;

    public static int[] cards_lv = new int[CARD_NUM];//�ȼ�
    public static int[] cards_ql = new int[CARD_NUM];//Ʒ��
    public static string[] cards_des = new string[CARD_NUM];//����

    public static void ChangeCard(int _idx, int _lv, int _ql, string _des)
    {
        if (_idx < 0 || _idx >= CARD_NUM) return;
        cards_lv[_idx] = _lv;
        cards_ql[_idx] = _ql;
        cards_des[_idx] = _des;
    }
}

public class global_gamesetting : MonoBehaviour
{
    public float tower_attack_rate = 1f;
    public float tower_bullet_fly_speed = 1f;
    public float boss_move_speed = 1f;

    public MyHandCard[] obj_cards;

    static public global_gamesetting _inst;
    // Start is called before the first frame update
    void Start()
    {
        _inst = this;

        //���ز��Դ���
        // int lv = 1, ql = 0; card_setting.ChangeCard(0, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 2; ql = 1; card_setting.ChangeCard(1, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 3; ql = 2; card_setting.ChangeCard(2, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 4; ql = 3; card_setting.ChangeCard(3, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 5; ql = 4; card_setting.ChangeCard(4, lv, ql, $"�˺�{lv * 2 + ql * 10}��");

        //ˢ�µ�UI��
        for (int i = 0; i < obj_cards.Length; ++i) obj_cards[i].RefreshUI();
    }

    private void OnDestroy()
    {
        _inst = null;
    }


}
