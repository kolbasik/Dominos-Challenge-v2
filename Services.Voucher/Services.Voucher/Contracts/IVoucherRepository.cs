using System;
using System.Collections.Generic;
using Services.Voucher.Contracts.Models;

namespace Services.Voucher.Contracts
{
  public interface IVoucherRepository
  {
    public IEnumerable<VoucherModel> GetVouchers(int count, int offset);

    public VoucherModel GetVoucherById(Guid id);

    public IEnumerable<VoucherModel> GetVouchersByName(string name, int count, int offset);

    public VoucherModel GetCheapestVoucherByProductCode(string productCode);
  }
}
