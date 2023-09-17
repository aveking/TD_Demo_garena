using UnityEngine;
using UnityEngine.UI;

public class mp_mana : MonoBehaviour
{
    static public mp_mana _inst;
    internal bool start_game = false;
    float get_mana_cd = 0f;
    int mana_cnt = 0;

    void Start()
    {
        _inst = this;
    }

    public float mana_add_damage()
    {
        if (mana_cnt <= 5) return 0f;
        else return (mana_cnt - 5) * 0.1f;
    }

    public bool CostMP(int _v)
    {
        if (_v <= mana_cnt)
        {
            mana_cnt -= _v;
            transform.GetChild(1).GetComponent<Text>().text = mana_cnt.ToString();
            return true;
        }
        return false;
    }

    private void Update()
    {
        //√ø√Î‘ˆº”1
        if (_inst.start_game && mana_cnt < 10)
        {
            get_mana_cd += Time.deltaTime;
            float get_mana_maxcd = Mathf.Pow(1.2f, mana_cnt);
            if (get_mana_cd >= get_mana_maxcd)
            {
                get_mana_cd = 0f;
                mana_cnt++;
                transform.GetChild(0).GetComponent<Image>().fillAmount = 0f;
                transform.GetChild(1).GetComponent<Text>().text = mana_cnt.ToString();
            }
            else
            {
                transform.GetChild(0).GetComponent<Image>().fillAmount = get_mana_cd / get_mana_maxcd;
            }
        }
    }

    private void OnDestroy()
    {
        _inst = null;
    }

}
