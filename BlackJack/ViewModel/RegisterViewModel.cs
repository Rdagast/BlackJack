using BlackJack.View;
using DataModel;
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
        private DataRegister register;
        public String UserName { get; set; }

        public RegisterViewModel()
        {
            register = new DataRegister();
        }


        public void Register()
        {
            Debug.WriteLine(UserName);
        }
        public bool CanRegister()
        {
            return false;
        }
        public ICommand Submit
        {
            get
            {
                return new RelayCommand(Register, CanRegister);
            }
        }


    }
}
