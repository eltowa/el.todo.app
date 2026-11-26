using System.Diagnostics;

namespace Todo.IntegrationTests.Helpers;

/// <summary>
/// Detects which container runtime is available on the host: Podman or Docker.
/// </summary>
public static class ContainerRuntimeDetector
{
    public enum RuntimeType
    {
        None,
        Podman,
        Docker
    }

    /// <summary>
    /// Detects the available container runtime. Prefers Podman over Docker.
    /// </summary>
    public static RuntimeType Detect()
    {
        if (IsCommandAvailable("podman") && IsPodmanSocketReachable())
            return RuntimeType.Podman;

        if (IsCommandAvailable("docker") && IsDockerDaemonReachable())
            return RuntimeType.Docker;

        return RuntimeType.None;
    }

    /// <summary>
    /// Returns the CLI name of the detected runtime (e.g. "podman" or "docker").
    /// </summary>
    public static string? GetCliName() => Detect() switch
    {
        RuntimeType.Podman => "podman",
        RuntimeType.Docker => "docker",
        _ => null
    };

    private static bool IsCommandAvailable(string command)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                ArgumentList = { "--version" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            return process != null && process.WaitForExit(2000) && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsPodmanSocketReachable()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "podman",
                ArgumentList = { "info", "--format", "{{.Host.Version}}" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            return process != null && process.WaitForExit(3000) && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsDockerDaemonReachable()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                ArgumentList = { "info", "--format", "{{.ServerVersion}}" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            return process != null && process.WaitForExit(3000) && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
