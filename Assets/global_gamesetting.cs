using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class card_setting
{
    public const int CARD_NUM = 6;

    public static int draw_num;

    public static int[] cards_lv = new int[CARD_NUM];//�ȼ�
    public static int[] cards_ql = new int[CARD_NUM];//Ʒ��
    public static string[] cards_des = new string[CARD_NUM];//����

    public static void ChangeCard(int _idx, int _lv, int _ql, string _des)
    {
        if (_idx < 0 || _idx >= CARD_NUM) return;
        cards_lv[_idx] = _lv;
        cards_ql[_idx] = _ql;
        cards_des[_idx] = _des;

        //Debug.LogError($"_idx={_idx} _lv={_lv} _ql={_ql} _des={_des}");
    }
}

public static class Achievement
{
    public static int curStage = 0;
    public static bool win5 = false;
    public static bool win10 = false;
    public static bool win20 = false;
    
}

public class global_gamesetting : MonoBehaviour
{
    public float tower_attack_rate = 1f;
    public float tower_bullet_fly_speed = 1f;
    public float boss_move_speed = 1f;

    public float boss_maxhp = 200;//Boss的血量
    public float minion_maxhp = 15;//僕從的血量

    //读不到关卡数据的时候，去读第0关
    public uint[] array_stage_h;//关卡的长度
    public uint[] array_stage_tower_cnt;//关卡的炮塔数量
    public uint[] array_stage_density;//炮塔密度，随机性0-1024之间
    public uint[] array_stage_density2;//密度的紧凑性0-1024之间

    //下面的数据由程序配置，策划别改
    public uint stage_h = 37;//关卡的长度
    public uint stage_tower_cnt = 8;//关卡的炮塔数量
    public uint stage_tower_density = 64;//炮塔密度，随机性0-1024之间
    public uint stage_tower_density2 = 4;//密度的紧凑性0-1024之间

    static public int current_stagelv = 1;//当前打到了第几个关卡
    public bool started_game = false; //是否开始了游戏
    public GameObject gameplay_main; //因为global_gamesetting要优先初始化

    public Text stage_name_txt;
    float stage_ctime = 0f; //关卡的耗时
    public Text stage_ctime_txt;

    public Text Boss_HP_txt;
    public MyHandCard[] obj_cards;

    public float boss_currenthp = 200;//Boss的血量

    static public global_gamesetting _inst;
    // Start is called before the first frame update
    void Start()
    {
        _inst = this;
        magic_book.all_Projectile.Clear();

        Boss_HP_txt.text = boss_maxhp.ToString("0") + "/" + boss_maxhp.ToString("0");

        stage_name_txt.text = "关卡" + current_stagelv;
        stage_h = GetStageSetting((uint)current_stagelv, array_stage_h);//关卡的长度
        stage_tower_cnt = GetStageSetting((uint)current_stagelv, array_stage_tower_cnt);//关卡的炮塔数量
        stage_tower_density = GetStageSetting((uint)current_stagelv, array_stage_density);//炮塔密度，随机性0-1024之间
        stage_tower_density2 = GetStageSetting((uint)current_stagelv, array_stage_density2);//密度的紧凑性0-1024之间

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

    uint GetStageSetting(uint _stage_lv, uint[] _a)
    {
        if (_a == null || _a.Length == 0) return 0;
        if (_stage_lv >= _a.Length) return _a[0];
        return _a[_stage_lv];
    }

    public void RefreshBossHP_UI(float _hp)
    {
        boss_currenthp = _hp;
        Boss_HP_txt.text = _hp.ToString("0") + "/" + boss_maxhp.ToString("0");
    }

    private void Update()
    {
        if (started_game)
        {
            stage_ctime += Time.deltaTime;
            stage_ctime_txt.text = stage_ctime.ToString("0.") + "秒";
        }
    }

    private void OnDestroy()
    {
        _inst = null;
    }


}
