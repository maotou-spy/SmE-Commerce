using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmE_CommerceUtilities;

public class BearerTokenUtil
{
    private readonly IConfiguration _configuration;

    public BearerTokenUtil(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateBearerToken(Guid userId, string role)
    {
        // Create claims list
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Sid, userId.ToString()),
            new Claim(ClaimTypes.Role, Encrypt(role))
        };

        // Get security key from configuration
        var securityKey = _configuration.GetSection("AppSettings:Token").Value
                          ?? throw new Exception("SERVER_ERROR: Token key is missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Set token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = TimeZoneInfo
                .ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                .AddDays(7),
            SigningCredentials = creds
        };

        // Create the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the written token
        return tokenHandler.WriteToken(token) ?? throw new Exception("SERVER_ERROR: Token generation failed");
    }

    public bool VerifyToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value
            ?? throw new Exception("SERVER_ERROR: Token key is missing"));

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        }, out var validatedToken);

        return validatedToken != null;
    }

    private string Encrypt(string plainText)
    {
        using var aesAlg = Aes.Create();
        var _encryptionKey = _configuration.GetSection("AppSettings:EncryptionKey").Value
                                        ?? throw new Exception("SERVER_ERROR: Encryption key is missing");
        aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
        aesAlg.IV = new byte[16];

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    private string Decrypt(string cipherText)
    {
        using var aesAlg = Aes.Create();
        var _encryptionKey = _configuration.GetSection("AppSettings:EncryptionKey").Value
                                        ?? throw new Exception("SERVER_ERROR: Encryption key is missing");
        aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
        aesAlg.IV = new byte[16];

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }

    public string GetRoleFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            throw new SecurityTokenException("Invalid token");

        var encryptedRole = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

        var decryptedRole = Decrypt(encryptedRole);

        return decryptedRole;
    }
}
