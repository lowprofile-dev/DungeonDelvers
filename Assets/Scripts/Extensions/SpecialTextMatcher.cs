using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

public class SpecialTextMatcher
{
    #region Singleton Implementation
    private static SpecialTextMatcher _instance;
    public static SpecialTextMatcher Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SpecialTextMatcher();
            return _instance;
        }
    }
    #endregion

    private Dictionary<string, Func<string>> SpecialKeywords;
    
    private SpecialTextMatcher()
    {
        SpecialKeywords = new Dictionary<string, Func<string>>();
        SpecialKeywords.Add("CGOLD", () => PlayerController.Instance.CurrentGold.ToString());
        SpecialKeywords.Add("PLVL", () => PlayerController.Instance.PartyLevel.ToString());
        //SpecialKeywords.Add();
    }

    private string match(Interactable context, string text)
    {
        Regex rx = new Regex(@"\{(.*?)\}");
        
        var match = rx.Match(text);

        while (match != System.Text.RegularExpressions.Match.Empty)
        {
            var keyword = match.Value.Substring(1, match.Value.Length - 2);
            var stringValue = match.Value.Substring(1, match.Value.Length - 2);

            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                var isSpecialKeyword = SpecialKeywords.TryGetValue(keyword, out var keywordFunction);
                if (isSpecialKeyword)
                {
                    var keywordValue = keywordFunction();
                    replace(match,keywordValue,ref text);
                }
                else if (stringValue[0] == 'G')
                {
                    var globalValue = GameController.Instance.Globals[stringValue.Substring(2)].ToString();
                    replace(match,globalValue,ref text);
                } else if (stringValue[0] == 'L')
                {
                    var localValue = context.GetLocal(stringValue.Substring(2)).ToString();
                    replace(match,localValue,ref text);
                } else if (stringValue[0] == 'I')
                {
                    var instanceValue = context.GetInstance(stringValue.Substring(2), "").ToString();
                    replace(match,instanceValue,ref text);
                }
            }
            match = rx.Match(text);
        }

        return text;
    }

    private void replace(Match match, string newValue, ref string text) => text = text.Substring(0, match.Index) + newValue + text.Substring(match.Index + match.Length);

    public static string Match([NotNull] Interactable context, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;
        return Instance.match(context, text);
    }
}
