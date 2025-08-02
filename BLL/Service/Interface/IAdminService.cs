using BLL.Model.DTO;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ServiceResponse;

namespace BLL.Service.Interface;

public interface IAdminService : IUserAuthService<RegisterMarketplaceAdmin, UpdateMarketplaceAdmin>
{
    public Task<ServiceResponse<MarketplaceShopDTO>> GetShopByIdAsync(int shopId);
    public Task<ServiceResponse<MarketplaceShopDTO>> GetShopsAsync();
    
    public Task<ServiceResponse<MarketplaceUserDTO>> GetUserAsync(int userId);
}