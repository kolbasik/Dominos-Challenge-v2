using System.Collections.Generic;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public interface IVoucherSearch
  {
    IEnumerable<VoucherModel> Search(string pattern, int count);
  }
}
