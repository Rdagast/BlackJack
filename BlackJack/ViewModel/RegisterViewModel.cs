using BlackJack;
using BlackJack.View;
using DataModel;
using Exo4.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace BlackJack.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {

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
      
        public RegisterViewModel()
        {
            
        }
        private ICommand registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null) {
                    registerCommand = registerCommand ?? (registerCommand = new RelayCommand(() => { Register(); }, CanRegister));
                }
                return registerCommand;
            }
        }
        public bool CanRegister()
        {
            //bool result = false;
            //if (_password == _rPassword)
            //{
            //    if(_email != null)
            //    {
            //       bool response = IsEmail(_email);
            //       if (response == true)
            //        {
            //            result = true;
            //        }
            //        else
            //        {
            //            this.dialog = new MessageDialog("The email must be a valid email address");
            //            BadTextBox(this.dialog);
            //        }
            //    }
            //    result = true; 
            //}
            //else
            //{
            //    this.dialog = new MessageDialog("Password and repeat password must be identic");
            //    BadTextBox(this.dialog);
            //}
            return true;
        }
        public async void BadTextBox(MessageDialog dialog)
        {
            await dialog.ShowAsync();
        }
        public void Register()
        {
            if (_password == _rPassword)
            {
                if (_email != null)
                {
                    bool response = IsEmail(_email);
                    if (response == true)
                    {
                        User user = new User();
                        user.username = this._userName;
                        user.firstname = this._firstName;
                        user.lastname = this._lastName;
                        user.email = this._email;
                        user.password = this.Password;

                        string json = JsonConvert.SerializeObject((User)user);
                        json = "{ \"user\" : " + json + " } ";
                        Debug.WriteLine(json);
                        CallApi(json);
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
        public async void CallApi(String json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.comte.re/api/auth/register");

                var itemJson = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, itemJson);
                Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {

                }
            }
        }
        public bool IsEmail(string _email)
        {
                return Regex.IsMatch(_email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

    }
}
