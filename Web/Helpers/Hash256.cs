using ApplicationCore.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Web.Helpers
{
    public static class Hash256
    {
        public static string GetSHA256(string password)
        {
            var passwordtoLower = password.ToLower();
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(passwordtoLower);
            var hash = sha256.ComputeHash(bts);

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
