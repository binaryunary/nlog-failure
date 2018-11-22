using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

class Program
{
    public static void Main(string[] args)
    {
        var servicesProvider = BuildDi();

        var runner = servicesProvider.GetRequiredService<Runner>();

        runner.DoAction("Action1");

        Console.WriteLine("Press ANY key to exit");
        Console.ReadLine();

        // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
        NLog.LogManager.Shutdown();
    }

    private static IServiceProvider BuildDi()
    {
        var services = new ServiceCollection();

        //Runner is the custom class
        services.AddTransient<Runner>();

        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

        var serviceProvider = services.BuildServiceProvider();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        //configure NLog
        loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
        NLog.LogManager.LoadConfiguration("nlog.config");

        return serviceProvider;
    }

    class Runner
    {
        private readonly ILogger<Runner> _logger;

        public Runner(ILogger<Runner> logger)
        {
            _logger = logger;
        }

        public void DoAction(string name)
        {
            _logger.LogTrace(20, "Doing hard work! {Action}", name);
            _logger.LogDebug(20, "Doing hard work! {Action}", name);
            _logger.LogInformation(20, "Doing hard work! {Action}", name);
            _logger.LogWarning(20, "Doing hard work! {Action}", name);
            _logger.LogError(20, "Doing hard work! {Action}", name);
            _logger.LogCritical(20, "Doing hard work! {Action}", name);
        }
    }
}

