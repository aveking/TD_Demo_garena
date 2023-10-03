using UnityEngine;
using UnityEngine.UI;



#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using System.Collections.Generic;


namespace TDTK
{

    public class CardMenu : MonoBehaviour
    {
        private const int CARD_NUM = 6;

        public List<string> lableList = new List<string>();

        public List<UICard> cards;
        public UIButton drawCardBtn;
        public UIButton startBtn;

        public Text DrawNum;

        public CardManager cardManager;

        public ReplaceCardMenu replaceCardMenu;

        public Image draws50;
        public Image draws100;
        public Image wins18;

        private int offsetX = 320; //注意这里的screen w=1920 h=1080
        private int offsetY = 380;

        // Start is called before the first frame update
        void Start()
        {
            offsetX = (int)((350 * Screen.width) / 1920);
            offsetY = (int)((400 * Screen.height) / 1080);

            lableList.Add("抽卡");
            lableList.Add("前往探险 >");

            // Draw Card Button
            drawCardBtn.Init();
            drawCardBtn.label.text = lableList[0];
            drawCardBtn.SetCallback(this.OnHoverButton, this.OnExitButton, this.OnDrawCard, null);

            // Start Button
            startBtn.Init();
            startBtn.label.text = lableList[1];
            startBtn.SetCallback(this.OnHoverButton, this.OnExitButton, this.OnStartGame, null);

            int transX = offsetX, transY = 0;
            // Init Cards
            for (int i = 0; i < CARD_NUM; i++)
            {
                if (i == 0)
                {
                    cards[0].Init();
                }
                else if (i > 0)
                {
                    cards.Add(UICard.Clone(cards[0].rootObj, "Card" + i));

                    cards[i].rootT.Translate(transX, transY, 0);

                    cards[i].rootT.localScale = new Vector3(1.2f, 1.2f);
                    transX += offsetX;
                    if (i == 2)
                    {
                        transX = 0;
                        transY = -offsetY;
                    }
                }
            }


        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < CARD_NUM; i++)
            {
                cardManager.UpdateUICard(cards[i], cardManager.GetCard(i));
            }

            DrawNum.text = string.Format("x {0}", cardManager.drawNum);

            draws50.enabled = Achievement.Draws50;
            draws100.enabled = Achievement.Draws100;
            wins18.enabled = Achievement.Wins18;
        }

        void OnDrawCard(GameObject butObj, int pointerID = -1)
        {
            Card card = cardManager.DrawCard();

            if (card == null)
            {
                drawCardBtn.button.GetComponents<AudioSource>()[0].Play();
            }
            else if (!cardManager.AddCard(card))
            {
                drawCardBtn.button.GetComponents<AudioSource>()[1].Play();
                replaceCardMenu.Show(cardManager, cardManager.GetCard(card.CardType), card);
            }

        }

        void OnStartGame(GameObject butObj, int pointerID = -1)
        {
            for (int i = 0; i < card_setting.CARD_NUM; i++)
            {
                Card card = cardManager.GetCard(i);
                card_setting.draw_num = cardManager.drawNum;
                card_setting.ChangeCard(card.CardType, card.Level, card.Quality, card.GetDescription());
            }

#if UNITY_5_3_OR_NEWER
            SceneManager.LoadScene("TD_Demo_Garena_GamePlay");
#else
			Application.LoadLevel("TD_Demo_Garena_GamePlay");
#endif
        }

        public void OnHoverButton(GameObject butObj)
        {

        }

        public void OnExitButton(GameObject butObj)
        {

        }
    }
}