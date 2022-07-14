using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClickTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, service) =>
                {
                    service.AddTransient<MainVM>();
                })
                .Build();

            ServiceProvider = host.Services;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowVM mainWindowVM = new MainWindowVM();
            mainWindowVM.Context = ServiceProvider.GetService<MainVM>();
            MainWindow window = new MainWindow();
            window.DataContext = mainWindowVM;

            window.ShowDialog();
        }
    }
}
