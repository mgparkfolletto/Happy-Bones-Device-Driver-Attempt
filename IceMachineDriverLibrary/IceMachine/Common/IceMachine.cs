namespace IceMachineDriverLibrary.IceMachine.Common;

public interface IIceMachine
{
    public bool Connect();
    public IIceMachineStatus GetStatus();
    public bool IsConnected();
    public bool Close();
    public bool DoProduct(IIceMachineDoProductData data);
}