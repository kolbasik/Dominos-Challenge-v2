using System.Collections.Generic;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public interface IVoucherRepository
  {
    IEnumerable<VoucherModel> GetVouchers();
  }
}
