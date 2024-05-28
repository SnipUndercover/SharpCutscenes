using Celeste.Mod.SharpCutscenes.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.SharpCutscenes.Abstractions;

/// <summary>
///   This is Madeline! Or rather, a simplified version of her.<br/>
///   This is a wrapper for the true <see cref="Player"/> used across Celeste,
///   which is full of documentation and helper methods commonly used in cutscenes.
/// </summary>
/// 
/// <remarks>
///   If at any point you need to access the true <see cref="Player"/> instance, simply cast it to <see cref="Player"/>.
/// </remarks>
public sealed class SharpPlayer
{
    // wait, it's all done? no more TODOs?
    // impossible...

    #region Implementation Details
    internal SharpPlayer(SharpCutscene parent, Player actualPlayer)
    {
        Parent = parent;
        ActualPlayer = actualPlayer;
    }

    private readonly SharpCutscene Parent;
    internal readonly Player ActualPlayer;

    // adopted from Leader.StoreStrawberries/RestoreStrawberries to work with any follower
    internal List<(Follower, Vector2)> StoreFollowers()
    {
        List<(Follower, Vector2)> followersAndOffsets
            = ActualPlayer.Leader.Followers.Select(follower => (follower, follower.Entity.Position - Position)).ToList();

        foreach ((Follower follower, _) in followersAndOffsets)
        {
            ActualPlayer.Leader.Followers.Remove(follower);
            follower.Leader = null;
            follower.Entity.AddTag(Tags.Global);
        }

        return followersAndOffsets;
    }

    internal void RestoreFollowers(List<(Follower, Vector2)> followersAndOffsets)
    {
        ActualPlayer.Leader.PastPoints.Clear();
        foreach ((Follower follower, Vector2 offset) in followersAndOffsets)
        {
            ActualPlayer.Leader.GainFollower(follower);
            follower.Entity.Position = Position + offset;
            follower.Entity.RemoveTag(Tags.Global);
        }
    }

    /// <summary>
    ///   Convert the simple <see cref="SharpPlayer"/> into the actual <see cref="Player"/> used across Celeste.
    /// </summary>
    /// 
    /// <param name="level">
    ///   The <see cref="SharpPlayer"/> to convert.
    /// </param>
    public static implicit operator Player(SharpPlayer player)
        => player.ActualPlayer;
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   Whether Madeline is allowed to move.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is done by checking whether her state is <see cref="SharpPlayerState.StNormal"/>.<br/>
    ///   Allowing/disallowing movement sets Madeline's state to <see cref="SharpPlayerState.StNormal"/> and <see cref="SharpPlayerState.StDummy"/> respectively.<br/>
    ///   If you need more fine-grained control over states, check out <see cref="State"/>.
    /// </remarks>
    public bool CanMove
    {
        get => State == SharpPlayerState.StNormal;
        set => State = value
            ? SharpPlayerState.StNormal
            : SharpPlayerState.StDummy;
    }

    /// <summary>
    ///   Whether Madeline is allowed to pause the game.
    /// </summary>
    public bool CanPause
    {
        get => !Parent.Level.ActualLevel.PauseLock;
        set => Parent.Level.ActualLevel.PauseLock = !value;
    }

    /// <summary>
    ///   Whether Madeline is allowed to retry.
    /// </summary>
    public bool CanRetry
    {
        get => Parent.Level.ActualLevel.CanRetry;
        set => Parent.Level.ActualLevel.CanRetry = value;
    }

    /// <summary>
    ///   Madeline's current dash count.
    /// </summary>
    public int CurrentDashes
    {
        get => ActualPlayer.Dashes;
        set => ActualPlayer.Dashes = value;
    }

    /// <summary>
    ///   Madeline's max dash count.
    /// </summary>
    /// 
    /// <remarks>
    ///   Her max dash count is stored in her <see cref="PlayerInventory"/>.
    /// </remarks>
    public int MaxDashes
    {
        get => ActualPlayer.MaxDashes;
        set => Parent.Session.ActualSession.Inventory.Dashes = value;
    }

