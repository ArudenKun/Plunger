using JetBrains.Annotations;

namespace Plunger.Core;

[PublicAPI]
public static class JsonHelper
{
    public static string ToJson(this object obj) => JsonSerializer.Serialize(obj);
}
