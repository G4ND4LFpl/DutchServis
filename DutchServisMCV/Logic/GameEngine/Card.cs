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
        public string Value { get; private set; }

        public Element(string text)
        {
            Value = text;
        }
    }
    public class Card : Element
    {
        public CardColor Color { get; private set; }
        public bool Visible { get; set; }

        public Card(string value, CardColor color) : base(value)
        {
            Color = color;
            Visible = false;
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