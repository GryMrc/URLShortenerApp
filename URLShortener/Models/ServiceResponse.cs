namespace URLShortener.Models
{
    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public string? ShortenedUrl { get; set; }

        public string? UniqueCode { get; set; }
        public static ServiceResponse SuccessfulResponse(string uniquCode) => new ServiceResponse { Success = true, UniqueCode = uniquCode };

        public static ServiceResponse FailedResponse(string message) => new ServiceResponse { Message = message };
    }
}
