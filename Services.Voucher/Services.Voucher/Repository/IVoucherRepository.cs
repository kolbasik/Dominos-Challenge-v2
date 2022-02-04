using System;
using System.Collections.Generic;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public interface IVoucherRepository
  {
    public IEnumerable<VoucherModel> GetVouchers(int count, int offset);

    public VoucherModel GetVoucherById(Guid id);

    public IEnumerable<VoucherModel> GetVouchersByName(string name, int count, int offset);

    public IEnumerable<VoucherModel> GetVouchersByNameSearch(string search, int count);

    public VoucherModel GetCheapestVoucherByProductCode(string productCode);
  }
}