    /// <summary>
    ///   Madeline's current inventory.
    /// </summary>
    /// 
    /// <remarks>
    ///   Her inventory determines: 
    ///   <list type="bullet">
    ///     <item>her max dash count (<see cref="PlayerInventory.Dashes"/>)</item>
    ///     <item>whether she has a backpack (<see cref="PlayerInventory.Backpack"/>)</item>
    ///     <item>whether dream blocks are activated (<see cref="PlayerInventory.DreamDash"/>)</item>
    ///     <item>whether her dashes refill when grounded (<see cref="PlayerInventory.NoRefills"/>)</item>
    ///   </list>
    /// </remarks>
    public PlayerInventory Inventory
    {
        get => ActualPlayer.Inventory;
        set => Parent.Session.ActualSession.Inventory = value;
    }

    /// <summary>
    ///   Madeline's current position.
    /// </summary>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to set Madeline's position outside the current room.
    /// </exception>
    public Vector2 Position
    {
        get => ActualPlayer.Position;
        set
        {
            LevelData targetRoom = Parent.Session.ActualSession.MapData.GetAt(value)
                ?? throw new InvalidOperationException("Attempted to set Madeline's position outside the level bounds.");

            if (targetRoom.Name != Parent.Session.ActualSession.Level)
                throw new InvalidOperationException(
                    $"Attempted to set Madeline's position outside the current room. Please use {nameof(SharpCutscene.Player)}.{nameof(ChangeRoom)} if this is intended.");

            ActualPlayer.Position = value;
        }
    }

    /// <summary>
    ///   Madeline's current state.
    /// </summary>
    /// 
    /// <remarks>
    ///   Madeline can be in one state at any given time, and all vanilla states can be seen in <see cref="SharpPlayerState"/>.
    /// </remarks>
    public SharpPlayerState State
    {
        get => (SharpPlayerState)ActualPlayer.StateMachine.State;
        set => ActualPlayer.StateMachine.State = (int)value;
    }

    /// <summary>
    ///   Whether Madeline's current state is locked.
    /// </summary>
    /// 
    /// <remarks>
    ///   When <see langword="true"/>, state changes have no effect.
    /// </remarks>
    public bool StateLocked
    {
        get => ActualPlayer.StateMachine.Locked;
        set => ActualPlayer.StateMachine.Locked = value;
    }
    #endregion

    #region Methods
    /// <summary>
    ///   Put Madeline in a bubble and move her by <paramref name="offset"/>, as if she just collected a cassette.
    /// </summary>
    /// 
    /// <remarks>
    ///   The movement follows a quadratic Bézier curve with a sine in/out ease.<br/>
    ///   <i>This method uses relative positions.</i>
    /// </remarks>
    /// 
    /// <param name="offset">
    ///   How much the bubble should move Madeline.
    /// </param>
    /// <param name="control">
    ///   A control point, adjusting the look of the quadratic Bézier curve.<br/>
    ///   If unspecified, the curve will be linear.
    /// </param>
    public void CassetteFly(Vector2 offset, Vector2? control = null)
    {
        // make the sound play from the camera's center
        Audio.Play(SFX.game_gen_cassette_bubblereturn, Parent.Level.ActualLevel.Camera.Position + new Vector2(Celeste.GameWidth / 2, Celeste.GameHeight / 2));
        ActualPlayer.StartCassetteFly(offset, control ?? (Position + offset) / 2);
    }

    /// <summary>
    ///   Put Madeline in a bubble and move her to <paramref name="target"/>, as if she just collected a cassette.
    /// </summary>
    /// 
    /// <remarks>
    ///   The movement follows a quadratic Bézier curve with a sine in/out ease.<br/>
    ///   <i>This method uses absolute positions.</i>
    /// </remarks>
    /// 
    /// <param name="target">
    ///   The absolute position Madeline should be bubbled to.
    /// </param>
    /// <param name="control">
    ///   A control point, adjusting the look of the quadratic Bézier curve.<br/>
    ///   If unspecified, the curve will be linear.
    /// </param>
    public void CassetteFlyTo(Vector2 target, Vector2? control = null)
    {
        // make the sound play from the camera's center
        Audio.Play(SFX.game_gen_cassette_bubblereturn, Parent.Level.ActualLevel.Camera.Position + new Vector2(Celeste.GameWidth / 2, Celeste.GameHeight / 2));
        ActualPlayer.StartCassetteFly(Position + target, Position + (control ?? (Position + target) / 2));
    }

