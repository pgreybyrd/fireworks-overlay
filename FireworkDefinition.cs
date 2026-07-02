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
    Ice,
    Purple,
    Pink,
    Green,
    Rainbow,
    WhiteGlitter
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

    public float Gravity { get; set; } = 0.045f;
}