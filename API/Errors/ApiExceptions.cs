namespace API.Errors
{
    public class ApiExceptions
    {
        public ApiExceptions(int statusCode, string messgae = null, string details = null)
        {
            StatusCode = statusCode;
            Messgae = messgae;
            Details = details;
        }

        public int StatusCode { get; set;}

        public string Messgae { get; set;}

        public string Details { get; set;}


        
        
    }
}