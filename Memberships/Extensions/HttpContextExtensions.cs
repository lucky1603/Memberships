using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Memberships.Extensions
{
    public static class HttpContextExtensions
    {
        private const string nameidentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public static string GetUserId(this HttpContextBase ctx)
        {
            string uid = String.Empty;
            try
            {
                var claim = ctx.GetOwinContext().Get<ApplicationSignInManager>().AuthenticationManager.User.Claims.FirstOrDefault(c => c.Type.Equals(nameidentifier));
                if(claim != default(Claim))
                {
                    uid = claim.Value;
                }
            } catch { }

            return uid;
        }
    }
}