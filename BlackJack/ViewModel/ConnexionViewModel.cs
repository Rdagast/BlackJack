using Exo4.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
                return new RelayCommand(Connexion, CanConnect);
            }
        }

        public void Connexion()
        {

        }

        public bool CanConnect()
        {
            if(Email != null && Password != null)
            {
                return true;
            }else { return false; }
        }
    }
}
