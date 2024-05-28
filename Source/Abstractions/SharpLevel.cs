using Celeste.Mod.SharpCutscenes.Enums;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Celeste.Mod.SharpCutscenes.Abstractions;

/// <summary>
///   This is a simplified wrapper for the true <see cref="Level"/> used across Celeste,
///   which is full of documentation and helper methods commonly used in cutscenes.
/// </summary>
/// 
/// <remarks>
///   If at any point you need to access the true <see cref="Level"/> instance, simply cast it to <see cref="Level"/>.
/// </remarks>
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simplify member access for new users")]
public sealed class SharpLevel
{
    // TODO: actual camera class and stuff

    #region Implementation Details
    internal SharpLevel(SharpCutscene parent, Level level)
    {
        Parent = parent;
        ActualLevel = level;
        Entities = new(this);
        Flags = new(this);
        Wind = new(this);
    }

    private readonly SharpCutscene Parent;
    internal readonly Level ActualLevel;

    /// <summary>
    ///   Convert the simple <see cref="SharpLevel"/> into the actual <see cref="Level"/> used across Celeste.
    /// </summary>
    /// 
    /// <param name="level">
    ///   The <see cref="SharpLevel"/> to convert.
    /// </param>
    public static implicit operator Level(SharpLevel level)
        => level.ActualLevel;

    /// <summary>
    ///   An abstraction class to simplify flag access.<br/>
    ///   Its sole purpose is to allow the use of indexing operators for flags.
    ///   (<c>Level.Flags["MyFlag"] = true;</c>)
    /// </summary>
    public sealed class SharpFlags
    {
        private readonly SharpLevel Level;

        internal SharpFlags(SharpLevel level)
        {
            Level = level;
        }

        /// <summary>
        ///   Access or modify the current state of a flag.
        /// </summary>
        /// 
        /// <remarks>
        ///   A flag in Celeste is at its heart an on/off switch with a name.<br/>
        ///   A practical example can be seen in Core: when the current core mode switches to Cold, the flag with the name <c>cold</c> is set.<br/>
        ///   That flag is what causes the stylegrounds and decals to respond to core mode changes.
        /// </remarks>
        /// 
        /// <param name="flag">
        ///   The flag name of interest.
        /// </param>
        public bool this[string flag]
        {
            get => Level.ActualLevel.Session.GetFlag(flag);
            set => Level.ActualLevel.Session.SetFlag(flag, value);
        }
    }
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   The identifier of this level's chapter and side.
    /// </summary>
    public AreaKey Area
        => Parent.Session.ActualSession.Area;

    // based on Set Bloom Base, Bloom Strength and Darkness triggers from Maddie's Helping Hand

    /// <summary>
    ///   Current bloom strength.
    /// </summary>
    /// 
    /// <remarks>
    ///   Bloom causes bright colors to appear to glow.<br/>
    ///   Bloom base determines how bright the color has to be for bloom to be applied.
    ///   Bloom strength determines how strong the effect is.
    /// </remarks>
    public float BloomStrength
    {
        get => ActualLevel.Bloom.Strength;
        set => ActualLevel.Bloom.Strength = value;
    }

    /// <summary>
    ///   Current bloom base. (between <c>0f</c> and <c>1f</c>)
    /// </summary>
    /// 
    /// <remarks>
    ///   Bloom causes bright colors to appear to glow.<br/>
    ///   Bloom base determines how bright the color has to be for bloom to be applied.
    ///   Bloom strength determines how strong the effect is.
    /// </remarks>
    public float BloomBase
    {
        get => ActualLevel.Bloom.Base;
        set
        {
            if (value < 0f || value > 1f)
                throw new ArgumentOutOfRangeException(null, value, "Bloom base must be normalized. (between 0 and 1)");

            ActualLevel.Bloom.Base = value;
            ActualLevel.Session.BloomBaseAdd = value - AreaData.Get(this).BloomBase;
        }
    }

