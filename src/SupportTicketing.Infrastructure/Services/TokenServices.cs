using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SupportTicketing.Application.Common.Interfaces;



namespace SupportTicketing.Services
{
      public class TokenServices : ITokenService
    {
        private readonly JwtSettings settings ;
        public TokenServices(IOptions<JwtSettings> options)
        {
            settings = options.Value ;
        }
        public string GenerateToken(int userId, string email,string userName,string role)
        {
            var Claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier,userId.ToString()),
                new(ClaimTypes.Email,email),
                new(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, role)
              
            };
            // foreach (var role in roles)
            // {
            //     Claims.Add(new Claim(ClaimTypes.Role, role));
            // }

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: Claims,
                expires: DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes),
                signingCredentials: credentials

            );
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }

     public class JwtSettings
    {
        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpirationMinutes { get; set; } = 60;
    }
}