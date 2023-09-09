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
        public int cardNum = 0;
        public int drawNum = 10;

        private List<Card> myCards = new List<Card>();

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public Card DrawCard()
        {
            Card card = new Card
            {
                cardType = (CardType)Random.Range(0, 8),
                Quality = Random.Range(0, 6),
                Level = Random.Range(0, 6)
            };

            return card;
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