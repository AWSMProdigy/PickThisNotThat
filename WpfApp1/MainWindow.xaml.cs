using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using LCUSharp;
using LCUSharp.Websocket;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler<LeagueEvent> GameFlowChanged;
        private readonly TaskCompletionSource<bool> _work = new TaskCompletionSource<bool>(false);
        public MainWindow()
        {
            InitializeComponent();
            findClient();
        }

        public async Task findClient()
        {
            Trace.WriteLine("Awaiting connection!");
            myText.Text = "Awaiting Connection!";
            var api = await LeagueClientApi.ConnectAsync();
            Trace.WriteLine("Connected!");
            myText.Text = "Connected!";
            GameFlowChanged += OnGameFlowChanged;
            api.EventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", GameFlowChanged);

            await _work.Task;
            Console.WriteLine("Done.");

        }

        private void OnGameFlowChanged(object sender, LeagueEvent e)
        {
            var result = e.Data.ToString();
            var state = string.Empty;

            switch (result)
            {
                case "None":
                    state = "main menu";
                    break;
                case "Lobby":
                    state = "lobby";
                    break;
                case "ChampSelect":
                    state = "champ select";
                    break;
                case "GameStart":
                    state = "game started";
                    break;
                case "InProgress":
                    state = "game";
                    break;
                case "WaitingForStats":
                    state = "waiting for stats";
                    break;
                default:
                    state = $"unknown state: {result}";
                    break;
            }

            // Print new state and set work to complete.
            Trace.WriteLine(state);
        }

    }
}
