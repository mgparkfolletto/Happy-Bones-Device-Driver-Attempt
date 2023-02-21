namespace IceMachineDriverLibrary.IceMachine.Nakazo.DataModel;

public class NakazoResponseDataModel
{
    public byte Status { get; set; }
    public byte Command { get; set; }
    public byte Data1 { get; set; } 
    public byte Data2 { get; set; }

    public NakazoResponseDataModel(IReadOnlyList<byte> response)
    {
        Status = response[1];
        Command = response[2];
        Data1 = response[3];
        Data2 = response[4];
    }
    
    public override string ToString()
    {
        return $"Status: {Status:X2}, Command: {Command:X2}, Data1: {Data1:X2}, Data2: {Data2:X2}";
    }
}