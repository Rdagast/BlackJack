using DataModel;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlackJack.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Properties
        // Current Window
        Frame currentFrame { get { return Window.Current.Content as Frame; } }

        // Variable for the alert
        MessageDialog dialog = new MessageDialog(" ");


        private String _userName;
        public String UserName
        {
            get { return _userName; }
            set
            {
                SetProperty<String>(ref this._userName, value);
            }
        }

        private String _firstName;
        public String FirstName
        {
            get { return _firstName; }
            set
            {
                SetProperty<String>(ref this._firstName, value);
            }
        }

        private String _lastName;
        public String LastName
        {
            get { return _lastName; }
            set
            {
                SetProperty<String>(ref this._lastName, value);
            }
        }

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

        private String _rPassword;

        public String RPassword
        {
            get { return _password; }
            set
            {
                SetProperty<String>(ref this._rPassword, value);
            }
        }
        #endregion
        #region Command
        // Command Register
        private ICommand registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                {
                    registerCommand = registerCommand ?? (registerCommand = new RelayCommand(p => { Register(); }));
                }
                return registerCommand;
            }
        }
        #endregion
        #region Function
        // Function for alert
        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }

        /*
         * Function Register with: 
         * Call api
         * check fields
         * Convert fields to json
         */
        public void Register()
        {
            if (Password == RPassword)
            {
                if (this.Email != null)
                {
                    bool response = IsEmail(this.Email);
                    if (response == true)
                    {
                        User user = new User();
                        user.UserName = this.UserName;
                        user.FirstName = this.FirstName;
                        user.Lastname = this.LastName;
                        user.Email = this.Email;
                        user.Password = this.Password;

                        string json = JsonConvert.SerializeObject(user);
                        json = "{ \"user\" : " + json + " } ";
                        //Debug.WriteLine(json);
                        RegisterApi(json);
                    }
                    else
                    {
                        this.dialog = new MessageDialog("The email must be a valid email address");
                        BadTextBox(this.dialog);
                    }
                }
                else
                {
                    this.dialog = new MessageDialog("The email must be a valid email address");
                    BadTextBox(this.dialog);
                }

            }
            else
            {
                this.dialog = new MessageDialog("The password and repeat password must be identic");
                BadTextBox(this.dialog);
            }
        }

        // Send Register fields to api 
        public async void RegisterApi(String json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/api/auth/register");

                var itemJson = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, itemJson);
                Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    currentFrame.Navigate(typeof(MainPage), null);
                }
            }
        }
        // Function check if email is valid
        public bool IsEmail(string _email)
        {
            return Regex.IsMatch(_email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        #endregion
    }
}
