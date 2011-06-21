using System;
using System.IO;
using System.Web;
using OrAdmin.Core.Enums.App;
using OrAdmin.Core.Settings;

namespace OrAdmin.Core.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Defines the file structure for getting and saving files
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userName"></param>
        /// <param name="dateSubmitted"></param>
        /// <returns></returns>
        private static object[] FileStructure(Applications.ApplicationName applicationName, string userName, DateTime dateSubmitted)
        {
            // Path parts should be defined in the order starting with the top most directory
            return new object[] { 
                    applicationName, 
                    userName,
                    dateSubmitted.ToString("yyyy"),
                    dateSubmitted.ToString("MMMM"),
                    dateSubmitted.ToString("dd")
                };
        }

        public static string GetRelativeFilePath(Applications.ApplicationName applicationName, string fileName, string userName, DateTime dateSubmitted)
        {
            return GetRelativeFilePath(applicationName, fileName, userName, dateSubmitted, false);
        }

        public static string GetRelativeFilePath(Applications.ApplicationName applicationName, string fileName, string userName, DateTime dateSubmitted, bool withUniqueName)
        {
            try
            {
                HttpServerUtility Server = HttpContext.Current.Server;

                string path = GlobalSettings.UploadPath;
                object[] pathParts = FileStructure(applicationName, userName, dateSubmitted);
                foreach (object part in pathParts)
                {
                    path = Path.Combine(path, String.Format("{0}/", part));
                    if (!Directory.Exists(Server.MapPath(path)))
                        Directory.CreateDirectory(Server.MapPath(path));
                }

                if (withUniqueName)
                {
                    int count = 0;

                    while (
                            File.Exists(
                                HttpContext.Current.Server.MapPath(
                                    Path.Combine(path,
                                        Path.GetFileNameWithoutExtension(fileName) + (count > 0 ? "-" + count : String.Empty) + Path.GetExtension(fileName)
                                    )
                                )
                            )
                        )
                    {
                        count++;
                    }

                    if (count > 0)
                        fileName = Path.GetFileNameWithoutExtension(fileName) + "-" + count + Path.GetExtension(fileName);
                }

                return Path.Combine(path, fileName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
