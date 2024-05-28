// we don't want to litter with dummy events
#if DEBUG
using Celeste.Mod.Entities;
using Celeste.Mod.SharpCutscenes.Enums;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.SharpCutscenes.Examples;

[CustomEvent("SharpCutscenes/ExampleCutscene")]
public class ExampleCutscene(EventTrigger trigger, Player player, string eventID) : SharpCutscene(trigger, player, eventID)
{
    // TODO: test everything
    // watch this comment be not removed for 3 years LMAO

    public override IEnumerator Cutscene()
    {
        yield return Level.Say("SharpCutscenes_ExampleCutscene_Intro");
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer");

        yield return TestMovement();
        yield return TestPausing();
        yield return TestRetrying();
        yield return TestDashCount();
        yield return TestInventory();
    }

    private IEnumerator TestMovement()
    {
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanMove");
        yield return 5f;

        Player.CanMove = true;
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanMove2");
        yield return 5f;

        Player.CanMove = false;
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanMove3");
        yield return 5f;
    }

    private IEnumerator TestPausing()
    {
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanPause");
        yield return 5f;

        Player.CanPause = false;
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanPause2");
        yield return 5f;

        Player.CanMove = true;
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanPause3");
        yield return 5f;
    }

    private IEnumerator TestRetrying()
    {
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanRetry");
        yield return 5f;

        Player.CanRetry = false;
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanRetry2");
        yield return 5f;

        Player.CanRetry = true;
        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_CanRetry3");
        yield return 5f;
    }

    private IEnumerator TestDashCount()
    {
        int previousMaxDashes = Player.MaxDashes;
        int previousDashes = Player.CurrentDashes;

        yield return Level.Say(TemporaryDialogId.FormatDialogId(
            "SharpCutscenes_ExampleCutscene_TestPlayer_DashCount1",
            Player.MaxDashes));

        Player.MaxDashes = 0;
        Player.CanMove = true;
        yield return 5f;
        Player.CanMove = false;

        yield return Level.Say(TemporaryDialogId.FormatDialogId(
            "SharpCutscenes_ExampleCutscene_TestPlayer_DashCount2",
            Player.CurrentDashes));

        Player.CurrentDashes = 2;

        yield return Level.Say("SharpCutscenes_ExampleCutscene_TestPlayer_DashCount3");

        Player.MaxDashes = previousMaxDashes;
        Player.CurrentDashes = previousDashes;
    }

    public IEnumerator TestInventory()
    {
        PlayerInventory previousInventory = Player.Inventory;


    }
}
#endif