using JetBrains.Annotations;

namespace Plunger.Core;

[PublicAPI]
public static class StringExtensions
{
    public static string SerializeToJson(this string source) => JsonSerializer.Serialize(source);
}
