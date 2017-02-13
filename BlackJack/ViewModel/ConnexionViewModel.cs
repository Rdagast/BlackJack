using DataModel;
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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using BlackJack.View;

namespace BlackJack.ViewModel
{
    public class ConnexionViewModel : BaseViewModel
    {
        #region Properties
        // Current window
        Frame currentFrame { get { return Window.Current.Content as Frame; } }

        // object for the alert
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
        private Api _api;
        public Api Api
        {
            get { return _api; }
            set {
                SetProperty<Api>(ref this._api, value);
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
        #endregion

        #region Command
        // Command connexion
        private ICommand connexionCommand;
        public ICommand ConnexionCommand
        {
            get
            {
                if(connexionCommand == null)
                {
                    connexionCommand = connexionCommand ?? (connexionCommand = new RelayCommand (param => { Connexion(); }));

                }
                return connexionCommand;
            }
        }
        #endregion

        #region Function
        /*
         * Connexion with : 
         * Call  function MD5 
         * Call function Connexion Api
         */
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
                    string json = JsonConvert.SerializeObject(user, settings);

                    Debug.WriteLine(json);
                    Connect(json);
                }
                else
                {
                    this.dialog = new MessageDialog("Remplissez les champs");
                    BadTextBox(this.dialog);
                }
            }
            else
            {
                this.dialog = new MessageDialog("Remplissez les champs");
                BadTextBox(this.dialog);
            }
        }
        //Function for MD5 and encode password
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
                md5Hash = CryptographicBuffer.EncodeToHexString(buffHash);

                md5Hash = Convert.ToBase64String(md5Hash.GetBytes());

            }
            

            return md5Hash;
        }

        // Function connect with call api
        public async void Connect(String json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/");

                var itemJson = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/api/auth/login", itemJson);
                String _response = await response.Content.ReadAsStringAsync();
                //Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {

                    this.Api = new Api();
                    this.Api = JsonConvert.DeserializeObject<Api>(_response);
                    Debug.WriteLine(_response);
                    Debug.WriteLine(this.Api.user.stack);
                    currentFrame.Navigate(typeof(ListTable), this.Api);
                }
                else if (response.IsSuccessStatusCode != true)
                {
                    Debug.WriteLine(_response);
                    ErrorApi erAp = new ErrorApi();
                    _response = _response.Remove(_response.Length - 57);
                    _response += "\"}";
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    erAp = JsonConvert.DeserializeObject<ErrorApi>(_response, settings);
                    if (erAp.Error_code == "login_data_fails")
                    {
                        this.dialog = new MessageDialog("Données non conformes aux attentes");
                        BadTextBox(this.dialog);
                    }
                    else if (erAp.Error_code == "bad_credentials")
                    {
                        this.dialog = new MessageDialog("Mauvais identifiants");
                        BadTextBox(this.dialog);
                    }
                    else if (erAp.Error_code == "login_no_result")
                    {
                        this.dialog = new MessageDialog("Pas d'utilisateur trouvé");
                        BadTextBox(this.dialog);
                    }
                }
            }
        }

        // Function for alert
        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }
        #endregion
    }
}
