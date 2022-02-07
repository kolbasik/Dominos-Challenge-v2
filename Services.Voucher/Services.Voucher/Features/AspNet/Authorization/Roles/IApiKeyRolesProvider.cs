namespace Services.Voucher.Features.AspNet.Authorization.Roles
{
  public interface IApiKeyRolesProvider
  {
    string[] GetRoles(string key);
  }
}