    /// <summary>
    ///   Change the room Madeline's in.
    /// </summary>
    /// 
    /// <param name="position">
    ///   The absolute position Madeline should be teleported to.
    /// </param>
    /// <param name="introType">
    ///   The intro type to use.
    ///   (default <see cref="SharpIntroTypes.Respawn"/>)
    /// </param>
    /// <param name="spawnNear">
    ///   The position near which Madeline should respawn.
    ///   (default <paramref name="position"/>)
    /// </param>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to teleport outside the level bounds.
    /// </exception>
    public void ChangeRoom(
        Vector2 position,
        SharpIntroTypes introType = SharpIntroTypes.Respawn,
        Vector2? spawnNear = null)
    {
        Level actualLevel = Parent.Level;

        LevelData roomData = Parent.Session.ActualSession.MapData.GetAt(position)
            ?? throw new InvalidOperationException("Attempted to teleport outside the level bounds.");

        // changing rooms mid-update is bad
        // add an event on frame end and change rooms there
        actualLevel.OnEndOfFrame += () => {
            actualLevel.TeleportTo(this, roomData.Name, (Player.IntroTypes)introType, spawnNear ?? position);

            ActualPlayer.Position = position;
        };
    }

    /// <summary>
    ///   Change the room Madeline's in.  
    /// </summary>
    /// 
    /// <param name="roomName">
    ///   The room to teleport to.
    /// </param>
    /// <param name="introType">
    ///   The intro type to use.
    ///   (default <see cref="SharpIntroTypes.Respawn"/>)
    /// </param>
    /// <param name="spawnNear">
    ///   The position near which Madeline should respawn.
    ///   (default <see cref="Vector2.Zero"/>)
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   Attempted to teleport to a room which does not exist.
    /// </exception>
    public void ChangeRoom(
        string roomName,
        SharpIntroTypes introType = SharpIntroTypes.Respawn,
        Vector2? spawnNear = null)
    {
        Level actualLevel = Parent.Level;

        LevelData roomData = Parent.Session.ActualSession.MapData.Get(roomName)
            ?? throw new ArgumentException($"Room \"{roomName}\" does not exist.");

        // changing rooms mid-update is bad
        // add an event on frame end and change rooms there
        actualLevel.OnEndOfFrame += () => {
            actualLevel.TeleportTo(this, roomData.Name, (Player.IntroTypes)introType, spawnNear);
        };
    }

    /// <summary>
    ///   Instantly swap the room Madeline's in.
    /// </summary>
    /// 
    /// <remarks>
    ///   This method behaves identically to the seamless teleport in the very first room of Farewell.
    /// </remarks>
    /// 
    /// <param name="position">
    ///   The absolute position Madeline should be teleported to.
    /// </param>
    /// <param name="spawnNear">
    ///   The position near which Madeline should respawn.
    ///   (default <see cref="Vector2.Zero"/>)
    /// </param>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to teleport outside the level bounds.
    /// </exception>
    public void ChangeRoomInstantly(
        Vector2 position,
        Vector2? spawnNear = null)
    {
        // adopted from LuaCutscenes' InstantTeleport

        Session actualSession = Parent.Session;
        Level actualLevel = Parent.Level;

        LevelData roomData = actualSession.MapData.GetAt(position)
            ?? throw new InvalidOperationException("Attempted to teleport outside the level bounds.");

        // changing rooms mid-update is bad
        // add an event on frame end and change rooms there
        actualLevel.OnEndOfFrame += () =>
        {
            Vector2 previousLevelOffset = actualLevel.LevelOffset;
            Vector2 previousPlayerOffset = Position - actualLevel.LevelOffset;
            Vector2 previousCameraOffset = actualLevel.Camera.Position - actualLevel.LevelOffset;
            Facings previousFacing = ActualPlayer.Facing;

            // we need to teleport the player's followers
            List<(Follower, Vector2)> followersAndOffsets = StoreFollowers();

            actualLevel.Remove(this);
            actualLevel.UnloadLevel();

            actualSession.Level = roomData.Name;
            actualSession.RespawnPoint = spawnNear ?? position;
            actualSession.FirstLevel = false;

            // add the player *before* loading the room
            // this fixes OutbackHelper portals and other entities
            // which expect the player to be in the scene during Awake
            actualLevel.Add(this);

            Vector2 playerRelativeOffset = position - actualLevel.LevelOffset - previousPlayerOffset;

            ActualPlayer.Position = position;
            ActualPlayer.Facing = previousFacing;
            ActualPlayer.Hair.MoveHairBy(actualLevel.LevelOffset - previousLevelOffset + playerRelativeOffset);

            RestoreFollowers(followersAndOffsets);

            actualLevel.LoadLevel(Player.IntroTypes.Transition);

            actualLevel.Camera.Position = actualLevel.LevelOffset + previousCameraOffset + playerRelativeOffset;
            actualLevel.Wipe?.Cancel();
        };
    }

