using System.Text.RegularExpressions;
using BLL.Model.Constants;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Microsoft.AspNetCore.Http;

namespace BLL.Service.HelperService;

public class FileService : IFileService
{
    private string directory;
    
    public FileService()
    {
        directory = "Pictures";
    }

    public async Task<ServiceResponse<string>> SavePictureAsync(IFormFile file, string webRootPath)
    {
        ServiceResponse<string> res = new ServiceResponse<string>();
        
        //validating argument
        if (file == null || file.Length == 0)
        {
            res.IsSuccess = false;
            res.Message = ServiceResponseMessages.FileEmpty;
            
            return res;
        }

        //size limit
        const long maxBytes = 5 * 1024 * 1024;
        if (file.Length > maxBytes)
        {
            res.IsSuccess = false;
            res.Message = ServiceResponseMessages.FileSizeTooLarge;
            
            return res;
        }

        //validating extensions
        string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        string[] allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (String.IsNullOrEmpty(ext) || !allowed.Contains(ext))
        {
            res.IsSuccess = false;
            res.Message = ServiceResponseMessages.UnsupportedFileType;
            
            return res;
        }

        //create directory for storage
        string webRoot = webRootPath;
        string picturesFolder = Path.Combine(webRoot, directory);
        Directory.CreateDirectory(picturesFolder);

        //sanitize original name
        string baseName = Path.GetFileNameWithoutExtension(file.FileName);
        baseName = Regex.Replace(baseName, @"[^a-zA-Z0-9_\-]+", "-");
        if (baseName.Length > 60) baseName = baseName.Substring(0, 60);

        //create a unique name
        string fileName = $"{Guid.NewGuid():N}_{baseName}{ext}";
        string physicalPath = Path.Combine(picturesFolder, fileName);

        try
        {
            //save picture
            await using (FileStream fs = new FileStream(
                physicalPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true))
            {
                await file.CopyToAsync(fs);
            }
        }
        catch (Exception ex)
        {
            res.IsSuccess = false;
            res.Message = ex.Message;
            
            return res;
        }

        //Return the file path
        string publicUrl = $"/{directory}/{fileName}";
        res.IsSuccess = true;
        res.Entity = publicUrl;
        
        return res;
    }

    public async Task<ServiceResponse> DeletePictureAsync(string path, string webRootPath)
    {
        ServiceResponse res = new ServiceResponse();

        try
        {
            //get file location
            string root = webRootPath;
            string pictureLocation = Path.Combine(root, directory);

            // sanitize to filename only
            string fileName = Path.GetFileName(path);

            //build the full physical path
            string fullPath = Path.Combine(pictureLocation, fileName);

            // Check and delete
            if (!File.Exists(fullPath))
            {
                res.IsSuccess = false;
                res.Message = ServiceResponseMessages.FileNotFound;
                return res;
            }

            File.Delete(fullPath);

            res.IsSuccess = true;
            return res;
        }
        catch (Exception ex)
        {
            res.IsSuccess = false;
            res.Message = ex.Message;
            
            return res;
        }
    }
}