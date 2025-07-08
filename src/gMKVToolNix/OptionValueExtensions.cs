using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gMKVToolNix
{
    public static class OptionValueExtensions
    {
        public static string ConvertOptionValueListToString<T>(this List<OptionValue<T>> listOptionValue)
        {
            StringBuilder optionString = new StringBuilder();
            foreach (OptionValue<T> optVal in listOptionValue)
            {
                optionString.Append(' ');
                optionString.Append(ConvertEnumOptionToStringOption(optVal.Option));
                if (!string.IsNullOrWhiteSpace(optVal.Parameter))
                {
                    optionString.Append(' ');
                    optionString.Append(optVal.Parameter);
                }
            }

            return optionString.ToString();
        }

        private static readonly Dictionary<Type, Dictionary<object, string>> _OptionsToStringMap = 
            new Dictionary<Type, Dictionary<object, string>>();

        private static void PrepareOptionsToStringMap<T>()
        {
            if (_OptionsToStringMap.ContainsKey(typeof(T)))
            {
                return; // Already prepared
            }

            var enumType = typeof(T);
            var optionsToStringMap = Enum.GetValues(enumType)
                .Cast<T>()
                .ToDictionary(
                    val => (object)val,
                    val => $"--{val.ToString().Replace("_", "-")}"
                );

            _OptionsToStringMap[enumType] = optionsToStringMap;
        }

        private static string ConvertEnumOptionToStringOption<T>(T argEnumOption)
        {
            PrepareOptionsToStringMap<T>();

            return _OptionsToStringMap[typeof(T)][argEnumOption];            
        }
    }
}
