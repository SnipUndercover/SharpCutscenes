using Celeste.Mod.SharpCutscenes.Abstractions;
using Celeste.Mod.SharpCutscenes.Enums;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.SharpCutscenes;

/// <summary>
///   The base class all cutscenes should extend from.
/// </summary>
public abstract class SharpCutscene : Entity
{
    // TODO: C h o i c e D i a l o g
    // TODO: add logging

    #region Fields & Properties
    /// <summary>
    ///   This is Madeline. Say hi!
    ///   Here you can access and change her properties.
    /// </summary>
    public SharpPlayer Player { get; private set; }
    private Player _Player;

    /// <summary>
    ///   This is the room Madeline's currently in.<br/>
    ///   Here you can access and change its properties.
    /// </summary>
    public SharpLevel Level { get; private set; }
    private Level _Level;

    /// <summary>
    ///   This is Madeline's current playthrough.<br/>
    ///   Here you can access its information and statistics.
    /// </summary>
    /// 
    /// <remarks>
    ///   A session starts when you enter a map, and ends when you exit or restart. (except for Save and Quit)
    /// </remarks>
    public SharpSession Session { get; private set; }
    private Session _Session;

    /// <summary>
    ///   This is Madeline's save data.<br/>
    ///   Here you can access its information and statistics.
    /// </summary>
    public SharpSaveData SaveData { get; private set; }
    private SaveData _SaveData;

    /// <summary>
    ///   The unique Event ID of this cutscene.
    /// </summary>
    /// 
    /// <remarks>
    ///   This will be equal to whatever ID you put in the <see cref="Entities.CustomEventAttribute"/>.
    /// </remarks>
    public readonly string EventID;

    /// <summary>
    ///   Whether the cutscene is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    ///   Whether the cutscene was skipped.
    /// </summary>
    public bool WasSkipped { get; private set; }
    #endregion

    #region Implementation Details
    // based on Celeste.CutsceneEntity
    public SharpCutscene(EventTrigger trigger, Player player, string eventID)
    {
        // grr so tempted to yoink player.level / trigger.SceneAs<Level>() here
        // but i probably shouldn't
        EventID = eventID;

        _Player = player;
        Player = new(this, _Player);

        _SaveData = global::Celeste.SaveData.Instance;
        SaveData = new(this, _SaveData);

        // we will set you in Added
        _Level = default!;
        Level = default!;

        _Session = default!;
        Session = default!;

        // keep us around even on room transitions to prevent the cutscene getting unloaded
        Tag = Tags.Global;
    }

    private bool WasSkippable;

    public sealed override void Added(Scene scene)
    {
        base.Added(scene);

        _Level = (Level)Scene;
        Level = new(this, _Level);

        _Session = _Level.Session;
        Session = new(this, _Session);

        // bail out if conditions unmet
        if (!BeginCondition)
        {
            RemoveSelf();
            return;
        }

        InternalOnBegin();
    }

    private void InternalOnBegin()
    {
        IsRunning = true;

        // copy value in case it changes later
        WasSkippable = CanSkip;

        if (WasSkippable)
            _Level.StartCutscene(SkipCutscene, FadeInOnSkip, CompleteLevelOnEnd, resetZoomOnSkip: true);

        try
        {
            OnBegin();
        }
        catch (Exception e)
        {
            throw new BrokenCutsceneException(EventID, e);
        }

        Add(new Coroutine(InternalCutscene()));
    }

    private IEnumerator InternalCutscene()
    {
        // cannot yield return inside a try/catch block
        // so we will cheat a liiiiittle bit...

        IEnumerator enumerator = Cutscene();
        while (true)
        {
            try
            {
                if (!enumerator.MoveNext())
                    break;
            }
            catch (Exception e)
            {
                // the crash handler will throw the current scene away,
                // so there's no point to _Level.EndCutscene()
                throw new BrokenCutsceneException(EventID, e);
            }
            yield return enumerator.Current;
        }

        InternalOnEnd();
    }

    private void InternalOnEnd()
    {
        IsRunning = false;
        if (WasSkippable)
            _Level.EndCutscene();

        try
        {
            OnEnd();
        }
        catch (Exception e)
        {
            throw new BrokenCutsceneException(EventID, e);
        }
        RemoveSelf();
    }

    private void SkipCutscene(Level level)
    {
        WasSkipped = true;
        InternalOnEnd();
    }
    #endregion

    #region Public Interface
    /// <summary>
    ///   The condition(s) that needs to be met for the cutscene to run.
    /// </summary>
    public virtual bool BeginCondition => true;

    /// <summary>
    ///   Whether the cutscene can be skipped.
    ///   (default <see langword="true"/>)
    /// </summary>
    public virtual bool CanSkip => true;

    /// <summary>
    ///   Whether the cutscene should do a fade in when skipped.
    /// </summary>
    public virtual bool FadeInOnSkip => true;

