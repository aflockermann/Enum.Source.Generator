﻿using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using EnumFastToStringGenerated;
using Perfolizer.Horology;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Console.Test.Benchmark;

[EnumGenerator]
public enum UserType
{
    [Display(Name = "مرد", Description = "men")] 
    Men,

    [Display(Name = "زن", Description = "women")] 
    Women,

    //[Display(Name = "نامشخص")]
    None
}

public class Program
{
    static void Main(string[] args)
    {
        Regex.CacheSize += 100;
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddAnalyser(EnvironmentAnalyser.Default)
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            //.AddColumn(StatisticColumn.AllStatistics)
            //.AddColumn(StatisticColumn.Median)
            //.AddColumn(StatisticColumn.StdDev)
            //.AddColumn(StatisticColumn.StdErr)
            .AddColumn(StatisticColumn.OperationsPerSecond)
            .AddColumn(BaselineRatioColumn.RatioMean)
            .AddColumn(RankColumn.Arabic)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80)
                .WithIterationCount(32)
                .WithInvocationCount(64)
                .WithIterationTime(TimeInterval.FromSeconds(120))
                .WithWarmupCount(6)
                .WithLaunchCount(1));

        BenchmarkRunner.Run<EnumBenchmark>(config);
    }
}

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class EnumBenchmark
{
    [Benchmark]
    public string NativeToString()
    {
        var state = UserType.Men;
        return state.ToString();
    }

    [Benchmark]
    public string FasterToString()
    {
        var state = UserType.Men;
        return state.ToStringFast();
    }

    [Benchmark]
    public bool NativeIsDefined()
    {
        return Enum.IsDefined(typeof(UserType), UserType.Men);
    }

    [Benchmark]
    public bool FastIsDefined()
    {
        return UserTypeEnumExtensions.IsDefinedFast(UserType.Men);
    }

    [Benchmark]
    public string NativeToDisplay()
    {
        return UserType.Men.ToDisplayNative();
    }

    [Benchmark]
    public string FastToDisplay()
    {
        return UserType.Men.ToDisplayFast();
    }

    [Benchmark]
    public string FastToDisplayExpression()
    {
        return UserType.Men.ToDisplayExpression();
    }



    [Benchmark]
    public string NativeToDescription()
    {
        return UserType.Men.ToDescriptionNative();
    }

    [Benchmark]
    public string FastToDescription()
    {
        return UserType.Men.ToDescriptionFast();
    }


    [Benchmark]
    public string FastToDescriptionExpression()
    {
        return UserType.Men.ToDescriptionExpression();
    }


    [Benchmark]
    public UserType[] NativeGetValues()
    {
        return (UserType[])Enum.GetValues(typeof(UserType));
    }

    [Benchmark]
    public UserType[] FastGetValues()
    {
        return UserTypeEnumExtensions.GetValuesFast();
    }

    [Benchmark]
    public string[] NativeGetNames()
    {
        return Enum.GetNames<UserType>();
    }

    [Benchmark]
    public string[] FastGetNames()
    {
        return UserTypeEnumExtensions.GetNamesFast();
    }

    [Benchmark]
    public int NativeGetLength()
    {
        return Enum.GetNames<UserType>().Length;
    }

    [Benchmark]
    public int FastGetLength()
    {
        return UserTypeEnumExtensions.GetLengthFast();
    }
    [Benchmark]
    public bool NativeTryParse()
    {
        return Enum.TryParse<UserType>("Men", out _);
    }
    [Benchmark]
    public bool FastTryParse()
    {
        return UserTypeEnumExtensions.TryParseFast("Men",out _);
    }
}

public static class Ext
{
    public static string ToDisplayNative(this Enum value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var attribute = value.GetType().GetField(value.ToString())?
            .GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

        if (attribute == null)
            return value.ToString();

        var propValue = attribute.GetType().GetProperty("Name")?.GetValue(attribute, null);

        return propValue?.ToString();
    }
    public static string ToDescriptionNative(this Enum value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var attribute = value.GetType().GetField(value.ToString())?
            .GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

        if (attribute == null)
            return value.ToString();

        var propValue = attribute.GetType().GetProperty("Description")?.GetValue(attribute, null);

        return propValue?.ToString();
    }
}
