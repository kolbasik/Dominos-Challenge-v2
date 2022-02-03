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
    private IVoucherRepository VoucherRepository { get; }

    public VoucherController(IVoucherRepository repository)
    {
      VoucherRepository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> Get([FromQuery] int count = int.MaxValue)
    {
      return VoucherRepository.GetVouchers().Take(Math.Max(1, count));
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetVoucherById([FromQuery] Guid id)
    {
      return VoucherRepository.GetVouchers().FirstOrDefault(it => it.Id == id);
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> GetVouchersByName([FromQuery] string name)
    {
      return VoucherRepository.GetVouchers().Where(it => string.Equals(it.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> GetVouchersByNameSearch([FromQuery] string search)
    {
      return VoucherRepository.GetVouchers().Where(it => it.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetCheapestVoucherByProductCode([FromQuery] string productCode)
    {
      var vouchers = VoucherRepository.GetVouchers()
        .Where(voucher =>
          voucher.ProductCodes.Split(",")
            .Any(code => string.Equals(code, productCode, StringComparison.OrdinalIgnoreCase)));
      return vouchers.OrderBy(it => it.Price).FirstOrDefault();
    }
  }
}
