using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Common
{
    /// <summary>
    /// Represent currency information in storefront. Contains some extra informations as exchnage rate, symbol, formating. 
    /// </summary>
    public class Currency : ValueObject
    {
        private static readonly IDictionary<string, string> _isoCurrencySymbolDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
        static Currency()
        {
            foreach (var ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    var ri = new RegionInfo(ci.LCID);
                    _isoCurrencySymbolDict[ri.ISOCurrencySymbol] = ri.CurrencySymbol;
                }
                catch (Exception)
                {
                    // No actions needed
                }
            }
        }

        private Language _language;
        private string _code;

        protected Currency()
        {
        }

        public Currency(Language language, string code, string name, string symbol, decimal exchangeRate)
            : this(language, code)
        {
            ExchangeRate = exchangeRate;

            if (!string.IsNullOrEmpty(name))
            {
                EnglishName = name;
            }
            if (!string.IsNullOrEmpty(symbol))
            {
                Symbol = symbol;
                NumberFormat.CurrencySymbol = symbol;
            }
        }

        public Currency(Language language, string code)
        {
            _language = language;
            _code = code;
            ExchangeRate = 1;
            Initialize();
        }

        /// <summary>
        /// Currency code may be used ISO 4217
        /// </summary>
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                Initialize();
            }
        }

        public string CultureName
        {
            get
            {
                return _language?.CultureName;
            }
            set
            {
                _language = new Language(value);
                Initialize();
            }
        }

        [JsonIgnore]
        public NumberFormatInfo NumberFormat { get; private set; }
        public string Symbol { get; set; }
        public string EnglishName { get; set; }
        /// <summary>
        /// Exchnage rate with primary currency
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/dwhawy9k%28v=vs.110%29.aspx?f=255&amp;MSPPError=-2147217396
        /// </summary>
        public string CustomFormatting { get; set; }


        private void Initialize()
        {
            if (_language != null)
            {
                if (!_language.IsInvariant)
                {
                    var cultureInfo = CultureInfo.GetCultureInfo(_language.CultureName);
                    NumberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
                    EnglishName = cultureInfo.NumberFormat.CurrencySymbol;

                    if (!cultureInfo.IsNeutralCulture)
                    {
                        var region = new RegionInfo(_language.CultureName);
                        EnglishName = region.CurrencyEnglishName;
                    }
                    if (_code != null)
                    {
                        Symbol = _isoCurrencySymbolDict[_code] ?? "N/A";
                        NumberFormat.CurrencySymbol = Symbol;
                    }
                }
                else
                {
                    NumberFormat = CultureInfo.InvariantCulture.NumberFormat.Clone() as NumberFormatInfo;
                }
            }
        }



        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            if (!result && obj is string code)
            {
                result = code.EqualsInvariant(Code);
            }
            return result;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
            yield return CultureName;
        }

    }
}
