using System;

using Todo.IntegrationTests.Helpers;

namespace Todo.IntegrationTests.Attributes;

/// <summary>
/// Attribute used to mark a test class or method as an integration test.
/// Integration tests require a container runtime (Podman or Docker) and external dependencies.
/// Tests are automatically skipped if the required container runtime is not available.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class IntegrationTestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the container runtime required for this test.
    /// </summary>
    public ContainerRuntimeDetector.RuntimeType RequiredRuntime { get; set; } = ContainerRuntimeDetector.RuntimeType.Podman;

    /// <summary>
    /// Gets or sets a description of what this integration test validates.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the test should be skipped if the required runtime is not available.
    /// Defaults to true.
    /// </summary>
    public bool SkipIfRuntimeUnavailable { get; set; } = true;
}

/// <summary>
/// Factory for creating integration test instances with container awareness.
/// </summary>
public static class IntegrationTestFactory
{
    /// <summary>
    /// Checks whether the required container runtime is available and returns a skip reason if not.
    /// </summary>
    public static string? GetSkipReason(IntegrationTestAttribute attribute)
    {
        if (!attribute.SkipIfRuntimeUnavailable)
            return null;

        var runtime = ContainerRuntimeDetector.Detect();
        if (runtime == ContainerRuntimeDetector.RuntimeType.None)
            return "No container runtime (Podman or Docker) is available on this host.";

        if (attribute.RequiredRuntime != ContainerRuntimeDetector.RuntimeType.None &&
            runtime != attribute.RequiredRuntime)
            return $"Required container runtime '{attribute.RequiredRuntime}' is not available. Detected: '{runtime}'.";

        return null;
    }

    /// <summary>
    /// Returns the detected container runtime type.
    /// </summary>
    public static ContainerRuntimeDetector.RuntimeType GetDetectedRuntime() =>
        ContainerRuntimeDetector.Detect();
}
