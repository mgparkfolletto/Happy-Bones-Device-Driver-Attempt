using System.Globalization;
using System.IO.Ports;
using GeneralUtils;
using IceMachineDriverLibrary.IceMachine.Common;
using IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;
using SerialCommunicationUtils.SerialPortUtils;
using Serilog;
using static IceMachineDriverLibrary.IceMachine.Nakazo.DataModel.NakazoSerialSettingDataModel;

namespace IceMachineDriverLibrary.IceMachine.Nakazo.Service;

public class NakazoIceMachine : IIceMachine
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.File("logs/NakazoIceMachineLog.txt", rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
        .MinimumLevel.Debug()
        .CreateLogger();
    
    private SerialPortUtils SerialPortUtils { get; }

    public NakazoIceMachine()
    {
        var serialPortNakazo = new SerialPort(PortName,
            BaudRate, NakazoSerialSettingDataModel.Parity,
            DataBits, NakazoSerialSettingDataModel.StopBits);
        SerialPortUtils = new SerialPortUtils(serialPortNakazo);
    }

    public bool Connect()
    {
        return SerialPortUtils.Open();
    }

    public IIceMachineStatus GetStatus()
    {
        var getStatusCommand = NakazoCommandConstructor.GetMachineStatusMessage();
        Logger.Debug($"Get status command: {DataUtils.ByteArrayToReadableString(getStatusCommand)}");
        var sendOk = SerialPortUtils.Write(getStatusCommand);
        Logger.Debug($"Send ok: {sendOk}");
        Thread.Sleep(200);
        if (SerialPortUtils.SerialPort.BytesToRead <= 0) return new InvalidStatusResponseDataModel();
        
        var result = new byte[SerialPortUtils.SerialPort.BytesToRead];
        Logger.Debug($"Result length: {result.Length}");
        while (SerialPortUtils.SerialPort.BytesToRead > 0)
        {
            SerialPortUtils.SerialPort.Read(result, 0, result.Length);
        }
        NakazoResponseDataModel responseDataModel = new(result);
        Logger.Debug($"Data received: {DataUtils.ByteArrayToReadableString(result)}");
        Logger.Debug($"responseDataModel: {responseDataModel}");
        NakazoMachineStatusDataModel statusDataModel = new(responseDataModel.Data1, responseDataModel.Data2);
        return statusDataModel;
    }

    public bool IsConnected()
    {
        return SerialPortUtils.IsOpen;
    }

    public bool Close()
    {
        Logger.Debug("Close");
        var closeOk = SerialPortUtils.Close();
        Logger.Debug($"Close ok: {closeOk}, IsOpen: {SerialPortUtils.IsOpen}");
        return closeOk;
    }

    public bool DoProduct(IIceMachineDoProductData data)
    {
        var doProductData = (NakazoDoProductDataModel)data;
        var iceEmitDuration = doProductData.IcePumpingDuration;
        var waterEmitDuration = doProductData.WaterPumpingDuration;

        var iceDurationData = NakazoDataUtils.ConvertTimeData(iceEmitDuration.ToString(CultureInfo.InvariantCulture));
        var waterDurationData = NakazoDataUtils.ConvertTimeData(waterEmitDuration.ToString(CultureInfo.InvariantCulture));
        
        var doProductCommand = NakazoCommandConstructor.GetDoProductMessage(iceDurationData, waterDurationData);
        Logger.Debug($"Do product command: {DataUtils.ByteArrayToReadableString(doProductCommand)}");
        var sendOk = SerialPortUtils.Write(doProductCommand);
        Logger.Debug($"Send ok: {sendOk}");
        Thread.Sleep(200);
        if (SerialPortUtils.SerialPort.BytesToRead <= 0) return false;
        
        var result = new byte[SerialPortUtils.SerialPort.BytesToRead];
        Logger.Debug($"Result length: {result.Length}");
        while (SerialPortUtils.SerialPort.BytesToRead > 0)
        {
            SerialPortUtils.SerialPort.Read(result, 0, result.Length);
        }
        NakazoResponseDataModel responseDataModel = new(result);
        Logger.Debug($"Data received: {DataUtils.ByteArrayToReadableString(result)}");
        Logger.Debug($"responseDataModel: {responseDataModel}");
        return responseDataModel.Command == NakazoProtocolDataModel.CommandDoProduct &&
               responseDataModel.Data1 == iceDurationData &&
                responseDataModel.Data2 == waterDurationData;
    }
}