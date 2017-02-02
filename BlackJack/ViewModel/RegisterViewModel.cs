using BlackJack;
using BlackJack.View;
using DataModel;
using Exo4.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace BlackJack.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {
      
        private String _userName;
        public String UserName
        {
            get { return _userName; }
            set {
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
            set {
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

        public ICommand RegisterCommand
        {
            get
            {
               
                   return new RelayCommand(Register,CanRegister);

                
                
                
            }
        }
        public bool CanRegister()
        {
            if(_password == _rPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Register()
        {

            User user = new User();
            user.username = this._userName;
            user.firstname = this._firstName;
            user.lastname = this._lastName;
            user.email = this._email;

            string json = JsonConvert.SerializeObject((User) user);
            json = "{ \"user\" : " + json + " } ";
            Debug.WriteLine(json);
        }
        

    }
}
