using System.Collections.Generic;
using UnityEngine;

public class magic_book : MonoBehaviour
{
    static public Dictionary<Transform, magic_book> all_Projectile = new Dictionary<Transform, magic_book>();
    static public void Push_One(Transform _tf)
    {
        if (!all_Projectile.ContainsKey(_tf)) all_Projectile.Add(_tf, null);
    }
    static public void Pop_One(Transform _tf)
    {
        if (all_Projectile.ContainsKey(_tf))
        {
            if (all_Projectile[_tf] != null) all_Projectile[_tf].CancelBlock();
            all_Projectile.Remove(_tf);
        }
    }

    static public int books_cnt; 
    static public float books_keepcd;
    static public float books_speedup_rate = 1f;
    static public float books_speedup_cd;
    public GameObject[] sub_books;
    public float sub_books_cd = 0;

    public GameObject card1_obj;
    public bool card1_enable = false;

    float speedup_cd;
    float cd = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CancelBlock()
    {

    }

    void Update()
    {
        if (sub_books_cd > 0f)
        {
            sub_books_cd -= Time.deltaTime;
            if (sub_books_cd <= 0f) for (int i = 0; i < sub_books.Length; ++i) sub_books[i].gameObject.SetActive(false);
        }

        if (sub_books != null)
        {
            if (books_keepcd > 0)
            {
                sub_books_cd = books_keepcd;
                books_keepcd = 0f;
                for (int i = 0; i < sub_books.Length && i < books_cnt; ++i) sub_books[i].gameObject.SetActive(true);
            }
        }

        if (hand_cards.card1_cd > 0)
        {
            hand_cards.card1_cd -= Time.deltaTime;
            if (hand_cards.card1_cd > 0)
            {
                if (card1_enable == false) card1_obj.SetActive(card1_enable = true);
            }
            else card1_obj.SetActive(card1_enable = false);
        }

        // Card1 will trigger a cd reset
        if (books_speedup_cd > 0)
        {
            speedup_cd = books_speedup_cd;
            books_speedup_cd = 0;
            cd = 0f;
        }
        else if (speedup_cd > 0)
        {
            speedup_cd -= Time.deltaTime;
        }
        else
        {
            books_speedup_rate = 1f;
        }

        cd -= Time.deltaTime;
        if (cd <= 0f)
        {
            float sqr_d = 16f; //����ʱ�����Ч
            Transform near_one = null;
            Vector3 a = transform.position;

            //��һ�����Լ�������ӵ���֪ͨ����������
            foreach (Transform tf in all_Projectile.Keys)
            {
                float d = (a - tf.position).sqrMagnitude;
                if (sqr_d > d)
                {
                    sqr_d = d;
                    near_one = tf;
                }
            }

            if (near_one != null)
            {
                cd = 2f * books_speedup_rate;

                Vector3 dir = a - near_one.position;
                dir.y = 0;
                transform.forward = dir;
                near_one.gameObject.layer = 30;
                //Debug.LogError($"near_one.gameObject.layer = {near_one.gameObject.layer}");
            }
        }
    }
}