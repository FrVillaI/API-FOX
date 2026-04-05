using System;
using System.Data;

namespace Apifox.App.Core.Utils;

public static class DbfFieldHelper
{
    public static string SafeGetString(IDataRecord row, string field)
    {
        try { return row[field]?.ToString()?.Trim() ?? ""; }
        catch { return ""; }
    }

    public static decimal SafeGetDecimal(IDataRecord row, string field)
    {
        try
        {
            var val = row[field];
            if (val == null || val == DBNull.Value) return 0;
            return Convert.ToDecimal(val);
        }
        catch { return 0; }
    }

    public static DateTime? SafeGetDate(IDataRecord row, string field)
    {
        try
        {
            var val = row[field];
            if (val == null || val == DBNull.Value) return null;
            if (val is DateTime dt) return dt;
            return DateTime.Parse(val.ToString()!);
        }
        catch { return null; }
    }

    public static bool SafeGetBool(IDataRecord row, string field)
    {
        try
        {
            var val = row[field];
            if (val == null || val == DBNull.Value) return false;
            if (val is bool b) return b;
            var s = val.ToString()?.ToUpper();
            return s == "T" || s == "Y" || s == "1";
        }
        catch { return false; }
    }
}
