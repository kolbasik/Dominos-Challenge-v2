using System;

namespace Services.Voucher.Contracts.Models
{
  public sealed record VoucherModel
  {
    public Guid Id { get; init; }

    public string Name { get; init; }

    public double Price { get; init; }

    public string ProductCodes { get; init; }
  }
}
