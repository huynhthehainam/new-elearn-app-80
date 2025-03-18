using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;

namespace eLearnApps.Models.D2L
{
    public class OAuth
    {
        private string _url = "";

        public const string OAuthVersion = "1.0";

        // List of known and used oauth parameters' names  
        public const string OAuthConsumerKeyKey = "oauth_consumer_key";
        public const string OAuthCallbackKey = "oauth_callback";
        public const string OAuthVersionKey = "oauth_version";
        public const string OAuthSignatureMethodKey = "oauth_signature_method";
        public const string OAuthSignatureKey = "oauth_signature";
        public const string OAuthTimestampKey = "oauth_timestamp";
        public const string OAuthNonceKey = "oauth_nonce";
        public const string OAuthTokenKey = "oauth_token";
        public const string OAuthTokenSecretKey = "oauth_token_secret";

        public const string HMACSHA1SignatureType = "HMAC-SHA1";
        public const string PlainTextSignatureType = "PLAINTEXT";
        public const string RSASHA1SignatureType = "RSA-SHA1";

        protected Random random = new Random();

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public OAuth()
        {
        }

        public OAuth(string url)
        {
            _url = url;

        }
        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }
        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("No Hash Algorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("No data to be encrypted");
            }

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }


        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        protected string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        protected NameValueCollection Sort(NameValueCollection list)
        {
            NameValueCollection optionInfoListSorted = new NameValueCollection();
            String[] sortedKeys = list.AllKeys;
            Array.Sort(sortedKeys);
            foreach (String key in sortedKeys)
            {
                optionInfoListSorted.Add(key, list[key]);
            }

            return optionInfoListSorted;
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters NameValueCollection</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IFormCollection parameters)
        {
            var sortedParams = parameters.OrderBy(p => p.Key);
            StringBuilder sb = new StringBuilder();

            foreach (var param in sortedParams)
            {
                if (param.Key != OAuthSignatureKey)
                {
                    if (sb.Length > 0) sb.Append("&");
                    sb.AppendFormat("{0}={1}", param.Key, UrlEncode(param.Value));
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Generate a Signature base from a HttpRequest
        /// assume it's a POST
        /// </summary>
        /// <param name="request">HttpRequest object</param>
        /// <returns>return a string encoded signature format [http method]&[normalized url]&[normalized parameters]</returns>
        public string GenerateSignatureBase(HttpRequest request, string signature_host, string signature_scheme)
        {
            return GenerateSignatureBase(request, "POST", signature_host, signature_scheme);
        }

        /// <summary>
        /// Generate a Signature base from a HttpRequest and HttpMethod
        /// </summary>
        /// <param name="request">HttpRequest object</param>
        /// <param name="httpMethod">http method (POST, GET)</param>
        /// <returns>return a string encoded signature format [http method]&[normalized url]&[normalized parameters]</returns>
        public string GenerateSignatureBase(HttpRequest request, string httpMethod, string signatureHost, string signatureScheme)
        {
            var parameters = request.Form;
            Uri url = new Uri($"{signatureScheme}://{signatureHost}{request.Path}");

            string normalizedRequestParameters = NormalizeRequestParameters(parameters);
            StringBuilder signatureBase = new StringBuilder();

            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(_url == "" ? url.ToString() : _url));
            signatureBase.Append(UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate a Signature base
        /// </summary>
        /// <param name="request">HttpRequest object</param>
        /// <param name="httpMethod">http method (POST, GET)</param>
        /// <param name="parameters">NameValueCollection of parameter List</param>
        /// <returns>return a string encoded signature format [http method]&[normalized url]&[normalized parameters]</returns>
        public string GenerateSignatureBase(Uri url, string httpMethod, IFormCollection parameters)
        {
            string normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;

            string normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());

            if (_url == "")
            {
                signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            }
            else
            {
                signatureBase.AppendFormat("{0}&", UrlEncode(_url));
            }

            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));
            //log.Debug(signatureBase.ToString());
            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }


        /// <summary>
        /// generate the signature value using HMACSHA1 harshing based on a given HttpRequest
        /// assume: it's a POST and without token secret
        /// </summary>
        /// <param name="request">HttpRequest object</param>
        /// <param name="consumerSecret">consumer secret</param>
        /// <returns>signature</returns>
        public string GenerateSignature(HttpRequest request, string consumerSecret, string signature_host, string signature_scheme)
        {
            return GenerateSignature(request, "POST", SignatureTypes.HMACSHA1, consumerSecret, "", signature_host, signature_scheme);
        }

        public string GenerateSignature(Uri url, IFormCollection parameters, string secretKey)
        {
            string signatureBase = GenerateSignatureBase(url, "POST", parameters);

            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(secretKey), ""));

            return GenerateSignatureUsingHash(signatureBase, hmacsha1);
        }
        /// <summary>
        /// generate the signature value based on a given HttpRequest
        /// </summary>
        /// <param name="request">HttpRequest object</param>
        /// <param name="httpMethod">http method (POST, GET)</param>
        /// <param name="signatureType">SignatureTypes</param>
        /// <param name="consumerSecret">consumer secret</param>
        /// <param name="tokenSecret">token secret</param>
        /// <returns>signature</returns>
        public string GenerateSignature(HttpRequest request, string httpMethod, SignatureTypes signatureType, string consumerSecret, string tokenSecret, string signatureHost, string signatureScheme)
        {

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return WebUtility.UrlEncode($"{consumerSecret}&{tokenSecret}");

                case SignatureTypes.HMACSHA1:
                    {
                        string signatureBase = GenerateSignatureBase(request, httpMethod, signatureHost, signatureScheme);
                        using var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes($"{UrlEncode(consumerSecret)}&{(string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret))}"));

                        return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                    }

                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException("Unknown signature type", nameof(signatureType));
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            Random random = new Random();
            return random.Next(123400, 9999999).ToString();

        }


    }
}