using Blazor.Chat.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Blazor.Chat.Utility
{
    public interface IMiaoMiaoJiangAuthenticationStateProvider
    {

        public Task<AuthenticationState> GetAuthenticationStateAsync();
        public void LoginAsync(string account, string password);
        public void LogOut();
    }

    public class MiaoMiaoJiangAuthenticationStateProvider(ProtectedLocalStorage storage, IFreeSql fsql) : AuthenticationStateProvider, IMiaoMiaoJiangAuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _storage = storage;
        private readonly IFreeSql _fsql = fsql;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userLocalStorage = await _storage.GetAsync<Person>("identity");
                var principal = CreateIdentityFromUser(userLocalStorage.Success ? userLocalStorage.Value : null);
                return new AuthenticationState(new ClaimsPrincipal(principal));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new AuthenticationState(new ClaimsPrincipal(CreateIdentityFromUser(null)));
            }
        }

        public async void LoginAsync(string account, string password)
        {
            if (account == "admin" && password == "211914AAAaaa...")
            {
                var identity = CreateIdentityFromUser(new Person
                {
                    Account = "admin",
                    Balance = 0,
                    Email = "1213095357@qq.com",
                    Roles = "admin",
                    Name = "管理员",
                    Password = "211914AAAaaa...",
                    Phone = "13011105654",
                    ID = -1
                });
                var principal = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(principal)));
            }


            var user = await _fsql.Select<Person>()
                .Where(p => p.Account == account && p.Password == password)
                .FirstAsync();

            if (user is not null && user.ID >= 0)
            {
                await _storage.SetAsync("identity", user);
                var identity = CreateIdentityFromUser(user);
                var principal = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(principal)));
            }
        }

        public async void LogOut()
        {
            await _storage.SetAsync("identity", new Person() { });
            var identity = CreateIdentityFromUser(null);
            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(principal)));
        }

        private static ClaimsPrincipal CreateIdentityFromUser(Person? user)
        {
            if (user is not null)
            {
                var claims = new List<Claim>
                {
                    new("Name",user.Name),
                    new("Account",user.Account),
                    new("Password",user.Password),
                    new("ID",user.ID.ToString()),
                };

                var roles = user.Roles?.Split(",");
                if (roles?.Length > 0)
                {
                    foreach (var item in roles)
                    {
                        claims.Add(new(ClaimTypes.Role, item));
                    }
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                return claimsPrincipal;

            }
            return new ClaimsPrincipal();
        }

    }



}
