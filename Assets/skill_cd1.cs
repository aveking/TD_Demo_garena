using UnityEngine;
using UnityEngine.UI;

public class skill_cd1 : MonoBehaviour
{
    static public skill_cd1 _inst;
    float remaining_time = 0f;
    float total_time = 0;

    void Start()
    {
        _inst = this;
        gameObject.SetActive(false);
    }

    public void StartCounting(float time)
    {
        gameObject.SetActive(true);
        total_time = time;
        remaining_time = time;
    }

    private void Update()
    {
        //ÿ������1
        if (remaining_time > 0)
        {
            remaining_time -= Time.deltaTime;

            transform.GetChild(0).GetComponent<Image>().fillAmount = remaining_time / total_time;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        _inst = null;
    }

}
