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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._api.token.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("/api/user/" + this.Api.user.Email + "/table/" + id + "/sit");
                Debug.WriteLine(response);
                string res = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine(res);
                if (response.IsSuccessStatusCode)
                {
                    currentFrame.Navigate(typeof(GameView), this.Api);
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
                var json = JsonConvert.SerializeObject(this.Api.user.Email);
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
        #endregion
    }
}
