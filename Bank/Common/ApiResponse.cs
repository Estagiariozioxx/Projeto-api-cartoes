public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; } //t = qualquer tipo de dado
    public ApiError Errors { get; set; }

    public ApiResponse()
    {
        Errors = new ApiError();
    }

    // Método auxiliar para sucesso
    public static ApiResponse<T> SuccessResponse(T data, string message = "Operação realizada com sucesso.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
    public static ApiResponse<bool> SuccessResponse(bool data, string message = "Operação realizada com sucesso.")
    {
        return new ApiResponse<bool>
        {
            Success = data,
            Message = message,
            Data = data
        };
    }
    public static ApiResponse<T> SuccessResponse(object data, string message = "Operação realizada com sucesso.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = (T)data
        };
    }

    // Método auxiliar para erro
    public static ApiResponse<T> ErrorResponse(string message, ApiError errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default(T),
            Errors = errors
        };
    }
}

public class ApiError
{
    public string Field { get; set; }
    public string Message { get; set; }
}
