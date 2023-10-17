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

        public GameObject imgDraws50;
        public GameObject imgDraws100;
        public GameObject imgWins18;

        public ParticleSystem particles;

        private int offsetX = 320; //注意这里的screen w=1920 h=1080
        private int offsetY = 380;

        // Start is called before the first frame update
        void Start()
        {
            offsetX = (int)((35 * Screen.width) / 1920);
            offsetY = (int)((40 * Screen.height) / 1080);

            lableList.Add("抽卡");
            lableList.Add("冲塔");

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

            imgDraws50.SetActive(Achievement.Draws50);
            imgDraws100.SetActive(Achievement.Draws100);
            imgWins18.SetActive(Achievement.Wins18);
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
                particles.Play();
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