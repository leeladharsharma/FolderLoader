using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderLoader.ViewModel
{
    public class FolderViewModel
    {

        #region Methods
        
        /// <summary>
        /// Calculates and returns the size of subfolders 
        /// </summary>
        public static long CalculateFolderSize(string folder)
        {
            long folderSize = 0;
            try
            {
                //Checking if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }

                        foreach (string dir in Directory.GetDirectories(folder))
                            folderSize += CalculateFolderSize(dir);
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine("Failed to calculate folder size: {0}", e.Message);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Failed to calculate folder size: {0}", e.Message);
            }
            return folderSize;
        }

        #endregion
    }
}
