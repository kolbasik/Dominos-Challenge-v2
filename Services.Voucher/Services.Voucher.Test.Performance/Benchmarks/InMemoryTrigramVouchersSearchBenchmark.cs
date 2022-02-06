using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Services.Voucher.Models;
using Services.Voucher.Repository;

namespace Services.Voucher.Test.Performance.Benchmarks
{
  [SimpleJob(RuntimeMoniker.Net50, baseline: true)]
  [SimpleJob(RuntimeMoniker.Net60)]
  [RPlotExporter]
  [MemoryDiagnoser]
  [ExcludeFromCodeCoverage]
  public class InMemoryTrigramVouchersSearchBenchmark
  {
    [Params(1000, 10000, 100000)] public int N;

    private InMemoryTrigramVoucherSearch search;
    private VoucherModel voucher;

    [GlobalSetup]
    public void Setup()
    {
      var fixture = new Fixture();
      var vouchers = fixture.CreateMany<VoucherModel>(N).ToList();
      voucher = fixture.Create<VoucherModel>() with
      {
        Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
        ProductCodes = "AAA"
      };
      vouchers.Add(voucher);
      search = new InMemoryTrigramVoucherSearch(vouchers);
    }

    [Benchmark]
    public List<VoucherModel> Search()
    {
      return search.Search("Lorem adipiscing", 25).ToList();
    }
  }
}
