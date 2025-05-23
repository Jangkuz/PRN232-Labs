// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace PRN232.Lab1.Api;

using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// It is recommended to use a custom type to hold references for
/// ActivitySource and Instruments. This avoids possible type collisions
/// with other components in the DI container.
/// </summary>
public sealed class InstrumentationSource : IDisposable
{
    internal const string ActivitySourceName = "Examples.AspNetCore";
    internal const string MeterName = "Examples.AspNetCore";
    private readonly Meter meter;

    public InstrumentationSource()
    {
        string? version = typeof(InstrumentationSource).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);
        FreezingDaysCounter = meter.CreateCounter<long>("weather.days.freezing", description: "The number of days where the temperature is below freezing");
    }

    public ActivitySource ActivitySource { get; }

    public Counter<long> FreezingDaysCounter { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
