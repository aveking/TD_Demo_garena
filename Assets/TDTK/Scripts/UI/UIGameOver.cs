using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using TDTK;
using UnityEngine.SceneManagement;

namespace TDTK
{

    public class UIGameOver : MonoBehaviour
    {

        private GameObject thisObj;
        private CanvasGroup canvasGroup;
        private static UIGameOver instance;

        public Text lbTitle;
        public Text lbReward;

        public GameObject buttonNext;
        public GameObject buttonMenu;


        public void Awake()
        {
            instance = this;
            thisObj = gameObject;
            canvasGroup = thisObj.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = thisObj.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
            thisObj.SetActive(false);
            thisObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        }


        public void OnNextButton()
        {
            if (global_gamesetting._inst.boss_currenthp > 0)
            {
                //GameControl.LoadNextScene();
                global_gamesetting.current_stagelv++;
                card_setting.AddDrawNum();
                GameControl.RestartScene();
            }
        }
        public void OnRestartButton()
        {
            GameControl.RestartScene();
        }
        public void OnMenuButton()
        {//返回抽卡菜单
         //GameControl.LoadMainMenu();
            TDTK.OnCardMenu();
            SceneManager.LoadScene("TD_Demo_Garena_Card");
        }


        public static void Show(bool won) { instance._Show(won); }
        public void _Show(bool won)
        {
            if (won) lbTitle.text = "You Win!";
            else lbTitle.text = "You Lose!";

            // if (!(UIMainControl.AlwaysShowNextButton() || won) && buttonNext != null) 
            // {
            //     buttonNext.SetActive(false);
            // }

            if (won)
            {
                buttonMenu.SetActive(false);
            }
            else
            {
                buttonNext.SetActive(false);
            }

            lbReward.enabled = won;
            UIMainControl.FadeIn(canvasGroup, 0.25f, thisObj);
        }
        public static void Hide() { instance._Hide(); }
        public void _Hide()
        {
            UIMainControl.FadeOut(canvasGroup, 0.25f, thisObj);
        }

    }

}