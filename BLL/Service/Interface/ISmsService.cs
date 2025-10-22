using BLL.Model.ResponseModel;

namespace BLL.Service.Interface;

public interface ISmsService
{
    public Task<ServiceResponse> SendOTPAsync(string phoneNumber);
}