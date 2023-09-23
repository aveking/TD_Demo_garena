using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;



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

        public List<string> levelNameList = new List<string>();
        //~ public List<string> levelDesp=new List<string>();
        public List<string> levelDespList = new List<string>();


        private int offsetX = 320;
        private int offsetY = 360;
        // Start is called before the first frame update
        void Start()
        {
            lableList.Add("抽卡");
            lableList.Add("前往探险 >");

            levelNameList.Add("TD_Demo_Garena_GamePlay");
            levelDespList.Add("Description - Replace me");

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
                    //cards[i].rootT.localScale = new Vector3(1.5f, 1.5f);
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
                card_setting.ChangeCard(card.CardType, card.Level, card.Quality, card.GetDescription());
            }

#if UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(levelNameList[0]);
#else
			Application.LoadLevel(levelNameList[0]);
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