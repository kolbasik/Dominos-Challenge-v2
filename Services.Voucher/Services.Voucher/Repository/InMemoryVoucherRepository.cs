using System;
using System.Collections.Generic;
using System.Linq;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public sealed class InMemoryVoucherRepository : IVoucherRepository
  {
    private readonly List<VoucherModel> _vouchers;
    private readonly Dictionary<Guid, VoucherModel> _indexVoucherById;
    private readonly Dictionary<string, List<VoucherModel>> _indexVouchersByName;
    private readonly Dictionary<string, VoucherModel> _indexCheapestVoucherByProductCode;
    private readonly InMemoryLuceneVoucherSearch _luceneIdsByName;

    public InMemoryVoucherRepository(List<VoucherModel> vouchers)
    {
      _vouchers = vouchers ?? throw new ArgumentNullException(nameof(vouchers));
      _indexVoucherById = _vouchers.ToDictionary(it => it.Id);
      _indexVouchersByName = _vouchers.GroupBy(it => it.Name.Trim())
        .ToDictionary(it => it.Key, it => it.ToList(), StringComparer.OrdinalIgnoreCase);
      _indexCheapestVoucherByProductCode = new Dictionary<string, VoucherModel>(StringComparer.Ordinal);
      foreach (var voucher in vouchers)
      foreach (var productCode in voucher.ProductCodes.Split(",").Select(it => it.Trim()))
        if (!_indexCheapestVoucherByProductCode.TryGetValue(productCode, out var cheapestVoucher) ||
            cheapestVoucher.Price > voucher.Price)
          _indexCheapestVoucherByProductCode[productCode] = voucher;
      _luceneIdsByName = new InMemoryLuceneVoucherSearch(vouchers);
    }

    public IEnumerable<VoucherModel> GetVouchers(int count, int offset)
    {
      return _vouchers.Paginate(count, offset);
    }

    public VoucherModel GetVoucherById(Guid id)
    {
      return _indexVoucherById.TryGetValue(id, out var voucher) ? voucher : null;
    }

    public IEnumerable<VoucherModel> GetVouchersByName(string name, int count, int offset)
    {
      return _indexVouchersByName.TryGetValue(name.Trim(), out var vouchers)
        ? vouchers.Paginate(count, offset)
        : Enumerable.Empty<VoucherModel>();
    }

    public IEnumerable<VoucherModel> GetVouchersByNameSearch(string search, int count)
    {
      count = Pagination.NormalizeLimit(count);
      return _luceneIdsByName.Search(search, count).Select(GetVoucherById);
    }

    public VoucherModel GetCheapestVoucherByProductCode(string productCode)
    {
      return _indexCheapestVoucherByProductCode.TryGetValue(productCode.Trim(), out var voucher) ? voucher : null;
    }
  }
}
