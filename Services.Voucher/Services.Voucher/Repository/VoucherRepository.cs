using System;
using System.Collections.Generic;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public class VoucherRepository : IVoucherRepository
  {
    private readonly IEnumerable<VoucherModel> _vouchers;

    public VoucherRepository(IEnumerable<VoucherModel> vouchers)
    {
      _vouchers = vouchers ?? throw new ArgumentNullException(nameof(vouchers));
    }

    public IEnumerable<VoucherModel> GetVouchers()
    {
      return _vouchers;
    }
  }
}
