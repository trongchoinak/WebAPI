using Microsoft.AspNetCore.Identity;

namespace Web2API.Models.IRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
