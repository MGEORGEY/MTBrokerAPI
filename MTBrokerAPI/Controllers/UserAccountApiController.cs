using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MTBrokerAPI.ControllerReturnTypes.UserMngt;
using MTBrokerAPI.ModelViewModels.UserMngt;
using MTBrokerAPI.Services.UserMngt;

namespace MTBrokerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountApiController : ControllerBase
    {
        #region Login
        [HttpPost(StaticVariables.LoginActionName)]
        public async Task<RegisterAppUserCRT> LoginAsync([FromBody] LoginMVM loginMVM, [FromServices] ApplicationDbContext applicationDbContext, [FromServices] IConfiguration configuration, [FromServices] RoleManager<IdentityRole<int>> roleManager, [FromServices] SignInManager<ApplicationUser> signInManager, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IUserAccount userAccountService)
        {
            return await userAccountService.LoginUserAsync(loginMVM, applicationDbContext, configuration, roleManager, signInManager, userManager);
        }
        #endregion
    }
}
