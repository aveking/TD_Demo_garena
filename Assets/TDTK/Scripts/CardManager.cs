using System.Collections.Generic;
using UnityEngine;


namespace TDTK
{
    public class CardManager : MonoBehaviour
    {
        public int drawNum;
        private int drawTimes = 0;

        private Dictionary<int, Card> myCards = new Dictionary<int, Card>();

        // Start is called before the first frame update
        void Start()
        {
            drawNum = card_setting.draw_num;

            for (int i = 0; i < card_setting.CARD_NUM; i++)
            {
                myCards.Add(i, new Card(i, card_setting.cards_ql[i], card_setting.cards_lv[i]));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool AddCard(Card card, bool force = false)
        {
            if (myCards.ContainsKey(card.CardType) && !force)
                return false;

            myCards[card.CardType] = card;
            return true;
        }

        public Card GetCard(int type)
        {
            Card card = null;
            myCards.TryGetValue(type, out card);

            return card;
        }

        public int CardNum
        {
            get { return myCards.Count; }
        }


        private int GenQuality()
        {
            int quality;
            int num = Random.Range(0, 10);

            if (card_setting.draw_times < 5)
            {
                quality = 0;
            }
            else if (card_setting.draw_times < 30)
            {
                if (num < 5)
                    quality = 0;
                else
                    quality = 1;
            }
            else if (card_setting.draw_times < 70)
            {
                if (num < 2)
                    quality = 0;
                else if (num < 6)
                    quality = 1;
                else
                    quality = 2;
            }
            else if (card_setting.draw_times < 130)
            {
                if (num < 2)
                    quality = 0;
                else if (num < 5)
                    quality = 1;
                else if (num < 8)
                    quality = 2;
                else
                    quality = 3;
            }
            else if (card_setting.draw_times < 200)
            {
                if (num < 1)
                    quality = 0;
                else if (num < 3)
                    quality = 1;
                else if (num < 5)
                    quality = 2;
                else if (num < 8)
                    quality = 3;
                else
                    quality = 4;
            }
            else
            {
                if (num < 1)
                    quality = 0;
                else if (num < 2)
                    quality = 1;
                else if (num < 4)
                    quality = 2;
                else if (num < 7)
                    quality = 3;
                else if (num < 9)
                    quality = 4;
                else
                    quality = 5;
            }

            return quality;
        }

        int GenLevel()
        {
            int level;
            int num = Random.Range(0, 20);

            if (num < 6)
                level = 0;
            else if (num < 11)
                level = 1;
            else if (num < 15)
                level = 2;
            else if (num < 18)
                level = 3;
            else
                level = 4;

            return level;
        }

        public Card DrawCard()
        {
            if (drawNum <= 0)
                return null;

            Card card = new Card
            {
                CardType = Random.Range(0, 6),
                Level = GenLevel(),
                Quality = GenQuality()
            };

            card_setting.draw_times++;
            drawTimes++;
            drawNum--;

            if (drawTimes == 50)
            {
                Achievement.Draws50 = true;
                card_setting.draw_times += 10;
            }
            else if (drawTimes == 100)
            {
                Achievement.Draws100 = true;
                card_setting.draw_times += 20;
            }

            return card;
        }

        public void UpdateUICard(UICard uiCard, Card card)
        {
            if (card != null)
            {
                uiCard.imgRoot.sprite = card.GetBackground();
                uiCard.imgQuality.sprite = card.GetQuality();
                uiCard.imgLevel.sprite = card.GetLevel();
                uiCard.imgName.sprite = card.GetName();
                uiCard.label.text = card.GetDescription();
            }
        }
    }

    public class Card
    {
        public int CardType;
        public int Quality;
        public int Level;

        public Card()
        { }

        public Card(int type, int quality, int level)
        {
            CardType = type;
            Quality = quality;
            Level = level;
        }

        public Sprite GetQuality()
        {
            return Resources.Load<Sprite>(string.Format("card_ql/{0}", Quality + 1));
        }

        public Sprite GetLevel()
        {
            return Resources.Load<Sprite>(string.Format("card_lv/{0}", Level + 1));
        }

        public Sprite GetBackground()
        {
            return Resources.Load<Sprite>(string.Format("card_bg/{0}", CardType + 1));
        }

        public Sprite GetName()
        {
            return Resources.Load<Sprite>(string.Format("card_bg/{0}a", CardType + 1));
        }

        public string GetDescription()
        {
            int quality = Quality + 1;
            int level = Level + 1;

            if (CardType == 0)
                return string.Format("{0}秒内法典\n额外+{1}本", 5 + 2 * level, quality + 1);
            else if (CardType == 1)
                return string.Format("{0}秒内法典\n攻速+{1}%", 3 + 1 * level, 205 + 75 * quality);
            else if (CardType == 2)
                return string.Format("{0}秒内移动\n+{1}%", 1.25f + 0.25f * level, 60 + (quality * 20));
            else if (CardType == 3)
                return string.Format("免伤{0}点\n不叠加", (level + quality * 5) * 3);
            else if (CardType == 4)
                return string.Format("无敌{0}秒\n变大{1}倍", 2.5f + 0.5f * quality, 2.7f + 0.3f * level);
            else
                return string.Format("塔暂停{0}秒\n子弹停{1}秒",2f + 0.5f * level, 1.5f + 0.25f * quality);
        }
    }

}