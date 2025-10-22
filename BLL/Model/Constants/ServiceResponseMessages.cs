namespace BLL.Model.Constants;

public static class ServiceResponseMessages
{
    public const string InvalidRefreshToken = "Invalid refresh token!";
    public const string UserNotFound = "User not found!";
    public static string UserNotFoundById(int id)
    {
        return $"User with id [{id}] could not be found!";
    }
    
    public const string InvalidLogin = "Invalid login!";
    public const string InvalidPassword = "Invalid password!";
    
    public const string CreateFailed = "Failed to create the entity.";
    public const string UpdateFailed = "Failed to update the entity.";
    public const string DeleteFailed = "Failed to delete the entity.";
    
    public const string FileEmpty = "File is empty.";
    public const string FileSizeTooLarge = "File too large (max 5 MB).";
    public const string UnsupportedFileType = "Unsupported file type.";
    public const string FileNotFound = "File not found.";
    
    public const string UnexpectedError = "An unexpected error occurred.";
    
    public static string ArgumentIsNull(string variable, string entityType) => $"The argument [{variable}] of type [{entityType}] is null.";
    public static string ArgumentsAreNull = "One or more arguments are null.";

}