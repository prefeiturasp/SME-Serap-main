using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;

namespace GestaoAvaliacao.Util
{
    public static class JwtHelper
	{
		public static ClaimsPrincipal ValidateToken(string tokenString)
		{
			var tokenHandler = new System.IdentityModel.Tokens.JwtSecurityTokenHandler();

			var securityKey = GetBytes(ConfigurationManager.AppSettings["securityKey"]);
			var validationParameters = new TokenValidationParameters()
			{
				IssuerSigningToken = new BinarySecretSecurityToken(securityKey),
				ValidIssuer = ConfigurationManager.AppSettings["ValidIssuer"],
				ValidateLifetime = true,
				ValidateAudience = false,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true
			};
			SecurityToken validatedToken = null;
			var principal = tokenHandler.ValidateToken(tokenString, validationParameters, out validatedToken);
			return principal;
		}


		public static string CreateToken(string userId = null, string role = null, string pes_id = null, string ent_id = null, string data = null, string userName = null)
		{
			var securityKey = GetBytes(ConfigurationManager.AppSettings["securityKey"]);

			var tokenHandler = new JwtSecurityTokenHandler();

			// Token Creation
			var now = DateTime.UtcNow;
			var claims = new List<Claim>();
			if (!string.IsNullOrEmpty(userId))
				claims.Add(new Claim(ClaimTypes.Name, userId));
			if (!string.IsNullOrEmpty(role))
				claims.Add(new Claim(ClaimTypes.Role, role));
			if (!string.IsNullOrEmpty(pes_id))
				claims.Add(new Claim(ClaimTypes.NameIdentifier, pes_id));
			if (!string.IsNullOrEmpty(ent_id))
				claims.Add(new Claim(ClaimTypes.System, ent_id));
			if (!string.IsNullOrEmpty(data))
				claims.Add(new Claim(ClaimTypes.UserData, data));
			if (!string.IsNullOrEmpty(userName))
				claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				TokenIssuerName = ConfigurationManager.AppSettings["ValidIssuer"],
				Lifetime = new Lifetime(now, now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["MinutesLifetimeToken"]))),
				SigningCredentials = new SigningCredentials(
					new InMemorySymmetricSecurityKey(securityKey),
					"http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
					"http://www.w3.org/2001/04/xmlenc#sha256"),
			};
			

			var token = tokenHandler.CreateToken(tokenDescriptor);

			// Generate Token and return string
			return tokenHandler.WriteToken(token);
		}

		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;

		}
	}
}
