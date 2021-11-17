using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Scriban;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    //TODO: Left only unique filters don't presents in Scriban
    public static partial class StandardFilters
    {
        /// <summary>
        /// Return the size of an array or of an string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Size(object input)
        {
            if (input is string str)
            {
                return str.Length;
            }

            return input is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;
        }

        /// <summary>
        /// Return a Part of a String
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Slice(string input, int start, int len = 1)
        {
            if (input == null || start > input.Length)
            {
                return null;
            }

            if (start < 0)
            {
                start += input.Length;
            }

            if (start + len > input.Length)
            {
                len = input.Length - start;
            }

            return input.Substring(start, len);
        }

        /// <summary>
        /// convert a input string to DOWNCASE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Downcase(string input)
        {
            return input == null ? input : input.ToLower();
        }

        /// <summary>
        /// convert a input string to UPCASE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Upcase(string input)
        {
            return input == null
                ? input
                : input.ToUpper();
        }

        /// <summary>
        /// capitalize words in the input sentence
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Capitalize(string input)
        {
            if (input.IsNullOrWhiteSpace())
            {
                return input;
            }

            return string.IsNullOrEmpty(input)
                ? input
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        public static string Escape(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            try
            {
                return WebUtility.HtmlEncode(input);
            }
            catch
            {
                return input;
            }
        }

        public static string H(string input)
        {
            return Escape(input);
        }

        /// <summary>
        /// Truncates a string down to x characters
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="truncateString"></param>
        /// <returns></returns>
        public static string Truncate(string input, int length = 50, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var l = length - truncateString.Length;

            var startIndex = l < 0 ? 0 : l;

            return input.Length > length
                ? input.Substring(0, startIndex) + truncateString
                : input;
        }

        public static string Truncatewords(string input, int words = 15, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var wordList = input.Split(' ').ToList();
            var l = words < 0 ? 0 : words;

            return wordList.Count > l
                ? string.Join(" ", wordList.Take(l).ToArray()) + truncateString
                : input;
        }

        /// <summary>
        /// Split input string into an array of substrings separated by given pattern.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] Split(string input, string pattern)
        {
            return input.IsNullOrWhiteSpace()
                ? new[] { input }
                : input.Split(new[] { pattern }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string StripHtml(object input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var inputString = input.ToString();

            return inputString.IsNullOrWhiteSpace()
                ? inputString
                : Regex.Replace(inputString, @"<.*?>", string.Empty);
        }

        /// <summary>
        /// Remove all newlines from the string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripNewlines(string input)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : Regex.Replace(input, @"(\r?\n)", string.Empty);
        }

        /// <summary>
        /// Join elements of the array with a certain character between them
        /// </summary>
        /// <param name="input"></param>
        /// <param name="glue"></param>
        /// <returns></returns>
        public static string Join(IEnumerable input, string glue = " ")
        {
            if (input == null)
            {
                return null;
            }

            var castInput = input.Cast<object>();

            return string.Join(glue, castInput);
        }

        /// <summary>
        /// Sort elements of the array
        /// provide optional property with which to sort an array of hashes or drops
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable Sort(object input, string property = null)
        {
            var ary = input is IEnumerable ? ((IEnumerable)input).Flatten().Cast<object>().ToList() : new List<object>(new[] { input });
            if (!ary.Any())
            {
                return ary;
            }

            if (string.IsNullOrEmpty(property))
            {
                ary.Sort();
            }
            else if ((ary.All(o => o is IDictionary)) && ((IDictionary)ary.First()).Contains(property))
            {
                ary.Sort((a, b) => Comparer.Default.Compare(((IDictionary)a)[property], ((IDictionary)b)[property]));
            }
            else if (ary.All(o => o.RespondTo(property)))
            {
                ary.Sort((a, b) => Comparer.Default.Compare(a.Send(property), b.Send(property)));
            }

            return ary;
        }

        /// <summary>
        /// Map/collect on a given property
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable Map(IEnumerable input, string property)
        {
            if (input == null)
            {
                return input;
            }

            var ary = input.Cast<object>().ToList();
            if (!ary.Any())
            {
                return ary;
            }

            if ((ary.All(o => o is IDictionary)) && ((IDictionary)ary.First()).Contains(property))
            {
                return ary.Select(e => ((IDictionary)e)[property]);
            }

            return ary.All(o => o.RespondTo(property)) ? ary.Select(e => e.Send(property)) : ary;
        }

        /// <summary>
        /// Replace occurrences of a string with another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Replace(object input, string @string, string replacement = "")
        {
            if (input == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(input.ToString()) || string.IsNullOrEmpty(@string))
            {
                return input.ToString();
            }

            input = input.ToString().Replace(@string, replacement);

            return input.ToString();
        }
        /// <summary>
        /// Replace the first occurence of a string with another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceFirst(string input, string @string, string replacement = "")
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(@string))
            {
                return input;
            }

            var doneReplacement = false;
            return Regex.Replace(input, @string, m =>
            {
                if (doneReplacement)
                {
                    return m.Value;
                }

                doneReplacement = true;
                return replacement;
            });
        }

        /// <summary>
        /// Remove a substring
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string Remove(string input, string @string)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : input.Replace(@string, string.Empty);
        }

        /// <summary>
        /// Remove the first occurrence of a substring
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string RemoveFirst(string input, string @string)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : ReplaceFirst(input, @string, string.Empty);
        }

        /// <summary>
        /// Add one string to another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string Append(object input, object @string)
        {
            return input + @string.ToSafeString();
        }

        /// <summary>
        /// Prepend a string to another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string Prepend(object input, object @string)
        {
            return @string.ToSafeString() + input;
        }

        /// <summary>
        /// Add <br /> tags in front of all newlines in input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NewlineToBr(string input)
        {
            return input.IsNullOrWhiteSpace()
                    ? input
                    : Regex.Replace(input, @"(\r?\n)", "<br />$1");
        }

        /// <summary>
        /// Formats a date using a .NET date format string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Date(TemplateContext context, object input, string format)
        {
            if (input == null)
            {
                return null;
            }

            if (format.IsNullOrWhiteSpace())
            {
                return input.ToString();
            }

            switch (format)
            {
                case "long":
                    //TODO: define which way to use. IMHO using modern style is more prefered
                    //format = "%d %b %Y %X";
                    format = "f";
                    break;
            }

            var result = input.ToString();
            DateTime date;
            var dateParsed = false;

            if (input.ToString().Equals("now", StringComparison.OrdinalIgnoreCase))
            {
                date = DateTime.Now;
                dateParsed = true;
            }
            else if (DateTime.TryParse(input.ToString(), out date))
            {
                dateParsed = true;
            }
            if (Regex.IsMatch(format, @"^[\w\d_\-]+$"))
            {
                var key = string.Concat("date_formats.", format);
                var newFormat = TranslationFilter.T(context, key);
                if (!newFormat.IsNullOrEmpty() && newFormat != key)
                {
                    format = newFormat;
                }
            }
            if (dateParsed)
            {
                var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
                TryFormatDateTime(date, format, out result, CultureInfo.GetCultureInfo(themeEngine.WorkContext.CurrentLanguage.CultureName));
            }

            return result;
        }

        /// <summary>
        /// Get the first element of the passed in array 
        /// 
        /// Example:
        ///   {{ product.images | first | to_img }}
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object First(IEnumerable array)
        {
            return array == null ? null : array.Cast<object>().FirstOrDefault();
        }

        /// <summary>
        /// Get the last element of the passed in array 
        /// 
        /// Example:
        ///   {{ product.images | last | to_img }}
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object Last(IEnumerable array)
        {
            return array == null ? null : array.Cast<object>().LastOrDefault();
        }

        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object Plus(object input, object operand)
        {

            return DoMathsOperation(input, operand, Expression.Add);
        }

        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object Minus(object input, object operand)
        {
            return DoMathsOperation(input, operand, Expression.Subtract);
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object Times(object input, object operand)
        {

            return DoMathsOperation(input, operand, Expression.Multiply);

        }

        /// <summary>
        /// Division
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object DividedBy(object input, object operand)
        {
            return DoMathsOperation(input, operand, Expression.Divide);
        }

        public static object Modulo(object input, object operand)
        {
            return DoMathsOperation(input, operand, Expression.Modulo);
        }

        private static object DoMathsOperation(object input, object operand, Func<Expression, Expression, BinaryExpression> operation)
        {
            input = input.ToNumber();
            operand = operand.ToNumber();

            return input == null || operand == null
                ? null
                : ExpressionUtility.CreateExpression(operation, input.GetType(), operand.GetType(), input.GetType(), true)
                    .DynamicInvoke(input, operand);
        }

        private static void TryFormatDateTime(DateTime input, string format, out string formated, IFormatProvider formatProvider = null)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            formated = null;

            try
            {
                formated = input.ToString(format, formatProvider);
            }
            catch
            {
                //Swallow any exception 
            }
        }
    }

    /// <summary>
    /// Some of this code was taken from http://www.yoda.arachsys.com/csharp/miscutil/usage/genericoperators.html.
    /// General purpose Expression utilities
    /// </summary>
    internal static class ExpressionUtility
    {
        /// <summary>
        /// Create a function delegate representing a binary operation
        /// </summary>
        /// <param name="body">Body factory</param>
        /// <param name="leftType"></param>
        /// <param name="rightType"></param>
        /// <param name="resultType"></param>
        /// <param name="castArgsToResultOnFailure">
        /// If no matching operation is possible, attempt to convert
        /// TArg1 and TArg2 to TResult for a match? For example, there is no
        /// "decimal operator /(decimal, int)", but by converting TArg2 (int) to
        /// TResult (decimal) a match is found.
        /// </param>
        /// <returns>Compiled function delegate</returns>
        public static Delegate CreateExpression(Func<Expression, Expression, BinaryExpression> body,
            Type leftType, Type rightType, Type resultType, bool castArgsToResultOnFailure)
        {
            var lhs = Expression.Parameter(leftType, "lhs");
            var rhs = Expression.Parameter(rightType, "rhs");

            Expression lhsExpression = lhs;
            Expression rhsExpression = rhs;
            UpdateVisit(ref lhsExpression, ref rhsExpression);
            try
            {
                try
                {

                    return Expression.Lambda(body(lhsExpression, rhsExpression), lhs, rhs).Compile();
                }
                catch (InvalidOperationException)
                {
                    if (castArgsToResultOnFailure && !( // if we show retry                                                        
                        leftType == resultType && // and the args aren't
                            rightType == resultType))
                    {
                        // already "TValue, TValue, TValue"...
                        // convert both lhs and rhs to TResult (as appropriate)
                        var castLhs = leftType == resultType ? lhs : (Expression)Expression.Convert(lhs, resultType);
                        var castRhs = rightType == resultType ? rhs : (Expression)Expression.Convert(rhs, resultType);

                        return Expression.Lambda(body(castLhs, castRhs), lhs, rhs).Compile();
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message; // avoid capture of ex itself
                return (Action)(delegate
                { throw new InvalidOperationException(msg); });
            }
        }

        private static void UpdateVisit(ref Expression left, ref Expression right)
        {
            var leftTypeCode = Type.GetTypeCode(left.Type);
            var rightTypeCode = Type.GetTypeCode(right.Type);

            if (leftTypeCode == rightTypeCode)
            {
                return;
            }

            if (leftTypeCode > rightTypeCode && leftTypeCode != TypeCode.String)
            {
                right = Expression.Convert(right, left.Type);
            }
            else
            {
                left = Expression.Convert(left, right.Type);
            }
        }
    }

    internal static class ObjectExtensionMethods
    {
        public static bool RespondTo(this object value, string member, bool ensureNoParameters = true)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var type = value.GetType();

            var methodInfo = type.GetMethod(member);
            if (methodInfo != null && (!ensureNoParameters || !methodInfo.GetParameters().Any()))
            {
                return true;
            }

            var propertyInfo = type.GetProperty(member);
            return propertyInfo != null && propertyInfo.CanRead;
        }

        public static object Send(this object value, string member, object[] parameters = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var type = value.GetType();

            var methodInfo = type.GetMethod(member);
            if (methodInfo != null)
            {
                return methodInfo.Invoke(value, parameters);
            }

            var propertyInfo = type.GetProperty(member);
            return propertyInfo != null ? propertyInfo.GetValue(value, null) : null;
        }
    }

    internal static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrEmpty(s) || s.Trim().Length == 0;
        }

        public static string ToSafeString(this object s)
        {
            return s == null ? "" : s.ToString();
        }

        public static object ToNumber(this object s)
        {
            if (s is double)
            {
                return s;
            }
            else if (s is float)
            {
                return s;
            }
            else if (s is decimal)
            {
                return s;
            }
            else if (s is int)
            {
                return s;
            }
            else if (s is Money money)
            {
                return money.Amount;
            }
            else if (s is string)
            {
                var match = Regex.Match(s as string, string.Format("(?-mix:{0})", @"^([+-]?\d[\d\.|\,]+)$"));
                if (match.Success)
                {
                    // For cultures with "," as the decimal separator, allow
                    // both "," and "." to be used as the separator.
                    // First try to parse using current culture.
                    float result;
                    if (float.TryParse(match.Groups[1].Value, out result))
                        return result;

                    // If that fails, try to parse using invariant culture.
                    return float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(s as string, string.Format("(?-mix:{0})", @"^([+-]?\d+).*$"));
                if (match.Success)
                {
                    return Convert.ToInt32(match.Groups[1].Value);
                }
            }

            return 0;
        }

        private static double Evaluate(string expression)
        {
            var xsltExpression =
                string.Format("number({0})",
                    new Regex(@"([\+\-\*])").Replace(expression, " ${1} ")
                                            .Replace("/", " div ")
                                            .Replace("%", " mod "));

            return (double)new XPathDocument
                (new StringReader("<r/>"))
                    .CreateNavigator()
                    .Evaluate(xsltExpression);
        }
    }

    internal static class EnumerableExtensionMethods
    {
        public static IEnumerable Flatten(this IEnumerable array)
        {
            foreach (var item in array)
            {
                if (item is string)
                {
                    yield return item;
                }
                else if (item is IEnumerable enumerableItem)
                {
                    foreach (var subItem in Flatten(enumerableItem))
                    {
                        yield return subItem;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        public static void EachWithIndex(this IEnumerable<object> array, Action<object, int> callback)
        {
            var index = 0;

            foreach (var item in array)
            {
                callback(item, index);
                ++index;
            }
        }
    }
}
