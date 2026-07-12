using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using myCurrencyMagic.Client.Configuration;
using myCurrencyMagic.Client.Input;
using myCurrencyMagic.Client.Services;
using myCurrencyMagic.Client.ViewModels;

namespace myCurrencyMagic.Client;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = CreateHost();
        await _host.StartAsync();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddSingleton<ClientRuntimeOptions>();
        builder.Services.AddSingleton<AmountInputFormatter>();
        builder.Services.AddTransient<MainWindowViewModel>();
        builder.Services.AddTransient<MainWindow>();

        builder.Services
            .AddHttpClient<ICurrencyConversionClient, CurrencyConversionClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<ClientRuntimeOptions>();
                client.BaseAddress = options.ServerBaseAddress;
                client.Timeout = Timeout.InfiniteTimeSpan;
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);
            });

        return builder.Build();
    }
}

