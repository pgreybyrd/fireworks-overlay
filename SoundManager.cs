using NAudio.Wave;

namespace FireworksOverlay;

public static class SoundManager
{
    private static readonly Random Random = new();

    private static readonly string BasePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

    private static readonly string[] LaunchSounds =
        Directory.GetFiles(Path.Combine(BasePath, "Launch"), "*.wav");

    private static readonly string[] ExplosionSounds =
        Directory.GetFiles(Path.Combine(BasePath, "Explosion"), "*.wav");

    private static readonly string[] CrackleSounds =
        Directory.GetFiles(Path.Combine(BasePath, "Crackle"), "*.wav");

    private static readonly string[] FinaleSounds =
        Directory.GetFiles(Path.Combine(BasePath, "Finale"), "*.wav");

    public static void PlayLaunch() => PlayRandom(LaunchSounds, 0.15f);
    public static void PlayExplosion() => PlayRandom(ExplosionSounds, 0.6f);  
    public static void PlayCrackle() => PlayRandom(CrackleSounds, 0.75f);
    public static void PlayFinale() => PlayRandom(FinaleSounds, 0.7f);    

    private static void PlayRandom(string[] sounds, float volume)
    {
        if (sounds.Length == 0)
            return;

        string file = sounds[Random.Next(sounds.Length)];
        Play(file, volume);
    }

    private static void Play(string file, float volume)
    {
        var reader = new AudioFileReader(file)
        {
            Volume = volume
        };

        var output = new WaveOutEvent();
        output.Init(reader);
        output.Play();

        output.PlaybackStopped += (_, _) =>
        {
            output.Dispose();
            reader.Dispose();
        };
    }
}