namespace PTTDigital.Email.Common.EncryptDecrypt.Helper;

public static class XreadFile
{
    public static string takexfile(string xMLKeyInfo, string v)
    {
        int found = xMLKeyInfo.IndexOf("<" + v + ">");
        int found2 = xMLKeyInfo.IndexOf("</" + v + ">");
        int total = found2 - found;
        string Result = xMLKeyInfo.Substring(found + v.Length + 2, total - v.Length - 2);
        return Result;
    }
}