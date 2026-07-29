using ClientesMixtos.Configuration;
using ClientesMixtos.DB;
using ClientesMixtos.DateUtils;
using ClientesMixtos.Repos;
using ClientesMixtos.Services;
using ClientesMixtos.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Windows;

namespace ClientesMixtos
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHost _host;

        public App()
        {
            Dispatcher.UnhandledException += OnUnhandledException;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Logs/logs-.txt"),
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 1_000_000,
                            rollOnFileSizeLimit: true)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices(ConfigureServices)
                .Build();

            _serviceProvider = _host.Services;
        }

        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ReportCrash(e.Exception);
            e.Handled = true;
        }

        private static void ReportCrash(Exception e)
        {
            var crashLogPath = Path.Combine(AppContext.BaseDirectory, "crash.log");

            Log.Fatal($"{e.Source}: {e.Message} - {e.StackTrace}");

            MessageBox.Show($"Ha ocurrido un error: {e.Message}. Mira crash.log para mas detalles");
            File.WriteAllText(crashLogPath, $"{e.Message}:{e.StackTrace}");
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Information("Iniciando host nativo");

            await _host.StartAsync();

            Log.Information("Host nativo iniciado");

            var uiFactory = _serviceProvider.GetRequiredService<UIFactory>();
            var passwordService = _serviceProvider.GetRequiredService<IPasswordService>();

            var mainWindow = uiFactory.Create<MainWindow>();
            if (await passwordService.HasUsers())
            {
                var passwordWindow = uiFactory.Create<PinDialog>();
                if (passwordWindow.ShowDialog() == true)
                {
                    mainWindow.Show();
                }
            }
            else
                mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            Log.Information("Deteniendo host nativo");

            await _host.StopAsync();
            _host.Dispose();

            Log.Information("Host nativo detenido");

            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            Log.Information("Configurando servicios de la aplicacion");

            services.AddSingleton<GlobalConfig>();
            services.AddSingleton<IMongoContext, MongoContext>();
            services.AddSingleton<IDateUtils, DateUtils.Utils>();
            services.AddSingleton<UIFactory>();

            services.AddSingleton<IClienteRepo, ClienteRepo>();
            services.AddSingleton<INotaRepo, NotaRepo>();
            services.AddSingleton<IPasswordRepo, PasswordRepo>();
            services.AddSingleton<IPagoRepo, PagoRepo>();

            services.AddSingleton<IClienteService, ClienteService>();
            services.AddSingleton<INotaService, NotaService>();
            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<IPagoService, PagoService>();

            services.AddSingleton<ViewModels.MainViewModel>();

            services.AddSingleton<ViewModels.PanelViewModel>();
            services.AddSingleton<ViewModels.ClientesViewModel>();
            services.AddSingleton<ViewModels.ConfigViewModel>();

            services.AddTransient<ViewModels.PinViewModel>();
            services.AddTransient<ViewModels.NewPinViewModel>();

            Log.Information("Servicios de la aplicacion configurados");
        }
    }
}
