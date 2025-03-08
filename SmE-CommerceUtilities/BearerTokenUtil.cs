using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SmE_CommerceUtilities;

public class BearerTokenUtil(IConfiguration configuration)
{
    public string GenerateBearerToken(Guid userId, string role)
    {
        // Encrypt role
        var encryptedRole = HashUtil.Hash(role);

        // Create claims list
        List<Claim> claims =
        [
            new(ClaimTypes.Sid, userId.ToString()),
            new(ClaimTypes.Role, encryptedRole),
        ];

        // Get security key from configuration
        var securityKey =
            configuration.GetSection("AppSettings:Token").Value
            ?? throw new Exception("SERVER_ERROR: Token key is missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Set token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = TimeZoneInfo
                .ConvertTime(
                    DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                )
                .AddDays(7),
            SigningCredentials = creds,
        };

        // Create the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the written token
        return tokenHandler.WriteToken(token)
            ?? throw new Exception("SERVER_ERROR: Token generation failed");
    }

    // private string Encrypt(string plainText)
    // {
    //     var value = configuration.GetSection("AppSettings:EncryptionKey").Value
    //                 ?? throw new Exception("SERVER_ERROR: Encryption key is missing");
    //     var key = Encoding.UTF8.GetBytes(value);
    //     using var aes = Aes.Create();
    //     aes.Key = key;
    //     aes.GenerateIV();
    //     using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
    //     using var msEncrypt = new MemoryStream();
    //     using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    //     using (var swEncrypt = new StreamWriter(csEncrypt))
    //     {
    //         swEncrypt.Write(plainText);
    //     }
    //
    //     var iv = aes.IV;
    //     var encryptedContent = msEncrypt.ToArray();
    //     var result = new byte[iv.Length + encryptedContent.Length];
    //     Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
    //     Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);
    //
    //     return Convert.ToBase64String(result);
    // }
    //
    // private string Decrypt(string cipherText)
    // {
    //     var fullCipher = Convert.FromBase64String(cipherText);
    //     var iv = new byte[16];
    //     var cipher = new byte[16];
    //
    //     Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
    //     Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
    //
    //     var value = configuration.GetSection("AppSettings:EncryptionKey").Value
    //                 ?? throw new Exception("SERVER_ERROR: Encryption key is missing");
    //     var key = Encoding.UTF8.GetBytes(value);
    //
    //     using var aes = Aes.Create();
    //     aes.Key = key;
    //     aes.IV = iv;
    //     using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    //     using var msDecrypt = new MemoryStream(cipher);
    //     using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
    //     using var srDecrypt = new StreamReader(csDecrypt);
    //
    //     return srDecrypt.ReadToEnd();
    // }
}
