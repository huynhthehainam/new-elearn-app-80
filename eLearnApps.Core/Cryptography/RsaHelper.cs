using System;
using System.Security.Cryptography;
using System.Text;

namespace eLearnApps.Core.Cryptography
{
    public static class RsaHelper
    {
        private static readonly string publicKey =
            "<RSAKeyValue><Modulus>1uiL6LYTmOJq47DvfbBYxmGlQXI8uSJjNUymMPnaNkCqlZvf8GMEIxmBja5WJjPNlm9pVGMIKCYASOeGilz0mXY+wlq1z0q34MzfWIhCmi0s2u8xjsxNH9W3G0OrB61zsGzlXhbY7bJcldsbKoOFkVpHhDUx3LRzNDHGoDsmvZU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static readonly string privateKey =
            "<RSAKeyValue><Modulus>1uiL6LYTmOJq47DvfbBYxmGlQXI8uSJjNUymMPnaNkCqlZvf8GMEIxmBja5WJjPNlm9pVGMIKCYASOeGilz0mXY+wlq1z0q34MzfWIhCmi0s2u8xjsxNH9W3G0OrB61zsGzlXhbY7bJcldsbKoOFkVpHhDUx3LRzNDHGoDsmvZU=</Modulus><Exponent>AQAB</Exponent><P>3Hx9jEQuiUurWpNYJnJmAKj81szo98a480HWAddlc5Zrl/q6p9keVo7/TaTWpnFi77Kwxfhd9snwXJ6dypqoxw==</P><Q>+YYOqmTI1WpwmT88dgfibB4R/GWej+XIlpeb/2S1Ck6b7z6MYgp4a74Cc1IkWXpb4RpCYiYs1hldnXmZE2Riww==</Q><DP>uPPY4RPEsbj++ZDGDiJCfGVFCu6Csm5JcQ0V9x93Y9mXUbqqXuhbcaKseLZAtQhCRO3xKXApaj6FWTxZAr5vuw==</DP><DQ>WVQaz/mAxJZ0dQhkdsTf1GD3g2pF17Ilm3PvTwEYpvX5cS7tRvQEpF3DttFDXh0l43JpLYYJKyStlQDBQmX/zQ==</DQ><InverseQ>GG1BEuYl3ycHZut0AB5njbZ4rmYL2o3mfu6mIf6YUe4RCXtdW/hH5fYTVqz+N0hBbDfQjpHuC4SWJZUi4nPnqg==</InverseQ><D>IZoUnhsx7zqfqeA1YfqbttD3rMb21Z+Z0XCdn5TBWLA9u8y2c/iXYgGP7x4uHCUHZRkgZ/BgCx799mg08hdcqwSDxZdXZr9g+21QgKWyGY4ynHpYIsPJNVb5XoijnXRNIv1eLfB5v9EjLm3z8HXsLC4QuvNHbQv+xOk53QGSvlU=</D>\r\n</RSAKeyValue>";

        public static string Encrypt(string plainText)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(publicKey);
                    var encryptedData = rsa.Encrypt(bytesToEncrypt, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted;
                }
                catch
                {
                    return string.Empty;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string Decrypt(string encryptText)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(privateKey);
                    var resultBytes = Convert.FromBase64String(encryptText);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData;
                }
                catch
                {
                    return string.Empty;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}