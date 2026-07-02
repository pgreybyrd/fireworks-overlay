using System.Drawing;

namespace FireworksOverlay;

public enum ParticleShape
{
    Circle,
    Plus,
    Star,
    Shell
}

public class Particle
{
    public float X, Y;
    public float VX, VY;

    public float TargetY;

    public float Life;
    public float MaxLife;

    public float Size;
    public Color Color;
    public ParticleShape Shape;

    public bool IsShell;
    public bool HasExploded;
    public bool IsSparkle;

    public FireworkType FireworkType;
}

public enum FireworkType
{
    Burst,
    Ring,
    Chrysanthemum,
    Willow,
    DoubleBurst,
    Comet,
    Crackle
}