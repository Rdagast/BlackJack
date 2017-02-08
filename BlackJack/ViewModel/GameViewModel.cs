using DataModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Popups;

namespace BlackJack.ViewModel
{
    class GameViewModel : BaseViewModel
    {

        public Game MyGame { get; set; }
        public User MyUser { get; set; }
        public User Bank { get; set; }
        private MessageDialog dialog;


        public GameViewModel(List<User> players, Api api)
        {
            this.MyGame = new Game(players);
            this.MyUser = api.user;
            this.Bank = new User();
        }

        public void CoreGame()
        {
            bool _run = true;
            while (_run) // run game
            {
                GetCard(Bank);
                GetCard(MyUser);

                _run = IsGameFinish();
            }
        }

        public void GetCard(User user)
        {
            user.MyCards[0].Add(MyGame.Decks[0].Cards[0]); // distribute card
            MyGame.Decks.RemoveAt(0);
        }

        public bool IsGameFinish()
        {
            List<int> bankScores = GetScore(Bank);
            List<int> MyScores = GetScore(MyUser);
            bool run = true;

            foreach (int score in MyScores)
            {
                if (MyGame.IsStop)
                {
                    foreach (int bankScore in bankScores)
                    {
                        if (score > bankScore)
                        {
                            UpdateStack(MyUser.Bet * 2.5);
                            this.dialog = new MessageDialog("vous avez gagné" + (MyUser.Bet * 2.5));
                            BadTextBox(this.dialog);
                            MyUser.Bet = 0;

                        }
                        else
                        {
                            MyUser.stack += MyUser.Bet;
                            this.dialog = new MessageDialog("égalité");
                            BadTextBox(this.dialog);
                            MyUser.Bet = 0;
                        }
                        run = false;
                    }
                }
                if (score > 21)
                {
                    UpdateStack(- MyUser.Bet);

                    this.dialog = new MessageDialog("Vous avez dépassez 21");
                    BadTextBox(this.dialog);
                    MyUser.Bet = 0;
                    run = false;


                }
            }
            return run;
        }

        public List<int> GetScore (User user)
        {
            List<int> scoreList = new List<int>();
            foreach (List<Card> listCard in user.MyCards)
            {
                int userScore = 0;

                foreach (Card card in listCard)
                {
                    userScore += card.Value;
                }
                scoreList.Add(userScore);
            }
                return scoreList;
        }

        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }

       
        public async void UpdateStack(Double earnings)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("user/"+ MyUser.email +"/stack/" + earnings);
                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
