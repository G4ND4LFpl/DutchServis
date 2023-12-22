using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    public class GameBot
    {
        Card[] cards;
        Random rand;

        public GameBot(Card[] botCards)
        {
            this.cards = botCards;

            rand = new Random();
        }

        public int LookUp(int n, int prev=0)
        {
            int card = rand.Next(0, 3-n);
            if (n != 0 && card >= prev) card++;
            return card;
        }
        public int DrawFrom(Card cardFromStack)
        {
            int maxValue = -1;
            for (int i = 0; i < 8; i++)
            {
                if (cards[i] == null) continue;
                if (cards[i].Cost() > maxValue)
                {
                    maxValue = cards[i].Cost();
                } 
            }

            if (cardFromStack.Cost() < 5 && cardFromStack.Cost() < maxValue) return 1;
            else return 0;
        }
        public int ThrownCard()
        {
            int maxValue = -1;
            int index = -1;

            for(int i = 0; i < 8; i++)
            {
                if (cards[i] == null) continue;

                if(!cards[i].BotKnown)
                {
                    return i;
                }
                if(cards[i].Cost() > maxValue)
                {
                    maxValue = cards[i].Cost();
                    index = i;
                }
            }

            return index;
        }
        public bool Dutch()
        {
            return false;
        }
        public int Dash(Card topofstack)
        {
            return -1;
        }
    }
}