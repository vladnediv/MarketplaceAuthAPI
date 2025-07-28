namespace BLL.Model.RequestModel.HelperModel;

public static class ServiceResponseMessages
{
    public const string InvalidRefreshToken = "Invalid refresh token!";
    public const string UserNotFound = "User not found!";
    public static string UserNotFoundById(int id)
    {
        return $"User with id [{id}] not found!";
    }
    
    public const string InvalidLogin = "Invalid login!";
    public const string InvalidPassword = "Invalid password!";
    
    public const string CreateFailed = "Failed to create the entity.";
    public const string UpdateFailed = "Failed to update the entity.";
    public const string DeleteFailed = "Failed to delete the entity.";
    
    public const string UnexpectedError = "An unexpected error occurred.";
}