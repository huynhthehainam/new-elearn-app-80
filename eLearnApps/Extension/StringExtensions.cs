using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace eLearnApps.Extension
{
    public static class StringExtensions
    {
        private static readonly HashSet<char> DefaultNonWordCharacters = new HashSet<char> {' ', '.', ',', ';', ':'};
        private static readonly string EncryptNRICDelimitter = "||";
        private static readonly int EncryptNRICElapseMinutesThreshold = 5;

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string CropWholeWords(
            this string value,
            int length,
            HashSet<char> nonWordCharacters = null)
        {
            if (value == null) throw new ArgumentNullException("value");

            if (length < 0) throw new ArgumentException(@"Negative values not allowed.", "length");

            if (nonWordCharacters == null) nonWordCharacters = DefaultNonWordCharacters;

            if (length >= value.Length) return value;
            var end = length;

            for (var i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace()) break;

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

        private static bool IsWhitespace(this char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }
        public static string NricEncrypt(this string plainText)
        {
            if(string.IsNullOrEmpty(plainText)) return string.Empty;

            try
            {
                var rand = new Random();
                int iRnum = rand.Next(0, 15);
                string hexValue = iRnum.ToString("X");

                char keyByIdx = Constants.PhotoKey[iRnum];
                string original = $"{iRnum}{EncryptNRICDelimitter}{plainText}{EncryptNRICDelimitter}{DateTime.UtcNow.ToString("ddMMyyyyHHmm")}";
                return $"{hexValue}{original.ToEncrypt(keyByIdx.ToString())}";
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string NricDecrypt(this string encrypted)
        {
            if(string.IsNullOrEmpty(encrypted)) return string.Empty;

            try
            {
                var hexValue = encrypted[0].ToString();
                int iRnum = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                char keyByIdx = Constants.PhotoKey[iRnum];
                var original = encrypted.Substring(1).ToDecrypt(keyByIdx.ToString());

                // decrypted text must contains 2 tokens
                var arr = original.Split(new string[] { EncryptNRICDelimitter }, StringSplitOptions.None);
                // incorrect data passed in, return empty string
                if (arr.Length != 3) return string.Empty;
                var decryptedKeyIndex = arr[0];
                var nric = arr[1];
                var timeStamp = arr[2];

                // plain key idx and encrypted key idx must match
                if (iRnum.ToString() != decryptedKeyIndex) 
                    return string.Empty;

                // time stamp must be of exact format and cannot be older than 5 mins
                if(DateTime.TryParseExact(timeStamp, "ddMMyyyyHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    // only return decrypted nric when elapse minutes is less than threshold
                    var elapseMinutes = (DateTime.UtcNow - dt).TotalMinutes;
                    if (elapseMinutes < EncryptNRICElapseMinutesThreshold) return nric;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }

        }
        public static string ToCsv<T>(this IEnumerable<T> collection)
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