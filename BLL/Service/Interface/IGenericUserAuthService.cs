using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.Interface;
using BLL.Model.RequestModel.HelperModel.RegisterModel;
using BLL.Model.ResponseModel;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service.Interface;

public interface IGenericUserAuthService<TRegisterModel, TUpdateModel>
    where TRegisterModel : IRegisterModel
    where TUpdateModel : IUpdateUser
{
    public Task<ServiceResponse<IdentityError>> RegisterAsync(GenericRegisterUserModel<TRegisterModel> genericRegisterUserModel);
    public Task<ServiceResponse<TokenModel>> LoginAsync(LoginUserModel loginUserModel);
    public Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<TUpdateModel> model, int userId);
    public Task<ServiceResponse> DeleteUserAsync(int userId);
}