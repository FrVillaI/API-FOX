namespace Apifox.App.Utils;

public static class PhoneNormalizer
{
    public static string Normalize(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (phone.StartsWith("0") && phone.Length > 1)
            phone = "593" + phone.Substring(1);

        if (!phone.StartsWith("593"))
            phone = "593" + phone;

        return phone;
    }

    public static bool IsValid(string? phone)
    {
        var normalized = Normalize(phone);
        if (string.IsNullOrEmpty(normalized))
            return false;

        if (normalized.Length < 12)
            return false;

        if (!normalized.StartsWith("593"))
            return false;

        return true;
    }
}