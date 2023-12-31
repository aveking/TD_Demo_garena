using System.Collections;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;

namespace TDTK
{

    public class ReplaceCardMenu : MonoBehaviour
    {
        public UICard oldUICard;
        public UICard newUICard;
        public UIButton keepBtn;
        public UIButton replaceBtn;

        private bool isDirty = false;
        private CardManager cardManager;

        private Card oldCard, newCard;

        // Start is called before the first frame update
        void Start()
        {
            oldUICard.Init();
            newUICard.Init();

            // Draw Card Button
            keepBtn.Init();
            keepBtn.SetCallback(this.OnHoverButton, this.OnExitButton, this.OnKeepCard, null);

            // Start Button
            replaceBtn.Init();
            replaceBtn.SetCallback(this.OnHoverButton, this.OnExitButton, this.OnReplaceCard, null);
        }

        // Update is called once per frame
        void Update()
        {
            if (isDirty)
            {
                cardManager.UpdateUICard(this.oldUICard, oldCard);
                cardManager.UpdateUICard(this.newUICard, newCard);
                isDirty = false;
            }
        }

        public void Show(CardManager cardManager, Card oldCard, Card newCard)
        {
            gameObject.SetActive(true);
            this.cardManager = cardManager;
            this.oldCard = oldCard;
            this.newCard = newCard;
            isDirty = true;
        }

        void OnKeepCard(GameObject butObj, int pointerID = -1)
        {
            gameObject.SetActive(false);
        }

        void OnReplaceCard(GameObject butObj, int pointerID = -1)
        {
            cardManager.GetComponent<AudioSource>().Play();
            cardManager.AddCard(newCard, true);
            gameObject.SetActive(false);
        }

        public void OnHoverButton(GameObject butObj)
        {

        }

        public void OnExitButton(GameObject butObj)
        {

        }
    }
}
