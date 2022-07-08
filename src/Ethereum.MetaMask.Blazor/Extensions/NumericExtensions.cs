using System.Globalization;
using System.Numerics;

namespace Ethereum.MetaMask.Blazor;

public static class NumericExtensions
{
    /// <summary>
    /// Converts integer to hexadecimal string where starts with '0x'.
    /// </summary>
    /// <param name="value">Integer value.</param>
    /// <returns>Hexadecimal string.</returns>
    public static string ToHexString(this int value)
    {
        return $"0x{value:x}";
    }

    /// <summary>
    /// Converts big integer to hexadecimal string where starts with '0x'.
    /// </summary>
    /// <param name="value">BigInteger value.</param>
    /// <returns>Hexadecimal string.</returns>
    public static string ToHexString(this BigInteger value)
    {
        return $"0x{value:x}";
    }

    /// <summary>
    /// Converts hexadecimal string to integer.
    /// </summary>
    /// <param name="hexString">hexadecimal string.</param>
    /// <returns>Integer value.</returns>
    public static int HexToInt(this string hexString)
    {
        if (hexString.StartsWith("0x"))
            hexString = hexString[2..];

        return int.Parse(hexString, NumberStyles.HexNumber);
    }

    /// <summary>
    /// Converts hexadecimal string to big integer.
    /// </summary>
    /// <param name="hexString">hexadecimal string.</param>
    /// <returns>BigInteger value.</returns>
    public static BigInteger HexToBigInteger(this string hexString)
    {
        hexString = hexString.Replace("0x", "0");
        return BigInteger.Parse(hexString, NumberStyles.HexNumber); ;
    }
}