using DataModel;
using Exo4.ViewModel;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Security.Cryptography;
using Cimbalino.Toolkit.Extensions;
using System.Net.Http;

namespace BlackJack.ViewModel
{
    public class ConnexionViewModel : BaseViewModel
    {
        private String _email;

        public String Email
        {
            get { return _email; }
            set
            {
                SetProperty<String>(ref this._email, value);
            }
        }

        private String _password;

        public String Password
        {
            get { return _password; }
            set
            {
                SetProperty<String>(ref this._password, value);
            }
        }

        public ICommand ConnexionCommand
        {
            get
            {
                return new RelayCommand(Connexion);
            }
        }

        public void Connexion()
        {
            User user = new User();
            user.email = this._email;
            user.password = this._password;
            if (Password != null && Email != null)
            {
                user.secret = EncodeToMd5(_password);

                if (Email != "" && Password != "")
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    string json = JsonConvert.SerializeObject((User)user, settings);

                    Debug.WriteLine(json);
                    Connect(json);

                }
                else { Debug.WriteLine("fail connect"); }

            }
            else
                Debug.WriteLine("can't encode");

            

            /*JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject((User)user, settings);
            Debug.WriteLine(json);*/

        }

        public bool CanConnect()
        {
            return true;
        }

        public String EncodeToMd5(String myString)
        {
            var myStringBytes = myString.GetBytes(); // this will get the UTF8 bytes for the string

            var md5Hash = myStringBytes.ComputeMD5Hash().ToBase64String();
            md5Hash = md5Hash.Remove(md5Hash.Length - 2);

            return md5Hash;
        }

        public async void Connect(String json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/");

                var itemJson = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/api/auth/login", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
            }
        }
    }
}
