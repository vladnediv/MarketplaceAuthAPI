using BLL.Service.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace BLL.Service;

public class OtpService : IOtpService
{
    private readonly IMemoryCache _cache;

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public string GenerateOtp(string login)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        _cache.Set(login, otp, TimeSpan.FromMinutes(5));
        return otp;
    }

    public bool VerifyOtp(string login, string otpCode)
    {
        if (_cache.TryGetValue(login, out string storedOtp))
        {
            return storedOtp == otpCode;
        }
        return false;
    }
}