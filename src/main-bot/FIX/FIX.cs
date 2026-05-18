using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// FIX - Adaptive Bot1 Greedy Radar
// ------------------------------------------------------------------
// Mix utama:
// 1. AdaptiveBot: movement acak + sensor wall avoidance ke tengah arena.
// 2. Bot1: greedy attack berdasarkan jarak musuh.
// 3. Radar sensor diperluas sedikit dengan sweep stabil 70 derajat,
//    bukan muter terlalu gila, tapi lebih luas dari sweep kecil.
// 4. Gun hanya diarahkan saat musuh ter-scan, lalu langsung fire.
// 5. Survival: dodge saat kena peluru, hindari wall, dan mundur jika musuh terlalu dekat.
// ------------------------------------------------------------------

public class FIX : Bot
{
    private readonly Random rnd = new Random();

    private bool movingForward = true;
    private int moveDirection = 1;
    private int dodgeDirection = 1;
    private int radarDirection = 1;
    private int scanCount = 0;
    private int hitByBulletCount = 0;

    private const double WallMargin = 135;
    private const double RadarSweepAngle = 70; // sensor radar sedikit diperluas

    static void Main(string[] args)
    {
        new FIX().Start();
    }

    FIX() : base(BotInfo.FromFile("FIX.json")) { }

