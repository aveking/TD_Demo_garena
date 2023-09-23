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
                cd = 2f;

                Vector3 dir = (a - near_one.position);
                dir.y = 0;
                transform.forward = dir;
                near_one.gameObject.layer = 30;
                //Debug.LogError($"near_one.gameObject.layer = {near_one.gameObject.layer}");
            }
        }







    }
}
