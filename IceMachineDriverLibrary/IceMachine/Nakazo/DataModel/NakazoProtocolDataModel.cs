namespace IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;

public static class NakazoProtocolDataModel
{
    public const byte Stx = 0x02;
    public const byte Etx = 0x03;
    public const byte StatusMaster = 0x01;
    public const byte StatusControlPcb = 0x00;
    public const byte CommandDoProduct = 0xB0;
    public const byte CommandReadExteriorTemperature = 0xC2;
    public const byte CommandReadEvaporatorAndCondenserTemperature = 0xC3;
    public const byte CommandGetMachineStatus = 0xCF;
    
    public static byte GetCrc(IEnumerable<byte> data)
    {
        return data.Aggregate<byte, byte>(0x00, (current, b) => (byte)(current ^ b));
    }
}