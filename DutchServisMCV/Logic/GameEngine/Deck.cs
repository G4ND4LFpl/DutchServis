using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    class Deck
    {
        // Variables
        List<Card> deck;

        // Constructor
        public Deck()
        {
            deck = new List<Card>();

            for (int c = 0; c < 4; c++)
            {
                deck.Add(new Card("A", (CardColor)c));

                for (int i = 2; i < 11; i++)
                {
                    deck.Add(new Card(i.ToString(), (CardColor)c));
                }

                deck.Add(new Card("J", (CardColor)c));
                deck.Add(new Card("Q", (CardColor)c));
                deck.Add(new Card("K", (CardColor)c));
            }
        }

        // Methods
        internal Card Draw()
        {
            Random rand = new Random();

            int idx = rand.Next(0, deck.Count - 1);

            Card card = deck[idx];
            deck.RemoveAt(idx);
            return card;
        }
    }
}