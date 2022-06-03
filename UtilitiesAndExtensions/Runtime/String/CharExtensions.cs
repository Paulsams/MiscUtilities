using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class CharExtensions
    {
        public static bool IsUpper(this char c) => c >= 65 && c <= 90;
    }
}
