using BLL.Model.DTO;
using BLL.Model.ResponseModel;

namespace BLL.Service.Interface;

public interface IShopService
{
    public Task<ServiceResponse<UserShopView>> GetUserInfoById(int userId);
    public Task<ServiceResponse<UserShopView>> GetUserInfoById(int userId, int addressId);
}