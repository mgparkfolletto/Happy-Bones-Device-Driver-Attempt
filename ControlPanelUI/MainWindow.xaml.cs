using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using IceMachineDriverLibrary.IceMachine.Common;
using IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;
using IceMachineDriverLibrary.IceMachine.Nakazo.Service;
using Serilog;

namespace IceMachineDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILogger Logger = new LoggerConfiguration()
            .WriteTo.File("logs/mainWindowLogs.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .MinimumLevel.Debug()
            .CreateLogger();
        
        public MainWindow()
        {
            InitializeComponent();
            Closing += ApplicationClosing;
            TbLog.Text += "MainWindow initialized";
            Logger.Debug("MainWindow initialized");
            var task = InitializeIceMachineConnection();
        }

        private void ApplicationClosing(object? sender, CancelEventArgs e)
        {
        }

        private async Task InitializeIceMachineConnection()
        {
            try
            {
                IIceMachine iceMachine = new NakazoIceMachine();
                Logger.Debug("Ice machine initialized");
                iceMachine.Connect();
                Logger.Debug($"Ice machine connected: {iceMachine.IsConnected()}");
                TbLog.Text += iceMachine.IsConnected();

                if (!iceMachine.IsConnected())
                {
                    Logger.Debug("Ice machine not connected");
                    return;
                }

                var status = await Task.Run(() => iceMachine.GetStatus());
                Logger.Debug($"Ice machine status: {status}");
                TbLog.Text += status;
                iceMachine.Close();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error while initializing ice machine connection: {e}");
            }
        }

        private void DoProductTestButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IIceMachine iceMachine = new NakazoIceMachine();
                Logger.Debug("Ice machine initialized");
                iceMachine.Connect();
                Logger.Debug($"Ice machine connected: {iceMachine.IsConnected()}");
                TbLog.Text += iceMachine.IsConnected();

                if (!iceMachine.IsConnected())
                {
                    Logger.Debug("Ice machine not connected");
                    return;
                }

                var doProductDataModel = new NakazoDoProductDataModel(1.0, 1.0);
                var result = iceMachine.DoProduct(doProductDataModel);
                Logger.Debug($"Ice machine do product result: {result}");
                
                iceMachine.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error while doing product test: {exception}");
            }
        }

        private async void GetTemperatureDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var iceMachine = new NakazoIceMachine();
                iceMachine.Connect();

                if (!iceMachine.IsConnected())
                {
                    Logger.Debug("ice machine not connected");
                    return;
                }

                var temperatureDataModel = await iceMachine.GetTemperatureData();

                TbLog.Text += $"{Environment.NewLine}" +
                              $"Exterior Temperature: {temperatureDataModel.ExteriorTemperature}{Environment.NewLine}" +
                              $"Evaporator Temperature: {temperatureDataModel.EvaporatorTemperature}{Environment.NewLine}" +
                              $"Condenser Temperature: {temperatureDataModel.CondenserTemperature}{Environment.NewLine}";
                iceMachine.Close();
            }
            catch (Exception exception)
            {
                Logger.Error($"Error while Reading Temperature: {exception}");
            }
        }
    }
}