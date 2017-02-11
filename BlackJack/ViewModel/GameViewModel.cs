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

        private User _myUser;
        public User MyUser
        {
            get { return _myUser; }
            set { SetProperty(ref _myUser, value); }
        }

        private User _bank;
        public User Bank
        {
            get { return _bank; }
            set { SetProperty(ref _bank, value); }
        }

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
                    newCardCommand = newCardCommand ?? (newCardCommand = new RelayCommand(p => { DistributeCard(); }));
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
            foreach (var item in Bank.UserHands)
            {
                // Bank don't want to have more than 21 with the next card
                if (item.Value <= 16)
                    GetCard(item);
            }
        }

        public void GetCard(UserHand userHand)
        {
            // distribute card
            userHand.Cards.Add(this.MyGame.Decks[0].Cards[0]);
            MyGame.Decks[0].Cards.RemoveAt(0);
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
            if (MyGame.Winner == MyUser)
            {
                UpdateStack(winnerHand.Bet * 2.5);
                this.dialog = new MessageDialog("win");
                BadTextBox(this.dialog);
                currentFrame.Navigate(typeof(GameView), null);
            }
            else if (MyGame.Winner == Bank)
            {
                UpdateStack(-winnerHand.Bet);
                this.dialog = new MessageDialog("loose");
                BadTextBox(this.dialog);
                currentFrame.Navigate(typeof(GameView), null);
            }
        }


        public void GetScore(User user)
        {
            foreach (var hand in user.UserHands)
            {
                foreach (var card in hand.Cards)
                {
                    hand.Value += card.Value;
                }

            }
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

                HttpResponseMessage response = await client.GetAsync("user/" + MyUser.Email + "/stack/" + earnings);
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
                    stopGameCommand = stopGameCommand ?? (stopGameCommand = new RelayCommand(p => { StopGame(); }));
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
        //private ICommand splitCommand;
        //public ICommand SplitCommand
        //{
        //    get
        //    {
        //        if (splitCommand == null)
        //        {
        //            splitCommand = splitCommand ?? (splitCommand = new RelayCommand(p => { Split(); }));
        //        }
        //        return splitCommand;
        //    }
        //}

        public void Split()
        {
            if (this.MyUser.UserHands[0].Cards.Count > 1)
            {
                this.MyUser.UserHands.Add(new UserHand(this.MyUser.UserHands[0].Cards[1], this.MyUser.UserHands[0].Bet));
                this.MyUser.UserHands[0].Cards.RemoveAt(1);
            }
        }
        #endregion

        #region AssuranceCommand
        //private ICommand assuranceCommand;
        //public ICommand AssuranceCommand
        //{
        //    get
        //    {
        //        if (assuranceCommand == null)
        //        {
        //            assuranceCommand = assuranceCommand ?? (assuranceCommand = new RelayCommand(p => { Assurance(); }));
        //        }
        //        return assuranceCommand;
        //    }
        //}

        public void Assurance()
        {
            if (Bank.UserHands[0].Value == 10 || Bank.UserHands[0].Value == 9 || Bank.UserHands[0].Value == 11)
            {
                this.MyUser.Assurance = this.MyUser.UserHands[0].Bet / 2;
            }
        }
        #endregion

        #region DoubleCommand
        private ICommand doubleCommand;
        public ICommand DoubleCommand
        {
            get
            {
                if (doubleCommand == null)
                {
                    doubleCommand = doubleCommand ?? (doubleCommand = new RelayCommand(p => { DoubleBet(); }));
                }
                return doubleCommand;
            }
        }

        public void DoubleBet()
        {

        }


        #endregion

        private ICommand betcommand;
        public ICommand Betcommand
        {
            get
            {
                if (betcommand == null)
                {
                    betcommand = betcommand ?? (betcommand = new RelayCommand(p => { Bet(); }));
                }
                return betcommand;
            }
        }

        public void Bet()
        {

        }
    }
}