    /// <summary>
    ///   Whether the cutscene should complete the level after it finishes.
    /// </summary>
    public virtual bool CompleteLevelOnEnd => false;

    /// <summary>
    ///   Setup code.<br/>
    ///   This is where you would do stuff <i>before</i> the cutscene, if needed.
    /// </summary>
    /// 
    /// <remarks>
    ///   By default, it disables movement during the cutscene.<br/>
    ///   Remember to insert <c>base.OnBegin()</c> if you want to keep that functionality!
    /// </remarks>
    public virtual void OnBegin()
    {
        Player.State = SharpPlayerState.StDummy;
    }

    //  visual studio refuses to render this properly in the overriden method unless i insert non-breaking space HTML entities.
    //  but not in the conventional way, no, because for some reason &nbsp; causes the entire doc to stop rendering.
    //  EXCEPT &#160; WORKS?????? WHAT THE ACTUAL HELL. i sincerely apologize to anyone who sees these.

    /// <summary>
    ///   Cutscene logic.
    /// </summary>
    /// 
    /// <remarks>
    ///   Cutscenes are <b>coroutines</b>.
    ///   Coroutines differ from normal functions by their ability to be <b>paused</b>.<br/>
    ///   A coroutine is a method that returns an <see cref="IEnumerator"/>.<br/>
    ///   <br/>
    ///   To pause a coroutine, use <see langword="yield"/>&#160;<see langword="return"/>.
    ///   The expression after <see langword="yield"/>&#160;<see langword="return"/> determines how long to wait for.
    ///   <list type="bullet">
    ///     <item>
    ///       <see langword="yield"/>&#160;<see langword="return"/>&#160;<c><see langword="null"/></c> pauses the coroutine for one frame.
    ///     </item>
    ///     <item>
    ///       <see langword="yield"/>&#160;<see langword="return"/>&#160;<c>1.5f</c> pauses the coroutine for <c>1.5</c> seconds.
    ///     </item>
    ///     <item>
    ///       <see langword="yield"/>&#160;<see langword="return"/>&#160;<c>AnotherCoroutine()</c> pauses this coroutine
    ///       and waits for <c>AnotherCoroutine()</c> to finish.
    ///     </item>
    ///   </list>
    /// </remarks>
    public abstract IEnumerator Cutscene();

    /// <summary>
    ///   Teardown code.<br/>
    ///   This is where you would do stuff <i>after</i> the cutscene, if needed.<br/>
    ///   Check whether the cutscene was skipped by looking at <see cref="WasSkipped"/>.
    /// </summary>
    /// 
    /// <remarks>
    ///   By default, it re-enables movement after the cutscene.<br/>
    ///   Remember to insert <c>base.OnEnd()</c> if you want to keep that functionality!
    /// </remarks>
    public virtual void OnEnd()
    {
        Player.State = SharpPlayerState.StNormal;
    }
    #endregion

    #region Hide inherited methods
    public sealed override void Awake(Scene scene)
        => base.Awake(scene);

    public sealed override void DebugRender(Camera camera)
        => base.DebugRender(camera);

    public sealed override void HandleGraphicsCreate()
        => base.HandleGraphicsCreate();

    public sealed override void HandleGraphicsReset()
        => base.HandleGraphicsReset();

    public sealed override void Removed(Scene scene)
        => base.Removed(scene);

    public sealed override void Render()
        => base.Render();

    public sealed override void SceneBegin(Scene scene)
        => base.SceneBegin(scene);

    public sealed override void SceneEnd(Scene scene)
        => base.SceneEnd(scene);

    public sealed override void Update()
    {
        if (!Player.ActualPlayer.Dead && _Level.RetryPlayerCorpse is null)
            base.Update();
        else
            Active = false;
    }

    public sealed override bool Equals(object? obj)
        => base.Equals(obj);

    public sealed override int GetHashCode()
        => base.GetHashCode();

    public sealed override string ToString()
        => $"{nameof(SharpCutscene)}:{EventID}";
    #endregion

    #region Helpers
    /// <summary>
    ///   Log a message to <c>log.txt</c>.
    /// </summary>
    /// 
    /// <param name="level">
    ///   The severity of the message.
    /// </param>
    /// <param name="msg">
    ///   The message to log.<br/>
    ///   Log levels lower than <see cref="LogLevel.Info"/> won't be visible by default.
    /// </param>
    public void Log(LogLevel level, string msg)
        => Logger.Log(level, ToString(), msg);

    /// <summary>
    /// Log a message to <c>log.txt</c> with severity <see cref="LogLevel.Info"/>.
    /// </summary>
    /// <param name="msg">The message to log.</param>
    public void Log(string msg)
        => Log(LogLevel.Info, msg);
    #endregion
}
