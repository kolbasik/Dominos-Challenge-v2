using System;

namespace Services.Voucher.Models
{
  public sealed record VoucherModel
  {
    public Guid Id { get; init; }

    public string Name { get; init; }

    public double Price { get; init; }

    public string ProductCodes { get; init; }
  }
}
