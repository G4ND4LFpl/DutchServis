using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    public enum GameMode
    {
        Deal,
        Lookup,
        Start,
        Draw,
        Throw,
        Dash,
        Peek
    }

    public class GameEngine
    {
        // Variables
        Deck deck;
        Stack<Card> stack;
        Card[] playerCards, botCards;
        public GameMode Mode { get; private set; }
        int count;

        //bool deckActive = false, stackActive = false, btnActive = false;

        public GameEngine()
        {
            Mode = GameMode.Deal;
        }

        // Methods

        public void Initialize()
        {
            deck = new Deck();
            stack = new Stack<Card>();

            playerCards = new Card[8];
            botCards = new Card[8];
        }

        private int Index(string id)
        {
            return Convert.ToInt32(id.Split('_')[2]);
        }
        private bool IfCardActive(Card card, bool player)
        {
            switch (Mode)
            {
                case GameMode.Lookup:
                    {
                        return player && card != null && card.Visible != true;
                    }
                case GameMode.Throw:
                    {
                        return player && card != null;
                    }
                default: return false;
            }
        }
        private Dictionary<string, Slot> GetState()
        {
            Dictionary<string, Slot> state = new Dictionary<string, Slot>();

            for(int i=0; i<8; i++)
            {
                state.Add(
                    "player_card_" + i.ToString(), 
                    new Slot(playerCards[i], IfCardActive(playerCards[i], true))
                    );
                state.Add(
                    "bot_card_" + i.ToString(),
                    new Slot(botCards[i], IfCardActive(playerCards[i], false))
                    );
            }

            if (stack.Count != 0) state.Add(
                "stack",
                new Slot(stack.Peek(), Mode == GameMode.Draw)
                );
            else state.Add("stack", new Slot(false));

            state.Add("deck", new Slot(Mode == GameMode.Draw));

            if(Mode == GameMode.Start)
                state.Add("button", new Slot(new Element("Rozpocznij"), true));
            else if (Mode == GameMode.Dash)
                state.Add("button", new Slot(new Element("Zakończ turę"), true));
            else state.Add("button", new Slot(new Element("Null"), false));

            return state;
        }

        public EngineResponse Deal()
        {
            for(int i=0; i<8; i++)
            {
                if(i < 4)
                {
                    playerCards[i] = deck.Draw();
                    botCards[i] = deck.Draw();
                }
                else
                {
                    playerCards[i] = null;
                    botCards[i] = null;
                }
            }
            Mode = GameMode.Lookup;
            count = 2;

            return new EngineResponse
            {
                RefreshAll = true,
                RefreshDeck = true,
                Args = new string[] { },
                State = GetState(),
            };
        }
        public EngineResponse Lookup(string str_id)
        {
            playerCards[Index(str_id)].Visible = true;
            count--;

            if(count > 0) return new EngineResponse
            {
                RefreshSingle = true,
                Args = new string[] { str_id },
                State = GetState(),
            };

            Mode = GameMode.Start;
            return new EngineResponse
            {
                RefreshAll = true,
                RefreshButton = true,
                Args = new string[] { "player" },
                State = GetState(),
            };
        }
        public EngineResponse Start()
        {
            foreach(Card card in playerCards)
            {
                if (card != null) card.Visible = false;
            }

            Mode = GameMode.Draw;
            return new EngineResponse
            {
                RefreshAll = true,
                RefreshDeck = true,
                RefreshButton = true,
                Args = new string[] { "player" },
                State = GetState(),
            };
        }
        private EngineResponse Draw(Card card)
        {
            card.Visible = true;

            for(int i=0; i<8; i++)
            {
                if(playerCards[i] == null)
                {
                    playerCards[i] = card;
                    break;
                }
            }

            Mode = GameMode.Throw;
            return new EngineResponse
            {
                RefreshAll = true,
                RefreshDeck = true,
                RefreshStack = true,
                Args = new string[] { "player" },
                State = GetState(),
            };
        }
        public EngineResponse DrawDeck()
        {
            return Draw(deck.Draw());
        }
        public EngineResponse DrawStack()
        {
            return Draw(stack.Pop());
        }
        public EngineResponse Throw(string str_id)
        {
            int idx = Index(str_id);

            stack.Push(playerCards[idx]);
            playerCards[idx] = null;

            Mode = GameMode.Dash;
            return new EngineResponse
            {
                RefreshSingle = true,
                RefreshStack = true,
                RefreshButton = true,
                Args = new string[] { str_id },
                State = GetState(),
            };
        }
        public EngineResponse Dash(string str_id)
        {
            int idx = Index(str_id);
            
            if(stack.Count != 0 && stack.Peek().Value == playerCards[idx].Value)
            {
                return Throw(str_id);
            }

            // mistake
            throw new NotImplementedException();
        }
        public EngineResponse EndTurn()
        {
            throw new NotImplementedException();
        }
    }
}