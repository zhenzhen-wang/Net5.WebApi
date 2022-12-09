using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace Mitac.Core.Filters
{

    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            //var httpcontext = context.GetHttpContext();
            //return httpcontext.User.Identity.IsAuthenticated;
            return true;  //hangfire看板页面不做权限认证,直接打开'/hangfire'
        }

    }
}