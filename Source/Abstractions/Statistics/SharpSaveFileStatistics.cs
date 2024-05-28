using System;

namespace Celeste.Mod.SharpCutscenes.Abstractions.Statistics;

/// <summary>
///   Save file statistics of a subset of chapters.
/// </summary>
public abstract class SharpSaveFileStatistics
{
    #region Implementation Details
    internal readonly SharpSaveData Parent;
    internal readonly SaveData SaveData;

    internal SharpSaveFileStatistics(SharpSaveData parent, SaveData saveData)
    {
        Parent = parent;
        SaveData = saveData;
    }
    #endregion

    #region Methods
    /// <summary>
    ///   Calculate total deaths.<br/>
    ///   <br/>
    ///   <b>IMPORTANT:</b> This method call can take a comparatively long time, especially when done on modded statistics.
    /// </summary>
    /// 
    /// <returns>
    ///   The total amount of deaths for these statistics.  
    /// </returns>
    public abstract int TotalDeaths();

    /// <summary>
    ///   Calculate total strawberries collected.<br/>
    ///   <br/>
    ///   <b>IMPORTANT:</b> This method call can take a comparatively long time, especially when done on modded statistics.
    /// </summary>
    /// 
    /// <returns>
    ///   The total amount of strawberries collected for these statistics.  
    /// </returns>
    public abstract int TotalStrawberries();

    /// <summary>
    ///   Calculate total time spent.<br/>
    ///   <br/>
    ///   <b>IMPORTANT:</b> This method call can take a comparatively long time, especially when done on modded statistics.
    /// </summary>
    /// 
    /// <returns>
    ///   The total amount of time spent for these statistics.  
    /// </returns>
    public abstract TimeSpan TotalTimePlayed();
    #endregion
}
