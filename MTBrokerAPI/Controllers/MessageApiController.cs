using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MTBrokerAPI.ControllerReturnTypes.MessageMngt;
using MTBrokerAPI.Filters;
using MTBrokerAPI.ModelViewModels.FileMngt;
using MTBrokerAPI.Services.Dossier;
using MTBrokerAPI.Services.MessageMngt;
using System.Runtime.CompilerServices;

namespace MTBrokerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageApiController : ControllerBase
    {
        #region Check Api Is Reachable
        [HttpGet(StaticVariables.CheckApiIsReachableActionName)]
        public string CheckApiIsReachable() => "This API is reachable.";
        #endregion


        #region Get Stored MT940s
        [Authorize(AuthenticationSchemes = StaticVariables.JwtAuthenticationType)]
        [HttpGet(StaticVariables.GetStoredMT940sActionName)]
        public async IAsyncEnumerable<MT940CRT> GetStoredMT940sAsync([FromServices] ApplicationDbContext applicationDbContext, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IMessageManager messageManagerService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var appUser = await StaticVariables.FetchApplicationUserFromJwtAsync(HttpContext.User, userManager);
            if (appUser == null) yield break;

            var serviceResult = messageManagerService.GetStoredMT940sAsync(applicationDbContext, appUser, cancellationToken);

            await foreach (var result in serviceResult)
                yield return result;
        }
        #endregion


        #region Parse Multiple MT940s
        [Authorize(AuthenticationSchemes = StaticVariables.JwtAuthenticationType)]
        [HttpPost(StaticVariables.ParseMultipleMt940sActionName)]
        public async Task<ParseMT940WithStatusCRT> ParseMultipleMt940sAsync([ModelBinder(BinderType = typeof(JsonModelBinder))] List<ApiUploadFileAttachMVM> apiUploadFileAttachMVMs, [FromForm] List<IFormFile> file_attachments, [FromServices] ApplicationDbContext applicationDbContext, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IDossier dossierService, [FromServices] IMessageManager messageManagerService, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var appUser = await StaticVariables.FetchApplicationUserFromJwtAsync(HttpContext.User, userManager);
            if (appUser == null) return null;

            return await messageManagerService.ParseMultipleMt940sAsync(applicationDbContext, appUser, dossierService, apiUploadFileAttachMVMs, file_attachments, webHostEnvironment.WebRootPath);

        }
        #endregion
    }
}
