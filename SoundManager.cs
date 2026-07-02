using System.Media;

namespace FireworksOverlay;

public static class SoundManager
{
    private static readonly Random _random = new();

    private static readonly string[] Launch =
        Directory.GetFiles("Sounds/Launch", "*.wav");

    private static readonly string[] Explosion =
        Directory.GetFiles("Sounds/Explosion", "*.wav");

    private static readonly string[] Crackle =
        Directory.GetFiles("Sounds/Crackle", "*.wav");

    private static readonly string[] Finale =
        Directory.GetFiles("Sounds/Finale", "*.wav");

    private static void PlayRandom(string[] sounds)
    {
        if (sounds.Length == 0)
            return;

        string file = sounds[_random.Next(sounds.Length)];

        SoundPlayer player = new(file);

        player.Play();
    }

    public static void PlayLaunch()
        => PlayRandom(Launch);

    public static void PlayExplosion()
        => PlayRandom(Explosion);

    public static void PlayCrackle()
        => PlayRandom(Crackle);

    public static void PlayFinale()
        => PlayRandom(Finale);
}