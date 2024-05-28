namespace Celeste.Mod.SharpCutscenes.Enums;

/// <summary>
///   Vanilla postcard fade in/out sounds.
/// </summary>
public readonly record struct SharpPostcardSounds
{
    /// <summary>
    ///   The sound event played when the postcard fades in.
    /// </summary>
    public readonly string FadeInSound;

    /// <summary>
    ///   The sound event played when the postcard fades out.
    /// </summary>
    public readonly string FadeOutSound;

    private SharpPostcardSounds(in string fadeInSound, in string fadeOutSound)
    {
        FadeInSound = fadeInSound;
        FadeOutSound = fadeOutSound;
    }

    /// <summary>
    ///   Postcard sounds used in Forsaken City A-Side.
    /// </summary>
    public static readonly SharpPostcardSounds ForsakenCity = new(SFX.ui_main_postcard_ch1_in, SFX.ui_main_postcard_ch1_out);

    /// <summary>
    ///   Postcard sounds used in Old Site A-Side.
    /// </summary>
    public static readonly SharpPostcardSounds OldSite = new(SFX.ui_main_postcard_ch2_in, SFX.ui_main_postcard_ch2_out);

    /// <summary>
    ///   Postcard sounds used in Celestial Resort A-Side.
    /// </summary>
    public static readonly SharpPostcardSounds CelestialResort = new(SFX.ui_main_postcard_ch3_in, SFX.ui_main_postcard_ch3_out);

    /// <summary>
    ///   Postcard sounds used in Golden Ridge A-Side.
    /// </summary>
    public static readonly SharpPostcardSounds GoldenRidge = new(SFX.ui_main_postcard_ch4_in, SFX.ui_main_postcard_ch4_out);

    /// <summary>
    ///   Postcard sounds used in Mirror Temple A-Side.
    /// </summary>
    public static readonly SharpPostcardSounds MirrorTemple = new(SFX.ui_main_postcard_ch5_in, SFX.ui_main_postcard_ch5_out);

    /// <summary>
    ///   Postcard sounds used in Reflection A-Side.
    /// </summary>
    public static readonly SharpPostcardSounds Reflection = new(SFX.ui_main_postcard_ch6_in, SFX.ui_main_postcard_ch6_out);

    /// <summary>
    ///   Postcard sounds used after unlocking C-Sides.
    /// </summary>
    public static readonly SharpPostcardSounds CSides = new(SFX.ui_main_postcard_csides_in, SFX.ui_main_postcard_csides_out);

    /// <summary>
    ///   Postcard sounds used after unlocking Variant Mode.
    /// </summary>
    public static readonly SharpPostcardSounds Variants = new(SFX.ui_main_postcard_variants_in, SFX.ui_main_postcard_variants_out);
}
