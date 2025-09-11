using BLL.Model.ResponseModel;
using Microsoft.AspNetCore.Http;

namespace BLL.Service.Interface;

public interface IFileService
{
    public Task<ServiceResponse<string>> SavePictureAsync(IFormFile file, string webRootPath);
    public Task<ServiceResponse> DeletePictureAsync(string path, string webRootPath);
}