using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public class VoucherRepository
  {
    private IEnumerable<VoucherModel> _vouchers;
    internal string DataFilename = $"{AppDomain.CurrentDomain.BaseDirectory}data.json";

    public IEnumerable<VoucherModel> GetVouchers()
    {
      if (_vouchers == null)
      {
        var text = File.ReadAllText(DataFilename);
        _vouchers = JsonConvert.DeserializeObject<IEnumerable<VoucherModel>>(text);
      }

      return _vouchers;
    }
  }
}
