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

namespace BlackJack.ViewModel
{
    public class ListTableViewModel : BaseViewModel
    {
        private Api _api;

        public Api Api
        {
            get { return _api; }
            set {
                SetProperty(ref this._api, value);
            }
        }

        public ListTableViewModel()
        {
            //GetTable();
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
                    string res = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(res);
                }
            }

        }
    }
}
