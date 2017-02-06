using BlackJack.View;
using DataModel;
using Exo4.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlackJack.ViewModel
{
    class GameViewModel : BaseViewModel
    {

        public Game MyGame { get; set; }
        public User MyUser { get; set; }
        public User Bank { get; set; }
        Frame currentFrame { get { return Window.Current.Content as Frame; } }

        private MessageDialog dialog;


        public GameViewModel(User apiUser)
        {
            this.MyGame = new Game();
            this.MyUser = apiUser;
            
            // check the user has no card
            if(this.MyUser.MyCards[0] != null || this.MyUser.MyCards.Count > 1) 
            {
                this.MyUser.MyCards.RemoveRange(0, this.MyUser.MyCards.Count);
                this.MyUser.MyCards.Add(new List<Card>());
            }
           
            //set Bank player
            this.Bank = new User();

            //2 starting cards for both player
            GetCard(Bank);
            GetCard(Bank);
            GetCard(MyUser);
            GetCard(MyUser);
        }

        private ICommand newCardCommand;
        public ICommand NewCardCommand
        {
            get
            {
                if (newCardCommand == null)
                {
                    newCardCommand = newCardCommand ?? (newCardCommand = new RelayCommand(() => { DistributeCard(); }));
                }
                return newCardCommand;
            }
        }

        public void DistributeCard()
        {
            //give a card and check victory or loose
            GetCard(MyUser);
            BankPlay();
            IsGameFinish();
        }

        public void BankPlay()
        {
            foreach (var item in GetScore(Bank))
            {
                // Bank don't want to have more than 21 with the next card
                if (item <= 16) 
                    GetCard(Bank);
            }
        }

        public void GetCard(User user)
        {
            // distribute card
            user.MyCards[0].Add(MyGame.Decks[0].Cards[0]); 
            MyGame.Decks.RemoveAt(0);
        }

        public void IsGameFinish()
        {
            List<int> bankScores = GetScore(Bank);
            List<int> MyScores = GetScore(MyUser);

            foreach (int score in MyScores)
            {
                if (MyGame.IsStop)
                {
                    foreach (int bankScore in bankScores)
                    {
                        if (score > bankScore)
                        {
                            //victory
                            UpdateStack(MyUser.Bet * 2.5);
                            this.dialog = new MessageDialog("vous avez gagné" + (MyUser.Bet * 2.5));
                            BadTextBox(this.dialog);
                            MyUser.Bet = 0;
                            currentFrame.Navigate(typeof(PlayAgain), MyUser);
                        }
                        else if(score == bankScore)
                        {
                            //draw
                            MyUser.stack += MyUser.Bet;
                            this.dialog = new MessageDialog("égalité, reprenez votre mise" + MyUser.Bet);
                            BadTextBox(this.dialog);
                            MyUser.Bet = 0;
                            currentFrame.Navigate(typeof(PlayAgain), MyUser);
                        }
                        else
                        {
                            //loose
                            UpdateStack(-MyUser.Bet);

                            this.dialog = new MessageDialog("Vous avez perdu face à la banque, vous perdez :"+ MyUser.Bet);
                            BadTextBox(this.dialog);
                            MyUser.Bet = 0;
                            currentFrame.Navigate(typeof(PlayAgain), MyUser);
                        }
                    }
                }
                if (score > 21)
                {
                    //loose
                    UpdateStack(- MyUser.Bet);

                    this.dialog = new MessageDialog("Vous avez dépassez 21");
                    BadTextBox(this.dialog);
                    MyUser.Bet = 0;
                    currentFrame.Navigate(typeof(PlayAgain), MyUser);
                }
            }
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
                //send the new stack to api in GET
                client.BaseAddress = new Uri("http://demo.comte.re/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("user/"+ MyUser.email +"/stack/" + earnings);
                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(res);
                }
            }
        }

        private ICommand stopGameCommand;
        public ICommand StopGameCommand
        {
            get
            {
                if (stopGameCommand == null)
                {
                    stopGameCommand = stopGameCommand ?? (stopGameCommand = new RelayCommand(() => { StopGame(); }));
                }
                return stopGameCommand;
            }
        }

        public void StopGame()
        {
            MyGame.IsStop = true;
        }

        //private ICommand betCommand;
        //public ICommand BetCommand
        //{
        //    get
        //    {
        //        if (betCommand == null)
        //        {
        //            betCommand = betCommand ?? (betCommand = new RelayCommand(() => { Bet(); }));
        //        }
        //        return betCommand;
        //    }
        //}

        //public void Bet()
        //{

        //}
    }
}
