using System;
using Robocode.TankRoyale.BotApi.Graphics;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

// ------------------------------------------------------------------
// AdaptiveBot
// ------------------------------------------------------------------
// A smart bot that combines unpredictable, randomized movements 
// with an active proximity sensor that reverses and turns to avoid walls.
// ------------------------------------------------------------------
public class AdaptiveBot : Bot
{
    private bool movingForward;    
    private int enemies;           
    private bool stopWhenSeeEnemy; 
    private Random rnd = new Random(); 

    static void Main()
    {
        new AdaptiveBot().Start();
    }

    public AdaptiveBot() : base(BotInfo.FromFile("AdaptiveBot.json")) { }

    public override void Run()
    {
        BodyColor = Color.CadetBlue;
        TurretColor = Color.DarkRed;
        RadarColor = Color.Gold;
        BulletColor = Color.Orange;
        ScanColor = Color.LimeGreen;

        enemies = EnemyCount;
        movingForward = true;
        stopWhenSeeEnemy = false;

        while (IsRunning)
        {
            AdaptiveMovement();
        }
    }

    private void AdaptiveMovement()
    {
        double margin = 120; // Proximity threshold

        // 1. Proactive Wall Avoidance (Emergency Reverse & Turn)
        if (X < margin || X > ArenaWidth - margin || Y < margin || Y > ArenaHeight - margin)
        {
            // Clear any lingering asynchronous commands
            SetForward(0);
            SetTurnRight(0);

            // STEP 1: Immediately reverse to counter momentum and back out of danger
            if (movingForward)
            {
                Back(100); 
            }
            else
            {
                Forward(100);
            }

            // STEP 2: Now that we are safe, calculate bearing to the exact center
            double centerDir = DirectionTo(ArenaWidth / 2.0, ArenaHeight / 2.0);
            
            // STEP 3: Turn to face the center
            TurnRight(CalcBearing(centerDir));

            // STEP 4: Drive safely toward the center of the map
            movingForward = true;
            Forward(150);
            
            return; // Restart the movement loop
        }

        // 2. Unpredictable Movements
        double distance = rnd.NextDouble() * 300 + 100; 
        double angle = rnd.NextDouble() * 180 - 90;     

        if (rnd.NextDouble() < 0.2)
        {
            movingForward = !movingForward;
        }

        if (movingForward)
        {
            SetForward(distance);
        }
        else
        {
            SetBack(distance);
        }

        SetTurnRight(angle);

        // Wait until the move completes OR the proximity sensor detects a wall
        WaitFor(new SafeMovementCondition(this, margin));
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double distance = DistanceTo(e.X, e.Y);

        if (stopWhenSeeEnemy)
        {
            // Optional: You could use Stop()/Resume() here if you want it to pause to shoot
            SmartFire(distance);
        }
        else
        {
            SmartFire(distance);
        }
    }

    private void SmartFire(double distance)
    {
        if (distance > 250 || Energy < 20)
        {
            Fire(1);
        }
        else if (distance > 100)
        {
            Fire(2);
        }
        else
        {
            Fire(3);
        }
    }

    public override void OnHitWall(HitWallEvent e)
    {
        // Fallback physical collision handler (should rarely happen now)
        ReverseDirection();
    }

    public override void OnHitBot(HitBotEvent e)
    {
        if (e.IsRammed)
        {
            ReverseDirection();
        }
    }

    private void ReverseDirection()
    {
        movingForward = !movingForward;
        
        if (movingForward)
        {
            SetForward(200);
        }
        else
        {
            SetBack(200);
        }
    }

    public override void OnDeath(DeathEvent e)
    {
        if (enemies == 0) return;

        if (EnemyCount / (double)enemies >= 0.75)
        {
            Console.WriteLine("I died early and performed poorly this round.");
        }
        else
        {
            Console.WriteLine("I survived against the majority of enemies. Good round.");
        }
    }
}

// ------------------------------------------------------------------
// SafeMovementCondition
// Acts as a proximity sensor against the walls
// ------------------------------------------------------------------
public class SafeMovementCondition : Condition
{
    private readonly Bot bot;
    private readonly double margin;

    public SafeMovementCondition(Bot bot, double margin)
    {
        this.bot = bot;
        this.margin = margin;
    }

    public override bool Test()
    {
        bool moveComplete = bot.DistanceRemaining == 0 && bot.TurnRemaining == 0;
        
        bool nearWall = bot.X < margin || bot.X > bot.ArenaWidth - margin || 
                        bot.Y < margin || bot.Y > bot.ArenaHeight - margin;
        
        return moveComplete || nearWall;
    }
}