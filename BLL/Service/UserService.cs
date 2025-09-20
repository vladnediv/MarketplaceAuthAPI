using AutoMapper;
using BLL.Model.Constants;
using BLL.Model.DTO;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service;

public class UserService : IUserService
{
    private readonly IGenericService<MarketplaceUser> _userService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManagerService;
    
    public UserService(
        IGenericService<MarketplaceUser> userService,
        IMapper mapper,
        IFileService fileService,
        UserManager<ApplicationUser> userManagerService)
    {
        _userService = userService;
        _mapper = mapper;
        _fileService = fileService;
        _userManagerService = userManagerService;
    }

    public async Task<ServiceResponse<MarketplaceUserDTO>> GetPersonalInfoAsync(int userId)
    {
        var response = new ServiceResponse<MarketplaceUserDTO>();

        var getUserRes = await _userManagerService.FindByIdAsync(userId.ToString());
        if (getUserRes == null || getUserRes.MarketplaceUserId == null)
        {
            response.IsSuccess = false;
            response.Message = ServiceResponseMessages.UserNotFound;
            
            return response;
        }

        var getRes = await _userService.GetAsync((int)getUserRes.MarketplaceUserId);

        if (!getRes.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = getRes.Message;
            
            return response;
        }

        response.IsSuccess = true;
        
        response.Entity = _mapper.Map<MarketplaceUser, MarketplaceUserDTO>(getRes.Entity);
        response.Entity.Email = getRes.Entity.ApplicationUser.Email;
        response.Entity.Phone = getRes.Entity.ApplicationUser.PhoneNumber;
        response.Entity.Address = getRes.Entity.Address == null ? null : _mapper.Map<Address, AddressDTO>(getRes.Entity.Address);
        
        return response;
    }

    public async Task<ServiceResponse> EditPersonalInfoAsync(UpdateUserModel<UpdateMarketplaceUser> updateUserModel, string webRootPath)
    {
        var response = new ServiceResponse();
        
        //get the user by Id
        var applicationUser = await _userManagerService.FindByIdAsync(updateUserModel.Id.ToString());
        if (applicationUser == null)
        {
            response.IsSuccess = false;
            response.Message = ServiceResponseMessages.UserNotFound;
            
            return response;
        }
        var getUser = await _userService.GetAsync((int)applicationUser.MarketplaceUserId);
        
        if (!getUser.IsSuccess)
        {
            response.IsSuccess = false;
            response.Message = getUser.Message;
            
            return response;
        }
        
        //rewrite his data to match update
        getUser.Entity.Address = _mapper.Map<Address>(updateUserModel.User.Address);
        getUser.Entity.FirstName = updateUserModel.User.FirstName;
        getUser.Entity.LastName = updateUserModel.User.LastName;
        //if new picture came, save it, delete old one and save new path to the picture
        if (updateUserModel.User.Picture != null)
        {
            //delete old picture
            if (getUser.Entity.PictureUrl != null && getUser.Entity.PictureUrl.Length > 0)
            { 
                await _fileService.DeletePictureAsync(getUser.Entity.PictureUrl, webRootPath);
            }
            
            var saveRes = await _fileService.SavePictureAsync(updateUserModel.User.Picture, webRootPath);
            if (saveRes.IsSuccess)
            {
                getUser.Entity.PictureUrl = saveRes.Entity;
            }
        }
        
        //update the user
        var updateRes = await _userService.UpdateAsync(getUser.Entity);
        if (updateRes.IsSuccess)
        {
            response.IsSuccess = true;

            return response;
        }
        response.IsSuccess = false;
        response.Message = updateRes.Message;
        
        return response;
    }
}