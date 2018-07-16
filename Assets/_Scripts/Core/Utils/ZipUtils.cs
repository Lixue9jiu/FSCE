using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

public class ZipUtils
{

    static ZipUtils()
    {
        ZipConstants.DefaultCodePage = 0;
    }

    public static void Unzip(Stream stream, string outFolder)
    {
        ZipFile file = null;
        try
        {
            file = new ZipFile(stream);
            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }
                string entryFileName = zipEntry.Name;

                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                string fullZipToPath = Path.Combine(outFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }
        catch (System.Exception e)
        {
            if (file != null)
            {
                file.Close();
            }
            throw new System.Exception("error while unzipping: " + e.Message);
        }
    }

    public static void Unzip(string path, string outFolder)
    {
        using (Stream s = File.OpenRead(path))
        {
            Unzip(s, outFolder);
        }
    }

    public static void Zip(string path, string output)
    {
        FileStream fsOut = File.Create(output);
        ZipOutputStream zipStream = new ZipOutputStream(fsOut);

        zipStream.SetLevel(3);

        CompressFolder(path, zipStream, path.Length);

        zipStream.IsStreamOwner = true;
        zipStream.Close();
    }

    // Recurses down the folder structure
    //
    private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
    {

        string[] files = Directory.GetFiles(path);

        foreach (string filename in files)
        {
            FileInfo fi = new FileInfo(filename);

            string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
            entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

            // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
            // A password on the ZipOutputStream is required if using AES.
            //   newEntry.AESKeySize = 256;

            // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
            // you need to do one of the following: Specify UseZip64.Off, or set the Size.
            // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
            // but the zip will be in Zip64 format which not all utilities can understand.
            //   zipStream.UseZip64 = UseZip64.Off;
            newEntry.Size = fi.Length;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();
        }
        string[] folders = Directory.GetDirectories(path);
        foreach (string folder in folders)
        {
            CompressFolder(folder, zipStream, folderOffset);
        }
    }
}
