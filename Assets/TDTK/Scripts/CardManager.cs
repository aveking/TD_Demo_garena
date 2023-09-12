using System.Collections.Generic;
using UnityEngine;


namespace TDTK
{
    public enum CardType
    {
        Type1,
        Type2,
        Type3,
        Type4,
        Type5,
        Type6,
        Type7,
        Type8
    }

    public class CardManager : MonoBehaviour
    {
        public int drawNum = 10;

        private int drawTimes = 0;

        private Dictionary<CardType, Card> myCards = new Dictionary<CardType, Card>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool AddCard(Card card, bool force = false)
        {
            if (myCards.ContainsKey(card.cardType) && !force)
                return false;

            myCards[card.cardType] = card;
            return true;
        }

        public Card GetCard(CardType type)
        {
            Card card = null;
            myCards.TryGetValue(type,out card);

            return card;
        }

        public int CardNum
        {
            get { return myCards.Count; }
        }


        private int GenQuality()
        {
            int quality = 0;
            int num = Random.Range(0, 10);

            if (drawTimes < 10)
            {
                quality = 0;
            }
            else if (drawTimes<20)
            {
                if (num < 7)     
                    quality = 0;
                else
                    quality = 1;
            }
            else if (drawTimes<50)
            {
                if (num < 2)
                    quality = 0;
                else if (num<6)
                    quality = 1;
                else
                    quality = 2;
            }
            else if (drawTimes < 100)
            {
                if (num < 2)
                    quality = 0;
                else if (num < 5)
                    quality = 1;
                else if (num< 9 )
                    quality = 2;
                else
                    quality = 3;
            }
            else if (drawTimes < 150)
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
            int level = 0;
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
            Card card = new Card
            {
                cardType = (CardType)Random.Range(0, 8),
                Level = GenLevel(),
                Quality = GenQuality()
            };

            drawTimes++;

            return card;
        }

        public void UpdateUICard(UICard uiCard, Card card)
        {
            if (card != null)
            {
                uiCard.imgRoot.color = card.GetColor();
                uiCard.labelType.text = card.cardType.ToString();
                uiCard.labelLevel.text = card.Level.ToString();
            }
        }
    }

    public class Card
    {
        private static List<Color> cardColors = new List<Color>()
        {Color.white, Color.green, Color.blue, Color.cyan, Color.yellow, Color.red};

        public CardType cardType;
        public int Quality;
        public int Level = 0;

        public Color GetColor()
        {
            return cardColors[Quality];
        }
    }

}