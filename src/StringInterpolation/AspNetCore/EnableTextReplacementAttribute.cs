using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using StringInterpolation.Core;
using StringInterpolation.Core.Domain;
using StringReplacement.Core;
using StringReplacement.Core.Domain;
using System.Text.RegularExpressions;

namespace StringReplacement.AspNetCore
{
    public class EnableTextReplacementAttribute : ActionFilterAttribute
    {
        private readonly SearchKey _searchKey = SearchKey.Null;
        private readonly string _keyName = null;
        private readonly string _keyPattern = null;

        public EnableTextReplacementAttribute() { }

        public EnableTextReplacementAttribute(
            SearchKey searchKey = SearchKey.Null, 
            string keyName = null, 
            string keyPattern = null
        )
        {
            _searchKey = searchKey;
            _keyName = keyName;
            _keyPattern = keyPattern;
        }

        public EnableTextReplacementAttribute(string keyName = null, string keyPattern = null)
        {
            _keyName = keyName;
            _keyPattern = keyPattern;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var key = GetKey(context);

            using var textStream = new TextReplacementStream(context.HttpContext.Response.Body, key, _keyPattern);
            context.HttpContext.Response.Body = textStream;

            await next();
        }

        private string GetKey(ActionExecutingContext context)
        {
            var search = _searchKey != SearchKey.Null ? _searchKey : InterpolationConfig.SearchKey;
            return search switch
            {
                SearchKey.Header => GetKeyFromHeader(context),
                SearchKey.Query => GetKeyFromQuery(context),
                SearchKey.Route => GetKeyFromRoute(context),
                _ => null
            };
        }

        private string GetKeyFromRoute(ActionExecutingContext context)
        {
            var index = context.HttpContext.Request.RouteValues.Keys
                .ToList()
                .FindIndex(x => x.Equals(_keyName ?? InterpolationConfig.KeyName));

            return context.HttpContext.Request.RouteValues.Values.ToList()[index].ToString();
        }

        private string GetKeyFromQuery(ActionExecutingContext context)
        {
            var queryString = context.HttpContext.Request.QueryString.Value;

            var regex = new Regex($"({_keyName ?? InterpolationConfig.KeyName}=)([^&]+)").Matches(queryString);

            return regex[0].Groups[2].Value;
        }

        private string GetKeyFromHeader(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue(_keyName ?? InterpolationConfig.KeyName, out StringValues partner);
            return partner.ToString();
        }
    }
}
