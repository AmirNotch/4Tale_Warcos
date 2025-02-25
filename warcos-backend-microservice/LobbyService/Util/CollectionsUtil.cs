namespace Lobby.Util;

public class CollectionsUtil {
    private CollectionsUtil() { }

    public static string FormatList<TValue>(IEnumerable<TValue> list) where TValue : notnull {
        return "[" + string.Join("; ", list) + "]";
    }

    public static string FormatDict<TKey, TValue>(Dictionary<TKey, TValue> dict, string keyName, string valueName) where TKey : notnull {
        return "{" + string.Join(", ", dict.Select(i => $"{keyName} {i.Key}: {valueName} {i.Value}").ToList()) + "}";
    }
}
