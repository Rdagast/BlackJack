using DataModel;
using Exo4.ViewModel;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Cimbalino.Toolkit.Extensions;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace BlackJack.ViewModel
{
    public class ConnexionViewModel : BaseViewModel
    {
        private MessageDialog dialog;

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

        private ICommand connexionCommand;
        public ICommand ConnexionCommand
        {
            get
            {
                if(connexionCommand == null)
                {
                   connexionCommand= connexionCommand ?? (connexionCommand = new RelayCommand(() => { Connexion(); }, CanConnect));

                }
                return connexionCommand;
            }
        }

        public void Connexion()
        {
            if (_email != null && _password != null)
            {
                if(_email != String.Empty && _password != String.Empty)
                {
                    User user = new User();
                    user.email = this._email;
                    user.password = this._password;
                    user.secret = EncodeToMd5(_password);

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    string json = JsonConvert.SerializeObject((User)user, settings);

                    Debug.WriteLine(json);
                    Connect(json);
                }
                else
                {
                    this.dialog = new MessageDialog("remplissez les champs");
                    BadTextBox(this.dialog);
                }
            }
            else
            {
                this.dialog = new MessageDialog("remplissez les champs");
                BadTextBox(this.dialog);
            }
           

                

            

            /*JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject((User)user, settings);
            Debug.WriteLine(json);*/

        }

        public bool CanConnect()
        {
            //bool canConnec = false;
            
            //if (_email != string.Empty && _email != null && _password != null && _password != string.Empty)
            //    {
            //        canConnec = true;

            //    }
            //    else
            //    {
            //        this.dialog = new MessageDialog("remplissez les champs");
            //        BadTextBox(this.dialog);
            //    }
            //else
            //{
            //    this.dialog = new MessageDialog("remplissez les champs");
            //    BadTextBox(this.dialog);
            //}
            return true;
        }

        public String EncodeToMd5(String myString)
        {
            String md5Hash = null;
            if (myString != null)
            {
                //var myStringBytes = myString.GetBytes(); 

                //md5Hash = myStringBytes.ComputeMD5Hash().ToString();
                //// md5Hash = md5Hash.Remove(md5Hash.Length - 1);

                //md5Hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(md5Hash));
                string strAlgName = HashAlgorithmNames.Md5;
                IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(myString, BinaryStringEncoding.Utf8);

                HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);
                md5Hash = objAlgProv.AlgorithmName;

                IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);
                md5Hash= CryptographicBuffer.EncodeToHexString(buffHash);

                md5Hash = Convert.ToBase64String(md5Hash.GetBytes());

            }
            

            return md5Hash;
        }

        public async void Connect(String json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/");

                var itemJson = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/api/auth/login", itemJson);
                Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
            }
        }

        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }
    }
}
