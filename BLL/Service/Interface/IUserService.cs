using BLL.Model.DTO;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;

namespace BLL.Service.Interface;

public interface IUserService
{
    public Task<ServiceResponse<MarketplaceUserDTO>> GetPersonalInfoAsync(int userId);

    public Task<ServiceResponse> EditPersonalInfoAsync(UpdateUserModel<UpdateMarketplaceUser> updateUserModel, string webRootPath);
}