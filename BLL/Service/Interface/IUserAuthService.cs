using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.ServiceResponse;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service.Interface;

public interface IUserAuthService<TRegisterModel, TUpdateModel>
    where TRegisterModel : IRegisterModel
    where TUpdateModel : IUpdateUser
{
    public Task<ServiceResponse<IdentityError>> RegisterAsync(RegisterUserModel<TRegisterModel> registerUserModel);
    public Task<ServiceResponse<TokenModel>> LoginAsync(LoginUserModel loginUserModel);
    public Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<TUpdateModel> model, int userId);
}