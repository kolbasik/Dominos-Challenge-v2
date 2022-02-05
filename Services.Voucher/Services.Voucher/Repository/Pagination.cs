using System;
using System.Collections.Generic;
using System.Linq;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public static class Pagination
  {
    public const int DefaultValue = 25;
    public const int MinValue = 1;
    public const int MaxValue = 100;

    public static IEnumerable<VoucherModel> Paginate(this IEnumerable<VoucherModel> source, int count, int offset)
    {
      return source.Skip(Math.Max(0, offset)).Take(Math.Max(MinValue, Math.Min(count, MaxValue)));
    }
  }
}
