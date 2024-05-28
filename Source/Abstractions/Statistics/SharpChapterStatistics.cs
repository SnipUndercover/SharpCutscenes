using System;
using System.Linq;

namespace Celeste.Mod.SharpCutscenes.Abstractions.Statistics;

/// <summary>
///   Save file statistics of a chapter.
/// </summary>
public sealed class SharpChapterStatistics
{
    #region Implementation Details
    internal readonly AreaStats ChapterStats;

    internal SharpChapterStatistics(AreaStats chapterStats)
    {
        ChapterStats = chapterStats;
        Sides = new(this);
    }
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   The lowest dash count this chapter has been completed in, or <see langword="null"/> if no sides have been completed yet.
    /// </summary>
    public int? BestTotalDashes
        => HasCompletedAnySides
            ? ChapterStats.BestTotalDashes
            : null;

    /// <summary>
    ///   The lowest death count this chapter side has been completed in, or <see langword="null"/> if no sides have been completed yet.
    /// </summary>
    public int? BestTotalDeaths
        => HasCompletedAnySides
            ? ChapterStats.BestTotalDeaths
            : null;

    /// <summary>
    ///   The shortest time this chapter has been completed in, or <see langword="null"/> if no sides have been completed yet.
    /// </summary>
    public TimeSpan? BestTotalTime
        => HasCompletedAnySides
            ? TimeSpan.FromTicks(ChapterStats.BestTotalTime)
            : null;

    /// <summary>
    ///   This chapter's ordinal ID.
    /// </summary>
    /// 
    /// <remarks>
    ///   This ID depends on the amount of loaded maps and shouldn't be used as a permanent reference.<br/>
    ///   Use <see cref="SID"/> for that purpose instead.
    /// </remarks>
    public int ID
        => ChapterStats.ID;

    /// <summary>
    ///   Whether the cassette has been collected in this chapter.
    /// </summary>
    public bool HasCollectedCassette
        => ChapterStats.Cassette;

    /// <summary>
    ///   Whether any chapter sides have been completed.
    /// </summary>
    public bool HasCompletedAnySides
        => ChapterStats.Modes.Any(static areaModeStats => areaModeStats.Completed);

    /// <summary>
    ///   This chapter's string ID.<br/>
    /// </summary>
    /// 
    /// <remarks>
    ///   This is equal to the path of the map, relative to the <c>Maps</c> folder.<br/>
    ///   Example: <c>Spooooky/SentientForest/Forest</c>
    /// </remarks>
    public string SID
        => ChapterStats.SID;

    /// <summary>
    ///   Statistics for individual chapter sides.
    /// </summary>
    public readonly SharpChapterSideStatistics.StatisticList Sides;

    /// <summary>
    ///   The total amount of deaths across all sides on this chapter.
    /// </summary>
    public int TotalDeaths
        => ChapterStats.TotalDeaths;

    /// <summary>
    ///   The total amount of strawberries collected across all sides on this chapter.
    /// </summary>
    public int TotalStrawberries
        => ChapterStats.TotalStrawberries;

    /// <summary>
    ///   The total time spent in this chapter.
    /// </summary>
    public TimeSpan TotalTimePlayed
        => TimeSpan.FromTicks(ChapterStats.TotalTimePlayed);
    #endregion
}
