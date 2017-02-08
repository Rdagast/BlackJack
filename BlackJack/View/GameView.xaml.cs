using BlackJack.ViewModel;
using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace BlackJack.View
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class GameView : Page
    {
        public Api Parameter;
        public GameView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GameViewModel gameViewModel = new GameViewModel((Api)e.Parameter);
            this.DataContext = gameViewModel;
        }
    }
}
