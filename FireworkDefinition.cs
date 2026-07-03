using System.Drawing;

namespace FireworksOverlay;

public enum FireworkType
{
    Burst,
    Ring,
    Chrysanthemum,
    Willow,
    DoubleBurst,
    Crackle,
    Comet
}

public enum FireworkPalette
{
    Gold,
    RedGold,
    Sunset,
    Patriotic,
    Ice,
    Purple,
    Lavender,
    Pink,
    Green,
    Emerald,
    Rainbow,
    WhiteGlitter,
    Autumn,
    Halloween,
    Christmas,
    CottonCandy,
    Tropical,
    Ocean,
    Galaxy,
    Sunrise,
    Aurora,
    AuroraBorealis
}

public enum ColorStyle
{
    Uniform,        // mostly one color
    Balanced,       // two colors
    Mixed,          // lots of colors
    Gradient,       // coming soon!
    CoreFlash,      // bright white center
    SplitHorizontal,// two colors split horizontally
    SplitVertical,  // two colors split vertically
    CoreToEdge,     // bright center fading to edges
}

public enum GlowShape
{
    Plus,
    Cross,
    Star,
    Horizontal,
    Vertical,
    None
}

public class FireworkDefinition
{
    public FireworkType Type { get; set; }
    public FireworkPalette Palette { get; set; }

    public int ParticleCount { get; set; } = 100;
    public float MinSpeed { get; set; } = 1.5f;
    public float MaxSpeed { get; set; } = 5.5f;

    public int MinLife { get; set; } = 1200;
    public int MaxLife { get; set; } = 2400;

    public bool HasSparkles { get; set; } = true;
    public bool HasCrackle { get; set; }
    public bool HasDoubleBurst { get; set; }
    public bool HasTrail { get; set; } = true;
    public bool IsFinale { get; set; } = false;
    public bool HasWhistle { get; set; } = false;

    public float Gravity { get; set; } = 0.045f;

    public string? LaunchSound { get; set; }
    public string? ExplosionSound { get; set; }
    public string? CrackleSound { get; set; }

    public ColorStyle ColorStyle { get; set; } = ColorStyle.Balanced;
    public Color PrimaryColor { get; set; } = Color.Empty;
    public Color AccentColor { get; set; } = Color.Empty;


}