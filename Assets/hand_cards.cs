using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class hand_cards : MonoBehaviour
{
    public Transform[] Deck_Cards; //active就算在手牌了
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
        //战斗中手牌区抽卡补卡系统实现：战斗初始时从卡池里随机抽3张牌到手牌区，出牌时消耗对应数量的水晶，打一张后抽补一张
        for (int i = 0; i < Deck_Cards.Length; ++i) Deck_Cards[i].gameObject.SetActive(false);
    }

    private void Update()
    {
        //自动补牌到手牌中
        AutoAddCardToHand();
        Update_CardBridge();
        Update_CardDrag();
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
        //结合CanvasScaler.cs中的参数
        float scaleFactor = Mathf.Min(Screen.width / 960f, Screen.height / 640f);
        float card_show_y = -Screen.height * 0.5f / scaleFactor - (CARD_UI_HEIGHT * 0.382f);

        //先算一条sin曲线出来
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

        //将牌按曲线摆上
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

        if (no_touch) //释放
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
        screen_pos.y -= 512f; //hard code 偏移
        MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition = screen_pos;
    }

    int[] card_costmp = { 3, 4, 2, 3, 5, 4, 7, 8, 9 };
    void DragOverCard()
    {
        //Debug.Log(MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition);
        if (MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition.y > -300f)
        {
            int card_id = MyHandCard.tf_dragone.GetComponent<MyHandCard>().card_id;
            int cost_mp = card_costmp[card_id];
            Debug.Log($"use card id = {card_id} cost={cost_mp}");
            if (mp_mana._inst.CostMP(cost_mp))
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

        magic_book.books_cnt = 1 + quality / 2;
        magic_book.books_keepcd = 5 + level * 2;
        magic_book.books_attack_rate = 100f / (100 + 5 + 5 * quality);

        //    return string.Format("召唤{0}本魔典\n作战{1}秒，攻速+{2}%", 1 + quality / 2, 5 + level * 2, 5 + 5 * quality);
    }

    public static float card1_cd = 0f;
    public static int card1_attack_cnt = 0;
    void Play_Card1()
    {
        int level = card_setting.cards_lv[1] + 1;
        int quality = card_setting.cards_ql[1] + 1;
        Debug.Log($"level = {level} quality={quality}");

        card1_cd = 3 + (level / 2);
        card1_attack_cnt = level + (quality * 6);

        //return string.Format("强力魔典持续{0}秒，最多攻击{1}次", 3 + (level / 2), level + (quality * 6));
    }

    void Play_Card2()
    {
        int level = card_setting.cards_lv[2] + 1;
        int quality = card_setting.cards_ql[2] + 1;

        //    return string.Format("{0}秒内移动+{1}%", 1 + (level / 2), 60 + (quality * 25));

    }

    void Play_Card3()
    {
        int level = card_setting.cards_lv[3] + 1;
        int quality = card_setting.cards_ql[3] + 1;

        //    return string.Format("获得{0}点护甲", (level + quality * 6) / 2);
    }

    void Play_Card4()
    {
        int level = card_setting.cards_lv[4] + 1;
        int quality = card_setting.cards_ql[4] + 1;

        //    return string.Format("{0}秒内变大{1}倍,免疫攻击", 0.5 + (0.3 * level), 2 + quality);
    }

    void Play_Card5()
    {
        int level = card_setting.cards_lv[5] + 1;
        int quality = card_setting.cards_ql[5] + 1;

        //    return string.Format("弹幕时停{0}秒，炮塔时停{1}秒", 0.1 + (0.5 * level), 0.1 + (0.5 * quality));

    }
}
