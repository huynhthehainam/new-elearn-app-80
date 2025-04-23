using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace eLearnApps.Extension
{
    public class StringExtensions
    {
        private readonly Constants _constants;
        private readonly IServiceProvider _serviceProvider;
        private readonly Extensions _extensions;
        public StringExtensions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            _extensions = new Extensions(serviceProvider);
            _constants = new Constants(configuration);
        }
        private static readonly HashSet<char> _defaultNonWordCharacters = new HashSet<char> { ' ', '.', ',', ';', ':' };
        private static readonly string _encryptNRICDelimitter = "||";
        private static readonly int _encryptNRICElapseMinutesThreshold = 5;
        public bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public string CropWholeWords(
             string value,
            int length,
            HashSet<char> nonWordCharacters = null)
        {
            if (value == null) throw new ArgumentNullException("value");

            if (length < 0) throw new ArgumentException(@"Negative values not allowed.", "length");

            if (nonWordCharacters == null) nonWordCharacters = _defaultNonWordCharacters;

            if (length >= value.Length) return value;
            var end = length;

            for (var i = end; i > 0; i--)
            {
                if (IsWhitespace(value[i])) break;

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                    //Removing a character that isn't whitespace but not part 
                    //of the word either (ie ".") given that the character is 
                    //followed by whitespace or the end of the string makes it
                    //possible to include the word, so we do that.
                    break;
                end--;
            }

            if (end == 0)
                //If the first word is longer than the length we favor 
                //returning it as cropped over returning nothing at all.
                end = length;

            return value.Substring(0, end);
        }

        private bool IsWhitespace(char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }
        public string NricEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            try
            {
                var rand = new Random();
                int iRnum = rand.Next(0, 15);
                string hexValue = iRnum.ToString("X");

                char keyByIdx = _constants.PhotoKey[iRnum];
                string original = $"{iRnum}{_encryptNRICDelimitter}{plainText}{_encryptNRICDelimitter}{DateTime.UtcNow.ToString("ddMMyyyyHHmm")}";
                return $"{hexValue}{_extensions.ToEncrypt(original, keyByIdx.ToString())}";
            }
            catch
            {
                return string.Empty;
            }
        }
        public string NricDecrypt(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted)) return string.Empty;

            try
            {
                var hexValue = encrypted[0].ToString();
                int iRnum = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                char keyByIdx = _constants.PhotoKey[iRnum];
                var original = _extensions.ToDecrypt(encrypted.Substring(1), keyByIdx.ToString());

                // decrypted text must contains 2 tokens
                var arr = original.Split(new string[] { _encryptNRICDelimitter }, StringSplitOptions.None);
                // incorrect data passed in, return empty string
                if (arr.Length != 3) return string.Empty;
                var decryptedKeyIndex = arr[0];
                var nric = arr[1];
                var timeStamp = arr[2];

                // plain key idx and encrypted key idx must match
                if (iRnum.ToString() != decryptedKeyIndex)
                    return string.Empty;

                // time stamp must be of exact format and cannot be older than 5 mins
                if (DateTime.TryParseExact(timeStamp, "ddMMyyyyHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    // only return decrypted nric when elapse minutes is less than threshold
                    var elapseMinutes = (DateTime.UtcNow - dt).TotalMinutes;
                    if (elapseMinutes < _encryptNRICElapseMinutesThreshold) return nric;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }

        }
        public string ToCsv<T>(IEnumerable<T> collection)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(collection);
                }
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}