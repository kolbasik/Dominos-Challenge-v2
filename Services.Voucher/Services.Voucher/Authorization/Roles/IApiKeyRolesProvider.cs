namespace Services.Voucher.Authorization.Roles
{
  public interface IApiKeyRolesProvider
  {
    string[] GetRoles(string key);
  }
}
