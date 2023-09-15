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


    //    public MyHand_CardMgr(hxh_teamrushboss _parent, Transform tfmy_hand_one, Transform _fx)
    //    {
    //        parent = _parent;
    //        tfcurhand_card_fx = _fx;
    //        Transform my_hand_parent = tfmy_hand_one.parent;
    //        for (int i = 0; i < hxh_gamelogic.MAXNUM_HANDCARDS; i++)
    //        {
    //            Transform tf = GameObject.Instantiate(tfmy_hand_one).transform;
    //            tf.SetParent(my_hand_parent);
    //            tf.localScale = Vector3.one * CARD_SCALE;
    //            handcard_a[i] = tf;
    //        }
    //    }

    //    public void Update()
    //    {
    //        Update_CardBridge();
    //        Update_CardDrag();
    //        Update_Card2Graveyard();
    //    }

    //    public void MyTurn_End()
    //    {
    //        MyHandCard.tf_dragone = null;
    //        tfcurhand_card_fx.position = new Vector3(0f, 65535f, 0f);
    //    }

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
        //screen_pos.y += CARD_UI_HEIGHT * 0.25f; //�ֻ�����ץ����һЩ����Ȼ��ָ��ס����
        screen_pos.y -= 512f; //hard code ƫ��
        MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition = screen_pos;
        //tfcurhand_card_fx.position = MyHandCard.tf_dragone.position;

        //if (MyHandCard.drag_cardsheet.target_monster_cnt > 0)
        //{
        //    //������ص�ѡ����
        //    float h = Screen.height;
        //    float step = h / 12f;
        //    float y = Input.mousePosition.y;// transform.position.y;
        //    if (y > step * 1f && y < step * 7f)
        //    {
        //        int target_pos = 5 - (int)Mathf.Floor((y - step * 1f) / step);
        //        MyHandCard.output_handler.Selecting_MonsterTarget(target_pos, MyHandCard.tf_dragone.position + card_line_start_offset);
        //    }
        //    else MyHandCard.output_handler.Selecting_MonsterTarget(-1, MyHandCard.tf_dragone.position + card_line_start_offset);
        //}
    }

    void DragOverCard()
    {
        //Debug.Log(MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition);
        if (MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition.y > -300f)
        {
            Debug.Log("use card id = " + MyHandCard.tf_dragone.GetComponent<MyHandCard>().card_id);
            MyHandCard.tf_dragone.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000f, -500f);
            MyHandCard.tf_dragone.gameObject.SetActive(false);
            --handcard_t;
        }
    }
}
