using System;
using System.Diagnostics.CodeAnalysis;

namespace Services.Voucher.Utils
{
  [ExcludeFromCodeCoverage]
  public static class Ensure
  {
    public static T NotNull<T>(T value, string paramName)
    {
      if (value == null)
      {
        throw new ArgumentNullException(paramName);
      }
      return value;
    }
  }
}
