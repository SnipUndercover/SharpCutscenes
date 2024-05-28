namespace Celeste.Mod.SharpCutscenes;

public class SharpCutscenesModule : EverestModule
{
    public static SharpCutscenesModule Instance { get; private set; } = default!;

    public SharpCutscenesModule()
    {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(SharpCutscenesModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(SharpCutscenesModule), LogLevel.Info);
#endif
    }

    public override void Load()
    {
    }

    public override void Unload()
    {
    }
}