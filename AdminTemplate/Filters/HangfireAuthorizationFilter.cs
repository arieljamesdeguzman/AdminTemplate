using Hangfire.Dashboard;

namespace AdminTemplate.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow only authenticated users
            // You can add role-based checks here if needed
            return httpContext.User.Identity?.IsAuthenticated ?? false;
        }
    }
}