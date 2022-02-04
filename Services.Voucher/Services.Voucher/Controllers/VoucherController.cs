using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public VoucherController(IVoucherRepository voucherRepository)
    {
      VoucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> Get(
      [FromQuery] [Range(Pagination.MinValue, Pagination.MaxValue)]
      int count = Pagination.DefaultValue,
      [FromQuery] [Range(0, int.MaxValue)] int offset = 0)
    {
      return VoucherRepository.GetVouchers(count, offset);
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetVoucherById([FromQuery] [Required] Guid id)
    {
      return VoucherRepository.GetVoucherById(id);
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> GetVouchersByName(
      [FromQuery] [Required] string name,
      [FromQuery] [Range(Pagination.MinValue, Pagination.MaxValue)]
      int count = Pagination.DefaultValue,
      [FromQuery] [Range(0, int.MaxValue)] int offset = 0)
    {
      return VoucherRepository.GetVouchersByName(name, count, offset);
    }

    [HttpGet]
    [Route("[action]")]
    public IEnumerable<VoucherModel> GetVouchersByNameSearch(
      [FromQuery] [Required] string search,
      [FromQuery] [Range(Pagination.MinValue, Pagination.MaxValue)]
      int count = Pagination.DefaultValue)
    {
      return VoucherRepository.GetVouchersByNameSearch(search, count);
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetCheapestVoucherByProductCode([FromQuery] [Required] string productCode)
    {
      return VoucherRepository.GetCheapestVoucherByProductCode(productCode);
    }
  }
}
