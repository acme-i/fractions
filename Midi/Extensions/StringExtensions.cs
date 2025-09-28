
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string ToProperCase(this string original)
    {
        if (string.IsNullOrEmpty(original))
            return original;

        var result = _properNameRx.Replace(original.ToLower(CultureInfo.CurrentCulture), HandleWord);
        return result;
    }

    public static string WordToProperCase(this string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        if (word.Length > 1)
            return Char.ToUpper(word[0], CultureInfo.CurrentCulture) + word.Substring(1);

        return word.ToUpper(CultureInfo.CurrentCulture);
    }

    private static readonly Regex _properNameRx = new Regex(@"\b(\w+)\b");

    private static readonly string[] _prefixes = { "mc" };

    private static string HandleWord(Match m)
    {
        var word = m.Groups[1].Value;

        foreach (var prefix in _prefixes)
        {
            if (word.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                return prefix.WordToProperCase() + word.Substring(prefix.Length).WordToProperCase();
        }

        return word.WordToProperCase();
    }

    public static string ReplaceAll(this string s, char[] find, string replace)
    {
        if (string.IsNullOrEmpty(s) || find.Any() == false)
            return s;

        if (-1 != s.IndexOfAny(find))
            foreach (var c in find)
                s = s.Replace(new string(c, 1), replace);
        return s;
    }

    public static string ReplaceAll(this string s, string find, string replace, RegexOptions options = RegexOptions.IgnoreCase)
    {
        if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(find))
            return s;

        return Regex.Replace(s, Regex.Escape(find), replace, options | RegexOptions.Multiline);
    }

    public static string ReplaceAll(this string s, Dictionary<string, string> replace)
    {
        if (string.IsNullOrEmpty(s) || replace is null || replace.Count != 0 == false)
            return s;

        var t = new StringBuilder(s);
        foreach (var kv in replace)
            t = t.Replace(kv.Key, kv.Value);

        return t.ToString();
    }

    public static string TrimmedOrNull(this string str)
    {
        return str?.Trim();
    }
}
