using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using TDTK;
using UnityEngine.SceneManagement;
using System;

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
        public GameObject buttonCard;
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
                GameControl.RestartScene();
            }
        }

        public void OnRestartButton()
        {
            GameControl.RestartScene();
        }

        public void OnMenuButton()
        {
            TDTK.OnCardOrMainMenu();
            Achievement.BestRecord = global_gamesetting.current_stagelv - 1 > Achievement.BestRecord ? 
                global_gamesetting.current_stagelv - 1 : Achievement.BestRecord;
            SceneManager.LoadScene("TD_Demo_Garena_LoginMenu");
            ResetGame();
        }

        public void OnCardButton()
        {//返回抽卡菜单
         //GameControl.LoadMainMenu();
            global_gamesetting.current_stagelv++;
            TDTK.OnCardOrMainMenu();
            SceneManager.LoadScene("TD_Demo_Garena_Card");
        }

        private void ResetGame()
        {
            card_setting.Reset();
            Achievement.Reset();
            global_gamesetting.Reset();
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
                Achievement.Combo++;
                if (Achievement.Combo == 18)
                    Achievement.Wins18 = true;
                lbReward.text = "小家伙们安全到家\n获得卡包：6个";
            }
            else if (Achievement.Retries > 0)
            {
                buttonNext.SetActive(false);
                buttonMenu.SetActive(false);
                Achievement.Retries--;
                Achievement.Combo = 0;
                lbReward.text = string.Format("损失一条命\n剩余复活机会：{0}次", Achievement.Retries);
            }
            else
            {
                buttonCard.SetActive(false);
                buttonNext.SetActive(false);
                lbReward.text = string.Format("你玩完了 - {0}", global_gamesetting.current_stagelv);
            }

            card_setting.AddDrawNum(won ? 6: 1);
            
            UIMainControl.FadeIn(canvasGroup, 0.25f, thisObj);
        }
        public static void Hide() { instance._Hide(); }
        public void _Hide()
        {
            UIMainControl.FadeOut(canvasGroup, 0.25f, thisObj);
        }

    }

}