    /// <summary>
    ///   Instantly swap the room Madeline's in.
    /// </summary>
    /// 
    /// <remarks>
    ///   This method behaves identically to the seamless teleport in the very first room of Farewell.
    /// </remarks>
    /// 
    /// <param name="roomName">
    ///   The room to teleport to.
    /// </param>
    /// <param name="spawnNear">
    ///   The position near which Madeline should respawn.
    ///   (default <see cref="Vector2.Zero"/>)
    /// </param>
    /// 
    /// <exception cref="ArgumentException">
    ///   Attempted to teleport to a room which does not exist.
    /// </exception>
    public void ChangeRoomInstantly(
        string roomName,
        Vector2? spawnNear = null)
    {
        // adopted from LuaCutscenes' InstantTeleport

        Session actualSession = Parent.Session;
        Level actualLevel = Parent.Level;

        LevelData roomData = actualSession.MapData.Get(roomName)
            ?? throw new ArgumentException($"Room \"{roomName}\" does not exist.");

        // changing rooms mid-update is bad
        // add an event on frame end and change rooms there
        actualLevel.OnEndOfFrame += () =>
        {
            Vector2 previousLevelOffset = actualLevel.LevelOffset;
            Vector2 previousPlayerOffset = Position - actualLevel.LevelOffset;
            Vector2 previousCameraOffset = actualLevel.Camera.Position - actualLevel.LevelOffset;
            Facings previousFacing = ActualPlayer.Facing;

            // we need to teleport the player's followers
            List<(Follower, Vector2)> followersAndOffsets = StoreFollowers();

            actualLevel.Remove(this);
            actualLevel.UnloadLevel();

            actualSession.Level = roomData.Name;
            actualSession.RespawnPoint = spawnNear;
            actualSession.FirstLevel = false;

            // add the player *before* loading the room
            // this fixes OutbackHelper portals and other entities
            // which expect the player to be in the scene during Awake
            actualLevel.Add(this);

            ActualPlayer.Position = actualLevel.LevelOffset + previousPlayerOffset;
            ActualPlayer.Facing = previousFacing;
            ActualPlayer.Hair.MoveHairBy(actualLevel.LevelOffset - previousLevelOffset);

            RestoreFollowers(followersAndOffsets);

            actualLevel.LoadLevel(Player.IntroTypes.Transition);

            actualLevel.Camera.Position = actualLevel.LevelOffset + previousCameraOffset;
            actualLevel.Wipe?.Cancel();
        };
    }

    /// <summary>
    ///   Cease Madeline's existence.
    /// </summary>
    /// 
    /// <param name="direction">
    ///   The direction Madeline will recoil towards, relative to her, or <see cref="Vector2.Zero"/> to die in-place.
    ///   (default <see cref="Vector2.Zero"/>)
    /// </param>
    /// <param name="evenIfInvincible">
    ///   Whether to ignore the <i>Invincibility</i> assist/variant.
    ///   (default <see langword="false"/>)
    /// </param>
    /// <param name="registerDeathInStats">
    ///   Whether the death should register in stats.
    ///   (default <see langword="false"/>)<br/>
    ///   An example of such a death is the one in the big mirror cutscene in Mirror Temple A-Side.
    /// </param>
    public void Die(
        Vector2 direction = default,
        bool evenIfInvincible = false,
        bool registerDeathInStats = true)
    {
        if (ActualPlayer != null && !ActualPlayer.Dead)
            ActualPlayer.Die(direction, evenIfInvincible, registerDeathInStats);
    }

    /// <summary>
    ///   Drop the currently picked up holdable directly down.
    /// </summary>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to drop a holdable without holding one.
    /// </exception>
    public void Drop()
    {
        if (ActualPlayer.Holding == null)
            throw new InvalidOperationException("Attempted to drop a holdable without holding one.");

        ActualPlayer.Drop();
    }

