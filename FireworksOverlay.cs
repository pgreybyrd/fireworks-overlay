using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;

namespace FireworksOverlay;

public class FireworksForm : Form
{
    private bool _glowEnabled = true;

    private readonly List<Particle> _particles = new();
    private readonly Random _random = new();
    private readonly System.Windows.Forms.Timer _timer = new();

    private bool _isPaused;
    private bool _finaleMode;

    private int _timeUntilNextLaunchMs;

    public FireworksForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;

        Screen targetScreen = Screen.AllScreens
            .OrderByDescending(s => s.Bounds.Height)
            .First();

        Bounds = targetScreen.Bounds;

        TopMost = true;
        ShowInTaskbar = false;

        BackColor = Color.FromArgb(1, 2, 3);
        TransparencyKey = Color.FromArgb(1, 2, 3);

        DoubleBuffered = true;
        KeyPreview = true;

        _timeUntilNextLaunchMs = _random.Next(500, 3000);

        _timer.Interval = 16;
        _timer.Tick += (_, _) => UpdateFireworks();
        _timer.Start();

        int x = _random.Next(100, Width - 100);
        int y = _random.Next(80, Height / 2);

        //CreateMenu();

        //SoundManager.load

        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();

            if (e.KeyCode == Keys.Space)
            {
                int x = _random.Next(100, Width - 100);
                int targetY = _random.Next(80, Height / 2);
                LaunchShell(x, Height - 40, targetY);

            }

            if (e.KeyCode == Keys.P)
                _isPaused = !_isPaused;

            if (e.KeyCode == Keys.F)
            {
                _finaleMode = !_finaleMode;
                _timeUntilNextLaunchMs = 0;
            }

            if (e.KeyCode == Keys.T)
                TopMost = !TopMost;

