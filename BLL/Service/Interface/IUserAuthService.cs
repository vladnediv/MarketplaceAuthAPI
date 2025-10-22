using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.RegisterModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;

namespace BLL.Service.Interface;

public interface IUserAuthService : IGenericUserAuthService<RegisterMarketplaceUser, UpdateMarketplaceUser>
{
    public Task<ServiceResponse> CheckLoginAsync(string login);
    public Task<ServiceResponse<TokenModel>> GoogleAuthAsync(GoogleLoginModel model);

    public Task<ServiceResponse> VerifyOtpAsync(string login, string otp);
}