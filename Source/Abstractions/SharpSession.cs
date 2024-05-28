using System;

namespace Celeste.Mod.SharpCutscenes.Abstractions;

/// <summary>
///   This is a simplified wrapper for the true <see cref="Session"/> used across Celeste,
///   which is full of documentation and helper methods commonly used in cutscenes.
/// </summary>
/// 
/// <remarks>
///   If at any point you need to access the true <see cref="Session"/> instance, simply cast it to <see cref="Session"/>.
/// </remarks>
public sealed class SharpSession
{
    // no more TODOs! banger

    #region Implementation Details
    internal SharpSession(SharpCutscene parent, Session actualSession)
    {
        Parent = parent;
        ActualSession = actualSession;
    }

    private readonly SharpCutscene Parent;
    internal readonly Session ActualSession;

    /// <summary>
    ///   Convert the simple <see cref="SharpSession"/> into the actual <see cref="Session"/> used across Celeste.
    /// </summary>
    /// 
    /// <param name="session">
    ///   The <see cref="Session"/> to convert.
    /// </param>
    public static implicit operator Session(SharpSession session)
        => session.ActualSession;
    #endregion

    /// <summary>
    ///   The name of the room Madeline's currently in.
    /// </summary>
    public string CurrentRoom
        => ActualSession.Level;

    /// <summary>
    ///   The amount of times Madeline dashed since starting the map.
    /// </summary>
    public int Dashes
        => ActualSession.Dashes;

    /// <summary>
    ///   The amount of times Madeline dashed in the current room.
    /// </summary>
    public int DashesInCurrentRoom
        => ActualSession.GetDashesInCurrentRoom();

    /// <summary>
    ///   The amount of times Madeline died since starting the map.
    /// </summary>
    public int Deaths
        => ActualSession.Deaths;

    /// <summary>
    ///   The amount of times Madeline died in the current room.
    /// </summary>
    public int DeathsInCurrentRoom
        => ActualSession.DeathsInCurrentLevel;

    /// <summary>
    ///   The name of the furthest room Madeline has been in.
    /// </summary>
    /// 
    /// <remarks>
    ///   This property works by checking whether Madeline has been in the current room before.<br/>
    ///   If not, then the room name is set as the value.
    /// </remarks>
    public string FurthestSeenRoom
        => ActualSession.FurthestSeenLevel;

    /// <summary>
    ///   Whether the cassette has been collected.
    /// </summary>
    public bool IsCassetteCollected
        => ActualSession.Cassette;

    /// <summary>
    ///   Whether the Crystal Heart has been collected.
    /// </summary>
    public bool IsCrystalHeartCollected
        => ActualSession.HeartGem;

    /// <summary>
    ///   Whether this chapter's C-Side has been unlocked.
    /// </summary>
    public bool IsCSideUnlocked
        => ActualSession.UnlockedCSide;

    /// <summary>
    ///   Whether Madeline's currently dreaming.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Old Site <see cref="DreamStars"/> styleground uses this to determine whether the stars should fall.<br/>
    ///   The <see cref="Bonfire"/> entity also uses this to determine whether it should burn green.
    /// </remarks>
    public bool IsDreaming
        => ActualSession.Dreaming;

    /// <summary>
    ///   Whether Madeline has picked the golden strawberry up.
    /// </summary>
    /// 
    /// <remarks>
    ///   This does not determine whether Madeline has <i>collected</i> a golden strawberry in the past.<br/>
    ///   Use the save data for this purpose.
    /// </remarks>
    public bool IsGoldenPickedUp
        => ActualSession.GrabbedGolden;

    /// <summary>
    ///   The amount of times Madeline jumped since starting the map.
    /// </summary>
    public int Jumps
        => ActualSession.GetTotalJumps();

    /// <summary>
    ///   The amount of times Madeline jumped in the current room.
    /// </summary>
    public int JumpsInCurrentRoom
        => ActualSession.GetJumpsInCurrentRoom();

    /// <summary>
    ///   Whether the player has just started the map.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is set to <see langword="false"/> after the player changes rooms.
    /// </remarks>
    public bool JustStarted
        => ActualSession.JustStarted;

    /// <summary>
    ///   The name of the checkpoint room Madeline started the map from.
    /// </summary>
    public string StartingCheckpointRoom
        => ActualSession.StartCheckpoint;

    /// <summary>
    ///   The amount of time Madeline has spent on the map.
    /// </summary>
    public TimeSpan TimeSpent
        => TimeSpan.FromTicks(ActualSession.Time);

    /// <summary>
    ///   The amount of time Madeline has spent in the current room.
    /// </summary>
    public TimeSpan TimeSpentInCurrentRoom
        => ActualSession.GetTimeSpentInCurrentRoom();

    /// <summary>
    ///   Whether the player started the map from the very first checkpoint.
    /// </summary>
    public bool WasStartedFromBeginning
        => ActualSession.StartedFromBeginning;
}
