using System.Collections.Generic;
using System;
using Celeste.Mod.SharpCutscenes.Enums;
using System.Linq;
using System.Numerics;

namespace Celeste.Mod.SharpCutscenes.Abstractions.Statistics;

/// <summary>
///   Save file statistics of all vanilla chapters.
/// </summary>
public sealed class SharpVanillaSaveFileStatistics : SharpSaveFileStatistics
{
    #region Implementation Details
    internal SharpVanillaSaveFileStatistics(SharpSaveData parent, SaveData saveData) : base(parent, saveData)
    {
        Chapters = new(parent.ActualSaveData.Areas);
    }

    // IEnumerable<T> doesn't support Sum<T>() if T is not a primitive type, even if T supports addition operators (like TimeSpan)
    // so we gotta use Aggregate<T>(Func<T, T, T>)
    // 
    // yes, i'm not using it to sum anything else other than int, but it might come in handy later
    internal T SumOverAllChapterSides<T>(Func<AreaModeStats, T> selector) where T : IAdditionOperators<T, T, T>
        => Parent.ActualSaveData.Areas
            .SelectMany(static chapterStats => chapterStats.Modes)
            .Select(selector)
            .Aggregate(static (acc, item) => acc + item);

    /// <summary>
    ///   A list of statistics for each vanilla chapter.
    /// </summary>
    public sealed class VanillaChapterStatisticList
    {
        private readonly IReadOnlyList<AreaStats> AreaStatsList;
        private readonly Dictionary<SharpVanillaChapter, SharpChapterStatistics> Cache = [];

        internal VanillaChapterStatisticList(IReadOnlyList<AreaStats> areaStatsList)
        {
            AreaStatsList = areaStatsList;
        }

        /// <summary>
        ///   Access statistics for a certain vanilla chapter.
        /// </summary>
        /// 
        /// <param name="chapter">
        ///   The vanilla chapter whose statistics are needed.
        /// </param>
        /// <returns>
        ///   The statistics of the desired vanilla chapter.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        ///   <paramref name="chapter"/> is not a valid chapter.
        /// </exception>
        public SharpChapterStatistics this[SharpVanillaChapter chapter]
        {
            get
            {
                if (Cache.TryGetValue(chapter, out SharpChapterStatistics? stats))
                    return stats!;

                if (chapter < SharpVanillaChapter.Prologue || chapter > SharpVanillaChapter.Farewell)
                    throw new ArgumentException($"The vanilla chapter ID ({chapter}) is invalid.");

                Cache.Add(chapter, stats = new(AreaStatsList[(int)chapter]));

                return stats!;
            }
        }
    }
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   Statistics for vanilla chapter sides.
    /// </summary>
    public readonly VanillaChapterStatisticList Chapters;
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
