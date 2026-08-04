namespace BlazorFrontend.Models.Base
{
    public class ApiResult<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}
