using System;

namespace Celeste.Mod.SharpCutscenes;

/// <summary>
///   A temporary Dialog ID.
/// </summary>
/// 
/// <remarks>
///   This is used to assign Dialog IDs for text generated at runtime,
///   for use in methods which need Dialog IDs instead of arbitrary strings.<br/>
///   Remember to put it in a <see langword="using"/> statement, so that it's cleaned up when it goes out of scope!
/// </remarks>
public sealed class TemporaryDialogId : IDisposable
{    
    #region Implementation Details
    /// <summary>
    ///   Generates a new temporary Dialog ID.
    /// </summary>
    /// 
    /// <param name="content">
    ///   The temporary Dialog ID's content, if any. (optional)
    /// </param>
    public TemporaryDialogId(string? content = null)
    {
        DialogId = $"__{nameof(SharpCutscenes)}_TMPDIALOG{Guid.NewGuid():N}";
        Logger.Log(LogLevel.Debug, $"{nameof(SharpCutscenes)}/{nameof(TemporaryDialogId)}", $"Generated temporary dialog ID: \"{DialogId}\"");

        if (string.IsNullOrWhiteSpace(content))
            return;

        _Content = content;
        UpdateDialogContent();
    }

    private string? _Content;

    private static Language FallbackLanguage
        => Dialog.Languages["english"];

    private static string CleanContent(string content)
    {
        if (!content.Contains('{'))
            return content;

        content = content.Replace("{n}", "\n");
        content = content.Replace("{break}", "\n");
        content = Language.command.Replace(content, "");

        return content;
    }

    private void UpdateDialogContent()
    {
        FallbackLanguage.Dialog[DialogId] = Content!;
        FallbackLanguage.Cleaned[DialogId] = CleanContent(Content!);
    }

    private void RemoveDialog()
    {
        FallbackLanguage.Dialog.Remove(DialogId);
        FallbackLanguage.Cleaned.Remove(DialogId);
    }

    public void Dispose()
        => RemoveDialog();

    /// <summary>
    ///   Convenience implicit cast operator to string, to allow for intuitively passing in a <see cref="TemporaryDialogId"/>,<br/>
    ///   instead of having to constantly access the <see cref="DialogId"/> property.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The <see cref="TemporaryDialogId"/> to cast.
    /// </param>
    public static implicit operator string(TemporaryDialogId dialogId)
        => dialogId.DialogId;
    #endregion

    #region Fields & Properties
    /// <summary>
    ///   The temporary dialog ID's content.
    /// </summary>
    public string? Content
    {
        get => _Content;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _Content = null;
                RemoveDialog();
            }
            else
            {
                _Content = value;
                UpdateDialogContent();
            }
        }
    }

    /// <summary>
    ///   The generated temporary Dialog ID.
    /// </summary>
    public string DialogId { get; }
    #endregion

    #region Helpers
    /// <summary>
    ///   Apply <see cref="string.Format(string, object[])"/> to an existing Dialog ID's contents.
    /// </summary>
    /// 
    /// <param name="dialogId">
    ///   The Dialog ID whose contents should be formatted.
    /// </param>
    /// 
    /// <param name="formatArgs">
    ///   The arguments to <see cref="string.Format(string, object[])"/>.
    /// </param>
    /// 
    /// <returns>
    ///   A temporary Dialog ID which has the formatted contents of <paramref name="dialogId"/>.
    /// </returns>
    public static TemporaryDialogId FormatDialogId(string dialogId, params object[] formatArgs)
    {
        if (!Dialog.Has(dialogId))
            throw new InvalidOperationException($"Attempted to format non-existent dialog id \"{dialogId}\".");

        return new(string.Format(Dialog.Get(dialogId), formatArgs));
    }
    #endregion
}
