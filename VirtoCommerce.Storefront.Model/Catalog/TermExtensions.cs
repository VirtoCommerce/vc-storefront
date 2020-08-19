using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public static class TermExtensions
    {

        public static IEnumerable<Term> ToTerms(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var result = input.Split(';')
                      .Select(x => x.Split(':'))
                      .Where(x => x.Length == 2)
                      .SelectMany(x => x[1].Split(',').Select(v => new Term { Name = x[0], Value = v }));
            return result;
        }
        /// <summary>
        /// Groups terms by name and converts each group to a string:
        /// name1:value1,value2,value3
        /// </summary>
        /// <param name="terms"></param>
        /// <returns></returns>
        public static List<string> ToStrings(this IEnumerable<Term> terms, bool encode = false)
        {
            List<string> result = null;

            const string commaEscapeString = "%x2C";

            if (terms != null)
            {
                var rangePattern = new Regex(@"^(\[|\(){1}.*(\]|\)){1}$");

                var strings = terms
                    .GroupBy(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(g => g.Key)
                    .Select(
                        g =>
                        // VP-3940: Need to encode spaces to allow search engine treat them differently - as delimiters and as pure spaces in name/values
                            string.Join(":", encode && g.Key.Contains(" ") ? $@"\""{g.Key}\""" : g.Key,
                                string.Join(",",
                                        g.Select(t =>
                                            {
                                                string termValue;
                                                if (encode)
                                                {
                                                    termValue = t.Value.Replace(",", commaEscapeString);

                                                    if (termValue.Contains(" ") && !rangePattern.IsMatch(termValue))
                                                    {
                                                        termValue = $@"\""{termValue}\""";
                                                    }
                                                }
                                                else
                                                {
                                                    termValue = t.Value;
                                                }

                                                return termValue;
                                            })
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .OrderBy(v => v)
                                    )
                                )
                        )

                    .ToList();

                if (strings.Any())
                {
                    result = strings;
                }
            }

            return result;
        }

        public static void ConvertTerm(this Term term, string currencyCode)
        {
            var values = term.Value.Split('-');

            var templateArgument = values.FirstOrDefault();
            var isLowerLimitInteger = int.TryParse(templateArgument, out _);
            int.TryParse(values.Skip(1).FirstOrDefault(), out var upperLimit);

            var format = GetPriceFilterValuePattern(isLowerLimitInteger, templateArgument);

            term.Value = string.Format(format, upperLimit);
            term.Name = $"{term.Name}_{currencyCode.ToLowerInvariant()}";
        }

        private static string GetPriceFilterValuePattern(bool isInt, string limit)
        {
            string result = null;

            if (isInt)
            {
                result = $@"[{limit} TO {{0}})";
            }
            else if (limit.EqualsInvariant("under"))
            {
                result = "[ TO {0})";
            }
            else if (limit.EqualsInvariant("over"))
            {
                result = "[{0} TO )";
            }

            return result;
        }
    }
}
