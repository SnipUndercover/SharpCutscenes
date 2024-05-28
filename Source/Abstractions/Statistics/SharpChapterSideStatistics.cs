using Celeste.Mod.SharpCutscenes.Enums;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SharpCutscenes.Abstractions.Statistics;

/// <summary>
///   Save file statistics of a chapter side.
/// </summary>
public sealed class SharpChapterSideStatistics
{
    #region Implementation Details
    // TODO: maybe some info about its parent?
    internal readonly AreaModeStats ChapterSideStats;

    internal SharpChapterSideStatistics(AreaModeStats chapterSideStats)
    {
        ChapterSideStats = chapterSideStats;
    }

    /// <summary>
    ///   A list of statistics for each chapter side.
    /// </summary>
    public sealed class StatisticList
    {
        private readonly SharpChapterStatistics Parent;
        private readonly Dictionary<SharpChapterSide, SharpChapterSideStatistics> Cache = [];

        internal StatisticList(SharpChapterStatistics parent)
        {
            Parent = parent;
        }

        /// <summary>
        ///   Access statistics for a certain chapter side.
        /// </summary>
        /// 
        /// <param name="side">
        ///   The side whose statistics are needed.
        /// </param>
        /// <returns>
        ///   The statistics of the desired chapter side.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        ///   <paramref name="side"/> is not a valid chapter side.
        /// </exception>
        public SharpChapterSideStatistics this[SharpChapterSide side]
        {
            get
            {
                if (Cache.TryGetValue(side, out SharpChapterSideStatistics? stats))
                    return stats!;

                if (side < SharpChapterSide.ASide || side > SharpChapterSide.CSide)
                    throw new ArgumentException($"The given chapter side ({side}) is not valid.");

                Cache.Add(side, stats = new(Parent.ChapterStats.Modes[(int)side]));

                return stats!;
            }
        }
    }
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   The lowest dash count this chapter side has been completed in.
    /// </summary>
    public int BestDashes
        => ChapterSideStats.BestDashes;

    /// <summary>
    ///   The lowest death count this chapter side has been completed in.
    /// </summary>
    public int BestDeaths
        => ChapterSideStats.BestDeaths;

    /// <summary>
    ///   The shortest time this chapter side has been full cleared in, or <see langword="null"/> if it has not been full cleared yet.
    /// </summary>
    /// 
    /// <remarks>
    ///   A full clear is done by beating a chapter while picking up all collectables.<br/>
    ///   This includes strawberries, the Crystal Heart and the cassette.
    /// </remarks>
    public TimeSpan? BestFullClearTime
        => HasBeenFullCleared
            ? TimeSpan.FromTicks(ChapterSideStats.BestFullClearTime)
            : null;

    /// <summary>
    ///   The shortest time this chapter side has been completed in, or <see langword="null"/> if it has not been completed yet.
    /// </summary>
    public TimeSpan? BestTime
        => HasBeenCompleted
            ? TimeSpan.FromTicks(ChapterSideStats.BestTime)
            : null;

    /// <summary>
    ///   A set of all collected strawberry IDs in this chapter side.
    /// </summary>
    public IReadOnlySet<EntityID> CollectedStrawberryIds
        => ChapterSideStats.Strawberries;

    /// <summary>
    ///   Whether this chapter side has been completed.
    /// </summary>
    public bool HasBeenCompleted
        => ChapterSideStats.Completed;

    /// <summary>
    ///   Whether this chapter side has been completed in a single session.
    /// </summary>
    /// 
    /// <remarks>
    ///   For a single session completion to be registered, Madeline must have completed this chapter side<br/>
    ///   from the beginning to the end in a single, uninterrupted session.<br/>
    ///   This means using <i>Save and Quit</i> is allowed, whereas <i>Return to Map</i> is not.
    /// </remarks>
    public bool HasBeenCompletedInOneSession
        => ChapterSideStats.SingleRunCompleted;

    /// <summary>
    ///   The shortest time this chapter side has been full cleared in.
    /// </summary>
    /// 
    /// <remarks>
    ///   A full clear is done by beating a chapter while picking up all collectables.<br/>
    ///   This includes strawberries, the Crystal Heart and the cassette.
    /// </remarks>
    public bool HasBeenFullCleared
        => ChapterSideStats.FullClear;

    /// <summary>
    ///   Whether the Crystal Heart has been collected on this chapter side.
    /// </summary>
    public bool HasCollectedCrystalHeart
        => ChapterSideStats.HeartGem;

    /// <summary>
    ///   The total amount of time spent on this chapter side.
    /// </summary>
    public TimeSpan TimePlayed
        => TimeSpan.FromTicks(ChapterSideStats.TimePlayed);

    /// <summary>
    ///   The total amount of deaths on this chapter side.
    /// </summary>
    public int TotalDeaths
        => ChapterSideStats.Deaths;

    /// <summary>
    ///   The total amount of strawberries collected on this chapter side.
    /// </summary>
    public int TotalStrawberries
        => ChapterSideStats.TotalStrawberries;

    /// <summary>
    ///   The checkpoint room names which Madeline unlocked.
    /// </summary>
    public IReadOnlySet<string> UnlockedCheckpointRoomNames
        => ChapterSideStats.Checkpoints;
    #endregion
}
