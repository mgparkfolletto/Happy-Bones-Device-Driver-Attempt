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
        
        var resultTask = SerialPortUtils.WriteAndGetResponseAsync(getStatusCommand);
        var result = resultTask.Result;

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
        var waterDurationData =
            NakazoDataUtils.ConvertTimeData(waterEmitDuration.ToString(CultureInfo.InvariantCulture));

        var doProductCommand = NakazoCommandConstructor.GetDoProductMessage(iceDurationData, waterDurationData);
        Logger.Debug($"Do product command: {DataUtils.ByteArrayToReadableString(doProductCommand)}");
        
        var resultTask = SerialPortUtils.WriteAndGetResponseAsync(doProductCommand);
        var result = resultTask.Result;

        NakazoResponseDataModel responseDataModel = new(result);
        Logger.Debug($"Data received: {DataUtils.ByteArrayToReadableString(result)}");
        Logger.Debug($"responseDataModel: {responseDataModel}");
        return responseDataModel.Command == NakazoProtocolDataModel.CommandDoProduct &&
               responseDataModel.Data1 == iceDurationData &&
               responseDataModel.Data2 == waterDurationData;
    }

    public async Task<NakazoTemperatureDataModel> GetTemperatureData()
    {
        var getExteriorTemperatureCommand = NakazoCommandConstructor.GetExteriorTemperatureMessage();
        var getEvaporatorAndCondenserTemperatureCommand =
            NakazoCommandConstructor.GetEvaporatorAndCondenserMessage();
        Logger.Debug(
            $"Get exterior temperature command: {DataUtils.ByteArrayToReadableString(getExteriorTemperatureCommand)}");
        Logger.Debug(
            $"Get evaporator and condenser temperature command: {DataUtils.ByteArrayToReadableString(getEvaporatorAndCondenserTemperatureCommand)}");
        
        var result = await SerialPortUtils.WriteAndGetResponseAsync(getExteriorTemperatureCommand);
        
        NakazoResponseDataModel responseDataModel = new(result);
        Logger.Debug($"Data received: {DataUtils.ByteArrayToReadableString(result)}");
        Logger.Debug($"responseDataModel: {responseDataModel}");
        var exteriorTemperature = NakazoDataUtils.ConvertTemperatureData(responseDataModel.Data1);
        
        await Task.Delay(200);
        
        result = await SerialPortUtils.WriteAndGetResponseAsync(getEvaporatorAndCondenserTemperatureCommand);
        responseDataModel = new NakazoResponseDataModel(result);
        Logger.Debug($"Data received: {DataUtils.ByteArrayToReadableString(result)}");
        Logger.Debug($"responseDataModel: {responseDataModel}");
        var evaporatorTemperature = NakazoDataUtils.ConvertTemperatureData(responseDataModel.Data1);
        var condenserTemperature = NakazoDataUtils.ConvertTemperatureData(responseDataModel.Data2);
        
        return new NakazoTemperatureDataModel(exteriorTemperature, evaporatorTemperature, condenserTemperature);
    }
}