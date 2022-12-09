using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Mitac.Core.Filters
{
    class ResourceFilter : Attribute, IResourceFilter
    {
        public ResourceFilter()
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            throw new NotImplementedException();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            throw new NotImplementedException();
        }

    }
}
