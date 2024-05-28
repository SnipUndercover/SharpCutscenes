using Celeste.Mod.SharpCutscenes.Enums;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.SharpCutscenes.Abstractions;

/// <summary>
///   A simple wrapper allowing you to control the wind in a level.
/// </summary>
public class SharpWind
{
    #region Implementation Details
    internal SharpWind(SharpLevel parent)
    {
        Parent = parent;
    }

    private readonly SharpLevel Parent;

    private WindController? WindController
        => Parent.ActualLevel.windController;

    private void EnsureWindControllerExists(out bool wasCreated, WindController.Patterns defaultPattern = WindController.Patterns.None)
    {
        if (WindController is null)
        {
            Parent.ActualLevel.Add(Parent.ActualLevel.windController = new WindController(defaultPattern));
            wasCreated = true;
        }
        else
            wasCreated = false;
    }
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   Target wind speed.
    /// </summary>
    /// 
    /// <remarks>
    ///   If you want custom wind, set <see cref="Pattern"/> to <see cref="WindPattern.None"/> and adjust <see cref="TargetSpeed"/>.
    /// </remarks>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to set target wind speed while there's a pattern set.
    /// </exception>
    public Vector2 TargetSpeed
    {
        get => WindController?.targetSpeed ?? Vector2.Zero;
        set
        {
            EnsureWindControllerExists(out bool wasCreated);

            if (!wasCreated && WindController!.pattern != WindController.Patterns.None)
                throw new InvalidOperationException($"Cannot set target speed while there's a wind pattern set.");

            WindController!.targetSpeed = value;
        }
    }

    /// <summary>
    ///   Current wind speed.
    /// </summary>
    public Vector2 CurrentSpeed
        => Parent.ActualLevel.Wind;

    /// <summary>
    ///   Current wind pattern.
    /// </summary>
    /// 
    /// <remarks>
    ///   If you want custom wind, set <see cref="Pattern"/> to <see cref="WindPattern.None"/> and adjust <see cref="TargetSpeed"/>.
    /// </remarks>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to set the pattern to <see cref="WindPattern.Custom"/>.
    /// </exception>
    public WindPattern Pattern
    {
        get
        {
            if (WindController is null)
                return WindPattern.None;

            if (WindController.pattern == WindController.Patterns.None
                    && WindController.targetSpeed != Vector2.Zero)
                return WindPattern.Custom;

            return (WindPattern)WindController.pattern;
        }
        set
        {
            if (value == WindPattern.Custom)
                throw new InvalidOperationException(
                    $"The {WindPattern.Custom} pattern does not exist in Celeste. Please set the {nameof(TargetSpeed)} property instead.");

            WindController.Patterns castedPattern = (WindController.Patterns)value;
            EnsureWindControllerExists(out bool wasCreated, castedPattern);

            if (wasCreated)
                return;
            WindController!.SetPattern(castedPattern);
        }
    }
    #endregion
}
