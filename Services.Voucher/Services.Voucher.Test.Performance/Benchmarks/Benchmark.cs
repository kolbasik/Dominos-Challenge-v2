using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Services.Voucher.Models;
using Services.Voucher.Repository;

namespace Services.Voucher.Test.Performance.Benchmarks
{
  [SimpleJob(RuntimeMoniker.Net50, baseline: true)]
  [SimpleJob(RuntimeMoniker.Net60)]
  [RPlotExporter]
  [ExcludeFromCodeCoverage]
  public class InMemoryVouchersBenchmark
  {
    [Params(100, 1000, 10000)] public int N;

    private InMemoryVoucherRepository repository;
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
      repository = new InMemoryVoucherRepository(vouchers);
    }

    [Benchmark]
    public List<VoucherModel> GetVouchers()
    {
      return repository.GetVouchers(25, 450).ToList();
    }

    [Benchmark]
    public VoucherModel GetVoucherById()
    {
      return repository.GetVoucherById(voucher.Id);
    }

    [Benchmark]
    public List<VoucherModel> GetVouchersByName()
    {
      return repository.GetVouchersByName(voucher.Name, 25, 0).ToList();
    }

    [Benchmark]
    public VoucherModel GetCheapestVoucherByProductCode()
    {
      return repository.GetCheapestVoucherByProductCode("AAA");
    }

    [Benchmark]
    public List<VoucherModel> GetVouchersByNameSearch()
    {
      return repository.GetVouchersByNameSearch("Lorem adipiscing", 25).ToList();
    }
  }

  [ExcludeFromCodeCoverage]
  public sealed class Program
  {
    public static void Main(string[] args)
    {
      BenchmarkRunner.Run<InMemoryVouchersBenchmark>();
      Console.ReadKey(true);
    }
  }
}
