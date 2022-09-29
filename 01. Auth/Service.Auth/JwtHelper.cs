using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Service.Auth
{
    public class JwtHelper
    {

        public static readonly int JWT_EXPIRATION_MINUTES = 60;
        /// <summary>
        /// Creates a new JWT token for a given user.
        /// This new token can be used from now on to call other services in the system
        /// </summary>
        /// <param name="userId">Internal identifier of the user</param>
        /// <param name="userEmail">Public identifier of the user</param>
        /// <param name="role">Identifier of the role the user will have authorized</param>
        /// <returns>The new JWT token string</returns>
        public static string GenerateJwtToken(string userId, string userEmail, string role)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKeyProvider());
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim("Custom.Email", userEmail),
                    new Claim("Custom.Role", role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(JWT_EXPIRATION_MINUTES),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }

        /// <summary>
        /// Provides the secret key used to sign JWT tokens.
        /// This key is static but generated obfuscated to avoid extraction by reverse engineering.
        /// </summary>
        /// <returns>The secret key in clear</returns>
        public static string SecretKeyProvider()
        {
            // Generating: ContentManagemenT_%15243&_Security_!60798!_jwt_KEY_48576

            string a = "ContentManagement"; // t to be uppercased.
            string b = "SecuritY"; // Y to be lowercased.
            string c = "JWT"; // To be fully lowercased.
            string d = "key"; // To be fully uppercased.

            string k = "1234567890"; // 'k' to be the final secret key. Initial value to be lost.

            // Generating %15243&
            string code1 = "" + (char)37 + "1"; // %1
            int v = 4;
            int x = 1;
            for (int n = 0; n < 5; n++)
            {
                x += (n % 2 == 0) ? v : -v;
                code1 += x.ToString();
                v--;
            }
            code1 += (char)38; // &

            // Generating !60798!
            string code2 = "" + (char)33 + "6"; // !6
            v = 4;
            x = 6;
            for (int n = 0; n < 5; n++)
            {
                x += (n % 2 == 0) ? (v % 10) : -(v % 10);
                code2 += x.ToString();
                v--;
            }
            code1 += (char)33; // !

            // Generating 48576
            string code3 = "" + "4";
            v = 4;
            x = 4;
            for (int n = 0; n < 5; n++)
            {
                x += (n % 2 == 0) ? v : -v;
                code3 += x.ToString();
                v--;
            }
            code1 += (char)33; // !

            // Concat parts.
            k = a + (char)95 + code1 + (char)95 + b + (char)95 + code2 + (char)95 + c.ToLower() + (char)95 + d.ToUpper() + (char)95 + code3;

            // Fix cases.
            k.Replace((char)108, (char)76); // l --> L
            k.Replace((char)89, (char)121); // Y --> y

            // Final key is the first 50 characters of the 'k' base 64 encoded.
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(k)).Substring(0, 50);
        }


    }
}