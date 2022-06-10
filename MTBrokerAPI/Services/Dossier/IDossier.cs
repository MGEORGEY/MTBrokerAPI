using MTBrokerAPI.Model.FileMngt;
using MTBrokerAPI.ModelViewModels.FileMngt;

namespace MTBrokerAPI.Services.Dossier
{
    public interface IDossier
    {
        #region Get File Extension
        public string GetFileExtension(string fileName) => Path.GetExtension(fileName).ToLowerInvariant();
        #endregion



        #region Save File
        Task<List<UserFile>> SaveFilesAsync(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, List<ApiUploadFileAttachMVM> apiUploadFileAttachMVMs, List<IFormFile> formFiles, string rootFolderLocation);
        #endregion
    }
}
