using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float boss_maxhp = 200;//Boss的血量
    public float minion_maxhp = 15;//僕從的血量

    public uint stage_h = 37;//关卡的长度
    public uint stage_tower_cnt = 8;//关卡的炮塔数量
    public uint stage_tower_density = 64;//炮塔密度，随机性0-1024之间
    public uint stage_tower_density2 = 4;//密度的紧凑性0-1024之间

    public GameObject gameplay_main; //因为global_gamesetting要优先初始化

    public Text Boss_HP_txt;
    public MyHandCard[] obj_cards;

    static public global_gamesetting _inst;
    // Start is called before the first frame update
    void Start()
    {
        _inst = this;

        Boss_HP_txt.text = boss_maxhp.ToString("0") + "/" + boss_maxhp.ToString("0");

        //���ز��Դ���
        // int lv = 1, ql = 0; card_setting.ChangeCard(0, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 2; ql = 1; card_setting.ChangeCard(1, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 3; ql = 2; card_setting.ChangeCard(2, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 4; ql = 3; card_setting.ChangeCard(3, lv, ql, $"�˺�{lv * 2 + ql * 10}��");
        // lv = 5; ql = 4; card_setting.ChangeCard(4, lv, ql, $"�˺�{lv * 2 + ql * 10}��");

        //ˢ�µ�UI��
        for (int i = 0; i < obj_cards.Length; ++i) obj_cards[i].RefreshUI();

        gameplay_main.SetActive(true);
    }

    public void RefreshBossHP_UI(float _hp)
    {
        Boss_HP_txt.text = _hp.ToString("0") + "/" + boss_maxhp.ToString("0");
    }

    private void OnDestroy()
    {
        _inst = null;
    }


}
