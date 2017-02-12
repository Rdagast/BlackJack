using BlackJack.View;
using DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Table GameTable { get; set; }
        public TableToGameNav Nav { get; set; }
        public User MyUser { get; set; }
        public User Bank { get; set; }
        private Double _bet;
        public Double Bet
        {
            get { return _bet; }
            set { SetProperty<Double>(ref this._bet, value); }
        }
        Frame currentFrame { get { return Window.Current.Content as Frame; } }
        private int _indexList;
        private MessageDialog dialog;
        #endregion

        public GameViewModel(TableToGameNav nav)
        {
            this.Nav = nav;
            this.MyGame = new Game();
            this.MyUser = this.Nav.MyApi.user;
            this._indexList = 0;
            this.GameTable = nav.GameTable;

            // guive a new hand
            this.MyUser.UserHands.Add(new UserHand());

            //set Bank player
            this.Bank = new User();
            this.Bank.UserHands.Add(new UserHand());

            //2 starting cards for player and 1 for bank
            ResetPlayerHand();
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
            BankPlay();

            PlayerPlay();

            //end game check
            if (IsGameFinish())
                WhoIsTheWinner();
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
                    stopGameCommand = stopGameCommand ?? (stopGameCommand = new RelayCommand(p => { StopHand(); }));
                }
                return stopGameCommand;
            }
        }

        public void StopHand()
        {

            MyUser.UserHands[this._indexList].IsFinish = true;

            if (IsGameFinish())
                WhoIsTheWinner();

        }
        #endregion

        #region SplitComand
        private ICommand _splitCommand;
        public ICommand SplitCommand
        {
            get
            {
                if (_splitCommand == null)
                {
                    _splitCommand = _splitCommand ?? (_splitCommand = new RelayCommand(p => { Split(); }));
                }
                return _splitCommand;
            }
        }

        public void Split()
        {
            ObservableCollection<Card> card = this.MyUser.UserHands[this._indexList].Cards;

            //check if you have the same card and split it in other hand
            if (card.Count > 1 && card[card.Count -1].Name == card[card.Count - 2].Name)
            {
                this.MyUser.UserHands.Add(new UserHand(this.MyUser.UserHands[this._indexList].Cards[card.Count-1], this.MyUser.UserHands[this._indexList].Bet));
                this.MyUser.UserHands[this._indexList].Cards.RemoveAt(card.Count-1);
            }
        }
        #endregion

        #region AssuranceCommand
        private ICommand _assuranceCommand;
        public ICommand AssuranceCommand
        {
            get
            {
                if (_assuranceCommand == null)
                {
                    _assuranceCommand = _assuranceCommand ?? (_assuranceCommand = new RelayCommand(p => { Assurance(); }));
                }
                return _assuranceCommand;
            }
        }

        public void Assurance()
        {
            if (Bank.UserHands[0].GetValue() == 10 || Bank.UserHands[0].GetValue() == 9 || Bank.UserHands[0].GetValue() == 11)
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
            MyUser.UserHands[this._indexList].Bet *= 2;
        }
        #endregion

        #region BetCommand
        private ICommand betCommand;
        public ICommand BetCommand
        {
            get
            {
                if (betCommand == null)
                {
                    betCommand = betCommand ?? (betCommand = new RelayCommand(p => { SendBetToHand(); }));
                }
                return betCommand;
            }
        }

        public void SendBetToHand()
        {
            MyUser.UserHands[this._indexList].Bet = Bet;
        }
        #endregion

        #region Methodes

        public void BankPlay()
        {
            if (Bank.UserHands[0].IsFinish == false)
            {
                // Bank don't want to have more than 21 with the next card
                if (Bank.UserHands[0].GetValue() <= 16)
                    GetCard(Bank.UserHands[0]);
                else
                    Bank.UserHands[0].IsFinish = true;
            }
        }

        public void PlayerPlay()
        {
            if (MyUser.UserHands[this._indexList].IsFinish == false)
            {
                //give a card and check victory or loose
                GetCard(MyUser.UserHands[this._indexList]);

                // check if score > 21
                if (MyUser.UserHands[this._indexList].GetValue() >= 21)
                {
                    MyUser.UserHands[this._indexList].IsFinish = true;
                }

                // next turn for the other hand of player if he had split
                if (MyUser.UserHands.Count > 1)
                    if (this._indexList != MyUser.UserHands.Count - 1)
                        this._indexList++;
                    else
                        this._indexList = 0;
            }
        }

        public void GetCard(UserHand userHand)
        {
            // distribute card
            userHand.Cards.Add(this.GameTable.Decks[0].Cards[0]);
            GameTable.Decks[0].Cards.RemoveAt(0);
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

            foreach (var item in checks)
            {
                if (item == false)
                    IsFinish = false;
            }

            // if user finish all his plays the bank finish her play
            if (Bank.UserHands[0].IsFinish == false && IsFinish == true)
            {
                while (Bank.UserHands[0].IsFinish == false)
                {
                    BankPlay();
                }
                IsFinish = true;
            }

            return IsFinish;
        }

        public void WhoIsTheWinner()
        {
            UserHand winnerHand = new UserHand();
            winnerHand.Cards.Add(new Card(Color.CLUBS, Name.TWO));

            

            foreach (var playerItem in MyUser.UserHands)
            {
                if (playerItem.GetValue() <= 21)
                {
                    if (playerItem.GetValue() > winnerHand.GetValue())
                    {
                        MyGame.Winner = MyUser;
                        winnerHand = playerItem;
                    }
                    else if (playerItem.GetValue() == winnerHand.GetValue())
                    {
                        MyGame.Winner = MyUser;
                        winnerHand = playerItem;
                    }
                }
            }


            if (Bank.UserHands[0].GetValue() <= 21)
            {
                if (Bank.UserHands[0].GetValue() > winnerHand.GetValue())
                {
                    MyGame.Winner = Bank;
                    winnerHand = Bank.UserHands[0];
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
            }
            else if (MyGame.Winner == Bank)
            {
                UpdateStack(-winnerHand.Bet);
                this.dialog = new MessageDialog("loose");
            }
            RestartTextBox(this.dialog);
        }


        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }

        public async void RestartTextBox(MessageDialog dialog)
        {
            dialog.Commands.Add(new UICommand("Restart") { Id = 0 });
            dialog.DefaultCommandIndex = 0;

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
                currentFrame.Navigate(typeof(GameView), this.Nav);
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

        public void ResetPlayerHand()
        {
            this.MyUser.UserHands.Clear();
            this.MyUser.UserHands.Add(new UserHand());
        }

        #endregion
    }
}
