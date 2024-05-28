using Monocle;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.SharpCutscenes;

internal static class SharpCutscenesExtensions
{
    private const string DynamicData_TimeSpentInCurrentRoom = "SharpCutscenes/TimeSpentInCurrentRoom";
    private const string DynamicData_JumpsInCurrentRoom = "SharpCutscenes/JumpsInCurrentRoom";
    private const string DynamicData_DashesInCurrentRoom = "SharpCutscenes/DashesInCurrentRoom";

    private const string DynamicData_TotalJumps = "SharpCutscenes/TotalJumps";

    internal static TimeSpan GetTimeSpentInCurrentRoom(this Session session)
        => DynamicData.For(session).Get<TimeSpan>(DynamicData_TimeSpentInCurrentRoom);

    internal static void SetTimeSpentInCurrentRoom(this Session session, TimeSpan timeSpentInCurrentRoom)
        => DynamicData.For(session).Set(DynamicData_TimeSpentInCurrentRoom, timeSpentInCurrentRoom);

    internal static void IncrementTimeSpentInCurrentRoom(this Session session)
    {
        DynamicData sessionData = DynamicData.For(session);
        sessionData.Set(DynamicData_TimeSpentInCurrentRoom, sessionData.Get<TimeSpan>(DynamicData_TimeSpentInCurrentRoom).Add(TimeSpan.FromSeconds(Engine.RawDeltaTime)));
    }


    internal static int GetJumpsInCurrentRoom(this Session session)
        => DynamicData.For(session).Get<int>(DynamicData_JumpsInCurrentRoom);

    internal static void SetJumpsInCurrentRoom(this Session session, int jumpsInCurrentRoom)
        => DynamicData.For(session).Set(DynamicData_JumpsInCurrentRoom, jumpsInCurrentRoom);

    internal static void IncrementJumpsInCurrentRoom(this Session session)
    {
        DynamicData sessionData = DynamicData.For(session);
        sessionData.Set(DynamicData_JumpsInCurrentRoom, sessionData.Get<int>(DynamicData_JumpsInCurrentRoom) + 1);
    }


    internal static int GetDashesInCurrentRoom(this Session session)
        => DynamicData.For(session).Get<int>(DynamicData_DashesInCurrentRoom);

    internal static void SetDashesInCurrentRoom(this Session session, int dashesInCurrentRoom)
        => DynamicData.For(session).Set(DynamicData_DashesInCurrentRoom, dashesInCurrentRoom);

    internal static void IncrementDashesInCurrentRoom(this Session session)
    {
        DynamicData sessionData = DynamicData.For(session);
        sessionData.Set(DynamicData_DashesInCurrentRoom, sessionData.Get<int>(DynamicData_DashesInCurrentRoom) + 1);
    }


    internal static int GetTotalJumps(this Session session)
        => DynamicData.For(session).Get<int>(DynamicData_TotalJumps);

    internal static void SetTotalJumps(this Session session, int totalJumps)
        => DynamicData.For(session).Set(DynamicData_TotalJumps, totalJumps);

    internal static void IncrementTotalJumps(this Session session)
    {
        DynamicData sessionData = DynamicData.For(session);
        sessionData.Set(DynamicData_TotalJumps, sessionData.Get<int>(DynamicData_TotalJumps) + 1);
    }


    internal static void ResetCurrentRoomStats(this Session session)
    {
        DynamicData sessionData = DynamicData.For(session);

        sessionData.Set(DynamicData_TimeSpentInCurrentRoom, TimeSpan.Zero);
        sessionData.Set(DynamicData_JumpsInCurrentRoom, 0);
        sessionData.Set(DynamicData_DashesInCurrentRoom, 0);
        sessionData.Set(DynamicData_TotalJumps, 0);
    }
}
