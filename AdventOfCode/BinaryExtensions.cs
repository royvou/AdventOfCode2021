using System.Collections;

namespace AdventOfCode;

public static class BinaryExtensions
{
    public static BitArray HexStringToBitArray(this string hexString)
    {
        return new BitArray(hexString.SelectMany(c => ParseHexChar(c).Select(hc => hc == '1')).ToArray());
    }

    public static string ParseHexChar(this char c)
    {
        if (c == '0') return "0000";
        if (c == '1') return "0001";
        if (c == '2') return "0010";
        if (c == '3') return "0011";
        if (c == '4') return "0100";
        if (c == '5') return "0101";
        if (c == '6') return "0110";
        if (c == '7') return "0111";
        if (c == '8') return "1000";
        if (c == '9') return "1001";
        if (c == 'A') return "1010";
        if (c == 'B') return "1011";
        if (c == 'C') return "1100";
        if (c == 'D') return "1101";
        if (c == 'E') return "1110";
        if (c == 'F') return "1111";
        throw new Exception("wtf");
    }
}