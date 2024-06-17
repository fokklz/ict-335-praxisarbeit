namespace SaveUpBackend.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            // Add additional specific catches for other exceptions here
            catch (Exception ex)
            {
                await HandleGenericException(context, ex);
            }
        }


        private Task HandleGenericException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500; // Internal Server Error
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                return context.Response.WriteAsJsonAsync(new { error = ex.Message, stackTrace = ex.StackTrace });

            return context.Response.WriteAsJsonAsync(new { error = "An error occurred processing your request." });
        }
    }
}
