using System;
using Robocode.TankRoyale.BotApi.Graphics;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Bot1 : Bot
{
    static void Main(string[] args)
    {
        new Bot1().Start();
    }

    Bot1() : base(BotInfo.FromFile("Bot1.json")) { }

    public override void Run()
    {
        // 🎨 COLOR SETUP
        BodyColor = Color.Pink;
        GunColor = Color.Black;
        RadarColor = Color.Cyan;   // 👈 biar lebih enak dilihat

        while (IsRunning)
        {
            TurnRight(360); // greedy scan terus
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double distance = DistanceTo(e.X, e.Y);

        // 🎯 GREEDY ATTACK
        if (distance < 150)
        {
            Fire(3);
            Forward(20);
        }
        else if (distance < 400)
        {
            Fire(2);
            Forward(40);
        }
        else
        {
            Fire(1);
            Forward(60);
        }

        // 🧠 GREEDY MOVE
        if (distance < 100)
        {
            Back(80);
            TurnRight(45);
        }
        else
        {
        TurnGunTo(e.X, e.Y);
        Forward(30);
        }
    }

    private void TurnGunTo(double x, double y)
    {
        double angle = GunBearingTo(x, y);
        TurnGunRight(angle);
    }

    public override void OnHitBot(HitBotEvent e)
    {
        Fire(3);
        Back(50);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        Back(100);
        TurnRight(90);
    }
}