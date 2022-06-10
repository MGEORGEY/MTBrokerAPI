using Microsoft.AspNetCore.Identity;
using MTBrokerAPI.ControllerReturnTypes;
using MTBrokerAPI.ControllerReturnTypes.UserMngt;
using MTBrokerAPI.ModelViewModels.UserMngt;

namespace MTBrokerAPI.Services.UserMngt
{
    public class UserAccountService : IUserAccount
    {
        #region Login User
        public async Task<RegisterAppUserCRT> LoginUserAsync(LoginMVM loginMVM, ApplicationDbContext applicationDbContext, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            if (loginMVM is null || loginMVM.UsernameorEmailAddress == null || loginMVM.Password == null) return new RegisterAppUserCRT { AppUserCookieCRT = new(), JwtTokenAndRolesCRT = new(), SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.ProblemErrorMessage, Success = false } };

            var user = await userManager.FindByNameAsync(loginMVM.UsernameorEmailAddress);

            if (user is null)
                user = await userManager.FindByEmailAsync(loginMVM.UsernameorEmailAddress);

            if (user is null) return new RegisterAppUserCRT { AppUserCookieCRT = new(), JwtTokenAndRolesCRT = new(), SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.InvalidUserNameOrEmailErrorMessage, Success = false } };

            var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, loginMVM.Password, false);

            if (!passwordCheck.Succeeded)
                return new RegisterAppUserCRT { AppUserCookieCRT = new(), JwtTokenAndRolesCRT = new(), SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.WrongPasswordErrorMessage, Success = false } };

            return new RegisterAppUserCRT
            {
                AppUserCookieCRT = new AppUserCookieCRT { Name = user.Name, Username = user.UserName },
                JwtTokenAndRolesCRT = await (this as IUserAccount).RefreshTokenAndRolesForAppUserAsync(configuration, user, userManager, roleManager),
                SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = string.Empty, Success = true }
            };

        }
        #endregion


        #region Register App User


        //Register User
        public async Task<RegisterAppUserCRT> RegisterAppUserAsync(ApplicationDbContext applicationDbContext, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager, UserManager<ApplicationUser> userManager, RegisterAppUserMVM registerAppUserMVM, string role)
        {
            if (StaticVariables.InputHasBlankValues(new List<string> { registerAppUserMVM.EmailAddress, registerAppUserMVM.Name, registerAppUserMVM.Password, role })) return new RegisterAppUserCRT { AppUserCookieCRT = new AppUserCookieCRT { Username = string.Empty }, JwtTokenAndRolesCRT = new JwtTokenAndRolesCRT { JwtToken = string.Empty, Roles = new List<string>() }, SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.InputContainsBlanksErrorMessage, Success = false } };



            if (await (this as IUserAccount).EmailAddressExistsAsync(applicationDbContext, registerAppUserMVM.EmailAddress)) return new RegisterAppUserCRT { AppUserCookieCRT = new AppUserCookieCRT { Username = string.Empty }, JwtTokenAndRolesCRT = new JwtTokenAndRolesCRT { JwtToken = string.Empty, Roles = new List<string>() }, SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.EmailTakenErrorMessage, Success = false } };



            var user = new ApplicationUser { Email = registerAppUserMVM.EmailAddress, Name = registerAppUserMVM.Name, UserName = registerAppUserMVM.UserName };

            var result = await userManager.CreateAsync(user, registerAppUserMVM.Password);

            if (result.Succeeded)
            {
                var addAppUserToRolesResult = await (this as IUserAccount).AddAppUserToRolesAsync(userManager, user, new List<string>() { role });


                if (!addAppUserToRolesResult.Succeeded) return new RegisterAppUserCRT { AppUserCookieCRT = new AppUserCookieCRT { Username = string.Empty }, JwtTokenAndRolesCRT = new JwtTokenAndRolesCRT { JwtToken = string.Empty, Roles = new List<string>() }, SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.AddUserToRolesErrorMessage, Success = false } };



                return new RegisterAppUserCRT
                {
                    AppUserCookieCRT = new AppUserCookieCRT { Username = user.UserName },
                    JwtTokenAndRolesCRT = await (this as IUserAccount).RefreshTokenAndRolesForAppUserAsync(configuration, user, userManager, roleManager),
                    SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = string.Empty, Success = true }
                };

            }
            else
            {
                var error = string.Empty;
                result.Errors.ToList().ForEach(n => error += n.Description + Environment.NewLine);

                return new RegisterAppUserCRT { AppUserCookieCRT = new AppUserCookieCRT { Username = string.Empty }, JwtTokenAndRolesCRT = new JwtTokenAndRolesCRT { JwtToken = string.Empty, Roles = new List<string>() }, SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = error, Success = false } };
            }
        }
        #endregion

    }
}