    /// <summary>
    ///   Current room darkness. (between <c>0f</c> and <c>1f</c>)
    /// </summary>
    /// 
    /// <remarks>
    ///   By default dark rooms use an alpha of <c>0.75f</c>.
    /// </remarks>
    public float Darkness
    {
        get => ActualLevel.Lighting.Alpha;
        set
        {
            if (value < 0f || value > 1f)
                throw new ArgumentOutOfRangeException(null, value, "Darkness must be normalized. (between 0 and 1)");

            ActualLevel.Lighting.Alpha = value;
            ActualLevel.Session.LightingAlphaAdd = value - AreaData.Get(this).BloomBase;
        }
    }

    /// <summary>
    ///   Current camera offset.
    /// </summary>
    /// 
    /// <remarks>
    ///   Ususally set by a Camera Offset trigger.
    /// </remarks>
    public Vector2 CameraOffset
    {
        get => ActualLevel.CameraOffset;
        set => ActualLevel.CameraOffset = value;
    }

    /// <summary>
    ///   Current core mode.
    /// </summary>
    /// 
    /// <remarks>
    ///   Changing the core mode to <see cref="Session.CoreModes.Cold"/> sets the <c>"cold"</c> flag.<br/>
    ///   Changing it to something else removes it.
    /// </remarks>
    public Session.CoreModes CoreMode
    {
        get => ActualLevel.CoreMode;
        set => ActualLevel.CoreMode = value;
    }

    // TODO: music state property

    public readonly SharpEntityList Entities;

    /// <summary>
    ///   A collection of flags.
    /// </summary>
    /// 
    /// <remarks>
    ///   A flag is similar to a named switch which can be flipped on and off.
    /// </remarks>
    public readonly SharpFlags Flags;

    /// <summary>
    ///   Current state of wind.
    /// </summary>
    /// 
    /// <remarks>
    ///   Allows you to access and modify the current state of the wind.
    /// </remarks>
    public readonly SharpWind Wind;
    #endregion

    #region Methods
    /// <summary>
    ///   Shake the camera.
    /// </summary>
    /// 
    /// <param name="duration">
    ///   How long the shake should last.
    ///   (default <c>0.3</c> seconds)
    /// </param>
    /// <param name="direction">
    ///   The direction in which the camera should shake.
    ///   (default no specific direction)
    /// </param>
    public void ShakeCamera(
        TimeSpan? duration = null,
        Vector2? direction = null)
    {
        float durationSeconds = (float)(duration?.TotalSeconds ?? 0.3f);

        if (!direction.HasValue)
            ActualLevel.Shake(durationSeconds);
        else
            ActualLevel.DirectionalShake(direction.Value, durationSeconds);
    }

    /// <summary>
    ///   Change the current colorgrade.
    /// </summary>
    /// 
    /// <remarks>
    ///   A colorgrade lets you change how Celeste renders individual colors.<br/>
    ///   See <a href="https://github.com/EverestAPI/Resources/wiki/Map-Metadata#colorgrades">the Everest wiki (link)</a> for details.
    /// </remarks>
    /// 
    /// <param name="colorGradeName">
    ///   The name of the colorgrade.
    /// </param>
    /// <param name="instant">
    ///   Whether the change should be instant or gradual.
    ///   (default gradual)
    /// </param>
    public void ChangeColorGrade(
        string colorGradeName,
        bool instant = false)
    {
        if (instant)
            ActualLevel.SnapColorGrade(colorGradeName);
        else
            ActualLevel.NextColorGrade(colorGradeName);
    }

