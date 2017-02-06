using DataModel;
using Exo4.ViewModel;
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
using System.Threading.Tasks;
using Windows.Data.Json;

namespace BlackJack.ViewModel
{
    public class ListTableViewModel : BaseViewModel
    {
        [JsonProperty("tables")]
        private ObservableCollection<Table> _listTable;


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

        private Api _api;

        public Api Api
        {
            get { return _api; }
            set
            {
                SetProperty(ref this._api, value);
            }
        }

        public ListTableViewModel()
        {
           
        }
        public ListTableViewModel(Api api)
        {
            this._api = api;
            GetTable();
        }

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
        public void JsonListTable()
        {
            Table _table = new Table();
            JObject table = JObject.Parse(this.json);

            // get JSON result objects into a list
            IList<JToken> results = table["tables"].Children().ToList();

            // serialize JSON results into .NET objects
            this.ListTable = new ObservableCollection<Table>();
            foreach (JToken result in results)
            {
                 _table = JsonConvert.DeserializeObject<Table>(result.ToString());
                this.ListTable.Add(_table);
               
            }
            foreach (var item in this.ListTable)
            {
                Debug.WriteLine(item.Id);
            }
           
        }


    }
}
