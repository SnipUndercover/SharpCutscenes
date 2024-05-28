using System.Collections.Generic;
using System;
using System.Linq;
using System.Numerics;

namespace Celeste.Mod.SharpCutscenes.Abstractions.Statistics;

/// <summary>
///   Save file statistics of all modded chapters.
/// </summary>
public sealed class SharpModdedSaveFileStatistics : SharpSaveFileStatistics
{
    #region Implementation Details
    internal SharpModdedSaveFileStatistics(SharpSaveData parent, SaveData saveData) : base(parent, saveData)
    {
        Chapters = new(parent.ActualSaveData.LevelSets, parent.ActualSaveData.LevelSetRecycleBin);
    }

    // IEnumerable<T> doesn't support Sum<T>() if T is not a primitive type, even if T supports addition operators (like TimeSpan)
    // so we gotta use Aggregate<T>(Func<T, T, T>)
    // 
    // yes, i'm not using it to sum anything else other than int, but it might come in handy later
    internal T SumOverAllChapterSides<T>(Func<AreaModeStats, T> selector) where T : IAdditionOperators<T, T, T>
        => Parent.ActualSaveData.LevelSets
            .SelectMany(static campaignStats => campaignStats.Areas)
            .SelectMany(static chapterStats => chapterStats.Modes)
            .Select(selector)
            .Aggregate(static (acc, item) => acc + item);

    /// <summary>
    ///   A list of statistics for each modded chapter.
    /// </summary>
    public sealed class ModdedChapterStatisticList
    {
        private readonly IReadOnlyList<LevelSetStats> CampaignStatsList;
        private readonly IReadOnlyList<LevelSetStats> UnloadedCampaignStatsList;
        private readonly Dictionary<string, SharpChapterStatistics> Cache = [];

        internal ModdedChapterStatisticList(IReadOnlyList<LevelSetStats> levelSetStatsList, IReadOnlyList<LevelSetStats> unloadedLevelSetStatsList)
        {
            CampaignStatsList = levelSetStatsList;
            UnloadedCampaignStatsList = unloadedLevelSetStatsList;
        }

        /// <summary>
        ///   Access statistics for a certain modded chapter, if it exists.
        /// </summary>
        /// 
        /// <param name="sid">
        ///   The modded chapter SID whose statistics are needed.
        /// </param>
        /// <returns>
        ///   The statistics of the desired modded chapter, or <see langword="null"/> if such a chapter does not exist,
        ///   or the player never loaded it.
        /// </returns>
        public SharpChapterStatistics? this[string sid]
        {
            get
            {
                if (Cache.TryGetValue(sid, out SharpChapterStatistics? stats))
                    return stats;

                static AreaStats? FindChapterBySID(IReadOnlyList<LevelSetStats> campaignStatsList, string sid)
                {
                    foreach (LevelSetStats campaignStats in campaignStatsList)
                    {
                        if (!sid.StartsWith(campaignStats.Name))
                            continue;

                        AreaStats? maybeFoundChapter = campaignStats.Areas.FirstOrDefault(areaStats => areaStats.SID == sid);
                        if (maybeFoundChapter is not null)
                            return maybeFoundChapter;
                    }

                    return null;
                }

                AreaStats? maybeChapterStats = FindChapterBySID(CampaignStatsList, sid) ?? FindChapterBySID(UnloadedCampaignStatsList, sid);

                if (maybeChapterStats is not null)
                    Cache.Add(sid, stats = new(maybeChapterStats));

                return stats;
            }
        }
    }
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   Statistics for individual modded chapters.
    /// </summary>
    public readonly ModdedChapterStatisticList Chapters;
    #endregion

    #region Methods
    /// <inheritdoc cref="SharpSaveFileStatistics.TotalDeaths"/>
    public override int TotalDeaths()
        => SumOverAllChapterSides(static chapterSideStats => chapterSideStats.Deaths);

    /// <inheritdoc cref="SharpSaveFileStatistics.TotalStrawberries"/>
    public override int TotalStrawberries()
        => SumOverAllChapterSides(static chapterSideStats => chapterSideStats.TotalStrawberries);

    /// <inheritdoc cref="SharpSaveFileStatistics.TotalTimePlayed"/>
    public override TimeSpan TotalTimePlayed()
        => TimeSpan.FromTicks(SumOverAllChapterSides(static chapterSideStats => chapterSideStats.TimePlayed));
    #endregion
}
