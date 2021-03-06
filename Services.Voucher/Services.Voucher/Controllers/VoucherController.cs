using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Contracts;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features;
using Services.Voucher.Features.AspNet.Authorization;

namespace Services.Voucher.Controllers
{
  [ApiController, ApiVersion("1.0")]
  [Route("api/{version:apiVersion}/[controller]")]
  [Authorize(AuthenticationSchemes = ApiKeys.SchemeName, Policy = "VOUCHER:READ")]
  public class VoucherController : ControllerBase
  {
    private IVoucherRepository VoucherRepository { get; }
    private IVoucherSearch VoucherSearch { get; }

    public VoucherController(IVoucherRepository voucherRepository, IVoucherSearch voucherSearch)
    {
      VoucherRepository = Ensure.NotNull(voucherRepository, nameof(voucherRepository));
      VoucherSearch = Ensure.NotNull(voucherSearch, nameof(voucherSearch));
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
      return VoucherSearch.Search(search, Pagination.NormalizeLimit(count));
    }

    [HttpGet]
    [Route("[action]")]
    public VoucherModel GetCheapestVoucherByProductCode([FromQuery] [Required] string productCode)
    {
      return VoucherRepository.GetCheapestVoucherByProductCode(productCode);
    }
  }
}
