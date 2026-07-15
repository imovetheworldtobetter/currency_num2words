using System.Windows;
using Microsoft.Extensions.Configuration;
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
        var runtimeOptions = builder.Configuration
            .GetSection(ClientRuntimeOptions.SectionName)
            .Get<ClientRuntimeOptions>() ?? new ClientRuntimeOptions();
        var uiOptions = builder.Configuration
            .GetSection(ClientUiOptions.SectionName)
            .Get<ClientUiOptions>() ?? new ClientUiOptions();

        builder.Services.AddSingleton(runtimeOptions);
        builder.Services.AddSingleton(uiOptions);
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
                options.Retry.MaxRetryAttempts = runtimeOptions.MaxRetryAttempts;
                options.AttemptTimeout.Timeout = runtimeOptions.AttemptTimeout;
                options.TotalRequestTimeout.Timeout = runtimeOptions.TotalRequestTimeout;
            });

        return builder.Build();
    }
}

