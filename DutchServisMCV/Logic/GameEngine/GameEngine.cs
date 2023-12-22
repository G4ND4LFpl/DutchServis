using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DutchServisMCV.Logic.GameEngine
{
    public enum GameMode
    {
        Ready,
        Lookup,
        Start,
        Draw,
        Throw,
        AfterTurn,
        Cancel,
        Peek, // nieużywany
        Summary
    }

    internal enum DutchState
    {
        None,
        Player,
        Bot
    }

    public class GameEngine
    {
        // Variables
        Deck deck;
        Stack<Card> stack;
        Card[] playerCards, botCards;
        GameBot bot;
        public GameMode Mode { get; private set; }
        int round;
        int count;
        DutchState dutch = DutchState.None;

        public GameEngine()
        {
            Mode = GameMode.Ready;
        }
        
        // Methods

        public void Initialize()
        {
            deck = new Deck();
            stack = new Stack<Card>();

            playerCards = new Card[8];
            botCards = new Card[8];

            bot = new GameBot(botCards);
        }

        private int GetIndex(string id)
        {
            return Convert.ToInt32(id.Split('_')[2]);
        }
        private string GetId(string row, int index)
        {
            return row + "_card_" + index;
        }

        private bool IfCardActive(Card card, bool player)
        {
            switch (Mode)
            {
                case GameMode.Draw:
                    {
                        return stack.Count != 0 && player && card != null;
                    }
                case GameMode.Lookup:
                    {
                        return player && card != null && card.Visible != true;
                    }
                case GameMode.Throw:
                    {
                        return player && card != null;
                    }
                case GameMode.AfterTurn:
                    {
                        return player && card != null;
                    }
                default: return false;
            }
        }
        private int CountPoints(Card[] cards)
        {
            int points = 0;
            foreach(Card card in cards)
            {
                if(card != null) points += card.Cost();
            }
            return points;
        }
        private Dictionary<string, Slot> GetState()
        {
            Dictionary<string, Slot> state = new Dictionary<string, Slot>();

            // Cards
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

            // Stack
            if (stack.Count != 0) state.Add(
                "stack",
                new Slot(stack.Peek(), Mode == GameMode.Draw)
                );
            else state.Add("stack", new Slot(false));

            // Deck
            state.Add("deck", new Slot(Mode == GameMode.Draw));

            // Button
            if (Mode == GameMode.Start)
                state.Add("button", new Slot(new Element("Rozpocznij"), true));
            else if (Mode == GameMode.AfterTurn && (dutch == DutchState.None || count > 0))
                state.Add("button", new Slot(new Element("Zakończ turę"), true));
            else if (Mode == GameMode.AfterTurn)
                state.Add("button", new Slot(new Element("Zakończ grę"), true));
            else if (Mode == GameMode.Summary)
                state.Add("button", new Slot(new Element("Rozpocznij od nowa"), true));
            else state.Add("button", new Slot(new Element("Null"), false));


            state.Add("round", new Slot(new Element(round.ToString()), (int)Mode > 1 && Mode != GameMode.Summary));
            state.Add("dutch", new Slot(round >= 3 && dutch == DutchState.None));

            if(Mode == GameMode.Summary)
            {
                state.Add("bot_summary", new Slot(new Element(CountPoints(botCards)), true));
                state.Add("player_summary", new Slot(new Element(CountPoints(playerCards)), true));
            }

            // Return
            return state;
        }

        private EngineResponse Draw(Card card)
        {
            card.Visible = true;

            for (int i = 0; i < 8; i++)
            {
                if (playerCards[i] == null)
                {
                    playerCards[i] = card;
                    break;
                }
            }

            Mode = GameMode.Throw;
            return new EngineResponse
            {
                RefreshAllCards = true,
                Args = new string[] { "player" },
                Refresh = new Dictionary<string, bool> { { "deck", true }, { "stack", true } },
                State = GetState(),
            };
        }
        private int BotMakeTurn()
        {
            BotDraw(bot.DrawFrom(stack.Peek()), true);

            int card = bot.ThrownCard();
            stack.Push(botCards[card]);
            botCards[card] = null;
            if (dutch != DutchState.None) count--;
            BotDash();

            if (round >= 3 && dutch == DutchState.None && bot.Dutch())
            {
                dutch = DutchState.Bot;
                count = 2;
                return 1;
            }
            return 0;
        }
        private void BotDash()
        {
            int card = bot.Dash(stack.Peek());
            if (card != -1)
            {
                if (botCards[card].Value.Equals(stack.Peek().Value))
                {
                    stack.Push(botCards[card]);
                    botCards[card] = null;
                }
                else BotDraw(0, false);
            }
        }
        private void BotDraw(int source, bool canSee)
        {
            for (int i = 0; i < 8; i++)
            {
                if (botCards[i] == null)
                {
                    if (source == 0) botCards[i] = deck.Draw();
                    else botCards[i] = stack.Pop();

                    botCards[i].BotKnown = botCards[i].BotKnown || canSee;
                    break;
                }
            }
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
                RefreshAllCards = true,
                Args = new string[] { },
                Refresh = new Dictionary<string, bool>{ { "deck", true} },
                State = GetState(),
            };
        }
        public EngineResponse Lookup(string str_id)
        {
            playerCards[GetIndex(str_id)].Visible = true;
            count--;

            if(count > 0) return new EngineResponse
            {
                RefreshSingleCard = true,
                Args = new string[] { str_id },
                Refresh = new Dictionary<string, bool> { },
                State = GetState(),
            };

            int c = bot.LookUp(0);
            botCards[c].BotKnown = true;
            botCards[bot.LookUp(1, c)].BotKnown = true;

            Mode = GameMode.Start;
            return new EngineResponse
            {
                RefreshAllCards = true,
                Args = new string[] { "player" },
                Refresh = new Dictionary<string, bool> { { "button", true } },
                State = GetState(),
                Round = round,
            };
        }
        public EngineResponse Start()
        {
            foreach(Card card in playerCards)
            {
                if (card != null) card.Visible = false;
            }

            Mode = GameMode.Draw;
            round = 1;

            return new EngineResponse
            {
                RefreshAllCards = true,
                Args = new string[] { "player" },
                Refresh = new Dictionary<string, bool> { { "deck", true }, { "button", true }, { "round", true } },
                State = GetState(),
                Round = round,
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
            int idx = GetIndex(str_id);

            stack.Push(playerCards[idx]);
            playerCards[idx] = null;
            stack.Peek().BotKnown = true;

            string visibleCard = null;
            for(int i = 0; i < 8; i++)
            {
                if (playerCards[i] != null && playerCards[i].Visible == true)
                {
                    playerCards[i].Visible = false;
                    visibleCard = "player_card_" + i.ToString();
                    break;
                }
            }

            Mode = GameMode.AfterTurn;
            if (dutch != DutchState.None) count--;

            Dictionary<string, bool> refresh = new Dictionary<string, bool> { { "stack", true }, { "button", true } };
            if (round == 3) refresh.Add("dutch", true);

            string[] args;
            if (visibleCard != null) args = new string[] { str_id, visibleCard };
            else args = new string[] { str_id };

            return new EngineResponse
            {
                RefreshSingleCard = true,
                Refresh = refresh,
                Args = args,
                State = GetState(),
            };
        }
        public EngineResponse Dash(string str_id)
        {
            int idx = GetIndex(str_id);

            if (stack.Count != 0 && stack.Peek().Value.Equals(playerCards[idx].Value))
            {
                stack.Push(playerCards[idx]);
                playerCards[idx] = null;
                stack.Peek().BotKnown = true;

                return new EngineResponse
                {
                    RefreshSingleCard = true,
                    Args = new string[] { str_id },
                    Refresh = new Dictionary<string, bool> { { "stack", true } },
                    State = GetState(),
                };
            }

            // mistake
            for (int i = 0; i < 8; i++)
            {
                if (playerCards[i] == null)
                {
                    playerCards[i] = deck.Draw();

                    return new EngineResponse
                    {
                        RefreshSingleCard = true,
                        Args = new string[] { GetId("player", i) },
                        Refresh = new Dictionary<string, bool> { },
                        State = GetState(),
                    };
                }
            }

            // too many cards
            throw new NotImplementedException();
        }
        public EngineResponse EndTurn()
        {
            BotDash();
            if (dutch != DutchState.None && count == 0)
            {
                return EndGame();
            }

            int botDutch = BotMakeTurn();

            if (dutch == DutchState.None || count > 0)
            {
                Mode = GameMode.Draw;
                round++;
            }

            Dictionary<string, bool> refresh = new Dictionary<string, bool> { { "deck", true }, { "stack", true }, { "button", true } };
            if (botDutch == 1) refresh.Add("dutch", true);
            else refresh.Add("round", true);

            return new EngineResponse
            {
                RefreshAllCards = true,
                Args = new string[] { "bot" }, //dopóki nie ma waleta
                Refresh = refresh,
                State = GetState(),
                Round = round,
            };
        }
        public EngineResponse Dutch()
        {
            dutch = DutchState.Player;
            count = 2;

            return new EngineResponse
            {
                Refresh = new Dictionary<string, bool> { { "dutch", true } },
                State = GetState(),
            };
        }
        public EngineResponse EndGame()
        {
            for(int i=0; i<8; i++)
            {
                if(botCards[i] != null) botCards[i].Visible = true;
                if(playerCards[i] != null) playerCards[i].Visible = true;
            }

            Mode = GameMode.Summary;
            return new EngineResponse
            {
                RefreshAllCards = true,
                Args = new string[] { },
                Refresh = new Dictionary<string, bool> { { "button", true }, { "round", true }, { "summary", true } },
                State = GetState(),
            };
        }
    }
}