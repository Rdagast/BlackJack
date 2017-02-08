using BlackJack.ViewModel;
using DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace BlackJack.View
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ListTable : Page
    {
        public Api parameters;
        public ListTableViewModel ListTableViewModel;
        public ListTable()
        {
            this.InitializeComponent();
        }
        // Function for received the parameter in this Event
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ListTableViewModel = new ListTableViewModel((Api)e.Parameter);
            //((ListTableViewModel)this.DataContext).Api = (Api)e.Parameter;
            this.DataContext = ListTableViewModel;
        }

    }

}
