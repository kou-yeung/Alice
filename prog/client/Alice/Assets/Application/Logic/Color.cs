using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice.Logic
{
    public static class ColorGen
    {
        public static Color Rare(int rare)
        {
            var aColor = Color.white;

            var colors = new[] { Color.yellow, Color.red, Color.blue };
            var bColor = colors[rare%colors.Length];

            var t = (Mathf.Max(rare / colors.Length, 0) + 1) / 20f;
            var r = Mathf.Lerp(aColor.r, bColor.r, t);
            var g = Mathf.Lerp(aColor.g, bColor.g, t);
            var b = Mathf.Lerp(aColor.b, bColor.b, t);
            return new Color(r, g, b, 1.0f);
        }
    }
}