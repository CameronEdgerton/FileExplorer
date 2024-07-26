namespace FileFolderExplorer.Utils;

public class GuidHelper
{
    public static Guid? TryParse(string value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        return Guid.TryParse(value, out var parsedValue) ? parsedValue : Guid.Empty;
    }
}