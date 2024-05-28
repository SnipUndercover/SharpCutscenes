using System;

namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   An enumeration of every vanilla player state.
/// </summary>
public enum SharpPlayerState
{
    /// <summary>
    ///   The default state.
    /// </summary>
    StNormal = Player.StNormal,

    /// <summary>
    ///   The climbing state.
    /// </summary>
    StClimb = Player.StClimb,

    /// <summary>
    ///   The dashing state.
    /// </summary>
    StDash = Player.StDash,

    /// <summary>
    ///   The swimming state.
    /// </summary>
    StSwim = Player.StSwim,

    /// <summary>
    ///   The state after entering a booster.<br/>
    ///   The state after exiting is <see cref="StDash"/> or <see cref="StRedDash"/>, depending on the booster color.
    /// </summary>
    StBoost = Player.StBoost,

    /// <summary>
    ///   The state after exiting a red booster.
    /// </summary>
    StRedDash = Player.StRedDash,

    /// <summary>
    ///   The state after colliding with a wall while in <see cref="StRedDash"/>.
    /// </summary>
    StHitSquash = Player.StHitSquash,

    /// <summary>
    ///   The state after getting launched, like from:
    ///   <list type="bullet">
    ///     <item>seeker respawn explosions</item>
    ///     <item>bumpers</item>
    ///     <item>Badeline bosses pushing Madeline away</item>
    ///     <item>non-final Badeline boosts</item>
    ///     <item>puffer explosions</item>
    ///   </list>
    /// </summary>
    StLaunch = Player.StLaunch,

    /// <summary>
    ///   The state after picking up a holdable, like a Theo Crystal or a jelly.
    /// </summary>
    StPickup = Player.StPickup,

    /// <summary>
    ///   The state when dashing inside a dream block.
    /// </summary>
    StDreamDash = Player.StDreamDash,

    /// <summary>
    ///   The state when launched by the final Badeline boost node.
    /// </summary>
    StSummitLaunch = Player.StSummitLaunch,

    /// <summary>
    ///   The state commonly used for scripted movement, like cutscenes.
    /// </summary>
    StDummy = Player.StDummy,

    /// <summary>
    ///   The state when walking from the side of the screen, right after entering a level.
    /// </summary>
    StIntroWalk = Player.StIntroWalk,

    /// <summary>
    ///   The state when jumping from the bottom of the screen, right after entering a level.
    /// </summary>
    StIntroJump = Player.StIntroJump,

    /// <summary>
    ///   The state when respawning.
    /// </summary>
    StIntroRespawn = Player.StIntroRespawn,

    /// <summary>
    ///   The state where Madeline wakes up, like at the beginning of Old Site A-Side.
    /// </summary>
    StIntroWakeUp = Player.StIntroWakeUp,

    /// <summary>
    ///   The state in Prologue, where the bird teaches Madeline how to dash.
    /// </summary>
    StBirdDashTutorial = Player.StBirdDashTutorial,

    /// <summary>
    ///   Unused in vanilla, except for the gondola cutscene in Golden Ridge A-Side. <i>(for some reason)</i>
    /// </summary>
    StFrozen = Player.StFrozen,

    /// <summary>
    ///   The state where Madeline falls at the beginning of Reflection A-Side, after Starjump.
    /// </summary>
    StReflectionFall = Player.StReflectionFall,

    /// <summary>
    ///   The state after collecting a feather.
    /// </summary>
    StStarFly = Player.StStarFly,

    /// <summary>
    ///   The state where Madeline falls and does a different recover animation after the mirror cutscene in Mirror Temple A-Side.
    /// </summary>
    StTempleFall = Player.StTempleFall,

    /// <summary>
    ///   The state which puts Madeline in a bubble after collecting a cassette.
    /// </summary>
    StCassetteFly = Player.StCassetteFly,

    /// <summary>
    ///   The state where Madeline is attracted to the Badeline Boss when nearby, making it easier to hit her.
    /// </summary>
    StAttract = Player.StAttract,

    /// <summary>
    ///   The state when Madeline jumps from the bottom of the level and falls down slowly in Farewell.
    /// </summary>
    StIntroMoonJump = Player.StIntroMoonJump,

    /// <summary>
    ///   The state where Madeline is flung by the bird.
    /// </summary>
    StFlingBird = Player.StFlingBird,

    /// <summary>
    ///   The state where Madeline walks for a bit, turns around, waits for around a second, and then turns around again.
    /// </summary>
    StIntroThinkForABit = Player.StIntroThinkForABit,


    // this only exists so that people don't get confused where the feather state is,
    // considering it's named unintuitively
    // and if you somehow manage to avoid that compiler error, it's literally identical to StStarFly

#pragma warning disable CA1069 // Enum values should not be duplicated
    /// <summary>
    ///   <see cref="StFeather"/> does not actually exist. The feather state is called <see cref="StStarFly"/> instead.
    /// </summary>
    [Obsolete($"StFeather does not actually exist. Use {nameof(StStarFly)} instead.", error: true)]
    StFeather = Player.StStarFly
#pragma warning restore CA1069 // Enum values should not be duplicated
}
