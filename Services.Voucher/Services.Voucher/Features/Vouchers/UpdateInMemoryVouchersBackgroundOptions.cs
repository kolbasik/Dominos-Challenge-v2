using System;

namespace Services.Voucher.Features.Vouchers
{
  public sealed class UpdateInMemoryVouchersBackgroundOptions
  {
    public SearchProviders SearchProvider { get; set; } = SearchProviders.Lucene;
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan RetryOnFailure { get; set; } = TimeSpan.FromMinutes(1);
  }
}
