using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class StringExtensions
    {

        private static readonly Regex _regexIllegal = new Regex(@"[\[, \]]", RegexOptions.Compiled);
        private static readonly Regex _regex1 = new Regex(@"([A-Z]+)([A-Z][a-z])", RegexOptions.Compiled);
        private static readonly Regex _regex2 = new Regex(@"([a-z\d])([A-Z])", RegexOptions.Compiled);
        private static readonly Regex _emailRegex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        private static char[] _maskChars = { '*', '?' };
        public static string PascalToKebabCase(this string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            name = _regexIllegal.Replace(name, "_").TrimEnd('_');
            // Replace any capital letters, apart from the first character, with _x, the same way Ruby does
            return _regex2.Replace(_regex1.Replace(name, "$1_$2"), "$1_$2").ToLower();
        }

        public static bool IsValidEmail(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return _emailRegex.IsMatch(input);
        }

        public static string JoinWithoutWhitespaces(this IEnumerable<string> inputs, string separator)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }
            var result = string.Join(separator, inputs.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()));
            return !string.IsNullOrEmpty(result) ? result : null;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/484085/an-algorithm-to-spacify-camelcased-strings
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spacer"></param>
        /// <returns></returns>
        public static string Decamelize(this string str, char spacer = '_')
        {
            if (string.IsNullOrEmpty(str))
                return str;

            /* Note that the .ToString() is required, otherwise the char is implicitly
             * converted to an integer and the wrong overloaded ctor is used */
            var sb = new StringBuilder(str[0].ToString());
            for (var i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str, i))
                    sb.Append(spacer);
                sb.Append(str[i]);
            }
            return sb.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Equals invariant
        /// </summary>
        /// <param name="str1">The STR1.</param>
        /// <param name="str2">The STR2.</param>
        /// <returns></returns>
        public static bool EqualsInvariant(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool FilePathHasMaskChars(this string path)
        {
            return (!string.IsNullOrEmpty(path) && path.IndexOfAny(_maskChars) >= 0);
        }

        public static bool FitsMask(this string fileName, string fileMask)
        {
            var mask = new Regex("^" + Regex.Escape(fileMask).Replace("\\.", "[.]").Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
            return mask.IsMatch(fileName);
        }

        public static int? ToNullableInt(this string str)
        {
            int retVal;
            if (int.TryParse(str, out retVal))
            {
                return retVal;
            }
            return null;
        }

        public static Tuple<string, string, string> SplitIntoTuple(this string input, char separator)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var pieces = input.Split(separator);
            return Tuple.Create(pieces.FirstOrDefault(), pieces.Skip(1).FirstOrDefault(), pieces.Skip(2).FirstOrDefault());
        }

        public static string RemoveAccent(this string txt)
        {
            var bytes = Encoding.ASCII.GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string Handelize(this string phrase)
        {
            var retVal = phrase;
            if (phrase != null)
            {
                retVal = phrase.ToLower();

                retVal = Regex.Replace(retVal, @"[^\p{L}\d\s-]", ""); // invalid chars           
                retVal = Regex.Replace(retVal, @"\s+", " ").Trim(); // convert multiple spaces into one space   
                retVal = retVal.Substring(0, retVal.Length <= 240 ? retVal.Length : 240).Trim(); // cut and trim it   
                retVal = Regex.Replace(retVal, @"\s", "-"); // hyphens   
            }
            return retVal;
        }

        //http://www.ietf.org/rfc/rfc3986.txt
        //section 4.2 Relative reference
        //Remove leading protocol scheme from uri. http://host/path -> //host/path
        public static string RemoveLeadingUriScheme(this string str)
        {
            if (!string.IsNullOrEmpty(str) && Uri.IsWellFormedUriString(str, UriKind.Absolute))
            {
                //remove scheme from image url
                //http://www.ietf.org/rfc/rfc3986.txt
                //section 4.2
                var uri = new Uri(str);
                str = "//" + uri.Authority + uri.PathAndQuery;
            }
            return str;
        }

        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            var place = source.LastIndexOf(find);
            var result = source;
            if (place >= 0)
            {
                result = source.Remove(place, find.Length).Insert(place, replace);
            }
            return result;
        }
        /// <summary>
        /// Add provided suffix to the end of file name
        /// </summary>
        /// <param name="originalFileUrl">File url</param>
        /// <param name="suffix">Suffix</param>
        /// Example: "1428965138000_1133723.jpg".AddSuffixToFileUrl("grande") 
        /// Result: 1428965138000_1133723_grande.jpg
        /// <returns></returns>
        public static string AddSuffixToFileUrl(this string originalFileUrl, string suffix)
        {
            if (originalFileUrl == null)
            {
                throw new ArgumentNullException(nameof(originalFileUrl));
            }
            var result = originalFileUrl;
            var fileName = Path.GetFileName(originalFileUrl);
            if (!string.IsNullOrEmpty(fileName))
            {
                var newFileName = Path.GetFileNameWithoutExtension(fileName) + suffix;
                var extension = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(extension))
                {
                    newFileName += extension;
                }
                result = result.ReplaceLastOccurrence(fileName, newFileName);
            }
            return result;
        }

    }
}
