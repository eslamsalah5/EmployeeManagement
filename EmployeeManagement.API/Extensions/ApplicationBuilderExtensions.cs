namespace EmployeeManagement.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app)
    {
        // CORS must be before Authentication/Authorization
        app.UseCors("AllowAll");

        // Static files
        app.UseStaticFiles();

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
