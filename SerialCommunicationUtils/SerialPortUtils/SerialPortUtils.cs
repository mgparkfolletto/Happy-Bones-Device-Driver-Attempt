using System.IO.Ports;
using Serilog;

namespace SerialCommunicationUtils.SerialPortUtils;

public class SerialPortUtils
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.File("logs/NakazoIceMachineLog.txt", rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
        .MinimumLevel.Debug()
        .CreateLogger();
    
    public SerialPort SerialPort { get; }
    public bool IsOpen => SerialPort.IsOpen;
    
    public SerialPortUtils(SerialPort serialPort)
    {
        SerialPort = serialPort;
    }
    
    public bool Open()
    {
        if (SerialPort.IsOpen)
        {
            return true;
        }
        try
        {
            SerialPort.Open();
            return true;
        }
        catch (IOException ex)
        {
            Logger.Debug($"IOException: {ex}");
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.Debug($"UnauthorizedAccessException {ex}");
            return false;
        }
        catch (Exception ex)
        {
            Logger.Debug($"Exception {ex}");
            return false;
        }
    }
    
    public bool Close()
    {
        if (!SerialPort.IsOpen) return false;
        SerialPort.Close();
        return true;
    }
    
    public bool Write(byte[] data)
    {
        if (!SerialPort.IsOpen) return false;
        SerialPort.Write(data, 0, data.Length);
        return true;
    }
}