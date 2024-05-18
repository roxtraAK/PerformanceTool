using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace PerformanceTool
{
    internal class Storage
    {
        public string TempPath { get; set; } = @"C:\Windows\Temp";
        public string PrefetchPath { get; set; } = @"C:\Windows\Prefetch";
        public string RecycleBinPath { get; set; } = @"C:\$Recycle.Bin";
        public bool IsDeleted { get; set; }

        const int FO_DELETE = 3;
        const int FOF_ALLOWUNDO = 0x40;
        const int FOF_NOCONFIRMATION = 0x0010;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        public Storage()
        {
            IsDeleted = false;
        }

        public void DeleteTempFiles()
        {
            try
            {
                if (Directory.Exists(TempPath))
                {
                    foreach (var file in Directory.GetFiles(TempPath))
                    {
                        if (!IsLocked(file))
                        {
                            try
                            {
                                File.Delete(file);
                                Console.WriteLine($"Deleted File: {file}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error Deleting File: {file}, Exception: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"File is locked: {file}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Temp directory not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Deleting Temp Files: {ex.Message}");
            }
        }

        public void DeletePrefetchFiles()
        {
            try
            {
                if (Directory.Exists(PrefetchPath))
                {
                    foreach (var file in Directory.GetFiles(PrefetchPath))
                    {
                        if (!IsLocked(file))
                        {
                            try
                            {
                                File.Delete(file);
                                Console.WriteLine($"Deleted File: {file}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error Deleting File: {file}, Exception: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"File is locked: {file}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Prefetch directory not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Deleting Prefetch Files: {ex.Message}");
            }
        }

        public void DeleteRecycleBin()
        {
            Console.WriteLine("Deleting files in Recyclebin...");

            if (Directory.Exists(RecycleBinPath))
            {
                string filesToDelete = RecycleBinPath + @"\*.*";

                SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT
                {
                    wFunc = FO_DELETE,
                    pFrom = filesToDelete + '\0',
                    fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION
                };

                int result = SHFileOperation(ref fileop);

                if (result == 0)
                {
                    Console.WriteLine("Files in Recyclebin successfully deleted.");
                }
                else
                {
                    Console.WriteLine("Error deleting files in Recyclebin. Code: " + result);
                }
            }
            else
            {
                Console.WriteLine("Recyclebin not found.");
            }
        }

        private static bool IsLocked(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            FileStream? stream = null;

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File is in use: {filePath}, Exception: {ex.Message}");
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }
    }
}
