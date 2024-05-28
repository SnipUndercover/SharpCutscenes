using Celeste.Mod.SharpCutscenes.Abstractions.Statistics;
using Celeste.Mod.SharpCutscenes.Enums;
using System;
using System.Diagnostics;

namespace Celeste.Mod.SharpCutscenes.Abstractions;

/// <summary>
///   This is a simplified wrapper for the true <see cref="SaveData"/> used across Celeste,
///   which is full of documentation and helper methods commonly used in cutscenes.
/// </summary>
/// 
/// <remarks>
///   If at any point you need to access the true <see cref="SaveData"/> instance, simply cast it to <see cref="SaveData"/>.
/// </remarks>
public sealed class SharpSaveData
{
    // TODO: i might have missed some fields, go over this with a fine tooth comb in the future (like save flags)

    #region Implementation Details
    internal SharpSaveData(SharpCutscene parent, SaveData actualSaveData)
    {
        Parent = parent;
        ActualSaveData = actualSaveData;
        VanillaStatistics = new(this, ActualSaveData);
        ModdedStatistics = new(this, ActualSaveData);
    }

    private readonly SharpCutscene Parent;
    internal readonly SaveData ActualSaveData;

    /// <summary>
    ///   Convert the simple <see cref="SharpSaveData"/> into the actual <see cref="SaveData"/> used across Celeste.
    /// </summary>
    /// 
    /// <param name="saveData">
    ///   The <see cref="SaveData"/> to convert.
    /// </param>
    public static implicit operator SaveData(SharpSaveData saveData)
        => saveData.ActualSaveData;

    #endregion

    #region Fields & Properties

    /// <summary>
    ///   Statistics for the currently open chapter side.
    /// </summary>
    public SharpChapterSideStatistics CurrentChapterSideStatistics
    {
        get
        {
            AreaKey currentArea = Parent.Session.ActualSession.Area;

            if (currentArea.ID < 0)
                // how.
                throw new UnreachableException("Area ID is negative!");

            if (currentArea.ID <= (int)SharpVanillaChapter.Farewell)
                return VanillaStatistics.Chapters[(SharpVanillaChapter)currentArea.ID].Sides[(SharpChapterSide)currentArea.Mode];

            return ModdedStatistics.Chapters[currentArea.SID]!.Sides[(SharpChapterSide)currentArea.Mode];
        }
    }

    /// <summary>
    ///   The total time spent in a map in this save file.
    /// </summary>
    public TimeSpan FileTime
        => TimeSpan.FromTicks(ActualSaveData.Time);

    /// <summary>
    ///   Whether assist mode is enabled.
    /// </summary>
    public bool IsAssistModeEnabled =>
        ActualSaveData.AssistMode;

    /// <summary>
    ///   Whether cheat mode is enabled.
    /// </summary>
    public bool IsCheatModeEnabled =>
        ActualSaveData.CheatMode;

    /// <summary>
    ///   Whether debug mode is enabled.
    /// </summary>
    public bool IsDebugModeEnabled =>
        ActualSaveData.DebugMode;

    /// <summary>
    ///   Whether variant mode is enabled.
    /// </summary>
    public bool IsVariantModeEnabled =>
        ActualSaveData.VariantMode;

    /// <summary>
    ///   Statistics for all modded chapters.
    /// </summary>
    public readonly SharpModdedSaveFileStatistics ModdedStatistics;

    /// <summary>
    ///   This save file's slot.
    /// </summary>
    public int SlotNumber =>
        ActualSaveData.FileSlot;

    /// <summary>
    ///   The chapter (and side) Madeline last visited.
    /// </summary>
    public AreaKey LastArea
        => ActualSaveData.LastArea_Safe;

    /// <summary>
    ///   The player name.
    /// </summary>
    public string PlayerName
        => ActualSaveData.Name;

    /// <summary>
    ///   Whether Farewell is revealed on the chapter selection.
    /// </summary>
    public bool IsFarewellRevealed
        => ActualSaveData.RevealedChapter9;

    /// <summary>
    ///   The name of Theo's sister.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is usually <c>Alex</c>, unless your current save file name is <c>Alex</c>, in which case this is set to <c>Maddy</c>.
    /// </remarks>
    public string TheoSisterName
        => ActualSaveData.TheoSisterName;

    /// <summary>
    ///   The total death count for this save file.
    /// </summary>
    public int TotalDeaths
        => ActualSaveData.TotalDeaths;

    /// <summary>
    ///   The total dash count for this save file.
    /// </summary>
    public int TotalDashes
        => ActualSaveData.TotalDashes;

    /// <summary>
    ///   The total jump count for this save file.
    /// </summary>
    public int TotalJumps
        => ActualSaveData.TotalJumps;

    /// <summary>
    ///   The total wall jump count for this save file.
    /// </summary>
    public int TotalWallJumps
        => ActualSaveData.TotalWallJumps;

    /// <summary>
    ///   Statistics for all vanilla chapters.
    /// </summary>
    public readonly SharpVanillaSaveFileStatistics VanillaStatistics;
    #endregion
}
