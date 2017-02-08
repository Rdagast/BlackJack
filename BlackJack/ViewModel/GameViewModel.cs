using BlackJack.View;
using DataModel;
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
        #region Properties
        public Game MyGame { get; set; }
        public Api api { get; set; }
        public User MyUser { get; set; }
        public User Bank { get; set; }
        Frame currentFrame { get { return Window.Current.Content as Frame; } }
        private int _indexList;

        private MessageDialog dialog;
        #endregion

        public GameViewModel(Api api)
        {
            this.api = api;
            this.MyGame = new Game();
            this.MyUser = this.api.user;
            this._indexList = 0;

            // guive a new hand
            this.MyUser.UserHands.Add(new UserHand());

            //set Bank player
            this.Bank = new User();
            this.Bank.UserHands.Add(new UserHand());

            //2 starting cards for both player
            GetCard(Bank.UserHands[0]);
            GetCard(Bank.UserHands[0]);
            GetCard(MyUser.UserHands[0]);
            GetCard(MyUser.UserHands[0]);

        }

        #region NewCard
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
            GetCard(MyUser.UserHands[this._indexList]);


            BankPlay();

            if (IsGameFinish())
                WhoIsTheWinner();

            if (MyUser.UserHands.Count > 1)
                if (this._indexList != MyUser.UserHands.Count - 1)
                    this._indexList++;
                else
                    this._indexList = 0;
        }

        public void BankPlay()
        {
            foreach (var item in GetScore(Bank))
            {
                // Bank don't want to have more than 21 with the next card
                if (item <= 16)
                    GetCard(Bank.UserHands[0]);
            }
        }

        public void GetCard(UserHand userHand)
        {
            // distribute card
            userHand.Cards.Add(this.MyGame.Decks[0].Cards[0]);
            MyGame.Decks.RemoveAt(0);
        }

        public bool IsGameFinish()
        {
            bool IsFinish = true;
            List<bool> checks = new List<bool>();
            foreach (var playerItem in MyUser.UserHands)
            {
                if (playerItem.IsFinish == true)
                    checks.Add(true);
                else
                    checks.Add(false);
            }
            foreach (var bankItem in Bank.UserHands)
            {
                if (bankItem.IsFinish == true)
                    checks.Add(true);
                else
                    checks.Add(false);
            }

            foreach (var item in checks)
            {
                if (item == false)
                    IsFinish = false;
            }
            return IsFinish;
        }

        public void WhoIsTheWinner()
        {
            UserHand winnerHand = new UserHand();
            MyGame.Winner = Bank;
            foreach (var playerItem in MyUser.UserHands)
            {
                foreach (var winnerItem in MyGame.Winner.UserHands)
                {
                    if (playerItem.Value > winnerItem.Value)
                    {
                        MyGame.Winner = MyUser;
                        winnerHand = playerItem;
                    }
                    else if (playerItem.Value == winnerItem.Value)
                    {
                        MyGame.Winner = MyUser;
                        winnerHand = playerItem;
                    }
                }                 
            }
            foreach (var bankItem in Bank.UserHands)
            {
                foreach (var winnerItem in MyGame.Winner.UserHands)
                {
                    if (bankItem.Value > winnerItem.Value)
                    {
                        MyGame.Winner = Bank;
                        winnerHand = bankItem;
                    }
                    else if (bankItem.Value == winnerItem.Value)
                    {
                        MyGame.Winner = Bank;
                        winnerHand = bankItem;
                    }
                }
            }
            EndGame(winnerHand);
        }

        public void EndGame(UserHand winnerHand)
        {
            if(MyGame.Winner == MyUser)
            {
                UpdateStack(winnerHand.Bet * 2.5);
                this.dialog = new MessageDialog("win");
                BadTextBox(this.dialog);
                currentFrame.Navigate(typeof(GameView), null);
            }
            else if(MyGame.Winner == Bank)
            {
                UpdateStack(-winnerHand.Bet);
                this.dialog = new MessageDialog("loose");
                BadTextBox(this.dialog);
                currentFrame.Navigate(typeof(GameView), null);
            }
        }


        public List<int> GetScore(User user)
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

                HttpResponseMessage response = await client.GetAsync("user/" + MyUser.email + "/stack/" + earnings);
                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(res);
                }
            }
        }
        #endregion

        #region StopGame
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
        #endregion

        #region SplitComand
        private ICommand splitCommand;
        public ICommand SplitCommand
        {
            get
            {
                if (splitCommand == null)
                {
                    splitCommand = splitCommand ?? (splitCommand = new RelayCommand(() => { Split(); }));
                }
                return splitCommand;
            }
        }

        public void Split()
        {
            if (this.MyUser.MyCards.Count > 1)
            {
                //create new hand for player with and split card
                this.MyUser.MyCards.Add(new List<Card>());
                this.MyUser.MyCards[this.MyUser.MyCards.Count - 1].Add(this.MyUser.MyCards[this.MyUser.MyCards.Count - 2][this.MyUser.MyCards[this.MyUser.MyCards.Count - 2].Count - 1]);
                this.MyUser.MyCards[this.MyUser.MyCards.Count - 2].RemoveAt(this.MyUser.MyCards[this.MyUser.MyCards.Count - 2].Count);

                //create new bet for the new hand
                this.MyUser.Bets.Add(this.MyUser.Bets[0]);
            }
        }
        #endregion

        #region AssuranceCommand
        private ICommand assuranceCommand;
        public ICommand AssuranceCommand
        {
            get
            {
                if (assuranceCommand == null)
                {
                    assuranceCommand = assuranceCommand ?? (assuranceCommand = new RelayCommand(() => { Assurance(); }));
                }
                return assuranceCommand;
            }
        }

        public void Assurance()
        {
            if (Bank.MyCards[0][0].Value == 10)
            {
                this.MyUser.Assurance = this.MyUser.Bets[0] / 2;
            }
        }
        #endregion
        private ICommand doubleCommand;
        public ICommand DoubleCommand
        {
            get
            {
                if (doubleCommand == null)
                {
                    doubleCommand = doubleCommand ?? (doubleCommand = new RelayCommand(() => { DoubleBet(); }));
                }
                return doubleCommand;
            }
        }

        public void DoubleBet()
        {
            this.MyUser
        }
        #region DoubleCommand

        #endregion

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
