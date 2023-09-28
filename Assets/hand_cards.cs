using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class hand_cards : MonoBehaviour
{
    public Transform[] Deck_Cards; //active������������
    float all_width = 296f;
    float two_width = 192f;
    float card_rot = 4f;
    float card_sinh = 22f;
    const int MAXNUM_HANDCARDS = 3;
    Vector3[] target_pos_a = new Vector3[MAXNUM_HANDCARDS];
    int handcard_t = 0;
    const float CARD_UI_HEIGHT = -64f;

    private void Start()
    {
        //ս�����������鿨����ϵͳʵ�֣�ս����ʼʱ�ӿ����������3���Ƶ�������������ʱ���Ķ�Ӧ������ˮ������һ�ź�鲹һ��
        for (int i = 0; i < Deck_Cards.Length; ++i) Deck_Cards[i].gameObject.SetActive(false);
    }

    private void Update()
    {
        //�Զ����Ƶ�������
        AutoAddCardToHand();
        Update_CardBridge();
        Update_CardDrag();

        if (card2_speed_cd > 0f)
        {
            card2_speed_cd -= Time.deltaTime;
            if (card2_speed_cd <= 0f) card2_speed_rate = 1f;
        }

        if (card4_cd > 0f)
        {
            card4_cd -= Time.deltaTime;
            if (card4_cd <= 0f) card4_bigger = 1f;
        }

        if (card5_stop_cd > 0f) card5_stop_cd -= Time.deltaTime;
        if (card5_tower_stop_cd > 0f) card5_tower_stop_cd -= Time.deltaTime;
    }

    private void OnDestroy()
    {

    }

    float add_cd = 0f;
    void AutoAddCardToHand()
    {
        add_cd += Time.deltaTime;
        if (add_cd >= 0.0625f)
        {
            add_cd = 0f;
            if (handcard_t < 3)
            {
                int pos = Random.Range(0, 100) % Deck_Cards.Length;
                if (Deck_Cards[pos].gameObject.activeSelf == false)
                {
                    Deck_Cards[pos].gameObject.SetActive(true);
                    ++handcard_t;
                }
            }
        }
    }

    void Update_CardBridge()
    {
        //���CanvasScaler.cs�еĲ���
        float scaleFactor = Mathf.Min(Screen.width / 960f, Screen.height / 640f);
        float card_show_y = -Screen.height * 0.5f / scaleFactor - (CARD_UI_HEIGHT * 0.382f);

        //����һ��sin���߳���
        if (handcard_t == 1)
        {
            target_pos_a[0] = new Vector3(0f, card_show_y, 0f);// Vector3.zero;
        }
        else
        {
            float min_width = all_width / (MAXNUM_HANDCARDS - 1);
            float cur_width = two_width - (two_width - min_width) * (handcard_t - 2) / (MAXNUM_HANDCARDS - 2);
            float Left_Start = (1 - handcard_t) * (cur_width * 0.5f) - 256f;
            float cardpos_percent = handcard_t > 1 ? Mathf.PI / (handcard_t - 1) : Mathf.PI;
            for (int i = handcard_t - 1; i >= 0; --i, Left_Start += cur_width)
            {
                target_pos_a[i] = new Vector3(-Left_Start, Mathf.Sin(cardpos_percent * i) * card_sinh + card_show_y, 0f);
            }
        }

        //���ư����߰���
        float start_rot = (1 - handcard_t) * card_rot * 0.5f;
        for (int i = handcard_t - 1, j = Deck_Cards.Length - 1; i >= 0; --i, start_rot += card_rot)
        {
            for (; j >= 0; --j)
            {
                if (Deck_Cards[j].gameObject.activeSelf == true)
                {
                    Deck_Cards[j].localPosition = Vector3.Lerp(Deck_Cards[j].localPosition, target_pos_a[i], Mathf.Min(1f, Time.deltaTime * 4f));
                    Deck_Cards[j].localRotation = Quaternion.Euler(0, 0, start_rot);

                    --j;
                    break;
                }

            }

        }
    }

    void Update_CardDrag()
    {
        bool no_touch = false;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount == 0) no_touch = true;
#else
        if (Input.GetMouseButton(0) == false) no_touch = true;
#endif

        if (no_touch) //�ͷ�
        {
            if (MyHandCard.tf_dragone != null) DragOverCard();
            MyHandCard.tf_dragone = null;
        }
        else
        {
            if (MyHandCard.tf_dragone != null) DragingCard();
        }
    }

    void DragingCard()
    {
        float scaleFactor = Mathf.Min(Screen.width / 960f, Screen.height / 640f);
        Vector3 card_line_start_offset = new Vector3(0f, (CARD_UI_HEIGHT * 0.5f) * scaleFactor, 0f);
        Vector3 screen_pos = Input.mousePosition;
        screen_pos.x /= scaleFactor;
        screen_pos.y /= scaleFactor;
        screen_pos.z = 0f;

        MyHandCard.tf_dragone.localRotation = Quaternion.identity;
        screen_pos.y -= 512f; //hard code ƫ��
        MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition = screen_pos;
    }

    public static int[] card_costmp = { 3, 4, 2, 3, 5, 4, 7, 8, 9 };
    void DragOverCard()
    {
        //Debug.Log(MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition);
        if (MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition.y > -300f)
        {
            int card_id = MyHandCard.tf_dragone.GetComponent<MyHandCard>().card_id;
            int cost_mp = card_costmp[card_id];
            Debug.Log($"use card id = {card_id} cost={cost_mp}");
            bool result = mp_mana._inst.CostMP(cost_mp);
            TDTK.TDTK.OnPlayCard(result);
            if (result)
            {
                switch (card_id)
                {
                    case 0: Play_Card0(); break;
                    case 1: Play_Card1(); break;
                    case 2: Play_Card2(); break;
                    case 3: Play_Card3(); break;
                    case 4: Play_Card4(); break;
                    case 5: Play_Card5(); break;
                }

                MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000f, -500f);
                MyHandCard.tf_dragone.gameObject.SetActive(false);
                --handcard_t;
            }
        }
    }

    void Play_Card0()
    {
        int level = card_setting.cards_lv[0] + 1;
        int quality = card_setting.cards_ql[0] + 1;

        magic_book.books_cnt = quality;
        magic_book.books_keepcd = 4 + level;
        magic_book.books_attack_rate = 100f / (100 + 3 * level + 4 * quality);

        //    return string.Format("�ٻ�{0}��ħ��\n��ս{1}�룬����+{2}%", quality, 4 + level, 3 * level + 4 * quality;
    }

    public static float card1_cd = 0f;
    public static int card1_attack_cnt = 0;
    void Play_Card1()
    {
        int level = card_setting.cards_lv[1] + 1;
        int quality = card_setting.cards_ql[1] + 1;
        Debug.Log($"level = {level} quality={quality}");

        card1_cd = 3f + 2f * quality;
        card1_attack_cnt = (level * 3) + (quality * 4);

        //return string.Format("ǿ��ħ�����{0}�룬��๥��{1}��", 3f + 2f * quality, (level * 3)  + (quality * 4));
    }

    public static float card2_speed_cd = 0f;
    public static float card2_speed_rate = 1f;
    void Play_Card2()
    {
        int level = card_setting.cards_lv[2] + 1;
        int quality = card_setting.cards_ql[2] + 1;

        card2_speed_cd = 2f + 0.8f * level;
        card2_speed_rate = (100 + 45 + (quality * 25)) / 100f;
        //    return string.Format("{0}�����ƶ�+{1}%",2 + 0.8f * level, 45 + (quality * 25));
    }

    public static float card3_armor = 0f;
    void Play_Card3()
    {
        int level = card_setting.cards_lv[3] + 1;
        int quality = card_setting.cards_ql[3] + 1;

        card3_armor = (level + quality * 6) * 2;
        global_gamesetting._inst.RefreshArmor();
        //    return string.Format("���{0}�㻤��", (level + quality * 6) * 2;
    }

    public static float card4_cd = 0f;
    public static float card4_bigger = 1f;
    void Play_Card4()
    {
        int level = card_setting.cards_lv[4] + 1;
        int quality = card_setting.cards_ql[4] + 1;

        //    return string.Format("{0}���ڱ��{1}��,���߹���", 0.5  2f + 0.8f * level, 3 + quality);

        card4_cd = 2f + 0.8f * level;
        card4_bigger = 1f + (0.5f * quality);
    }

    public static float card5_stop_cd = 0f;
    public static float card5_tower_stop_cd = 0f;
    void Play_Card5()
    {
        int level = card_setting.cards_lv[5] + 1;
        int quality = card_setting.cards_ql[5] + 1;
        card5_stop_cd = 2f + 0.8f * level;
        card5_tower_stop_cd = 2f + 0.8f * quality;
        //    return string.Format("��Ļʱͣ{0}�룬����ʱͣ{1}��", 2 + (0.8 * level), 2f + (0.8 * quality));
    }
}
