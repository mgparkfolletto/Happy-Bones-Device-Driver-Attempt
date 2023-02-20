namespace GeneralUtils;

public static class DataUtils
{
    public static string ByteToBinaryString(byte b)
    {
        return Convert.ToString(b, 2).PadLeft(8, '0');
    }
    
    public static string ByteArrayToBinaryString(IEnumerable<byte> bytes)
    {
        return string.Join("", bytes.Select(ByteToBinaryString));
    }
    
    public static byte BinaryStringToByte(string binaryString)
    {
        return Convert.ToByte(binaryString, 2);
    }
    
    public static byte[] BinaryStringToByteArray(string binaryString)
    {
        return Enumerable.Range(0, binaryString.Length / 8)
            .Select(i => binaryString.Substring(i * 8, 8))
            .Select(BinaryStringToByte)
            .ToArray();
    }
    
    public static string ByteArrayToReadableString(IEnumerable<byte> bytes)
    {
        return string.Join(" ", bytes.Select(b => b.ToString("X2")));
    }
}