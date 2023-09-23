using System.Collections;
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

    static public int books_cnt;// = 1 + quality / 2;
    static public float books_keepcd;// = 5 + level * 2;
    static public float books_attack_rate = 1f;// = 100f / (100 + 5 + 5 * quality);

    public GameObject[] sub_books;
    public float sub_books_cd = 0;

    public GameObject card1_obj;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CancelBlock()
    {

    }

    float cd = 2f;
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
                if (card1_obj.activeInHierarchy == false) card1_obj.SetActive(true);
            }
            else card1_obj.SetActive(false);
        }

        cd -= Time.deltaTime;
        if (cd <= 0f)
        {
            float sqr_d = 16f; //近的时候才有效
            Transform near_one = null;
            Vector3 a = transform.position;

            //找一个离自己最近的子弹，通知它自我销毁
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
                cd = 2f * books_attack_rate;

                Vector3 dir = (a - near_one.position);
                dir.y = 0;
                transform.forward = dir;
                near_one.gameObject.layer = 30;
                //Debug.LogError($"near_one.gameObject.layer = {near_one.gameObject.layer}");
            }
        }







    }
}
