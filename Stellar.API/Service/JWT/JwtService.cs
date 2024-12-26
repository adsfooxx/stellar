using Microsoft.IdentityModel.Tokens;
using Stellar.API.JWT.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stellar.API.Service.JWT
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthResultDto GenerateToken(string userName, int expireMinutes = 30)
        {
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

            // 設定要加入到 JWT Token 中的聲明
            var claims = new List<Claim>();

            //使用定義的規格  https://datatracker.ietf.org/doc/html/rfc7519#section-4.1
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            //宣告集合所宣告的身分識別
            var userClaimsIdentity = new ClaimsIdentity(claims);

            //建立一組對稱式的金鑰 ,主要用於 JWT 的相關設定
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            //用來生成數位簽章的密碼編譯演算法
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //預留位置, 適用於和已發行權杖相關的所有屬性, 用來定義JWT的相關設定
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            //用來產生JWT 
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return new AuthResultDto()
            {
                Token = serializeToken,
                ExpireTime = new DateTimeOffset(tokenDescriptor.Expires.Value).ToUnixTimeSeconds(),
            };
        }
    }
}