    /// <summary>
    ///   Make Madeline jump for a given duration.
    /// </summary>
    /// 
    /// <param name="duration">
    ///   How long should the jump button should be held.
    ///   (default <c>2</c> seconds)
    /// </param>
    public void Jump(TimeSpan? duration = null)
    {
        ActualPlayer.Jump();
        ActualPlayer.AutoJump = true;
        ActualPlayer.AutoJumpTimer = (float)(duration?.TotalSeconds ?? 2f);
    }

    /// <summary>
    ///   Pick up a nearby holdable, like a Theo Crystal or a jelly.
    /// </summary>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to pick up a holdable while already holding one.
    /// </exception>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline is done picking a holdable up, if any.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator PickUp()
    {
        if (ActualPlayer.Holding != null)
            throw new InvalidOperationException("Attempted to pick up a holdable while already holding one.");

        // GetComponents<T>() returns List<Component> instead of List<T> :catplush:
        foreach (Holdable hold in Parent.Level.ActualLevel.Tracker.GetComponents<Holdable>().Cast<Holdable>())
        {
            if (hold.Check(ActualPlayer) && ActualPlayer.Pickup(hold))
            {
                yield return WaitUntilState(SharpPlayerState.StNormal);
                yield break;
            }
        }
    }

    /// <summary>
    ///   Make Madeline run a certain distance in a given direction.
    /// </summary>
    /// 
    /// <remarks>
    ///   <i>This method uses relative positions.</i>
    /// </remarks>
    /// 
    /// <param name="direction">
    ///   Direction in which Madeline should run.
    /// </param>
    /// <param name="amount">
    ///   How far Madeline should run.
    /// </param>
    /// <param name="unit">
    ///   The unit <paramref name="amount"/> is in.
    /// </param>
    /// <param name="useFastAnimation">
    ///   Whether Madeline should use the <c>runFast</c> animation instead of <c>runSlow</c>.
    ///   (default <see langword="false"/>)
    /// </param>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to run a negative distance.
    /// </exception>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline is done running.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator Run(
        SharpHorizontalDirection direction,
        float amount,
        SharpMapUnit unit,

