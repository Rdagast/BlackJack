using BlackJack.View;
using DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlackJack.ViewModel
{
    public class ListTableViewModel : BaseViewModel
    {
        #region Properties
        // Current window 
        Frame currentFrame { get { return Window.Current.Content as Frame; } }

        //Alert for the user
        MessageDialog dialog = new MessageDialog(" ");

        private ObservableCollection<User> _listUserConnect;

        public ObservableCollection<User> ListUserConnect
        {
            get { return _listUserConnect; }
            set { SetProperty(ref _listUserConnect, value); }
        }

        [JsonProperty("tables")]
        private ObservableCollection<Table> _listTable;
        /* 
         * List of Table with : 
         * id , max_seat , seat_available, min_bet, last_activity, is_closed , created_at , updated_at
         */
        public ObservableCollection<Table> ListTable
        {
            get { return _listTable; }
            set
            {
                SetProperty(ref _listTable, value);
            }
        }
        private Double _currentStack;

        public Double CurrentStack
        {
            get { return _currentStack; }
            set
            {
                SetProperty(ref _currentStack, value);
            }
        }
        private String json;

        public String Json
        {
            get { return json; }
            set
            {
                SetProperty(ref json, value);
            }
        }

        // Object Api with object user and object token 
        private Api _api;

        public Api Api
        {
            get { return _api; }
            set
            {
                SetProperty(ref this._api, value);
            }
        }
        #endregion
        #region Constructor
        // Constructor
        public ListTableViewModel(Api api)
        {
            this._api = api;
            GetTable();
            ListUser();
        }
        #endregion

        #region Function
        // function for the alert
        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }
        // Get Table with call api
        public async void GetTable()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._api.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage req = new HttpRequestMessage();



                HttpResponseMessage response = await client.GetAsync("/api/table/opened");
                Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    Debug.WriteLine(res);
                    this.json = res;
                    JsonListTable();
                    CurrentStack = this.Api.user.stack;
                }
            }

        }
        // Add Json Table api to the list ListTable
        public void JsonListTable()
        {
            Table _table = new Table();
            JObject table = JObject.Parse(this.json);
            IList<JToken> results = table["tables"].Children().ToList();
            this.ListTable = new ObservableCollection<Table>();
            foreach (JToken result in results)
            {
                _table = JsonConvert.DeserializeObject<Table>(result.ToString());
                this.ListTable.Add(_table);

            }
            // Show item in ListTable
            //foreach (var item in this.ListTable)
            //{
            //    Debug.WriteLine(item.Id);
            //}

        }
        // Command for sit on the table
        private RelayCommand _sitOnTable;
        public ICommand SitOnTable
        {
            get
            {
                if (_sitOnTable == null)
                {
                    _sitOnTable = _sitOnTable ?? (_sitOnTable = new RelayCommand(p => { ChoseTable(p); }));
                }
                return _sitOnTable;
            }
        }
        // Chose Table
        public void ChoseTable(object param)
        {
            SitOnTableApi((int)param);
        }
        // Sit on table Api 
        public async void SitOnTableApi(int id)
        {
            Double _minBetTable = 0;
            foreach (var item in ListTable)
            {
                if(item.Id == id)
                {
                   _minBetTable  = item.Min_bet;
                }
            }

            if(this.Api.user.stack >= _minBetTable)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://demo.comte.re");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._api.token.access_token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync("/api/user/" + this.Api.user.email + "/table/" + id + "/sit");
                    //Debug.WriteLine(response);
                    string res = await response.Content.ReadAsStringAsync();
                    //Debug.WriteLine(res);



                    if (response.IsSuccessStatusCode)
                    {
                        TableToGameNav navParam = null;
                        foreach (var item in _listTable)
                        {
                            if (item.Id == id)
                                navParam = new TableToGameNav(this.Api, item);
                        }

                        currentFrame.Navigate(typeof(GameView), navParam);
                    }
                    else if (response.IsSuccessStatusCode != true)
                    {
                        ErrorApi erAp = new ErrorApi();
                        erAp = JsonConvert.DeserializeObject<ErrorApi>(res);
                        if (erAp.Error_code == "user_already_on_table")
                        {
                            LeaveTable(id);


                        }

                    }
                }
            }
            else
            {
                this.dialog = new MessageDialog("Your Stack is insufficient to this table");
                BadTextBox(this.dialog);
            }
           

        }
        public async void LeaveTable(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._api.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/api/user/" + this.Api.user.email + "/table/" + id + "/leave");
                string str = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(str);


                if (response.IsSuccessStatusCode)
                {
                    SitOnTableApi(id);
                }
            }
        }

        private RelayCommand _refilltokens;
        public ICommand RefillTokens
        {
            get
            {
                if (_refilltokens == null)
                {
                    _refilltokens = _refilltokens ?? (_refilltokens = new RelayCommand(p => { RefillTokensUser(); }));
                }
                return _refilltokens;
            }
        }
        public async void RefillTokensUser()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(this.Api.token.token_type, this._api.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/api/user/" + this.Api.user.email + "/refill");
                string str = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(str);


                if (response.IsSuccessStatusCode)
                {
                    this.Api = JsonConvert.DeserializeObject<Api>(str);
                    this.dialog = new MessageDialog("Your refill is success, you now have" + this.Api.user.stack + "tokens");
                    BadTextBox(this.dialog);
                    CurrentStack = this.Api.user.stack;
                    
                }else
                {
                    
                    Debug.WriteLine(str);
                    ErrorApi erAp = new ErrorApi();
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    erAp = JsonConvert.DeserializeObject<ErrorApi>(str, settings);
                    this.dialog = new MessageDialog(erAp.Message);
                    BadTextBox(this.dialog);
                }
            }
        }

        //Command for logout
        private RelayCommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (_sitOnTable == null)
                {
                    _logoutCommand = _logoutCommand ?? (_logoutCommand = new RelayCommand(p => { Logout(); }));
                }
                return _logoutCommand;
            }
        }

        // logout
        public void Logout()
        {
            LogoutApi();
        }

        // logout with send to api 
        public async void LogoutApi()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re");
                var json = JsonConvert.SerializeObject(this.Api.user.email);
                json = "{\"email\":" + json + "}";
                var itemJson = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/api/auth/logout", itemJson);
                Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    this.dialog = new MessageDialog("Logout success");
                    BadTextBox(this.dialog);
                    this.Api = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    currentFrame.Navigate(typeof(MainPage), null);
                }
            }
        }

        public async void ListUser()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(this.Api.token.token_type, this._api.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage req = new HttpRequestMessage();



                HttpResponseMessage response = await client.GetAsync("/api/user/connected");
               // Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(res);
                    this.json = res;
                    JsonListUser();
                    CurrentStack = this.Api.user.stack;
                }
            }
        }
        public void JsonListUser()
        {
            User user = new User();
            JObject table = JObject.Parse(this.json);
            IList<JToken> results = table["users"].Children().ToList();
            this.ListUserConnect = new ObservableCollection<User>();
            foreach (JToken result in results)
            {
                user = JsonConvert.DeserializeObject<User>(result.ToString());
                this.ListUserConnect.Add(user);

            }
        }
        #endregion
    }
}
