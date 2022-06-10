using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MTBrokerAPI.ControllerReturnTypes.UserMngt;
using MTBrokerAPI.ModelViewModels.UserMngt;

namespace MTBrokerAPI.Services.UserMngt
{
    public interface IUserAccount
    {
        #region Add AppUser To Roles
        public async Task<IdentityResult> AddAppUserToRolesAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, IEnumerable<string> roles) => await userManager.AddToRolesAsync(applicationUser, roles);
        #endregion


        #region Check if Email Exists (2 Overloads)

        public async Task<bool> EmailAddressExistsAsync(ApplicationDbContext applicationDbContext, string emailAddress) => await applicationDbContext.ApplicationUsers.AnyAsync(n => n.Email == emailAddress);

        #endregion


        #region Find AppUser By Email
        public async Task<ApplicationUser> FindAppUserByEmailAsync(UserManager<ApplicationUser> userManager, string emailAddress) => await userManager.FindByEmailAsync(emailAddress);
        #endregion


        #region Refresh Token and Roles for App User
        public async Task<JwtTokenAndRolesCRT> RefreshTokenAndRolesForAppUserAsync(IConfiguration configuration, ApplicationUser applicationUser, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager) => new JwtTokenAndRolesCRT { JwtToken = await StaticVariables.GenerateJwtTokenAsync(configuration, applicationUser, userManager, roleManager), Roles = await StaticVariables.GetRolesAssignedToAppUserAsync(applicationUser, userManager) };
        #endregion


        #region Register App Users



        //Admin
        public async Task<RegisterAppUserCRT> RegisterAdminAsync(ApplicationDbContext applicationDbContext, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager, UserManager<ApplicationUser> userManager, RegisterAppUserMVM registerAppUserMVM) => await RegisterAppUserAsync(applicationDbContext, configuration, roleManager, userManager, registerAppUserMVM, StaticVariables.AdminRole);



        //Register User
        Task<RegisterAppUserCRT> RegisterAppUserAsync(ApplicationDbContext applicationDbContext, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager, UserManager<ApplicationUser> userManager, RegisterAppUserMVM registerAppUserMVM, string role);


        #endregion




        #region Login
        Task<RegisterAppUserCRT> LoginUserAsync(LoginMVM loginMVM, ApplicationDbContext applicationDbContext, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager);
        #endregion

    }
}
