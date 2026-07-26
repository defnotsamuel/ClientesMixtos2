using ClientesMixtos.DB;
using ClientesMixtos.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ClientesMixtos
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            Services.ThemeManager.Apply(Configuration.GlobalConfig.Theme());

            var passwordRepo = _serviceProvider.GetRequiredService<Repositories.PasswordRepository>();
            try
            {
                var passwords = await passwordRepo.GetAll();

                if (passwords.Count > 0)
                {
                    var passwordWindow = new PinView
                    {
                        DataContext = _serviceProvider.GetRequiredService<ViewModels.PinViewModel>()
                    };

                    passwordWindow.Show();
                    return;
                }
            }
            catch
            {
            }

            var mainWindow = new MainView
            {
                DataContext = _serviceProvider.GetRequiredService<ViewModels.MainViewModel>()
            };

            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MongoContext>();

            services.AddSingleton<Repositories.ClienteRepository>();
            services.AddSingleton<Repositories.NotaRepository>();
            services.AddSingleton<Repositories.PasswordRepository>();
            services.AddSingleton<Repositories.PagosClienteRepository>();

            services.AddSingleton<Services.ClienteService>();
            services.AddSingleton<Services.NotaService>();
            services.AddSingleton<Services.PasswordService>();

            services.AddTransient<ViewModels.MainViewModel>();
            services.AddTransient<ViewModels.PanelViewModel>();
            services.AddTransient<ViewModels.ClientesViewModel>();
            services.AddTransient<ViewModels.ConfigViewModel>();
            services.AddTransient<ViewModels.PinViewModel>();
            services.AddTransient<ViewModels.NewPinViewModel>();
        }
    }
}
