using UnityEngine;
using UnityEngine.UI;

public static class card_setting
{
    public const int CARD_NUM = 6;

    public static int draw_num = Achievement.BestRecord > 5 ? Achievement.BestRecord : 5;

    public static int draw_times = 0;

    public static int[] cards_lv = new int[CARD_NUM];//�ȼ�
    public static int[] cards_ql = new int[CARD_NUM];//Ʒ��
    public static string[] cards_des = new string[CARD_NUM];//����

    public static void Reset()
    {
        draw_num = Achievement.BestRecord > 5 ? Achievement.BestRecord : 5;
        draw_times = Achievement.Draws50 ? 10 : 0;
        draw_times += Achievement.Draws100 ? 20 : 0;
        draw_times += Achievement.Wins18 ? 30 : 0;

        for (int i = 0; i < CARD_NUM; i++)
        {
            cards_lv[i] = 0;
            cards_ql[i] = 0;
        }
    }

    public static void AddDrawNum(int num)
    {
        draw_num += num;
    }

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
    public static uint Retries = 2;
    public static int Combo = 0;

    private static int _bestRecord = 0;
    public static int BestRecord
    {
        get
        {
            return _bestRecord;
        }
        set
        {
            _bestRecord = value;
            PlayerPrefs.SetInt("BestRecord", _bestRecord);
        }
    }

    private static bool _draws50 = false;
    public static bool Draws50
    {
        get
        {
            return _draws50;
        }
        set
        {
            _draws50 = value;
            PlayerPrefs.SetInt("Draws50", _draws50 ? 1 : 0);
        }
    }

    private static bool _draws100 = false;
    public static bool Draws100
    {
        get
        {
            return _draws100;
        }
        set
        {
            _draws100 = value;
            PlayerPrefs.SetInt("Draws100", _draws100 ? 1 : 0);
        }
    }

    private static bool _wins18 = false;
    public static bool Wins18
    {
        get
        {
            return _wins18;
        }
        set
        {
            _wins18 = value;
            PlayerPrefs.SetInt("Wins18", _wins18 ? 1 : 0);
        }
    }

    public static void Reset()
    {
        Retries = 2;
        Combo = 0;
    }

    public static void Init()
    {
        if (PlayerPrefs.HasKey("BestRecord"))
            _bestRecord = PlayerPrefs.GetInt("BestRecord");

        if (PlayerPrefs.HasKey("Draws50"))
            _draws50 = PlayerPrefs.GetInt("Draws50") > 0;

        if (PlayerPrefs.HasKey("Draws100"))
            _draws100 = PlayerPrefs.GetInt("Draws100") > 0;

        if (PlayerPrefs.HasKey("Wins18"))
            _wins18 = PlayerPrefs.GetInt("Wins18") > 0;
    }
}

public class global_gamesetting : MonoBehaviour
{
    public AudioSource boss_as;

    public AudioSource getcardsound;

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
    public Scrollbar Boss_HP_bar;
    public MyHandCard[] obj_cards;

    public float boss_currenthp = 200;//Boss的血量

    static public global_gamesetting _inst;
    // Start is called before the first frame update

    public static void Reset()
    {
        current_stagelv = 1;
    }

    void Start()
    {
        _inst = this;
        magic_book.all_Projectile.Clear();

        //Boss_HP_txt.text = boss_maxhp.ToString("0") + "/" + boss_maxhp.ToString("0");
        hand_cards.card3_armor = 0f;
        Boss_HP_txt.text = hand_cards.card3_armor.ToString("0."); //_hp.ToString("0") + "/" + boss_maxhp.ToString("0");

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
        boss_as.Stop();
        boss_as.Play();
        boss_currenthp = _hp;
        Boss_HP_txt.text = hand_cards.card3_armor.ToString("0."); //_hp.ToString("0") + "/" + boss_maxhp.ToString("0");
        Boss_HP_bar.size = _hp / boss_maxhp;
        if (_hp <= 0) Boss_HP_bar.gameObject.SetActive(false);
    }

    public void RefreshArmor()
    {
        Boss_HP_txt.text = hand_cards.card3_armor.ToString("0."); //_hp.ToString("0") + "/" + boss_maxhp.ToString("0");
    }

    int loc = 0;
    public void PlayHitEffect(Vector3 _pos)
    {
        int last = loc - 1;
        if (last < 0) last = transform.childCount - 1;
        transform.GetChild(last).gameObject.SetActive(false);
        transform.GetChild(loc).transform.position = _pos;
        transform.GetChild(loc++).gameObject.SetActive(true);
        if (loc >= transform.childCount) loc = 0;
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
