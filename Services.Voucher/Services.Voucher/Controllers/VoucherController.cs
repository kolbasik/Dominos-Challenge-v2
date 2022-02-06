using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Models;
using Services.Voucher.Repository;

namespace Services.Voucher.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class VoucherController : ControllerBase
  {
    private VoucherRepository _voucherRepository;

    public VoucherRepository Repository
    {
      get => _voucherRepository ?? (_voucherRepository = new VoucherRepository());
      set => _voucherRepository = value;
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> Get([FromQuery] int skip = 0, [FromQuery] int count = 50)
    {
      return Repository.GetVouchers().Skip(skip).Take(Math.Min(count, 100));
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetVoucherById([FromQuery] Guid id)
    {
      return Repository.GetVouchers().FirstOrDefault(it => it.Id == id);
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> GetVouchersByName([FromQuery] string name)
    {
      return Repository.GetVouchers().Where(it => string.Equals(it.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> GetVouchersByNameSearch([FromQuery] string search)
    {
      return Repository.GetVouchers().Where(it => it.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetCheapestVoucherByProductCode([FromQuery] string productCode)
    {
      var vouchers = Repository.GetVouchers()
        .Where(voucher =>
          voucher.ProductCodes.Split(",")
            .Any(code => string.Equals(code, productCode, StringComparison.OrdinalIgnoreCase)));
      return vouchers.OrderBy(it => it.Price).FirstOrDefault();
    }
  }
}
