namespace Services.Voucher.Features.ServiceAccessors
{
  public sealed class ServiceAccessor<T> : IServiceAccessor<T> where T : class
  {
    private volatile T _current;

    public T Current
    {
      get => _current;
      set => _current = value;
    }
  }
}