    /// <summary>
    ///   Complete the level.
    /// </summary>
    /// 
    /// <param name="useSpotlightWipe">
    ///   Whether to use the spotlight wipe or the fadeout wipe.
    ///   (default <see langword="true"/>)
    /// </param>
    /// <param name="skipScreenWipe">
    ///   Whether to skip the wipe.
    ///   (default <see langword="false"/>)
    /// </param>
    /// <param name="skipEndscreen">
    ///   Whether to skip the end screen.
    ///   (default <see langword="false"/>)
    /// </param>
    public void CompleteLevel(
        bool useSpotlightWipe = true,
        bool skipScreenWipe = false,
        bool skipEndscreen = false)
        => ActualLevel.CompleteArea(useSpotlightWipe, skipScreenWipe, skipEndscreen);

    /// <summary>
    ///   Give Madeline a key.
    /// </summary>
    /// 
    /// <returns>
    ///   The newly spawned key object.
    /// </returns>
    public Key GiveKey()
    {
        EntityID keyID = new(Parent.Session.ActualSession.Level, Calc.Random.Next());
        Key key = new(Parent.Player, keyID);
        ActualLevel.Add(key);
        Parent.Session.ActualSession.Keys.Add(keyID);
        return key;
    }

    /// <summary>
    ///   Play a given music track.
    /// </summary>
    /// 
    /// <param name="musicEvent">
    ///   The music event to play.
    /// </param>
    /// <param name="progress">
    ///   The music progress. Must be a non-negative integer.
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   <paramref name="musicEvent"/> is not a valid event.
    /// </exception>
    /// 
    /// <exception cref="ArgumentException">
    ///   <paramref name="progress"/> is not a non-negative integer.
    /// </exception>
    public void PlayMusic(string musicEvent, int? progress = null)
    {
        if (Audio.GetEventDescription(musicEvent) is null)
            throw new ArgumentException($"The music event {musicEvent} does not exist.");

        if (progress < 0)
            throw new ArgumentException($"Music progression must be non-negative.");

        AudioState audioState = Parent.Session.ActualSession.Audio;
        audioState.Music.Event = musicEvent;

        if (progress.HasValue)
            audioState.Music.Progress = progress.Value;

        audioState.Apply();
    }

    /// <summary>
    ///   Play a sound.
    /// </summary>
    /// 
    /// <param name="sound">
    ///   The sound ID to play.
    /// </param>
    /// <param name="position">
    ///   The position to play the sound at.
    ///   (defaults to the center of the screen)
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   Attempted to play no sound or a sound which doesn't exist.
    /// </exception>
    /// 
    /// <returns>
    ///   The sound event instance, allowing you to customize its volume, among other things.
    /// </returns>
    public EventInstance PlaySound(
        string sound,
        Vector2? position = null)
    {
        // fail fast if sound is empty or doesn't exist

        if (sound == "")
            throw new ArgumentException("Attempted to play no sound.");

        else if (Audio.GetEventDescription(sound) is null)
            throw new ArgumentException($"The sound ID {sound} does not exist.");

        if (!position.HasValue)
            return Audio.Play(sound);
        else
            return Audio.Play(sound, position.Value);
    }

    /// <summary>
    ///   Cause a rumble on the controller, if one is plugged in.
    /// </summary>
    /// 
    /// <remarks>
    ///   For specific strengths/lengths, see <see cref="RumbleSpecific(float, TimeSpan)"/>.
    /// </remarks>
    /// 
    /// <param name="strength">
    ///   A preset rumble strength.
    /// </param>
    /// <param name="length">
    ///   A preset rumble length.
    /// </param>
    public void Rumble(RumbleStrength strength, RumbleLength length)
        => Input.Rumble(strength, length);

    /// <summary>
    ///   Cause a rumble on the controller, if one is plugged in.<br/>
    /// </summary>
    /// 
    /// <remarks>
    ///   For preset strengths/lengths, see <see cref="Rumble(RumbleStrength, RumbleLength)"/>
    /// </remarks>
    /// 
    /// <param name="strength">
    ///   A normalized rumble strength.
    ///   (between <c>0</c> and <c>1</c>)
    /// </param>
    /// <param name="length">
    ///   The rumble duration.
    /// </param>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The rumble strengths is not normalized.
    /// </exception>
    public void RumbleSpecific(float strength, TimeSpan length)
    {
        if (strength < 0 || strength > 1)
            throw new ArgumentOutOfRangeException(null, strength, "Rumble strength must be normalized. (between 0 and 1)");

        Input.RumbleSpecific(strength, (float)length.TotalSeconds);
    }

