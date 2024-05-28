using Monocle;
using MonoMod.Utils;
using System;

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
        // track statistics in current room
        On.Celeste.Session.ctor += SetInitialCurrentRoomStats;

        On.Celeste.Level.UpdateTime += TrackTimeInRoom;
        On.Celeste.Player.Jump += TrackJumpsInRoom;
        On.Celeste.Player.SuperJump += TrackJumpsInRoom;
        On.Celeste.Player.CallDashEvents += TrackDashesInRoom;

        On.Celeste.Level.Reload += ResetTrackedStatsOnRoomReload;
    }

    public override void Unload()
    {
        On.Celeste.Session.ctor -= SetInitialCurrentRoomStats;

        On.Celeste.Level.UpdateTime -= TrackTimeInRoom;
        On.Celeste.Player.Jump -= TrackJumpsInRoom;
        On.Celeste.Player.SuperJump -= TrackJumpsInRoom;
        On.Celeste.Player.CallDashEvents -= TrackDashesInRoom;

        On.Celeste.Level.Reload -= ResetTrackedStatsOnRoomReload;
    }

    private void SetInitialCurrentRoomStats(On.Celeste.Session.orig_ctor orig, Session self)
    {
        orig(self);

        self.ResetCurrentRoomStats();
    }

    private void TrackTimeInRoom(On.Celeste.Level.orig_UpdateTime orig, Level self)
    {
        orig(self);

        self.Session.IncrementTimeSpentInCurrentRoom();
    }

    private void TrackJumpsInRoom(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
    {
        orig(self, particles, playSfx);

        Session session = self.SceneAs<Level>().Session;
        session.IncrementJumpsInCurrentRoom();
        session.IncrementTotalJumps();
    }

    private void TrackJumpsInRoom(On.Celeste.Player.orig_SuperJump orig, Player self)
    {
        orig(self);

        Session session = self.SceneAs<Level>().Session;
        session.IncrementJumpsInCurrentRoom();
        session.IncrementTotalJumps();
    }

    private void TrackDashesInRoom(On.Celeste.Player.orig_CallDashEvents orig, Player self)
    {
        orig(self);

        self.SceneAs<Level>().Session.IncrementDashesInCurrentRoom();
    }

    private void ResetTrackedStatsOnRoomReload(On.Celeste.Level.orig_Reload orig, Level self)
    {
        orig(self);

        self.Session.ResetCurrentRoomStats();
    }
}