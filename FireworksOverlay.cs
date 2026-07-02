using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FireworksOverlay;

public class FireworksForm : Form
{
    private readonly List<Particle> _particles = new();
    private readonly Random _random = new();
    private readonly System.Windows.Forms.Timer _timer = new();

    private int _timeUntilNextLaunchMs;

    public FireworksForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Bounds = Screen.PrimaryScreen.Bounds;

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
        };

        MouseDown += (_, e) =>
        {
            LaunchFirework(e.X, e.Y);
        };
    }

    private void UpdateFireworks()
    {
        const int dt = 16;

        _timeUntilNextLaunchMs -= dt;

        if (_timeUntilNextLaunchMs <= 0)
        {
            int bursts = _random.Next(1, 4); // sometimes multiple!

            for (int i = 0; i < bursts; i++)
            {
                int x = _random.Next(100, Width - 100);
                int y = _random.Next(80, Height / 2);

                LaunchFirework(x, y);
            }

            _timeUntilNextLaunchMs = _random.Next(600, 5000);
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            Particle p = _particles[i];

            p.X += p.VX;
            p.Y += p.VY;

            p.VY += p.IsShell ? 0.025f : 0.04f; // gravity
            p.VX *= 0.992f;
            p.VY *= 0.992f;

            if (p.IsShell && !p.HasExploded && p.Y <= p.TargetY)
            {
                p.HasExploded = true;
                LaunchFirework(p.X, p.Y);
                _particles.RemoveAt(i);
                continue;
            }

            if (!p.IsShell && !p.IsSparkle && p.Life < p.MaxLife * 0.35f && _random.NextDouble() < 0.008)
            {
                AddSparkle(p.X, p.Y, p.Color);
            }

            if (p.IsShell && !p.HasExploded && (p.Y <= p.TargetY || p.VY >= 0))
            {
                p.HasExploded = true;
                LaunchFirework(p.X, p.Y);
                _particles.RemoveAt(i);
                continue;
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
            Shape = _random.NextDouble() < 0.5 ? ParticleShape.Plus : ParticleShape.Star
        });
    }

    private void LaunchShell(float startX, float startY, float targetY)
    {
        float distance = startY - targetY;

        // Bigger distance = stronger launch
        float upwardSpeed = (float)Math.Sqrt(distance * 0.18f);

        _particles.Add(new Particle
        {
            X = startX,
            Y = startY,
            TargetY = targetY,

            VX = (float)(_random.NextDouble() * 1.4 - 0.7),
            VY = -upwardSpeed,

            Life = 3000,
            MaxLife = 3000,
            Size = 7,
            Color = Color.White,
            Shape = ParticleShape.Shell,
            IsShell = true
        });
    }

    private void LaunchFirework(float x, float y)
    {
        Color[][] palettes =
        {
            new[] { Color.Gold, Color.Orange, Color.White, Color.Yellow },
            new[] { Color.Red, Color.OrangeRed, Color.Gold, Color.White },
            new[] { Color.DeepSkyBlue, Color.Cyan, Color.White, Color.LightSteelBlue },
            new[] { Color.MediumPurple, Color.Violet, Color.White, Color.HotPink },
            new[] { Color.HotPink, Color.DeepPink, Color.White, Color.Pink },
            new[] { Color.LimeGreen, Color.SpringGreen, Color.White, Color.Cyan },
            new[] { Color.Red, Color.Orange, Color.Yellow, Color.LimeGreen, Color.DeepSkyBlue, Color.MediumPurple, Color.HotPink, Color.White },
            new[] { Color.White, Color.Gainsboro, Color.LightYellow }
        };

        Color[] palette = _random.Next(5) == 0
            ? palettes[6]   // rainbow
            : palettes[_random.Next(palettes.Length)];

        int count = _random.Next(70, 130);
        int style = _random.Next(3);

        for (int i = 0; i < count; i++)
        {
            double angle = style == 0
                ? _random.NextDouble() * Math.PI * 2
                : (Math.PI * 2 / count) * i + _random.NextDouble() * 0.15;

            double speed = style == 2
                ? 2.5 + _random.NextDouble() * 3.0
                : 1.5 + _random.NextDouble() * 5.5;

            Color particleColor = i < 4
                ? Color.White
                : palette[_random.Next(palette.Length)];

            _particles.Add(new Particle
            {
                X = x,
                Y = y,
                VX = (float)(Math.Cos(angle) * speed),
                VY = (float)(Math.Sin(angle) * speed),
                Life = _random.Next(1200, 2400),
                MaxLife = 2400,
                Size = _random.Next(2, 6),
                Color = particleColor,
                Shape = (ParticleShape)_random.Next(0, 3)
            });
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.None;

        foreach (Particle p in _particles)
        {
            float alphaPercent = Math.Max(0, p.Life / p.MaxLife);
            int alpha = (int)(255 * alphaPercent);

            using var pen = new Pen(Color.FromArgb(alpha, p.Color), 2);
            using var brush = new SolidBrush(Color.FromArgb(alpha, p.Color));

            float s = p.Size;

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