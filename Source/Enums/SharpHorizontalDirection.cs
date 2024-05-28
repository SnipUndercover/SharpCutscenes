namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   A horizontal direction.
/// </summary>
public enum SharpHorizontalDirection
{
    /// <summary>
    ///   The left horizontal direction.
    /// </summary>
    /// 
    /// <remarks>
    ///   In Celeste, left is negative on the X axis. Therefore, moving left will decrease Madeline's X position.
    /// </remarks>
    Left = -1,

    /// <summary>
    ///   The right horizontal direction.
    /// </summary>
    /// 
    /// <remarks>
    ///   In Celeste, right is positive on the X axis. Therefore, moving right will increase Madeline's X position.
    /// </remarks>
    Right = 1
}
