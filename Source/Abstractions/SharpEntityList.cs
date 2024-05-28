using Monocle;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;

namespace Celeste.Mod.SharpCutscenes.Abstractions;

/// <summary>
///   An interface for the loaded entities in Celeste.
/// </summary>
public sealed class SharpEntityList : IEnumerable<Entity>, IEnumerable
{
    #region Implementation Details
    internal SharpEntityList(SharpLevel parent)
    {
        Parent = parent;
    }

    private readonly SharpLevel Parent;
    #endregion

    #region Methods
    /// <summary>
    ///   Add an entity to the scene.
    /// </summary>
    /// 
    /// <param name="entity">
    ///   The entity to add.
    /// </param>
    public void Add(Entity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(null, "Attempted to add a null entity to the scene.");
        Parent.ActualLevel.Add(entity);
    }

    /// <summary>
    ///   Remove an entity from the scene.
    /// </summary>
    /// 
    /// <param name="entity">
    ///   The entity to remnove.
    /// </param>
    public void Remove(Entity entity)
        => Parent.ActualLevel.Remove(entity);

    /// <summary>
    ///   Returns an <see cref="IEnumerator{Entity}"/>, which allows you to <see langword="foreach"/> over all entities in the current room.
    /// </summary>
    /// 
    /// <returns>
    ///   An <see cref="IEnumerator{Entity}"/> iterating over all the entities in the current room.
    /// </returns>
    public IEnumerator<Entity> GetEnumerator()
        => Parent.ActualLevel.GetEnumerator();

    /// <summary>
    ///   Returns an <see cref="IEnumerator"/>, which allows you to <see langword="foreach"/> over all entities in the current room.
    /// </summary>
    /// 
    /// <returns>
    ///   An <see cref="IEnumerator"/> iterating over all the entities in the current room.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    ///   Returns the first entity of type <typeparamref name="T"/> from the tracker.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of a tracked entity to look for.
    /// </typeparam>
    /// 
    /// <exception cref="ArgumentException">
    ///   The type <typeparamref name="T"/> is not a tracked entity.
    /// </exception>
    /// 
    /// <returns>
    ///   The first found instance of such an entity, or <see langword="null"/> if none was found.
    /// </returns>
    public T? GetFirstTrackedEntity<T>() where T : Entity
    {
        if (!IsEntityTracked<T>())
            throw new ArgumentException($"Attempted to get the first entity of type {typeof(T).Name}, but it is not tracked.");
        return Parent.ActualLevel.Tracker.GetEntity<T>();
    }

    /// <summary>
    ///   Returns all entities of type <typeparamref name="T"/> from the tracker.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of a tracked entity to look for.
    /// </typeparam>
    /// 
    /// <exception cref="ArgumentException">
    ///   The type <typeparamref name="T"/> is not a tracked entity.
    /// </exception>
    /// 
    /// <returns>
    ///   An <see cref="IEnumerable{T}"/> containing all the entities of type <typeparamref name="T"/>.
    /// </returns>
    public IEnumerable<T> GetAllTrackedEntities<T>() where T : Entity
    {
        if (!IsEntityTracked<T>())
            throw new ArgumentException($"Attempted to get all entities of type {typeof(T).Name}, but it is not tracked.");
        return Parent.ActualLevel.Tracker.GetEntities<T>().Cast<T>();
    }

    /// <summary>
    ///   Returns all entity of type <typeparamref name="T"/> from the tracker.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of a tracked entity to look for.
    /// </typeparam>
    /// 
    /// <exception cref="ArgumentException">
    ///   The type <typeparamref name="T"/> is not a tracked entity.
    /// </exception>
    /// 
    /// <returns>
    ///   The nearest instance of such an entity, or <see langword="null"/> if none was found.
    /// </returns>
    public T? GetNearestTrackedEntity<T>(Vector2 position) where T : Entity
    {
        if (!IsEntityTracked<T>())
            throw new ArgumentException($"Attempted to get the nearest entity of type {typeof(T).Name}, but it is not tracked.");
        return Parent.ActualLevel.Tracker.GetNearestEntity<T>(position);
    }

    /// <summary>
    ///   Returns the first entity of type <typeparamref name="T"/> from the room's entity list.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of an entity to look for.
    /// </typeparam>
    /// 
    /// <returns>
    ///   The first found instance of such an entity, or <see langword="null"/> if none was found.
    /// </returns>
    public T? GetFirstEntity<T>() where T : Entity
        => Parent.ActualLevel.Entities.FindFirst<T>();

    /// <summary>
    ///   Returns the first entity of type <typeparamref name="T"/> from the room's entity list.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of an entity to look for.
    /// </typeparam>
    /// 
    /// <returns>
    ///   An <see cref="IEnumerable{T}"/> containing all the entities of type <typeparamref name="T"/>.
    /// </returns>
    public IEnumerable<T> GetAllEntities<T>() where T : Entity
        => Parent.ActualLevel.Entities.FindAll<T>();


    /// <summary>
    ///   Returns the nearest entity of type <typeparamref name="T"/> from the room's entity list.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of an entity to look for.
    /// </typeparam>
    /// 
    /// <returns>
    ///   The nearest instance of such an entity, or <see langword="null"/> if none was found.
    /// </returns>
    public T? GetNearestEntity<T>(Vector2 position) where T : Entity
        => (T?)Parent.ActualLevel.Entities.Where(e => e is T).MinBy(e => e.Position - position);

    /// <summary>
    ///   Count all loaded entities.
    /// </summary>
    /// 
    /// <returns>
    ///   The count of all loaded entities.
    /// </returns>
    public int Count()
        => Parent.ActualLevel.Entities.Count;

    /// <summary>
    ///   Count all loaded entities of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <returns>
    ///   The count of all loaded entities of type <typeparamref name="T"/>.
    /// </returns>
    public int Count<T>() where T : Entity
        => Parent.ActualLevel.Entities.AmountOf<T>();

    /// <summary>
    ///   Count all loaded, tracked entities of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <returns>
    ///   The count of all loaded, tracked entities of type <typeparamref name="T"/>.
    /// </returns>
    public int CountTracked<T>() where T : Entity
        => Parent.ActualLevel.Tracker.CountEntities<T>();

    /// <summary>
    ///   Check whether the entity type <typeparamref name="T"/> is tracked.
    /// </summary>
    /// 
    /// <typeparam name="T">
    ///   The type of entity to check.
    /// </typeparam>
    /// 
    /// <returns>
    ///   Whether the entity type <typeparamref name="T"/> is tracked.
    /// </returns>
    public bool IsEntityTracked<T>() where T : Entity
        => Parent.ActualLevel.Tracker.IsEntityTracked<T>();
    #endregion
}
