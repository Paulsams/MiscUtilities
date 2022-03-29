using System;
using System.Collections.Generic;

namespace Paulsams.MicsUtil
{
    public static class MyCompare
    {
        public static class Numbers
        {
            public enum CompareNumberType
            {
                Less,
                LessOrEqual,
                Equal,
                Greater,
                GreaterOrEqual,
            };

            private static Dictionary<CompareNumberType, Func<float, float, bool>> _compareTypes = new Dictionary<CompareNumberType, Func<float, float, bool>>()
        {
            { CompareNumberType.Less, (first, second) => first < second},
            { CompareNumberType.LessOrEqual, (first, second) => first <= second},
            { CompareNumberType.Equal, (first, second) => first == second},
            { CompareNumberType.Greater, (first, second) => first > second},
            { CompareNumberType.GreaterOrEqual, (first, second) => first >= second},
        };

            public static bool Compare(float first, float second, CompareNumberType compareType) => _compareTypes[compareType].Invoke(first, second);
        }

        public static class Strings
        {
            public enum CompareStringType
            {
                Equal,
            };

            private static Dictionary<CompareStringType, Func<string, string, bool>> _compareTypes = new Dictionary<CompareStringType, Func<string, string, bool>>()
        {
            { CompareStringType.Equal, (first, second) => first == second},
        };

            public static bool Compare(string first, string second, CompareStringType compareType) => _compareTypes[compareType].Invoke(first, second);
        }
    }

    public enum TypeCompare
    {
        None,
        Number,
        String,
    }
}