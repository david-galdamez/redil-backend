namespace redil_backend.Services
{
    public record ServiceResult<T>(bool Success, T? Data, string? ErrorMessage)
    {
        public static ServiceResult<T> Ok(T data) => new ServiceResult<T>(true, data, null);
        public static ServiceResult<T> Fail(string errorMessage) => new ServiceResult<T>(false, default, errorMessage);
    }
}