    public override void Run()
    {
        BodyColor = Color.CadetBlue;
        TracksColor = Color.Cyan;
        TurretColor = Color.DarkRed;
        GunColor = Color.Black;
        RadarColor = Color.Gold;
        ScanColor = Color.LimeGreen;
        BulletColor = Color.Orange;

        movingForward = true;

        while (IsRunning)
        {
            if (IsNearWall())
            {
                EscapeWallToCenter();
                continue;
            }

            // Radar sweep diperluas sedikit:
            // 70 derajat bolak-balik agar area scan lebih luas,
            // tapi tetap stabil dan tidak terlalu liar.
            TurnRadarRight(RadarSweepAngle * radarDirection);
            radarDirection *= -1;

            // Adaptive movement:
            // gerak acak sedang agar tidak mudah diprediksi,
            // tapi tetap diawasi oleh SafeMovementCondition agar tidak nabrak wall.
            double moveDistance = 150 + rnd.NextDouble() * 190; // 150 - 340
            double turnAngle = 35 + rnd.NextDouble() * 55;      // 35 - 90

            if (rnd.NextDouble() < 0.20)
                moveDirection *= -1;

            if (moveDirection > 0)
            {
                SetForward(moveDistance);
                movingForward = true;
            }
            else
            {
                SetBack(moveDistance);
                movingForward = false;
            }

            if (dodgeDirection > 0)
                SetTurnRight(turnAngle);
            else
                SetTurnLeft(turnAngle);

            dodgeDirection *= -1;

            WaitFor(new FIXSafeMovementCondition(this, WallMargin));
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        if (IsTeammate(e.ScannedBotId))
            return;

        scanCount++;

        double distance = DistanceTo(e.X, e.Y);
        double gunBearing = GunBearingTo(e.X, e.Y);
        double bodyBearing = BearingTo(e.X, e.Y);

        // Gun tidak muter sembarangan.
        // Begitu musuh ke-scan, arahkan gun ke musuh lalu fire.
        TurnGunRight(gunBearing);

        double power = ChooseGreedyPower(distance, e.Energy);

        // Greedy attack ala Bot1, tapi lebih hemat dan aman.
        if (Energy > power + 0.7)
        {
            Fire(power);
        }

        // Setelah menembak, geser agar tidak jadi target statis.
        if (distance < 110)
        {
            StrongDodge(bodyBearing);
        }
        else if (distance < 280)
        {
            MediumDodge(bodyBearing);
        }
        else if (scanCount % 3 == 0)
        {
            LightDodge();
        }
    }

    private double ChooseGreedyPower(double distance, double enemyEnergy)
    {
        double power;

        // Bot1-style greedy attack:
        // makin dekat musuh, makin besar power.
        if (distance < 100)
            power = 2.7;
        else if (distance < 180)
            power = 2.1;
        else if (distance < 330)
            power = 1.55;
        else if (distance < 550)
            power = 1.05;
        else if (distance < 850)
            power = 0.70;
        else
            power = 0.40;

        // Jangan overkill musuh yang hampir mati.
        if (enemyEnergy <= 3)
            power = Math.Min(power, 0.55);
        else if (enemyEnergy <= 8)
            power = Math.Min(power, 0.90);
        else if (enemyEnergy <= 14)
            power = Math.Min(power, 1.25);

        // Kalau musuh masih banyak, hemat energi untuk survival score.
        if (EnemyCount >= 7)
            power = Math.Min(power, 0.90);
        else if (EnemyCount >= 4)
            power = Math.Min(power, 1.15);

        // Late game: lebih agresif untuk finishing.
        if (EnemyCount <= 2 && Energy > 35 && distance < 350)
            power = Math.Min(power + 0.35, 2.7);

        // Kalau energi rendah, hemat.
        if (Energy < 12)
            power = Math.Min(power, 0.35);
        else if (Energy < 25)
            power = Math.Min(power, 0.65);
        else if (Energy < 40)
            power = Math.Min(power, 1.0);

        return Clamp(power, 0.35, 2.7);
    }

    private void StrongDodge(double enemyBearing)
    {
        if (IsNearWall())
        {
            EscapeWallToCenter();
            return;
        }

        if (enemyBearing >= 0)
            TurnLeft(80);
        else
            TurnRight(80);

        Back(190);
        movingForward = false;

        moveDirection *= -1;
        dodgeDirection *= -1;
    }

    private void MediumDodge(double enemyBearing)
    {
        if (IsNearWall())
        {
            EscapeWallToCenter();
            return;
        }

        if (enemyBearing >= 0)
            TurnLeft(55);
        else
            TurnRight(55);

        if (dodgeDirection > 0)
        {
            Forward(140);
            movingForward = true;
        }
        else
        {
            Back(140);
            movingForward = false;
        }

        dodgeDirection *= -1;
    }

    private void LightDodge()
    {
        if (IsNearWall())
        {
            EscapeWallToCenter();
            return;
        }

        TurnRight(35 * dodgeDirection);
        Forward(90);

        movingForward = true;
        dodgeDirection *= -1;
    }

    public override void OnHitByBullet(HitByBulletEvent e)
    {
        hitByBulletCount++;

        double bulletBearing = CalcBearing(e.Bullet.Direction);

        // Refleks survival: geser dari arah peluru.
        if (bulletBearing >= 0)
            TurnLeft(75);
        else
            TurnRight(75);

        if (IsNearWall())
        {
            EscapeWallToCenter();
        }
        else
        {
            if (hitByBulletCount % 2 == 0)
            {
                Back(190);
                movingForward = false;
            }
            else
            {
                Forward(190);
                movingForward = true;
            }
        }

        moveDirection *= -1;
        dodgeDirection *= -1;
    }

    public override void OnHitWall(HitWallEvent e)
    {
        EscapeWallToCenter();
    }

    public override void OnHitBot(HitBotEvent e)
    {
        double gunBearing = GunBearingTo(e.X, e.Y);
        double bodyBearing = BearingTo(e.X, e.Y);

        // Kalau tabrakan, musuh sangat dekat.
        // Tembak, tapi jangan power terlalu besar supaya energi aman.
        TurnGunRight(gunBearing);

        if (Energy > 25)
            Fire(1.4);
        else if (Energy > 10)
            Fire(0.7);

        // Jangan stuck nempel musuh.
        if (bodyBearing >= 0)
            TurnLeft(80);
        else
            TurnRight(80);

        Back(180);

        movingForward = false;
        moveDirection *= -1;
        dodgeDirection *= -1;
    }

    private bool IsNearWall()
    {
        return X < WallMargin ||
               Y < WallMargin ||
               X > ArenaWidth - WallMargin ||
               Y > ArenaHeight - WallMargin;
    }

    private void EscapeWallToCenter()
    {
        // AdaptiveBot-style wall avoidance:
        // mundur dulu untuk lepas dari wall, lalu arahkan ke tengah.
        if (movingForward)
            Back(100);
        else
            Forward(100);

        double centerX = ArenaWidth / 2.0;
        double centerY = ArenaHeight / 2.0;

        TurnRight(BearingTo(centerX, centerY));
        Forward(230);

        movingForward = true;
        moveDirection = 1;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}

// Sensor movement agar bot berhenti kalau gerakan selesai atau sudah dekat wall.
public class FIXSafeMovementCondition : Condition
{
    private readonly Bot bot;
    private readonly double margin;

    public FIXSafeMovementCondition(Bot bot, double margin)
    {
        this.bot = bot;
        this.margin = margin;
    }

    public override bool Test()
    {
        bool moveComplete = bot.DistanceRemaining == 0 && bot.TurnRemaining == 0;

        bool nearWall =
            bot.X < margin ||
            bot.Y < margin ||
            bot.X > bot.ArenaWidth - margin ||
            bot.Y > bot.ArenaHeight - margin;

        return moveComplete || nearWall;
    }
}
