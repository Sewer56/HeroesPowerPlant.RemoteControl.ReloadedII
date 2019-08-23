using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeroesPowerPlant.RemoteControl.Shared;

namespace HeroesPowerPlant.RemoteControl.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Client HeroesClient;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var heroes = Process.GetProcessesByName("tsonic_win")[0];
                HeroesClient = new Client(heroes);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, $"Failed to Connect/Find Remote Control Mod | {e.Message} | {e.StackTrace} | Try running as admin.");
            }
        }

        private async void LoadCollisionFile(object sender, RoutedEventArgs e)
        {
            await HeroesClient.LoadCollision("s03");
        }
    }
}