        bool useFastAnimation = false)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(null, "Attempted to run a negative distance.");

        float actualAmount = amount * (int)direction * (int)unit;
        return ActualPlayer.DummyRunTo(Position.X + actualAmount, useFastAnimation);
    }

    /// <summary>
    ///   Make Madeline run to a given X position.
    /// </summary>
    /// 
    /// <remarks>
    ///   <i>This method uses absolute positions.</i>
    /// </remarks>
    /// 
    /// <param name="targetX">
    ///   The X position Madeline should run towards.
    /// </param>
    /// <param name="useFastAnimation">
    ///   Whether Madeline should use the <c>runFast</c> animation instead of <c>runSlow</c>.
    ///   (default <see langword="false"/>)
    /// </param>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline is done running.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator RunTo(
        float targetX,

        bool useFastAnimation = false)
        => ActualPlayer.DummyRunTo(targetX, useFastAnimation);

    /// <summary>
    ///   Teleport Madeline and snap the camera to her new position.
    /// </summary>
    /// 
    /// <param name="position">
    ///   The position to teleport to.
    /// </param>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to teleport outside the level bounds.
    /// </exception>
    public void Teleport(Vector2 position)
    {
        LevelData targetRoom = Parent.Session.ActualSession.MapData.GetAt(position)
            ?? throw new InvalidOperationException("Attempted to teleport outside the level bounds.");

        if (targetRoom.Name != Parent.Session.ActualSession.Level)
            throw new InvalidOperationException(
                $"Attempted to teleport outside the current room. Please use {nameof(SharpCutscene.Player)}.{nameof(ChangeRoomInstantly)} if this is intended.");

        Vector2 moveBy = position - Position;

        Position = position;
        Parent.Level.ActualLevel.Camera.Position += moveBy;
        ActualPlayer.Hair.MoveHairBy(moveBy);
    }

    /// <summary>
    ///   Throw the currently picked up object in the direction Madeline's facing.
    /// </summary>
    /// 
    /// <exception cref="InvalidOperationException">
    ///   Attempted to throw a holdable without holding one.
    /// </exception>
    public void Throw()
    {
        if (ActualPlayer.Holding == null)
            throw new InvalidOperationException("Attempted to throw a holdable without holding one.");

        // reimplementing Player.Throw() to remove the -80 speed for use in cutscenes
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
        ActualPlayer.Holding.Release(Vector2.UnitX * (int)ActualPlayer.Facing);
        ActualPlayer.Play("event:/char/madeline/crystaltheo_throw");
        ActualPlayer.Sprite.Play("throw");
        ActualPlayer.Holding = null;
    }

    /// <summary>
    ///   Wait until Madeline is on the ground.
    /// </summary>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline is on the ground.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator WaitUntilOnGround()
    {
        while (!ActualPlayer.OnGround())
            yield return null;
    }

    /// <summary>
    ///   Wait until Madeline enters a certain state.
    /// </summary>
    /// 
    /// <param name="targetState">
    ///   The state to wait for.
    /// </param>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline's state is <paramref name="targetState"/>.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator WaitUntilState(SharpPlayerState targetState)
    {
        while (State != targetState)
            yield return null;
    }

    /// <summary>
    ///   Wait until Madeline's state changes.
    /// </summary>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline's state changes.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator WaitUntilStateChange()
    {
        SharpPlayerState currentState = State;
        while (State == currentState)
            yield return null;
    }

    /// <summary>
    ///   Make Madeline walk a certain distance in a given direction.
    /// </summary>
    /// 
    /// <remarks>
    ///   <i>This method uses relative positions.</i>
    /// </remarks>
    /// 
    /// <param name="direction">
    ///   Direction in which Madeline should walk.
    /// </param>
    /// <param name="amount">
    ///   How far Madeline should walk.
    /// </param>
    /// <param name="unit">
    ///   The unit <paramref name="amount"/> is in.
    /// </param>
    /// <param name="walkBackwards">
    ///   Whether Madeline should appear to walk backwards.
    ///   (default <see langword="false"/>)
    /// </param>
    /// <param name="speedMultiplier">
    ///   Changes Madeline's walking speed.
    ///   (default <c>1f</c>, which is <c>100%</c>)
    /// </param>
    /// <param name="keepRunningIntoWalls">
    ///   Whether Madeline should keep running into walls if she hits one.
    ///   (default <see langword="false"/>)
    /// </param>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">
    ///   Attempted to walk a negative distance.
    /// </exception>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline is done walking.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator Walk(
        SharpHorizontalDirection direction,
        float amount,
        SharpMapUnit unit,

        bool walkBackwards = false,
        float speedMultiplier = 1f,
        bool keepRunningIntoWalls = false)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(null, "Attempted to walk a negative distance.");

        float actualAmount = amount * (int)direction * (int)unit;
        return ActualPlayer.DummyWalkTo(ActualPlayer.ExactPosition.X + actualAmount, walkBackwards, speedMultiplier, keepRunningIntoWalls);
    }

    /// <summary>
    ///   Make Madeline walk to a given X position.
    /// </summary>
    /// 
    /// <remarks>
    ///   <i>This method uses absolute positions.</i>
    /// </remarks>
    /// 
    /// <param name="targetX">
    ///   The X position Madeline should walk towards.
    /// </param>
    /// <param name="walkBackwards">
    ///   Whether Madeline should appear to walk backwards.
    ///   (default <see langword="false"/>)
    /// </param>
    /// <param name="speedMultiplier">
    ///   Changes Madeline's walking speed.
    ///   (default <c>1f</c>, which is <c>100%</c>)
    /// </param>
    /// <param name="keepRunningIntoWalls">
    ///   Whether Madeline should keep running into walls if she hits one.
    ///   (default <see langword="false"/>)
    /// </param>
    /// 
    /// <returns>
    ///   A coroutine which waits until Madeline is done walking.<br/>
    ///   Remember to put <see langword="yield"/> <see langword="return"/> before the call, else it won't run.
    /// </returns>
    public IEnumerator WalkTo(
        float targetX,

        bool walkBackwards = false,
        float speedMultiplier = 1f,
        bool keepRunningIntoWalls = false)
        => ActualPlayer.DummyWalkTo(targetX, walkBackwards, speedMultiplier, keepRunningIntoWalls);
    #endregion
}
