namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   The different ways Madeline can enter a room.
/// </summary>
public enum SharpIntroTypes
{
    /// <summary>
    ///   Madeline transitioned into this room from another room.
    /// </summary>
    Transition,

    /// <summary>
    ///   Madeline respawned into the room.
    /// </summary>
    Respawn,

    /// <summary>
    ///   Madeline walked into the room from the left edge of the screen.
    /// </summary>
    WalkInFromLeft,

    /// <summary>
    ///   Madeline walked into the room from the right edge of the screen.
    /// </summary>
    WalkInFromRight,

    /// <summary>
    ///   Madeline jumped into the room from the bottom of the screen.
    /// </summary>
    Jump,

    /// <summary>
    ///   Madeline woke up into the room.
    /// </summary>
    WakeUp,

    /// <summary>
    ///   Madeline fell into the room from the top of the screen, like in Reflection.
    /// </summary>
    Fall,

    /// <summary>
    ///   Madeline was sucked up into the room by the big mirror in Mirror Temple.
    /// </summary>
    MirrorTempleVoid,

    /// <summary>
    ///   Madeline entered the room by popping into existence. <i>*magic!*</i>
    /// </summary>
    None,
    
    /// <summary>
    ///   Madeline thinks for a bit as she enters the room, like in Farewell.
    /// </summary>
    ThinkForABit
}
