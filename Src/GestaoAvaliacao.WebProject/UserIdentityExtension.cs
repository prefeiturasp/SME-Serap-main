using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace GestaoAvaliacao.WebProject
{
    public static class UserIdentityExtension
    {
        public static string GetEntityId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.PrimarySid);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUsuLogin(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Name);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetGrupoId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.GroupSid);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static void AddGrupoId(this IIdentity identity, System.Web.HttpRequest request, string grupoId)
        {
            var identityUser = (ClaimsIdentity)identity;
            var claimGrupo = new Claim(ClaimTypes.GroupSid, grupoId);
            identityUser.AddClaim(claimGrupo);
            request.GetOwinContext().Authentication.SignIn(identityUser);
        }

        public static string GetToken(this IPrincipal user)
        {
            var principal = user as ClaimsPrincipal;

            var token = from c in principal.Identities.First().Claims
                        where c.Type == "access_token"
                        select c.Value;

            return token.FirstOrDefault();
        }

        public static string GetUserId(this IPrincipal user)
        {
            var principal = user as ClaimsPrincipal;

            var token = from c in principal.Identities.First().Claims
                        where c.Type == "sub"
                        select c.Value;

            return token.FirstOrDefault();
        }
    }
}
