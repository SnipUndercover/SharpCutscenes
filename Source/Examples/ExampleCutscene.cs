// we don't want to litter with dummy events
#if DEBUG
using Celeste.Mod.Entities;
using Celeste.Mod.SharpCutscenes.Enums;
using System.Collections;

namespace Celeste.Mod.SharpCutscenes.Examples;

[CustomEvent("SharpCutscenes/ExampleCutscene")]
public class ExampleCutscene(EventTrigger trigger, Player player, string eventID) : SharpCutscene(trigger, player, eventID)
{
    // TODO: test everything
    // watch this comment be not removed for 3 years LMAO

    public override IEnumerator Cutscene()
    {
        Level.Flags["MyFlag"] = true;
        Player.Jump();
        yield return Player.WaitUntilOnGround();
        yield return Player.Walk(HorizontalDirection.Left, 2, MapUnit.Tile);
        yield return 0.5f;
        yield return Player.Walk(HorizontalDirection.Right, 4, MapUnit.Tile);
        yield return 1.0f;
        yield return Player.Walk(HorizontalDirection.Left, 2, MapUnit.Tile);
        yield return Level.Say("WAVEDASH_PAGE2_IMPOSSIBLE");

        int totalVanillaDeaths = SaveData.VanillaStatistics.TotalDeaths();
        int totalOldSiteDeaths = SaveData.VanillaStatistics.Chapters[VanillaChapter.OldSite].TotalDeaths;
        int totalOldSiteBSideDeaths = SaveData.VanillaStatistics.Chapters[VanillaChapter.OldSite].Sides[ChapterSide.BSide].TotalDeaths;

        int totalModdedDeaths = SaveData.ModdedStatistics.TotalDeaths();
        int totalSentientForestDeaths = SaveData.ModdedStatistics.Chapters["Spooooky/SentientForest/Forest"]!.TotalDeaths;
        int totalSentientForestBSideDeaths = SaveData.ModdedStatistics.Chapters["Spooooky/SentientForest/Forest"]!.Sides[ChapterSide.BSide].TotalDeaths;
    }
}
#endif