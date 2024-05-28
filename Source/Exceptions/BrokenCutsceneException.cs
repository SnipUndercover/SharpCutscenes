using System;

namespace Celeste.Mod.SharpCutscenes;
public class BrokenCutsceneException : Exception
{
    private const string DetailedMessage = "[Cutscene \"{0}\"]\n{1}: {2}";
    private const string GenericMessage = "[Cutscene \"{0}\"]\nEncountered an unexpected exception.";

    public readonly string EventID;

    internal BrokenCutsceneException(string eventId, Exception? inner) : base(
        inner == null
            ? string.Format(GenericMessage, eventId)
            : string.Format(DetailedMessage, eventId, inner.GetType().FullName, inner.Message),
        inner)
    {
        EventID = eventId;
    }
}
