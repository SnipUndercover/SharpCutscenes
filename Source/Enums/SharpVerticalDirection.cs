namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   A vertical direction.
/// </summary>
public enum SharpVerticalDirection
{
    /// <summary>
    ///   The up vertical direction.
    /// </summary>
    /// 
    /// <remarks>
    ///   In Celeste, up is negative on the Y axis. Therefore, moving up will decrease Madeline's Y position.
    /// </remarks>
    Up = -1,

    /// <summary>
    ///   The down vertical direction.
    /// </summary>
    /// 
    /// <remarks>
    ///   In Celeste, up is negative on the Y axis. Therefore, moving up will decrease Madeline's Y position.
    /// </remarks>
    Down = 1
}
