using System.Collections.Generic;
using Services.Voucher.Contracts.Models;

namespace Services.Voucher.Contracts
{
  public interface IVoucherProvider
  {
    IEnumerable<VoucherModel> GetVouchers();
  }
}
