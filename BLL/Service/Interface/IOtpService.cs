namespace BLL.Service.Interface;

public interface IOtpService
{
    public string GenerateOtp(string login);
    public bool VerifyOtp(string login, string otpCode);
}