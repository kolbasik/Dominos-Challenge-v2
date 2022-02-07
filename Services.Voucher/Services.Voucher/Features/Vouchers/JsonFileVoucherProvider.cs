using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Services.Voucher.Contracts;
using Services.Voucher.Contracts.Models;

namespace Services.Voucher.Features.Vouchers
{
  public sealed class JsonFileVoucherProvider : IVoucherProvider
  {
    public JsonFileVoucherProvider(string jsonPath = null)
    {
      JsonPath = jsonPath ?? $"{AppDomain.CurrentDomain.BaseDirectory}data.json";
    }

    private string JsonPath { get; }

    public IEnumerable<VoucherModel> GetVouchers()
    {
      // It isn't possible to use dotnet core json serializer due to not supporting: Allow property names without quotes
      var json = File.ReadAllText(JsonPath);
      var vouchers =  JsonConvert.DeserializeObject<List<VoucherModel>>(json);
      return vouchers;
    }
  }
}
