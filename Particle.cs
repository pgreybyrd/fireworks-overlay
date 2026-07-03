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

    public Color StartColor;
    public Color EndColor;

    public GlowShape GlowShape;

    public bool IsShell;
    public bool HasExploded;
    public bool IsSparkle;
    public bool CanReexplode;
    public bool HasReexploded;
    public bool IsSmoke;

    public FireworkType FireworkType;
    public FireworkDefinition? Definition;
}
