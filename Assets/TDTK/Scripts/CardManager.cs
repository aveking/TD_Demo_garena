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
            myCards.Add(0, new Card(0, 0, 0));
            myCards.Add(1, new Card(1, 0, 0));
            myCards.Add(2, new Card(2, 0, 0));
            myCards.Add(3, new Card(3, 0, 0));
            myCards.Add(4, new Card(4, 0, 0));
            myCards.Add(5, new Card(5, 0, 0));

            drawNum = card_setting.draw_num;
            
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

            if (drawTimes < 5)
            {
                quality = 0;
            }
            else if (drawTimes < 20)
            {
                if (num < 7)
                    quality = 0;
                else
                    quality = 1;
            }
            else if (drawTimes < 40)
            {
                if (num < 2)
                    quality = 0;
                else if (num < 6)
                    quality = 1;
                else
                    quality = 2;
            }
            else if (drawTimes < 60)
            {
                if (num < 2)
                    quality = 0;
                else if (num < 5)
                    quality = 1;
                else if (num < 9)
                    quality = 2;
                else
                    quality = 3;
            }
            else if (drawTimes < 100)
            {
                if (num < 1)
                    quality = 0;
                else if (num < 3)
                    quality = 1;
                else if (num < 7)
                    quality = 2;
                else if (num < 9)
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

            if (num < 8)
                level = 0;
            else if (num < 14)
                level = 1;
            else if (num < 17)
                level = 2;
            else if (num < 19)
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

            drawTimes++;
            drawNum--;

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
                return string.Format("再召唤{0}本魔典{1}秒，施法+{2}%", quality, 5 + level, 3 * level + 4 * quality);
            else if (CardType == 1)
                return string.Format("{0}秒内攻击塔{1}次", 3 + 2 * quality, level * 3 + (quality * 4));
            else if (CardType == 2)
                return string.Format("{0}秒内移动+{1}%", 2 + 0.8 * level, 45 + (quality * 25));
            else if (CardType == 3)
                return string.Format("获得耐久为{0}的护盾", (level + quality * 6) * 2);
            else if (CardType == 4)
                return string.Format("{0}秒内变大{1}倍,免疫攻击", 2f + 0.8f * level, 1 + 0.5f * quality);
            else
                return string.Format("弹幕定住{0}秒，炮塔沉默{1}秒", 2 + 0.8 * level, 2 + 0.8 * quality);
        }
    }

}