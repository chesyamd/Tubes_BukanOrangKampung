using System;
using Robocode.TankRoyale.BotApi.Graphics;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Bot2 : Bot
{
    static void Main(string[] args)
    {
        new Bot2().Start();
    }

    // pastikan nanti kamu punya file Bot2.json
    Bot2() : base(BotInfo.FromFile("Bot2.json")) { }

    public override void Run()
    {
        BodyColor = Color.Red;        
        GunColor = Color.Black;
        RadarColor = Color.Yellow;

        // Radar muter terus
        while (IsRunning)
        {
            TurnRadarRight(360);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double distance = DistanceTo(e.X, e.Y);

        
        TurnGunTo(e.X, e.Y);

        
        if (distance < 150)
            Fire(3);
        else if (distance < 300)
            Fire(2);
        else
            Fire(1);

        
        if (distance < 250)
        {
            TurnRight(90);
            Forward(80);
            TurnLeft(90);
        }
        else
        {
            TurnRight(30);
            Forward(100);
            TurnLeft(60);
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        Fire(3);
        Back(60);
        TurnRight(45);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        Back(120);
        TurnRight(120);
    }

    private void TurnGunTo(double x, double y)
    {
        double angle = GunBearingTo(x, y);
        TurnGunRight(angle);
    }
}
