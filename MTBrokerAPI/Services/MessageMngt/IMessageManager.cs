using MTBrokerAPI.ControllerReturnTypes.MessageMngt;
using MTBrokerAPI.ModelViewModels.FileMngt;
using MTBrokerAPI.Services.Dossier;

namespace MTBrokerAPI.Services.MessageMngt
{
    public interface IMessageManager
    {
        #region Get Stored MT940s
        IAsyncEnumerable<MT940CRT> GetStoredMT940sAsync(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, CancellationToken cancellationToken);
        #endregion


        #region Parse MT940
        Task<ParseMT940WithStatusCRT> ParseMT940(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, string filePath);
        #endregion


        #region Parse Multiple MT940s
        Task<ParseMT940WithStatusCRT> ParseMultipleMt940sAsync(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, IDossier dossierService, List<ApiUploadFileAttachMVM> apiUploadFileAttachMVMs, List<IFormFile> formFiles, string rootFolderLocation);
        #endregion

    }
}
