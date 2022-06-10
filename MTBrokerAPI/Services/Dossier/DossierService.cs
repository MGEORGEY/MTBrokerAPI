using MTBrokerAPI.Model.FileMngt;
using MTBrokerAPI.ModelViewModels.FileMngt;
using System.Net;

namespace MTBrokerAPI.Services.Dossier
{
    public class DossierService : IDossier
    {
        #region Save File
        public async Task<List<UserFile>> SaveFilesAsync(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, List<ApiUploadFileAttachMVM> apiUploadFileAttachMVMs, List<IFormFile> formFiles, string rootFolderLocation)
        {
            if (formFiles == null) return new();

            List<UserFile> userFiles = new List<UserFile>();

            try
            {
                for (int formFileIndex = 0; formFileIndex < formFiles.Count() && formFileIndex < apiUploadFileAttachMVMs.Count; formFileIndex++)
                {
                    if (formFiles[formFileIndex] == null) continue;

                    var extension = (this as IDossier).GetFileExtension(WebUtility.HtmlEncode(apiUploadFileAttachMVMs[formFileIndex].FileName));



                    //var fileNameWithoutExtension = $@"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{DateTime.Now.ToString("_ddMMyyyy_HHmmss")}";

                    var fileNameWithoutExtension = $@"{WebUtility.HtmlEncode(Path.GetFileNameWithoutExtension(apiUploadFileAttachMVMs[formFileIndex].FileName))}";


                    var targetFolder = Path.Combine(new string[] { /*rootFolderLocation, */StaticVariables.FilesDirectory/*, applicationUser.Id.ToString()*/ });


                    if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);



                    var fileNameWithExtension = fileNameWithoutExtension + extension;

                    var completeFileName = Path.Combine(new string[] { targetFolder, fileNameWithExtension });


                    // if (File.Exists(completeFileName)) return userFiles;

                    if (!string.IsNullOrEmpty(extension) && !string.IsNullOrEmpty(completeFileName))
                    {
                        using (var fileStream = new FileStream(completeFileName, FileMode.Create))
                        {
                            await formFiles[formFileIndex].CopyToAsync(fileStream);

                            var fileSize = unchecked(formFiles[formFileIndex].Length);

                            var userFile = new UserFile
                            {
                                ContentType = apiUploadFileAttachMVMs[formFileIndex].ContentType,
                                FileExtension = extension,
                                FileName = fileNameWithExtension,
                                FileSize = (ulong)fileSize,
                                FileSizeReadable = StaticVariables.GetReadableFileSize(fileSize),
                                FileUri = completeFileName,
                                Owner = applicationUser
                            };


                            userFiles.Add(userFile);

                        }
                    }
                }

                applicationDbContext.UserFiles.AddRange(userFiles);

                await applicationDbContext.SaveChangesAsync();

                return userFiles;
            }
            catch (Exception exc)
            {
                return userFiles;
            }
        }
        #endregion
    }
}
