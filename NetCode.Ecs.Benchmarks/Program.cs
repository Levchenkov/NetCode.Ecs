// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using NetCode.Ecs.Benchmarks;

BenchmarkRunner.Run<CompareBenchmark>();

// var compareBenchmark = new CompareBenchmark();
// compareBenchmark.NetCodeEcs();