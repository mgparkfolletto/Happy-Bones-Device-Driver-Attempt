using IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;
using static IceMachineDriverLibrary.IceMachine.Nakazo.DataModel.NakazoProtocolDataModel;

namespace IceMachineDriverLibrary.IceMachine.Nakazo.Service;

public static class NakazoCommandConstructor
{
    public static byte[] GetMachineStatusMessage()
    {
        byte[] header = { Stx };
        byte[] footer = { Etx };
        byte[] control = { StatusMaster };
        byte[] command = { CommandGetMachineStatus };
        byte[] data = { 0x00, 0x00 };
        var crcData =
            control
                .Concat(command)
                .Concat(data)
                .ToArray();
        byte[] crc = { GetCrc(crcData) };

        return header
            .Concat(control)
            .Concat(command)
            .Concat(data)
            .Concat(crc)
            .Concat(footer)
            .ToArray();
    }

    public static byte[] GetDoProductMessage(byte iceDuration, byte waterDuration)
    {
        byte[] header = { Stx };
        byte[] footer = { Etx };
        byte[] control = { StatusMaster };
        byte[] command = { CommandDoProduct };
        byte[] data = { iceDuration, waterDuration };
        var crcData =
            control
                .Concat(command)
                .Concat(data)
                .ToArray();
        byte[] crc = { GetCrc(crcData) };
        
        return header
            .Concat(control)
            .Concat(command)
            .Concat(data)
            .Concat(crc)
            .Concat(footer)
            .ToArray();
    }

    public static byte[] GetExteriorTemperatureMessage()
    {
        byte[] header = { Stx };
        byte[] footer = { Etx };
        byte[] control = { StatusMaster };
        byte[] command = { CommandReadExteriorTemperature };
        byte[] data = { 0x00, 0x00 };
        var crcData =
            control
                .Concat(command)
                .Concat(data)
                .ToArray();
        byte[] crc = { GetCrc(crcData) };
        
        return header
            .Concat(control)
            .Concat(command)
            .Concat(data)
            .Concat(crc)
            .Concat(footer)
            .ToArray();
    }

    public static byte[] GetEvaporatorAndCondenserMessage()
    {
        byte[] header = { Stx };
        byte[] footer = { Etx };
        byte[] control = { StatusMaster };
        byte[] command = { CommandReadEvaporatorAndCondenserTemperature };
        byte[] data = { 0x00, 0x00 };
        var crcData =
            control
                .Concat(command)
                .Concat(data)
                .ToArray();
        byte[] crc = { GetCrc(crcData) };
        
        return header
            .Concat(control)
            .Concat(command)
            .Concat(data)
            .Concat(crc)
            .Concat(footer)
            .ToArray();
    }
}