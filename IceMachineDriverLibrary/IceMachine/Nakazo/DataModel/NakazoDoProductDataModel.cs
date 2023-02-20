using IceMachineDriverLibrary.IceMachine.Common;

namespace IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;

public class NakazoDoProductDataModel : IIceMachineDoProductData
{
    public double IcePumpingDuration { get; set; }
    public double WaterPumpingDuration { get; set; }

    public NakazoDoProductDataModel(double icePumpingDuration, double waterPumpingDuration)
    {
        IcePumpingDuration = icePumpingDuration;
        WaterPumpingDuration = waterPumpingDuration;
    }
}