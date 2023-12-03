// See https://aka.ms/new-console-template for more information

using System.Reflection;
using BenchmarkDotNet.Running;
using NetCode.Ecs.Benchmarks;

BenchmarkSwitcher.FromAssembly(Assembly.GetCallingAssembly()).Run();

