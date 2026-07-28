using ClientesMixtos.DB;
using ClientesMixtos.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ClientesMixtos
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App ()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Services.ThemeManager.Apply(Configuration.GlobalConfig.Theme());

            var passwordRepo = _serviceProvider.GetRequiredService<Repos.PasswordRepo>();
            try
            {
                var passwords = await passwordRepo.GetAll();

                if (passwords.Count > 0)
                {
                    var passwordWindow = ActivatorUtilities.CreateInstance<PinDialog>(_serviceProvider);

                    passwordWindow.Show();
                    return;
                }
            }
            catch
            {
            }

            var mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(_serviceProvider);
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MongoContext>();

            services.AddSingleton<Repos.ClienteRepo>();
            services.AddSingleton<Repos.NotaRepo>();
            services.AddSingleton<Repos.PasswordRepo>();
            services.AddSingleton<Repos.PagoRepo>();

            services.AddSingleton<Services.ClienteService>();
            services.AddSingleton<Services.NotaService>();
            services.AddSingleton<Services.PasswordService>();
            services.AddSingleton<Services.PagoService>();

            services.AddTransient<ViewModels.MainViewModel>();
            services.AddTransient<ViewModels.PanelViewModel>();
            services.AddTransient<ViewModels.ClientesViewModel>();
            services.AddTransient<ViewModels.ConfigViewModel>();
            services.AddTransient<ViewModels.PinViewModel>();
            services.AddTransient<ViewModels.NewPinViewModel>();
            services.AddTransient<ViewModels.PagosViewModel>();
        }
    }
}
