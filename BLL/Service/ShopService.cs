using AutoMapper;
using BLL.Model.DTO;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Domain.Model;

namespace BLL.Service;

public class ShopService : IShopService
{
    private readonly IGenericService<MarketplaceUser> _userService;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public ShopService(
        IGenericService<MarketplaceUser> userService,
        IMapper mapper,
        IAuthService authService)
    {
        _userService = userService;
        _mapper = mapper;
        _authService = authService;
    }
    
    public async Task<ServiceResponse<UserShopView>> GetUserInfoById(int userId)
    {
        var response = new ServiceResponse<UserShopView>();
        
        //get user info
        var res = await _userService.GetAsync(userId);

        if (!res.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = res.Message;
            
            return response;
        }
        response.Entity = _mapper.Map<UserShopView>(res.Entity);
        response.Entity.Address = null;
        response.IsSuccess = true;
        
        return response;
    }

    public async Task<ServiceResponse<UserShopView>> GetUserInfoById(int userId, int addressId)
    {
        var response = new ServiceResponse<UserShopView>();
        
        //get user info
        var res = await _userService.GetAsync(userId);

        if (!res.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = res.Message;
            
            return response;
        }
        response.Entity = _mapper.Map<UserShopView>(res.Entity);
        response.Entity.Address = new AddressDTO();

        if (res.Entity.Address != null)
        {
            response.Entity.Address = _mapper.Map<AddressDTO>(res.Entity.Address);
        }
        
        response.IsSuccess = true;
        
        return response;
    }
}