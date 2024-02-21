// Copyright (c) 2019-2022 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Disposables;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using R3;
using ReactiveMarbles.PropertyChanged;
using ReactiveUI;
using CompositeDisposable = System.Reactive.Disposables.CompositeDisposable;

namespace ReactiveMarbles.Mvvm.Benchmarks.Memory;

/// <summary>
/// Benchmark for the R3RxObject.
/// </summary>
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class R3RxObjectMemoryBenchmark
{
    /// <summary>
    /// Gets or sets a parameter for how many numbers to create.
    /// </summary>
    [Params(1, 100, 4000)]
    public int CreateNumber { get; set; }

    /// <summary>
    /// A benchmark for subject subscription.
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Memory")]
    public void SubjectWithSubscribe()
    {
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new Subject<int>())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.Subscribe();
        }
    }

    /// <summary>
    /// A benchmark for the object creation time.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void R3RxObjectCreation()
    {
        var unused = Enumerable.Range(0, CreateNumber)
            .Select(_ => new DummyR3RxObject())
            .ToList();
    }

    /// <summary>
    /// A benchmark for the object change time.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void R3RxObjectWithChange()
    {
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new DummyR3RxObject())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.IsNotNullString = "New";
        }
    }

    /// <summary>
    /// A benchmark for subject subscription and disposal.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void SubjectWithSubscribeAndDispose()
    {
        var disposables = new CompositeDisposable();
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new Subject<int>())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.Subscribe().DisposeWith(disposables);
        }
    }

    /// <summary>
    /// A benchmark for R3Rx object and when any value subscription.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void R3RxObjectWhenAnyValueWithSubscribe()
    {
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new DummyR3RxObject())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.WhenAnyValue(x => x.IsNotNullString).Subscribe();
        }
    }

    /// <summary>
    /// A benchmark for R3Rx object and when any value subscription and disposal.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void R3RxObjectWhenAnyValueWithSubscribeAndDispose()
    {
        var disposables = new CompositeDisposable();
        var frameProvider = new FakeFrameProvider();
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new DummyR3RxObject())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.WhenAnyValue(x => x.IsNotNullString).Subscribe().DisposeWith(disposables);
        }
    }

    /// <summary>
    /// A benchmark for R3Rx object and when changed subscription.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void R3RxObjectWhenChangedWithSubscribe()
    {
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new DummyR3RxObject())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.WhenAnyValue(x => x.IsNotNullString).Subscribe();
        }
    }

    /// <summary>
    /// A benchmark for R3Rx object and when changed subscription and disposal.
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Memory")]
    public void R3RxObjectWhenChangedWithSubscribeAndDispose()
    {
        var disposables = new CompositeDisposable();
        var thing = Enumerable.Range(0, CreateNumber)
            .Select(_ => new DummyR3RxObject())
            .ToList();

        foreach (var dummy in thing)
        {
            dummy.WhenAnyValue(x => x.IsNotNullString).Subscribe().DisposeWith(disposables);
        }
    }
}