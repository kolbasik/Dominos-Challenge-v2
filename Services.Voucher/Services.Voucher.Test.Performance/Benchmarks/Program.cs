using System;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Running;

namespace Services.Voucher.Test.Performance.Benchmarks
{
  [ExcludeFromCodeCoverage]
  public sealed class Program
  {
    public static void Main(string[] args)
    {
      // BenchmarkRunner.Run<InMemoryVouchersRepositoryBenchmark>();
      BenchmarkRunner.Run<InMemoryLuceneVouchersSearchBenchmark>();
      // BenchmarkRunner.Run<InMemoryTrigramVouchersSearchBenchmark>();
      Console.ReadKey(true);
    }
  }
}
