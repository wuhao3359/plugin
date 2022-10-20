using WoAutoCollectionPlugin.Manager;

namespace WoAutoCollectionPlugin.SeFunctions;

public class Executor
{

    private readonly CommandManager _commandManager = new(DalamudApi.GameGui, DalamudApi.SigScanner);

    public void DoGearChange(string set)
    {
        _commandManager.Execute($"/gearset change \"{set}\"");
    }
}
