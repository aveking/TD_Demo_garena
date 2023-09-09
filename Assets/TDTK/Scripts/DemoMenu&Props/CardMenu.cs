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
        private const int CARD_NUM = 8;

        public List<string> lableList = new List<string>();
        
        public List<UICard> cards;
        public UIButton drawCardBtn;
        public UIButton startBtn;

        public CardManager cardManager;

        public List<string> levelNameList = new List<string>();
        //~ public List<string> levelDesp=new List<string>();
        public List<string> levelDespList = new List<string>();

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

            int transX = 120, transY = 0;
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
                    transX += 120;
                    if (i == 3)
                    {
                        transX = 0;
                        transY = -120;
                    }
                }
            }


        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnDrawCard(GameObject butObj, int pointerID = -1)
        {
            Card card = cardManager.DrawCard();
            UpdateUICard(cardManager.cardNum++, card);
        }

        void UpdateUICard(int index, Card card)
        {
            cards[index].imgRoot.color = card.GetColor();
            cards[index].labelType.text = card.cardType.ToString();
            cards[index].labelLevel.text = card.Level.ToString();
        }

        void OnStartGame(GameObject butObj, int pointerID = -1)
        {
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