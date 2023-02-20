using System.IO.Ports;

namespace IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;

public static class NakazoSerialSettingDataModel
{
    public static string PortName { get; set; } = "COM1";
    public static int BaudRate { get; set; } = 9600;
    public static int DataBits { get; set; } = 8;
    public static Parity Parity { get; set; } = Parity.None;
    public static StopBits StopBits { get; set; } = StopBits.One;
}