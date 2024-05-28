namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   The units used in mapping.
/// </summary>
public enum SharpMapUnit
{
    /// <summary>
    ///   A single pixel.
    /// </summary>
    Pixel = 1,

    /// <summary>
    ///   A single tile, which is 8 pixels.
    /// </summary>
    Tile = 8,

    /// <summary>
    ///   The screen width, which is 320 pixels.
    /// </summary>
    ScreenWidth = Celeste.GameWidth,

    /// <summary>
    ///   The screen height, which is 180 pixels.
    /// </summary>
    ScreenHeight = Celeste.GameHeight
}
