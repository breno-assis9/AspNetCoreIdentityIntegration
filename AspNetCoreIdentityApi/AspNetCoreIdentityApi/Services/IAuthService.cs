using AspNetCoreIdentityApi.Data;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApi.Services
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