            if (e.KeyCode == Keys.G)
                _glowEnabled = !_glowEnabled;
        };

        MouseDown += (_, e) =>
        {
            FireworkType type = (FireworkType)_random.Next(5);

            LaunchFirework(e.X, e.Y, CreateRandomDefinition());
        };
    }

    private void DrawParticleGlow(Graphics g, Particle p, Color glowColor, int alpha)
    {
        if (!_glowEnabled || p.Shape == ParticleShape.Shell || p.GlowShape == GlowShape.None)
            return;

        float lifePercent = Math.Max(0, p.Life / p.MaxLife);

        if (lifePercent < 0.30f)
            return;

        // twinkle: some glows blink lightly, but not too obnoxiously
        if (p.GlowShape is GlowShape.Star or GlowShape.Cross)
        {
            int blink = (Environment.TickCount / 90 + p.GetHashCode()) % 4;
            if (blink == 0)
                return;
        }

        int glowAlpha = (int)(alpha * 0.40f);
        float s = p.Size * 3.6f;

        using Pen mainPen = new(Color.FromArgb(glowAlpha, glowColor), 1);
        using Pen faintPen = new(Color.FromArgb(glowAlpha / 2, glowColor), 1);

        switch (p.GlowShape)
        {
            case GlowShape.Plus:
                DrawPlusGlow(g, mainPen, p.X, p.Y, s);
                break;

            case GlowShape.Cross:
                DrawCrossGlow(g, mainPen, p.X, p.Y, s);
                break;

            case GlowShape.Star:
                DrawPlusGlow(g, mainPen, p.X, p.Y, s);
                DrawCrossGlow(g, faintPen, p.X, p.Y, s * 0.75f);
                break;

            case GlowShape.Horizontal:
                g.DrawLine(mainPen, p.X - s, p.Y, p.X + s, p.Y);
                break;

            case GlowShape.Vertical:
                g.DrawLine(mainPen, p.X, p.Y - s, p.X, p.Y + s);
                break;
        }
    }

    private void DrawPlusGlow(Graphics g, Pen pen, float x, float y, float s)
    {
        g.DrawLine(pen, x - s, y, x + s, y);
        g.DrawLine(pen, x, y - s, x, y + s);
    }

    private void DrawCrossGlow(Graphics g, Pen pen, float x, float y, float s)
    {
        g.DrawLine(pen, x - s, y - s, x + s, y + s);
        g.DrawLine(pen, x - s, y + s, x + s, y - s);
    }

    private GlowShape PickGlowShape()
    {
        double roll = _random.NextDouble();

        if (roll < 0.35)
            return GlowShape.None;

        if (roll < 0.55)
            return GlowShape.Plus;

        if (roll < 0.72)
            return GlowShape.Cross;

        if (roll < 0.88)
            return GlowShape.Star;

        if (roll < 0.94)
            return GlowShape.Horizontal;

        return GlowShape.Vertical;
    }

    private void AddSmoke(float x, float y)
    {
        _particles.Add(new Particle
        {
            IsSmoke = true,

            X = x,
            Y = y,

            VX = (float)(_random.NextDouble() - 0.5) * 0.25f,
            VY = -(float)_random.NextDouble() * 0.2f,

            Life = 700,
            MaxLife = 700,

            Size = _random.Next(2, 4),

            Color = Color.LightGray
        });
    }

    private void CreateMenu()
    {
        FlowLayoutPanel menu = new()
        {
            AutoSize = true,
            BackColor = Color.FromArgb(40, 40, 40),
            Padding = new Padding(6),
            Location = new Point(20, 20),
            WrapContents = false
        };

        Button startPauseButton = MakeButton("Pause");
        Button finaleButton = MakeButton("FINALE");
        Button topButton = MakeButton("Top: On");
        Button exitButton = MakeButton("Exit");

        startPauseButton.Click += (_, _) =>
        {
            _isPaused = !_isPaused;
            startPauseButton.Text = _isPaused ? "Start" : "Pause";
        };

        finaleButton.Click += (_, _) =>
        {
            _finaleMode = !_finaleMode;
            finaleButton.Text = _finaleMode ? "FINALE ON" : "FINALE";
            _timeUntilNextLaunchMs = 0;
        };

        topButton.Click += (_, _) =>
        {
            TopMost = !TopMost;
            topButton.Text = TopMost ? "Top: On" : "Top: Off";
        };

        exitButton.Click += (_, _) => Close();

        menu.Controls.Add(startPauseButton);
        menu.Controls.Add(finaleButton);
        menu.Controls.Add(topButton);
        menu.Controls.Add(exitButton);

        Controls.Add(menu);
        menu.BringToFront();
    }

    private Button MakeButton(string text)
    {
        return new Button
        {
            Text = text,
            AutoSize = true,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(70, 70, 70),
            Margin = new Padding(3)
        };
    }

    private void UpdateFireworks()
    {
        if (_particles.Count > 1200)
        {
            _particles.RemoveRange(0, _particles.Count - 1200);
        }

        const int dt = 16;

        if (_isPaused)
        {
            Invalidate();
            return;
        }

        _timeUntilNextLaunchMs -= dt;

        if (_timeUntilNextLaunchMs <= 0)
        {
            int bursts = _finaleMode ? _random.Next(1, 3) : _random.Next(1, 3);

            for (int i = 0; i < bursts; i++)
            {
                int x = _random.Next(100, Width - 100);
                int targetY = _random.Next(80, Height / 2);

                LaunchShell(x, Height - 40, targetY);
            }

            _timeUntilNextLaunchMs = _finaleMode
                ? _random.Next(250, 700)
                : _random.Next(900, 5000);
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            Particle p = _particles[i];

            p.X += p.VX;
            p.Y += p.VY;

            if (p.IsShell && _random.NextDouble() < 0.03)
            {
                AddSmoke(p.X, p.Y + 10);
            }

            p.VY += p.IsShell ? 0.025f : (p.Definition?.Gravity ?? 0.045f);
            p.VX *= 0.992f;
            p.VY *= 0.992f;

            if (p.IsSmoke)
            {
                p.VY -= 0.004f;

                p.VX *= 0.995f;

                p.Size += 0.003f;
            }

            if (p.Definition?.Type == FireworkType.Willow)
            {
                p.VX *= 0.985f;
                p.VY += 0.025f;
            }

            if (p.IsShell && !p.HasExploded && (p.Y <= p.TargetY || p.VY >= 0))
            {
                p.HasExploded = true;

                if (_finaleMode && _random.NextDouble() < 0.25)
                    SoundManager.PlayFinale();
                else
                    SoundManager.PlayExplosion();

                LaunchFirework(p.X, p.Y, p.Definition ?? CreateRandomDefinition());

                _particles.RemoveAt(i);
                continue;
            }

            if (p.CanReexplode && !p.HasReexploded && p.Life < p.MaxLife * 0.45f)
            {
                p.HasReexploded = true;

                FireworkDefinition mini = new()
                {
                    Type = FireworkType.Burst,
                    Palette = p.Definition?.Palette ?? FireworkPalette.Gold,
                    MinLife = 500,
                    MaxLife = 1000,
                    MinSpeed = 0.8f,
                    MaxSpeed = 2.2f,
                    HasSparkles = false
                };

                SoundManager.PlayExplosion();

                LaunchFirework(p.X, p.Y, mini);
            }

            if (p.Definition?.HasCrackle == true && _random.NextDouble() < 0.015)
                AddSparkle(p.X, p.Y, Color.White);

            if (!p.IsShell && !p.IsSparkle && p.Definition?.HasSparkles == true &&
                p.Life < p.MaxLife * 0.12f && _random.NextDouble() < 0.008)
            {
                AddSparkle(p.X, p.Y, p.Color);
            }

            p.Life -= dt;

            if (p.Life <= 0)
                _particles.RemoveAt(i);
        }

        Invalidate();
    }

    private void AddSparkle(float x, float y, Color baseColor)
    {
        _particles.Add(new Particle
        {
            X = x,
            Y = y,
            IsSparkle = true,
            VX = (float)(_random.NextDouble() * 0.6 - 0.3),
            VY = 0.8f + (float)_random.NextDouble() * 1.2f,
            Life = _random.Next(300, 700),
            MaxLife = 700,
            Size = _random.Next(2, 5),
            Color = Color.White,
            Shape = _random.NextDouble() < 0.5 ? ParticleShape.Plus : ParticleShape.Star,
            GlowShape = _random.NextDouble() < 0.5 ? GlowShape.Star : GlowShape.Plus
        });
    }

    private void LaunchShell(float startX, float startY, float targetY)
    {
        float distance = startY - targetY;

        // Bigger distance = stronger launch
        float upwardSpeed = (float)Math.Sqrt(distance * 0.32f);

        SoundManager.PlayLaunch();

        _particles.Add(new Particle
        {
            X = startX,
            Y = startY,
            TargetY = targetY,

            VX = (float)(_random.NextDouble() * 1.4 - 0.7),
            VY = -upwardSpeed,

            FireworkType = (FireworkType)_random.Next(5),
            Definition = CreateRandomDefinition(),

            Life = 3000,
            MaxLife = 3000,
            Size = 7,
            Color = Color.White,
            Shape = ParticleShape.Shell,
            IsShell = true
        });
    }

    private void LaunchFirework(float x, float y, FireworkDefinition def)
    {
        Color[] palette = GetPalette(def.Palette);

        switch (def.Type)
        {
            case FireworkType.Burst:
                CreateBurst(x, y, def, palette);
                break;
            case FireworkType.Ring:
                CreateRing(x, y, def, palette);
                break;
            case FireworkType.Chrysanthemum:
                CreateChrysanthemum(x, y, def, palette);
                break;
            case FireworkType.Willow:
                CreateWillow(x, y, def, palette);
                break;
            case FireworkType.DoubleBurst:
                CreateDoubleBurst(x, y, def, palette);
                break;
            case FireworkType.Crackle:
                CreateCrackle(x, y, def, palette);
                break;
            case FireworkType.Comet:
                CreateComet(x, y, def, palette);
                break;
        }
    }

    private ParticleShape PickShape()
    {
        return (ParticleShape)_random.Next(0, 3);
    }

    private void AddExplosionParticle(
        float x, float y,
        float vx, float vy,
        FireworkDefinition def,
        Color color,
        ParticleShape shape,
        bool canReexplode = false)
    {
        _particles.Add(new Particle
        {
            X = x,
            Y = y,
            VX = vx,
            VY = vy,
            Life = _random.Next(def.MinLife, def.MaxLife),
            MaxLife = def.MaxLife,
            Size = _random.Next(2, 6),
            Color = color,
            Shape = shape,
            Definition = def,
            CanReexplode = canReexplode,
            StartColor = color,
            EndColor = GetFadeColor(color),
            GlowShape = PickGlowShape()
        });
    }

    private Color GetFadeColor(Color color)
    {
        if (color.GetBrightness() > 0.75f)
            return Color.Gold;

        return Color.FromArgb(
            Math.Min(255, color.R + 60),
            Math.Min(255, color.G + 35),
            Math.Min(255, color.B + 10)
        );
    }

    private void CreateBurst(float x, float y, FireworkDefinition def, Color[] palette)
    {
        int count = _random.Next(70, 130);

        for (int i = 0; i < count; i++)
        {
            double angle = _random.NextDouble() * Math.PI * 2;
            double speed = def.MinSpeed + _random.NextDouble() * (def.MaxSpeed - def.MinSpeed);

            AddExplosionParticle(x, y,
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed),
                def,
                PickFireworkColor(def, palette, i, count, angle, speed, def.MaxSpeed),
                PickShape());
        }
    }

    private void CreateRing(float x, float y, FireworkDefinition def, Color[] palette)
    {
        int count = _random.Next(80, 120);
        double baseSpeed = 4.2 + _random.NextDouble();

        for (int i = 0; i < count; i++)
        {
            double angle = Math.PI * 2 * i / count + (_random.NextDouble() - 0.5) * 0.05;
            double speed = baseSpeed + (_random.NextDouble() - 0.5) * 0.3;

            AddExplosionParticle(x, y,
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed),
                def,
                PickFireworkColor(def, palette, i, count, angle, speed, baseSpeed + 0.5),
                PickShape());
        }
    }

    private void CreateChrysanthemum(float x, float y, FireworkDefinition def, Color[] palette)
    {
        int count = _random.Next(140, 220);

        for (int i = 0; i < count; i++)
        {
            double angle = Math.PI * 2 * i / count + _random.NextDouble() * 0.08;
            double speed = 2.0 + _random.NextDouble() * 4.2;

            AddExplosionParticle(x, y,
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed),
                def,
                PickFireworkColor(def, palette, i, count, angle, speed, def.MaxSpeed),
                PickShape());
        }
    }

    private void CreateWillow(float x, float y, FireworkDefinition def, Color[] palette)
    {
        int count = _random.Next(90, 140);

        def.MinLife = 2600;
        def.MaxLife = 4600;
        def.Gravity = 0.07f;

        for (int i = 0; i < count; i++)
        {
            double angle = Math.PI * 2 * i / count + _random.NextDouble() * 0.08;
            double speed = 1.3 + _random.NextDouble() * 2.0;

            AddExplosionParticle(x, y,
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed - 1.1f),
                def,
                PickFireworkColor(def, palette, i, count, angle, speed, def.MaxSpeed),
                ParticleShape.Plus);
        }
    }

    private void CreateDoubleBurst(float x, float y, FireworkDefinition def, Color[] palette)
    {
        int count = _random.Next(70, 110);

        for (int i = 0; i < count; i++)
        {
            double angle = _random.NextDouble() * Math.PI * 2;
            double speed = 1.6 + _random.NextDouble() * 4.0;

            AddExplosionParticle(x, y,
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed),
                def,
                PickFireworkColor(def, palette, i, count, angle, speed, def.MaxSpeed),
                PickShape(),
                canReexplode: _random.NextDouble() < 0.25);
        }
    }

    private void CreateCrackle(float x, float y, FireworkDefinition def, Color[] palette)
    {
        def.HasCrackle = true;
        def.MinLife = 1200;
        def.MaxLife = 2600;

        SoundManager.PlayCrackle();

        CreateChrysanthemum(x, y, def, palette);
    }

    private void CreateComet(float x, float y, FireworkDefinition def, Color[] palette)
    {
        int cometCount = _random.Next(4, 8);

        for (int c = 0; c < cometCount; c++)
        {
            float offsetX = (float)(_random.NextDouble() * 160 - 80);
            float offsetY = (float)(_random.NextDouble() * 80 - 40);

            CreateBurst(x + offsetX, y + offsetY, def, palette);
        }
    }

    private FireworkDefinition CreateRandomDefinition()
    {
        return new FireworkDefinition
        {
            ColorStyle = PickRandomColorStyle(),
            Type = (FireworkType)_random.Next(Enum.GetValues<FireworkType>().Length),
            Palette = (FireworkPalette)_random.Next(Enum.GetValues<FireworkPalette>().Length)
        };
    }

    private ColorStyle PickRandomColorStyle()
    {
        double roll = _random.NextDouble();

        if (roll < 0.55)
            return ColorStyle.Mixed;          // rainbow soup favorite

        if (roll < 0.62)
            return ColorStyle.Balanced;       // nice two-color blend

        if (roll < 0.76)
            return ColorStyle.CoreToEdge;     // structured shell

        if (roll < 0.86)
            return ColorStyle.SplitHorizontal;

        if (roll < 0.96)
            return ColorStyle.SplitVertical;

        return ColorStyle.Uniform;            // rare mostly-one-color
    }

    private Color[] GetPalette(FireworkPalette palette)
    {
        return palette switch
        {
            FireworkPalette.Gold => new[]
            {
                Color.Gold,
                Color.Goldenrod,
                Color.Yellow,
                Color.White
            },

            FireworkPalette.RedGold => new[]
            {
                Color.Red,
                Color.OrangeRed,
                Color.Gold,
                Color.White
            },

            FireworkPalette.Sunset => new[]
            {
                Color.Red,
                Color.OrangeRed,
                Color.Orange,
                Color.Gold,
                Color.Yellow
            },

            FireworkPalette.Patriotic => new[]
            {
                Color.Red,
                Color.White,
                Color.DeepSkyBlue,
                Color.RoyalBlue
            },

            FireworkPalette.Ice => new[]
            {
                Color.DeepSkyBlue,
                Color.Cyan,
                Color.LightBlue,
                Color.White,
                Color.LightSteelBlue
            },

            FireworkPalette.Purple => new[]
            {
                Color.DarkViolet,
                Color.MediumPurple,
                Color.Violet,
                Color.White,
                Color.HotPink
            },

            FireworkPalette.Lavender => new[]
            {
                Color.Plum,
                Color.Orchid,
                Color.Thistle,
                Color.White,
                Color.Lavender
            },

            FireworkPalette.Pink => new[]
            {
                Color.DeepPink,
                Color.HotPink,
                Color.Pink,
                Color.White
            },

            FireworkPalette.Green => new[]
            {
                Color.LimeGreen,
                Color.SpringGreen,
                Color.GreenYellow,
                Color.White
            },

            FireworkPalette.Emerald => new[]
            {
                Color.ForestGreen,
                Color.SeaGreen,
                Color.MediumSeaGreen,
                Color.SpringGreen,
                Color.White
            },

            FireworkPalette.Rainbow => new[]
            {
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.LimeGreen,
                Color.DeepSkyBlue,
                Color.MediumPurple,
                Color.HotPink,
                Color.White
            },

            FireworkPalette.WhiteGlitter => new[]
            {
                Color.White,
                Color.Gainsboro,
                Color.LightYellow,
                Color.LightGray
            },

            FireworkPalette.Autumn => new[]
            {
                Color.Firebrick,
                Color.DarkOrange,
                Color.Goldenrod,
                Color.SaddleBrown,
                Color.Yellow
            },

            FireworkPalette.Halloween => new[]
            {
                Color.Orange,
                Color.DarkOrange,
                Color.DarkViolet,
                Color.Black,
                Color.White
            },

            FireworkPalette.Christmas => new[]
            {
                Color.Red,
                Color.DarkRed,
                Color.ForestGreen,
                Color.LimeGreen,
                Color.White
            },

            FireworkPalette.CottonCandy => new[]
            {
                Color.Pink,
                Color.LightPink,
                Color.LightSkyBlue,
                Color.White
            },

            FireworkPalette.Tropical => new[]
            {
                Color.HotPink,
                Color.Orange,
                Color.Yellow,
                Color.LimeGreen,
                Color.DeepSkyBlue
            },

            FireworkPalette.Ocean => new[]
            {
                Color.Aqua,
                Color.DeepSkyBlue,
                Color.CornflowerBlue,
                Color.Blue,
                Color.White
            },

            FireworkPalette.Galaxy => new[]
            {
                Color.MediumPurple,
                Color.DeepSkyBlue,
                Color.Cyan,
                Color.White,
                Color.HotPink
            },

            FireworkPalette.Sunrise => new[]
            {
                Color.Pink,
                Color.HotPink,
                Color.Orange,
                Color.Gold,
                Color.LightYellow
            },

            FireworkPalette.Aurora => new[]
            {
                Color.MediumPurple,
                Color.DeepSkyBlue,
                Color.SpringGreen,
                Color.Yellow,
                Color.White
            },

            FireworkPalette.AuroraBorealis => new[]
            {
                Color.LimeGreen,
                Color.Cyan,
                Color.DeepSkyBlue,
                Color.MediumPurple,
                Color.White
            },

            _ => new[] { Color.White }
        };
    }

    private Color PickFireworkColor(
        FireworkDefinition def,
        Color[] palette,
        int index,
        int count,
        double angle,
        double speed,
        double maxSpeed)
    {
        Color primary = def.PrimaryColor;
        Color accent = def.AccentColor;

        if (primary == Color.Empty || accent == Color.Empty)
        {
            primary = palette[_random.Next(palette.Length)];
            accent = palette[_random.Next(palette.Length)];

            if (accent == primary && palette.Length > 1)
                accent = palette[(_random.Next(palette.Length - 1) + 1) % palette.Length];

            def.PrimaryColor = primary;
            def.AccentColor = accent;
        }

        // tiny bright core
        if (index < count * 0.05)
            return Color.White;

        return def.ColorStyle switch
        {
            ColorStyle.Uniform => PickWeighted(primary, accent, palette, 0.78, 0.17),

            ColorStyle.Balanced => PickWeighted(primary, accent, palette, 0.45, 0.40),

            ColorStyle.Mixed => palette[_random.Next(palette.Length)],

            ColorStyle.SplitHorizontal => Math.Sin(angle) < 0
                ? primary
                : accent,

            ColorStyle.SplitVertical => Math.Cos(angle) < 0
                ? primary
                : accent,

            ColorStyle.CoreToEdge => speed < maxSpeed * 0.55
                ? primary
                : accent,

            _ => PickWeighted(primary, accent, palette, 0.55, 0.30)
        };
    }

    private Color PickWeighted(Color primary, Color accent, Color[] palette, double primaryChance, double accentChance)
    {
        double roll = _random.NextDouble();

        if (roll < primaryChance)
            return primary;

        if (roll < primaryChance + accentChance)
            return accent;

        return palette[_random.Next(palette.Length)];
    }

    private void DrawSignature(Graphics g)
    {
        string text = "Paulina's Fireworks";

        using Font font = new("Segoe UI", 14, FontStyle.Regular);
        using Brush brush = new SolidBrush(Color.FromArgb(55, Color.White));

        SizeF size = g.MeasureString(text, font);

        float x = Width - size.Width - 24;
        float y = Height - size.Height - 24;

        g.DrawString(text, font, brush, x, y);
    }

    private Color BlendColors(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);

        int r = (int)(from.R + (to.R - from.R) * amount);
        int g = (int)(from.G + (to.G - from.G) * amount);
        int b = (int)(from.B + (to.B - from.B) * amount);

        return Color.FromArgb(r, g, b);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.None;

        foreach (Particle p in _particles)
        {
            float alphaPercent = Math.Max(0, p.Life / p.MaxLife);
            int alpha = (int)(255 * alphaPercent);

            float lifePercent = Math.Max(0, p.Life / p.MaxLife);
            float fadeAmount = 1f - lifePercent;

            Color displayColor = BlendColors(p.StartColor == Color.Empty ? p.Color : p.StartColor,
                                             p.EndColor == Color.Empty ? p.Color : p.EndColor,
                                             fadeAmount);

            DrawParticleGlow(e.Graphics, p, displayColor, alpha);

            using var pen = new Pen(Color.FromArgb(alpha, displayColor), 2);
            using var brush = new SolidBrush(Color.FromArgb(alpha, displayColor));

            float s = p.Size;

            //DrawSignature(e.Graphics);

            switch (p.Shape)
            {
                case ParticleShape.Circle:
                    e.Graphics.FillEllipse(brush, p.X - s / 2, p.Y - s / 2, s, s);
                    break;

                case ParticleShape.Plus:
                    e.Graphics.DrawLine(pen, p.X - s, p.Y, p.X + s, p.Y);
                    e.Graphics.DrawLine(pen, p.X, p.Y - s, p.X, p.Y + s);
                    break;

                case ParticleShape.Star:
                    e.Graphics.DrawLine(pen, p.X - s, p.Y - s, p.X + s, p.Y + s);
                    e.Graphics.DrawLine(pen, p.X - s, p.Y + s, p.X + s, p.Y - s);
                    e.Graphics.DrawLine(pen, p.X - s, p.Y, p.X + s, p.Y);
                    e.Graphics.DrawLine(pen, p.X, p.Y - s, p.X, p.Y + s);
                    break;
                case ParticleShape.Shell:
                    //e.Graphics.DrawLine(Pens.Yellow, 0, p.TargetY, Width, p.TargetY);
                    e.Graphics.FillEllipse(brush, p.X - s / 2, p.Y - s / 2, s, s);
                    e.Graphics.DrawLine(pen, p.X, p.Y + 4, p.X, p.Y + 18);
                    break;
            }

            
        }
    }
}