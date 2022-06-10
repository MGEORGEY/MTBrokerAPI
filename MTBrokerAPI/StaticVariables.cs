using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Security.Claims;

namespace MTBrokerAPI
{
    public static class StaticVariables
    {
        #region Application Constants

        public static string AppName { get => "MT Broker"; }

        public static string FilesDirectory { get => "Files"; }


        public const string JwtAuthenticationType = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;


        #region File Signature
        private static Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>>
        {
            //{ ".DOC", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
            { ".DOCX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
            { ".PDF", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
            { ".ZIP", new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x55 },
                new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 }
            }
            },


            { ".PNG", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".JPG", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
            }

            },
            /*{ ".JPEG", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
            }
            },*/
            { ".GIF", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
            { ".BMP", new List<byte[]> { new byte[] { 0x42, 0x4D } } },
            { ".JFIF", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 } } },



            { ".MP4", new List<byte[]> {
                new byte[] { 0x00,0x00,0x00,0x18,0x66,0x74,0x79,0x70,0x6D,0x70,0x34,0x32 },
                new byte[] { 0x00,0x00,0x00,0x1C,0x66,0x74,0x79,0x70,0x6D,0x70,0x34,0x32 },
                new byte[] { 0x00,0x00,0x00,0x20,0x66,0x74,0x79,0x70,0x69,0x73,0x6F, 0x6D },
            }
            },

                    { ".MKV", new List<byte[]> {
                        new byte[] { 0x1A, 0x45, 0xDF, 0xA3, 0xA3, 0x42, 0x86, 0x81, 0x01, 0x42, 0xF7  }
                    }
            },

            { ".MOV", new List<byte[]> {
                new byte[] { 0x00, 0x00, 0x00, 0x14, 0x66, 0x74, 0x79, 0x70, 0x71, 0x74 }
            }
            },
            { ".M4V", new List<byte[]> {
                new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x56 }
            }
            },

            { ".WEBM", new List<byte[]> {
                new byte[] { 0x1A, 0x45, 0xDF, 0xA3, 0x9F, 0x42, 0x86, 0x81, 0x01, 0x42, 0xF7, 0x81, 0x01, 0x42, 0xF2 }
            }
            },

            { ".WMV", new List<byte[]> {
                new byte[] { 0x30,0x26,0xB2,0x75,0x8E,0x66,0xCF,0x11}
            }
            },

            { ".AVI", new List<byte[]> {
                new byte[] { 0x52, 0x49, 0x46, 0x46, 0x84, 0x4A, 0x1E, 0x00, 0x41, 0x56, 0x49 }
            }
            },

            { ".FLV", new List<byte[]> {
                new byte[] { 0x46, 0x4C, 0x56 }
            }
            },



            { ".XLS", new List<byte[]>
            {
                new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 },
                new byte[] { 0xFD, 0xFF, 0xFF, 0xFF }
            }
            },

            { ".XLSX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
        };
        #endregion

        #endregion


        #region Roles And Policies

        public const string AdminRole = "AdminRole";


        public static string[] RoleNames { get; } = { AdminRole };

        #endregion


        #region Controller Action Names  

        #region App User

        public const string LoginActionName = "login";

        #endregion


        #region Message Manager

        public const string CheckApiIsReachableActionName = "checkApiIsReachable";

        public const string GetStoredMT940sActionName = "getStoredMT940s";

        public const string ParseMultipleMt940sActionName = "parseMultipleMt940s";

        #endregion


        #endregion


        #region Messages


        #region User and General Messages

        public static string AddUserToRolesErrorMessage => "Could not assign roles to this user.";

        public static string CreateRolesErrorMessage => "Roles could not be established.";

        public static string EmailTakenErrorMessage => "Try a different email";

        public static string InputContainsBlanksErrorMessage => "Input contains blanks";

        public static string InvalidUserNameOrEmailErrorMessage => "Invalid username or email address";

        public static string ProblemErrorMessage => "There was a problem. Please try again.";

        public static string UnknownUserErrorMessage => "Unknown user.";

        public static string UnreadableFileErrorMessage => "File is unreadable. Only .txt files can be uploaded";

        public static string UsernameTakenErrorMessage => "Try a different username";

        public static string WrongPasswordErrorMessage => "Wrong password";

        #endregion


        #endregion


        #region Methods

        #region Get Readable File Size
        public static string GetReadableFileSize(long fileSize)
        {
            // Get absolute value

            //long absolute_i = fileSize < 0 ? -fileSize : fileSize;
            long absolute_i = Math.Abs(fileSize);

            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = fileSize >> 50;
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = fileSize >> 40;
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = fileSize >> 30;
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = fileSize >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = fileSize >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = fileSize;
            }
            else
            {
                return fileSize.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = readable / 1024;

            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
        #endregion


        #region Input Has Blank Values
        public static bool InputHasBlankValues(List<string> inputValues) => inputValues.Any(n => string.IsNullOrWhiteSpace(n));
        #endregion


        #region Get File Extension
        public static string GetFileExtension(byte[] fileData)
        {
            if (fileData == null || fileData.Length == 0)
            {
                return string.Empty;
            }


            var extension = string.Empty;
            for (int extensionSignatureIndex = 0; extensionSignatureIndex < fileSignature.Count; extensionSignatureIndex++)
            {
                if (extension != string.Empty) break;

                var byteList = fileSignature.ElementAt(extensionSignatureIndex).Value;
                foreach (byte[] b in byteList)
                {
                    var curFileSig = new byte[b.Length];
                    Array.Copy(fileData, curFileSig, b.Length);
                    if (curFileSig.SequenceEqual(b))
                    {
                        extension = fileSignature.ElementAt(extensionSignatureIndex).Key;
                        break;
                    }
                }

            }
            return extension.ToLowerInvariant();
        }
        #endregion


        #region Is Valid File Extension

        public static bool IsValidFileExtension(string fileExtension, byte[] fileData, byte[] allowedChars = null)
        {
            if (string.IsNullOrEmpty(fileExtension) || fileData == null || fileData.Length == 0)
            {
                return false;
            }
            bool flag = false;


            if (string.IsNullOrEmpty(fileExtension))
            {
                return false;
            }
            fileExtension = fileExtension.ToUpperInvariant();

            if (fileExtension.Equals(".TXT") || fileExtension.Equals(".CSV") || fileExtension.Equals(".PRN"))
            {
                foreach (byte b in fileData)
                {
                    if (b > 0x7F)
                    {
                        if (allowedChars != null)
                        {
                            if (!allowedChars.Contains(b))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            if (!fileSignature.ContainsKey(fileExtension))
            {
                return true;
            }
            List<byte[]> sig = fileSignature[fileExtension];
            foreach (byte[] b in sig)
            {
                var curFileSig = new byte[b.Length];
                Array.Copy(fileData, curFileSig, b.Length);
                if (curFileSig.SequenceEqual(b))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        #endregion


        #region Generate Jwt Token String
        public static async Task<string> GenerateJwtTokenAsync(Microsoft.Extensions.Configuration.IConfiguration configuration, ApplicationUser applicationUser, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            return await Task.Run(async () =>
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = System.Text.Encoding.UTF8.GetBytes(configuration["UserSettings:Secret"]);
                var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);

                var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(await GetValidClaims(applicationUser, userManager, roleManager), Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme),

                    Expires = DateTime.UtcNow.AddDays(7D),
                    SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
                };

                var jwtToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(jwtToken);
                return tokenString;
            });
        }
        #endregion


        #region Get Roles Assigned to App User
        public static async Task<List<string>> GetRolesAssignedToAppUserAsync(ApplicationUser applicationUser, UserManager<ApplicationUser> userManager) => await userManager.GetRolesAsync(applicationUser) as List<string>;
        #endregion


        #region Get Valid Claims
        public static async Task<List<Claim>> GetValidClaims(ApplicationUser appUser, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, appUser.UserName)
        };
            var userClaims = await userManager.GetClaimsAsync(appUser);
            var userRoles = await GetRolesAssignedToAppUserAsync(appUser, userManager);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }
        #endregion


        #region Fetch Application User From JWT
        public static async Task<ApplicationUser> FetchApplicationUserFromJwtAsync(ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
        {
            var appUserEmail = user.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(appUserEmail))
            {
                return null;
            }

            var appUser = await userManager.FindByNameAsync(appUserEmail);

            if (appUser == null) appUser = await userManager.FindByEmailAsync(appUserEmail);
            if (appUser == null)
            {

                return null;
            }

            //if (appUser.UserAccountStatus != UserAccountStatus.Active) return null;

            return appUser;
        }
        #endregion


        #region Application Specific

        #region To Date
        public static DateTime ToDate(this string dateString) => DateTime.ParseExact(dateString, "yyMMdd", CultureInfo.InvariantCulture);
        #endregion


        #region To Money
        public static decimal ToMoney(this string moneyString) => Convert.ToDecimal(moneyString.Replace(",", "."));
        #endregion

        #endregion

        #endregion
    }
}
