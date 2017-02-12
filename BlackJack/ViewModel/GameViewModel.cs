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
            this.MyUser.UserHands.Add(new UserHand(this.Nav.GameTable.Min_bet));
            this.MyUser.stack -= this.Nav.GameTable.Min_bet;


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
        private ICommand _doubleCommand;
        public ICommand DoubleCommand
        {
            get
            {
                if (_doubleCommand == null)
                {
                    _doubleCommand = _doubleCommand ?? (_doubleCommand = new RelayCommand(p => { DoubleBet(); }));
                }
                return _doubleCommand;
            }
        }

        public void DoubleBet()
        {
            MyUser.UserHands[this._indexList].Bet *= 2;
        }
        #endregion

        #region BetCommand
        private ICommand _betCommand;
        public ICommand BetCommand
        {
            get
            {
                if (_betCommand == null)
                {
                    _betCommand = _betCommand ?? (_betCommand = new RelayCommand(p => { SendBetToHand(); }));
                }
                return _betCommand;
            }
        }

        public void SendBetToHand()
        {
            MyUser.UserHands[this._indexList].Bet += Bet;
            MyUser.stack -= Bet;
        }
        #endregion

        #region LeaveTableCommand

        private ICommand _leaveTableCommand;
        public ICommand LeaveTableCommand
        {
            get
            {
                if (_leaveTableCommand == null)
                {
                    _leaveTableCommand = _leaveTableCommand ?? (_leaveTableCommand = new RelayCommand(p => { LeaveTable(); }));
                }
                return _leaveTableCommand;
            }
        }

        public async void LeaveTable()
        {
            using (var client = new HttpClient())
            {
                //send the new stack to api in GET
                client.BaseAddress = new Uri("http://demo.comte.re/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.Nav.MyApi.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/api/user/"+ this.MyUser.email +"/table/"+ this.GameTable.Id +"/leave");
                Debug.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    this.dialog = new MessageDialog("Win assurance : " + MyUser.Assurance);
                    BadTextBox(dialog);
                    currentFrame.Navigate(typeof(ListTable), this.Nav.MyApi);
                }
            }
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
            if (this.GameTable.Deck.Cards[0].IsCutCard)
            {
                //reinit the deck
                this.Nav.GameTable.Deck.Cards.Clear();
                this.Nav.GameTable.CreateGameDeck();
                this.Nav.GameTable.Deck.Cards.RemoveRange(0, 4);
            }
            else
            {
                // distribute card
                userHand.Cards.Add(this.GameTable.Deck.Cards[0]);
                GameTable.Deck.Cards.RemoveAt(0);
            }
            
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
                foreach (var item in MyUser.UserHands)
                {
                    if(item != winnerHand)
                        UpdateStack(-item.Bet);
                }
                this.dialog = new MessageDialog("win : " + winnerHand.Bet * 2.5);
            }
            else if (MyGame.Winner == Bank)
            {
                foreach (var item in MyUser.UserHands)
                {
                    UpdateStack(-item.Bet);
                }
                UpdateStack(-winnerHand.Bet);
                this.dialog = new MessageDialog("loose : "+ -winnerHand.Bet);
            }
            // check assurance
            if (MyGame.Winner == Bank && winnerHand.GetValue() == 21)
            {
                UpdateStack(MyUser.Assurance);
                this.dialog = new MessageDialog("Win assurance : " + MyUser.Assurance);
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
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.Nav.MyApi.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/api/user/" + MyUser.email + "/stack/" + earnings);
                Debug.WriteLine(response);
                
            }
        }

        public void ResetPlayerHand()
        {
            this.MyUser.UserHands.Clear();
            this.MyUser.UserHands.Add(new UserHand(this.Nav.GameTable.Min_bet));
        }
        #endregion
    }
}
