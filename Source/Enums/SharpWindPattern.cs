namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   Wind patterns used in Celeste.
/// </summary>
public enum SharpWindPattern
{
    /// <summary>
    ///   No wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is the default wind pattern.
    /// </remarks>
    None,

    /// <summary>
    ///   Regular left wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player left 40px/s.
    /// </remarks>
    Left,

    /// <summary>
    ///   Regular right wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player right 40px/s.
    /// </remarks>
    Right,

    /// <summary>
    ///   Strong left wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player left 80px/s.
    /// </remarks>
    LeftStrong,

    /// <summary>
    ///   Strong right wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player right 80px/s.
    /// </remarks>
    RightStrong,

    /// <summary>
    ///   Periodic strong left wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Switches between strong left wind and no wind every 3 seconds.
    /// </remarks>
    LeftOnOff,

    /// <summary>
    ///   Periodic strong right wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Switches between strong right wind and no wind every 3 seconds.
    /// </remarks>
    RightOnOff,

    /// <summary>
    ///   Fast periodic strong left wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Switches between strong left wind and no wind every 2 seconds.
    /// </remarks>
    LeftOnOffFast,

    /// <summary>
    ///   Fast periodic strong right wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Switches between strong right wind and no wind every 2 seconds.
    /// </remarks>
    RightOnOffFast,

    /// <summary>
    ///   Alternating left/right wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Switches between regular left wind and regular right wind every 3 seconds, with a 2 second pause in-between.
    /// </remarks>
    Alternating,

    /// <summary>
    ///   Regular left wind, if Madeline is holding a strawberry seed.
    /// </summary>
    LeftSeedsOnly,

    /// <summary>
    ///   Crazy right wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player right 120px/s.
    /// </remarks>
    RightCrazy,

    /// <summary>
    ///   Down wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player down 30px/s.
    /// </remarks>
    Down,

    /// <summary>
    ///   Regular up wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player up 40px/s.
    /// </remarks>
    Up,

    /// <summary>
    ///   Strong up wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moves the player up 60px/s.
    /// </remarks>
    UpStrong,

    /// <summary>
    ///   User-defined wind pattern. 
    /// </summary>
    Custom
}