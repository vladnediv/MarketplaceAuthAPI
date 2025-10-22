namespace BLL.Model.ResponseModel;

public class ServiceResponse<T> where T : class
{
    public bool IsSuccess { get; set; }
    public T? Entity { get; set; }
    public List<T> Entities { get; set; }
    public string? Message { get; set; }
}

public class ServiceResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}