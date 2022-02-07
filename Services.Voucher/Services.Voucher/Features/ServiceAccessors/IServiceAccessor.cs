namespace Services.Voucher.Features.ServiceAccessors
{
  public interface IServiceAccessor<out T> where T: class
  {
    public T Current { get; }
  }
}
