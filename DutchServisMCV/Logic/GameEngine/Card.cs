using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    public enum CardColor
    {
        Heart,
        Diamonds,
        Spades,
        Clubs
    }

    public class Element
    {
        public object Value { get; private set; }

        public Element(object text)
        {
            Value = text;
        }
    }
    public class Card : Element
    {
        public CardColor Color { get; private set; }
        public bool Visible { get; set; }

        public bool BotKnown { get; set; }

        public Card(string value, CardColor color) : base(value)
        {
            Color = color;
            Visible = false;
            BotKnown = false;
        }

        public int Cost()
        {
            switch (Value)
            {
                case "A": return 1;
                case "J": return 11;
                case "Q": return 12;
                case "K":
                    {
                        if (Color == CardColor.Heart || Color == CardColor.Diamonds) return 0;
                        else return 13;
                    }
                default: return Convert.ToInt32(Value);
            }
        }
    }

    public class Slot
    {
        public Element Card { get; set; }
        public bool Active { get; set; }
        public Slot(Element card, bool active)
        {
            Card = card;
            Active = active;
        }
        public Slot(bool active)
        {
            Card = null;
            Active = active;
        }
    }
}