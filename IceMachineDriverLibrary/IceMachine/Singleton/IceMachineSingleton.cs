using IceMachineDriverLibrary.IceMachine.Common;
using IceMachineDriverLibrary.IceMachine.Nakazo.Service;

namespace IceMachineDriverLibrary.IceMachine.Singleton;

public class IceMachineSingleton
{
    private static readonly Lazy<IceMachineSingleton> Lazy = new(() => new IceMachineSingleton());

    public static IceMachineSingleton Instance => Lazy.Value;
        
    public IIceMachine IceMachine { get; } = new NakazoIceMachine();

    private IceMachineSingleton()
    {
    }
}