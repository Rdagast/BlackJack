﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Table
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("max_seat")]
        public int Max_seat { get; set; }
        [JsonProperty("seats_available")]
        public int Seats_available { get; set; }
        [JsonProperty("min_bet")]
        public Double Min_bet { get; set; }
        [JsonProperty("last_activity")]
        public DateTime Last_activity { get; set; }
        [JsonProperty("is_closed")]
        public Double Is_closed { get; set; }
        [JsonProperty("created_at")]
        public DateTime Created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime Updated_at { get; set; }

        public Deck Deck { get; set; }

        public Table()
        {
            this.Id = 0;
            this.Max_seat = 0;
            this.Seats_available = 0;
            this.Min_bet = 0;
            this.Last_activity = new DateTime();
            this.Is_closed = 0;
            this.Created_at = new DateTime();
            this.Updated_at = new DateTime();

            CreateGameDeck();
        }

        public void CreateGameDeck()
        {
            List<Deck> decks = new List<Deck>();
            for (int i = 0; i < 6; i++)
            {
                decks.Add(new Deck());
            }
            foreach (Deck d in decks)
            {
                d.ShuffleList();
            }
            decks[2].AddCutCard(); // add cut card between 50% and 80% of game Cards

            this.Deck = new Deck();
            this.Deck.Cards.Clear();
            foreach (var deck in decks)
            {
                foreach (var card in deck.Cards)
                {
                    this.Deck.Cards.Add(card);
                }
            }
        }

    }
}
