using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlackBank.Models;

namespace BlackBank.Services
{
    public class JwtServices
    {
        private readonly IConfiguration _configuration;
        private readonly DaneUzytkownikaDbContext _context;

        public JwtServices(IConfiguration configuration, DaneUzytkownikaDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(string email)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["TokenValidityMins"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]

           };
               
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}