    /// <summary>
    ///   Open a dialog textbox.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The Dialog ID to use.
    /// </param>
    /// <param name="events">
    ///   Coroutines to run when hitting a <c>{trigger n ...}</c> in the dialog, where <c>n</c> is a number.
    ///   (optional)
    /// </param>
    /// 
    /// <returns>
    ///   A coroutine which displays the textbox and waits until it's closed.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator Say(string dialogId, params Func<IEnumerator>[] events)
        => Textbox.Say(dialogId, events);

    public IEnumerator Say(TemporaryDialogId dialogId, params Func<IEnumerator>[] events)
    {
        using (dialogId)
        {
            IEnumerator textboxCoroutine = Textbox.Say(dialogId, events);
            while (textboxCoroutine.MoveNext())
                yield return textboxCoroutine.Current;
        }
    }

    /// <summary>
    ///   Open a mini dialog textbox.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The Dialog ID to use.
    /// </param>
    public void SayMini(string dialogId)
        => ActualLevel.Add(new MiniTextbox(dialogId));

    /// <summary>
    ///   Change the player's spawn point.
    /// </summary>
    /// 
    /// <remarks>
    ///   This method works by picking the closest existing spawnpoint entity to <paramref name="spawnNear"/>.
    /// </remarks>
    /// 
    /// <param name="spawnNear">
    ///   The position from which the nearest spawn should be determined.
    ///   (defaults to the player's position)
    /// </param>
    public void SetSpawnPoint(Vector2? spawnNear = null)
    {
        Parent.Session.ActualSession.HitCheckpoint = true;
        Parent.Session.ActualSession.RespawnPoint = ActualLevel.GetSpawnPoint(spawnNear ?? Parent.Player.Position);
    }

    /// <summary>
    ///   Show a postcard.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The Dialog ID which has the postcard's contents.
    /// </param>
    /// 
    /// <returns>
    ///   A coroutine which shows the postcard and waits until it is closed.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator ShowPostcard(string dialogId)
    {
        string content = Dialog.Get(dialogId);

        Postcard postcard = new(content);
        ActualLevel.Add(postcard);
        postcard.BeforeRender();

        return postcard.DisplayRoutine();
    }

