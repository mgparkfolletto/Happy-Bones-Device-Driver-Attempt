namespace IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;

public class NakazoTemperatureDataModel
{
    public int ExteriorTemperature { get; set; } = 0;
    public int EvaporatorTemperature { get; set; } = 0;
    public int CondenserTemperature { get; set; } = 0;

    public NakazoTemperatureDataModel(int exteriorTemperature, int evaporatorTemperature, int condenserTemperature)
    {
        ExteriorTemperature = exteriorTemperature;
        EvaporatorTemperature = evaporatorTemperature;
        CondenserTemperature = condenserTemperature;
    }

    public override string ToString()
    {
        return
            $"ExteriorTemperature: {ExteriorTemperature}, EvaporatorTemperature: {EvaporatorTemperature}, CondenserTemperature: {CondenserTemperature}";
    }
}