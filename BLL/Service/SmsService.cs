using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BLL.Service;

public class SmsService : ISmsService
{
    private readonly IOtpService _otpService;
    
    public SmsService(IOtpService otpService)
    {
        _otpService = otpService;
    }
    
    public async Task<ServiceResponse> SendOTPAsync(string phoneNumber)
    {
        //this should be stored in a key vault
        var accountSid = "ACadb12f326d6e630dfae11447174e8437";
        var authToken = "e307b637f3e6ffd0256df57d9d422365";
        
        TwilioClient.Init(accountSid, authToken);
        var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber));
        messageOptions.From = new PhoneNumber("+12707275712");
        var otp = _otpService.GenerateOtp(phoneNumber);
        messageOptions.Body = $"Залишився лише один крок! Введіть цей код на сайті: {otp}";

        try
        {
            var message = await MessageResource.CreateAsync(messageOptions);
            
            if (message.Status == MessageResource.StatusEnum.Canceled || message.Status == MessageResource.StatusEnum.Failed || message.Status == MessageResource.StatusEnum.Undelivered)
            {
                return new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = message.ErrorMessage + " " + message.Status
                };
            }

            return new ServiceResponse()
            {
                IsSuccess = true,
                Message = message.Status.ToString()
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse()
            {
                IsSuccess = true,
                Message = ex.Message
            }; 
        }
    }
}