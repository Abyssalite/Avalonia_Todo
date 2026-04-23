using System;

public static class GlobalVariables
{
    public const string Important = "Important";
    public const string Quick = "Quick";
    public static readonly bool IsAndroid = OperatingSystem.IsAndroid();
    public static readonly bool IsBrowser = OperatingSystem.IsBrowser();
}