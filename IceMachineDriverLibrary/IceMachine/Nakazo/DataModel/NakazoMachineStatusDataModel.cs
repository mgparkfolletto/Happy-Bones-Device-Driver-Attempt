using GeneralUtils;
using IceMachineDriverLibrary.IceMachine.Common;
using Serilog;

namespace IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;

public class NakazoMachineStatusDataModel : IIceMachineStatus
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.File("logs/NakazoStatusDataModel.txt", rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
        .MinimumLevel.Debug()
        .CreateLogger();

    public DeviceStatusE DeviceStatus { get; set; } = DeviceStatusE.Ng;
    private byte StatusData { get; }
    private byte ErrorData { get; }

    #region StatusData

    public int NoIceDetectedInPreviousCycle { get; set; } // ignore
    public int IceDetectedInPreviousCycle { get; set; } // ignore
    public int CompressorRunning { get; set; }
    public int GearMotorRunning { get; set; }
    public int IceOutSolDetected { get; set; }
    public int CupLeverDetected { get; set; }
    public int IsInCommunicationMode { get; set; }
    public int IceIsFull { get; set; }

    #endregion

    #region ErrorData

    public int WaitingForIceOut { get; set; }

    public int WaterSupplyFaulty { get; set; }
    public int WaterDrainageFaulty { get; set; }
    public int ExteriorTemperatureLow { get; set; }
    public int ExteriorTemperatureHigh { get; set; }
    public int CondenserTemperatureHigh { get; set; }
    public int EvaporatorTemperatureLow { get; set; }
    public int GmFaulty { get; set; }
    public int IceFunctionFaulty { get; set; }
    public int InspectionPeriod { get; set; }
    public int DcCommunicationError { get; set; }
    public int FanMotorFaulty { get; set; }

    #endregion

    public NakazoMachineStatusDataModel()
    {
    }
    
    public NakazoMachineStatusDataModel(byte statusData, byte errorData)
    {
        StatusData = statusData;
        ErrorData = errorData;
        Logger.Debug($"status: {statusData:X2}, error: {errorData:X2}");
        HandleStatusData();
    }

    private void HandleStatusData()
    {
        var statusBinary = DataUtils.ByteToBinaryString(StatusData);
        var errorBinary = DataUtils.ByteToBinaryString(ErrorData);
        Logger.Debug($"StatusData: {statusBinary}");
        Logger.Debug($"ErrorData: {errorBinary}");
        
        if (ValidateStatusData(statusBinary, errorBinary) == false) return;
        AssignMachineStatusBitValues(statusBinary);

        var errorCode = errorBinary.Substring(4, errorBinary.Length - 4);
        errorCode = errorCode.PadLeft(8, '0');
        AssignErrorBitValues(errorCode, errorBinary);

        DeviceStatus = ErrorData == 0x00 ? DeviceStatusE.Ok : DeviceStatusE.Ng;
    }

    private void AssignMachineStatusBitValues(string statusBinary)
    {
        #region Status Value Assignments
        IceIsFull = statusBinary[0] == '1' ? 1 : 0;
        IsInCommunicationMode = statusBinary[1] == '1' ? 1 : 0;
        CupLeverDetected = statusBinary[2] == '1' ? 1 : 0;
        IceOutSolDetected = statusBinary[3] == '1' ? 1 : 0;
        GearMotorRunning = statusBinary[4] == '1' ? 1 : 0;
        CompressorRunning = statusBinary[5] == '1' ? 1 : 0;
        IceDetectedInPreviousCycle = statusBinary[6] == '1' ? 1 : 0;
        NoIceDetectedInPreviousCycle = statusBinary[7] == '1' ? 1 : 0;
        #endregion
    }

    private void AssignErrorBitValues(string errorCode, string errorBinary)
    {
        var errorCodeByte = DataUtils.BinaryStringToByte(errorCode);
        #region Error Code Assignments
        switch (errorCodeByte)
        {
            case 0x00:
                return;
            case 0x01:
                WaterSupplyFaulty = 1;
                break;
            case 0x02:
                WaterDrainageFaulty = 1;
                break;
            case 0x03:
                ExteriorTemperatureLow = 1;
                break;
            case 0x04:
                ExteriorTemperatureHigh = 1;
                break;
            case 0x05:
                CondenserTemperatureHigh = 1;
                break;
            case 0x06:
                EvaporatorTemperatureLow = 1;
                break;
            case 0x07:
                GmFaulty = 1;
                break;
            case 0x08:
                IceFunctionFaulty = 1;
                break;
            case 0x09:
                InspectionPeriod = 1;
                break;
            case 0x0C:
                DcCommunicationError = 1;
                break;
            case 0x0F:
                FanMotorFaulty = 1;
                break;
        }
        #endregion
        WaitingForIceOut = errorBinary[3];
    }

    private bool ValidateStatusData(string statusBinary, string errorBinary)
    {
        if (statusBinary.Length < 8)
        {
            DeviceStatus = DeviceStatusE.Ng;
            return false;
        }

        if (errorBinary.Length < 8)
        {
            DeviceStatus = DeviceStatusE.Ng;
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{Environment.NewLine}" +
               $"StatusData: {StatusData:X2}{Environment.NewLine}" +
               $"ErrorData: {ErrorData:X2}{Environment.NewLine}" +
               $"NoIceDetectedInPreviousCycle: {NoIceDetectedInPreviousCycle}{Environment.NewLine}" +
               $"IceDetectedInPreviousCycle: {IceDetectedInPreviousCycle}{Environment.NewLine}" +
               $"CompressorRunning: {CompressorRunning}{Environment.NewLine}" +
               $"GearMotorRunning: {GearMotorRunning}{Environment.NewLine}" +
               $"IceOutSolDetected: {IceOutSolDetected}{Environment.NewLine}" +
               $"CupLeverDetected: {CupLeverDetected}{Environment.NewLine}" +
               $"IsInCommunicationMode: {IsInCommunicationMode}{Environment.NewLine}" +
               $"IceIsFull: {IceIsFull}{Environment.NewLine}" +
               $"WaitingForIceOut: {WaitingForIceOut}{Environment.NewLine}" +
               $"WaterSupplyFaulty: {WaterSupplyFaulty}{Environment.NewLine}" +
               $"WaterDrainageFaulty: {WaterDrainageFaulty}{Environment.NewLine}" +
               $"ExteriorTemperatureLow: {ExteriorTemperatureLow}{Environment.NewLine}" +
               $"ExteriorTemperatureHigh: {ExteriorTemperatureHigh}{Environment.NewLine}" +
               $"CondenserTemperatureHigh: {CondenserTemperatureHigh}{Environment.NewLine}" +
               $"EvaporatorTemperatureLow: {EvaporatorTemperatureLow}{Environment.NewLine}" +
               $"GmFaulty: {GmFaulty}{Environment.NewLine}" +
               $"IceFunctionFaulty: {IceFunctionFaulty}{Environment.NewLine}" +
               $"InspectionPeriod: {InspectionPeriod}{Environment.NewLine}" +
               $"DcCommunicationError: {DcCommunicationError}{Environment.NewLine}" +
               $"FanMotorFaulty: {FanMotorFaulty}{Environment.NewLine}";
    }
}