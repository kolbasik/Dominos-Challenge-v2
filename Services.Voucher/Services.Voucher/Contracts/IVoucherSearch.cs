using System.Collections.Generic;
using Services.Voucher.Contracts.Models;

namespace Services.Voucher.Contracts
{
  public interface IVoucherSearch
  {
    IEnumerable<VoucherModel> Search(string pattern, int count);
  }
}
