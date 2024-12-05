using System.Diagnostics;

namespace Plunger.Services.Hosted;

public class TailwindService : IHostedService
{
    private readonly ILogger<TailwindService> _logger;
    private Process? _process;

    public TailwindService(ILogger<TailwindService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "bun",
                    Arguments = "run css:watch",
                    WorkingDirectory = "../",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            )!;

            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += (_, e) => LogInfo(e);
            _process.ErrorDataReceived += (_, e) => LogInfo(e);

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }
        catch (Exception e)
        {
            _logger.LogError("{Message}", e.Message);
        }

        return Task.CompletedTask;
    }

    private void LogInfo(DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
        {
            _logger.LogInformation("{Message}", e.Data);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Tailwind service");
        _process?.Kill(entireProcessTree: true);
        return Task.CompletedTask;
    }
}
