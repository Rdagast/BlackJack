using BlackJack;
using BlackJack.View;
using Exo4.ViewModel;
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


        public RegisterViewModel()
        {

        }

        public ICommand RegisterCommand
        {
            get
            {
               
                   return new RelayCommand(CanRegister);

                
                
                
            }
        }
        public void CanRegister()
        {
            Debug.WriteLine(this._userName);
        }
        

    }
}