    /// <summary>
    ///   Show a postcard, using preset fade in/out sounds.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The Dialog ID which has the postcard's contents.
    /// </param>
    /// <param name="sounds">
    ///   Postcard sounds to use when fading in/out.
    /// </param>
    /// 
    /// <returns>
    ///   A coroutine which shows the postcard and waits until it is closed.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator ShowPostcard(
        string dialogId,
        SharpPostcardSounds sounds)
    {
        // no need to validate, those are vanilla
        // ... unless you're sneaking in here round back by using reflection.
        // not my fault if you break something :snip_pog:

        string content = Dialog.Get(dialogId);

        Postcard postcard = new(content, sounds.FadeInSound, sounds.FadeOutSound);
        ActualLevel.Add(postcard);
        postcard.BeforeRender();

        return postcard.DisplayRoutine();
    }

    /// <summary>
    ///   Show a postcard with given fade in/out sounds.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The Dialog ID which has the postcard's contents.
    /// </param>
    /// <param name="fadeInSound">
    ///   The fade-in sound ID, or <see langword="null"/> for no sound.
    /// </param>
    /// <param name="fadeOutSound">
    ///   The fade-out sound ID, or <see langword="null"/> for no sound.
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   Attempted to play a sound which doesn't exist.
    /// </exception>
    /// 
    /// <returns>
    ///   A coroutine which shows the postcard and waits until it is closed.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator ShowPostcard(
        string dialogId,
        string? fadeInSound,
        string? fadeOutSound)
    {
        // fail fast if the sound id doesn't exist, instead of continuing with wrong behavior

        if (!string.IsNullOrEmpty(fadeInSound) && Audio.GetEventDescription(fadeInSound) is null)
            throw new ArgumentException($"The sound ID {fadeInSound} does not exist.", nameof(fadeInSound));

        if (!string.IsNullOrEmpty(fadeOutSound) && Audio.GetEventDescription(fadeOutSound) is null)
            throw new ArgumentException($"The sound ID {fadeOutSound} does not exist.", nameof(fadeOutSound));

        string content = Dialog.Get(dialogId);

        Postcard postcard = new(content, fadeInSound, fadeOutSound);
        ActualLevel.Add(postcard);
        postcard.BeforeRender();

        return postcard.DisplayRoutine();
    }

    /// <summary>
    ///   Spawn a lightning strike.
    /// </summary>
    /// 
    /// <param name="position">
    ///   The position of the lightning strike.
    /// </param>
    /// <param name="height">
    ///   The height of the lightning strike.
    /// </param>
    /// <param name="delay">
    ///   How long to wait for before the lightning strike actually hits.
    /// </param>
    /// <param name="seed">
    ///   A seed determining the look of the lightning strike.
    ///   (determined by <paramref name="position"/> by default)
    /// </param>
    public void SpawnLightning(
        Vector2 position,
        float height,
        TimeSpan delay,
        int? seed = null)
    {
        ActualLevel.Add(new LightningStrike(position, seed ?? position.GetHashCode(), height, (float)delay.TotalSeconds));
    }

    /// <summary>
    ///   Spawn one or more Badeline chasers.
    /// </summary>
    /// 
    /// <param name="isRelative">
    ///   Whether <paramref name="position"/> is relative to Madeline.
    /// </param>
    /// <param name="position">
    ///   The position at which the chaser(s) should spawn.
    /// </param>
    /// <param name="chaserCount">
    ///   The amount of chasers to spawn.
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   <paramref name="chaserCount"/> is not a positive integer.
    /// </exception>
    /// 
    /// <returns>
    ///   A list of <see cref="BadelineOldsite"/> (Badeline chasers) which were just spawned.
    /// </returns>
    public IEnumerable<BadelineOldsite> SpawnBadelineChaser(bool isRelative, Vector2 position, int chaserCount = 1)
    {
        if (chaserCount < 1)
            throw new ArgumentException($"Chaser count must be a positive integer.");

        if (isRelative)
            position += Parent.Player.Position;

        List<BadelineOldsite> chasers = [];
        for (int i = 0; i < chaserCount; i++)
        {
            BadelineOldsite chaser = new(position, i);
            chasers.Add(chaser);
            ActualLevel.Add(chaser);
        }
        return chasers;
    }

    /// <summary>
    ///   Spawn one or more Badeline chasers.
    /// </summary>
    /// 
    /// <param name="isRelative">
    ///   Whether <paramref name="positions"/> is relative to Madeline.
    /// </param>
    /// <param name="positions">
    ///   The positions at which the Badeline chasers should spawn.
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   <paramref name="positions"/> is an empty list.
    /// </exception>
    /// 
    /// <returns>
    ///   A list of <see cref="BadelineOldsite"/> (Badeline chasers) which were just spawned.
    /// </returns>
    public IEnumerable<BadelineOldsite> SpawnBadelineChasers(bool isRelative, Vector2[] positions)
    {
        if (positions.Length == 0)
            throw new ArgumentException($"No positions to spawn the Badeline chasers at. Did you forget to put them in?");

        List<BadelineOldsite> chasers = [];
        for (int i = 0; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            if (isRelative)
                position += Parent.Player.Position;

            BadelineOldsite chaser = new(position, i);
            chasers.Add(chaser);
            ActualLevel.Add(chaser);
        }
        return chasers;
    }
    #endregion
}
