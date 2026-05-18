using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// CodeCey - Greedy Smart Gunner
// ------------------------------------------------------------------
// Strategi Greedy:
// 1. Bot bergerak aktif seperti Crazy agar sulit ditembak.
// 2. Radar selalu berputar agar musuh terus terpindai.
// 3. Saat musuh terpindai, bot memilih aksi terbaik saat itu:
//    - arahkan gun ke musuh,
//    - tembak hanya jika arah gun cukup tepat,
//    - pilih power peluru berdasarkan jarak, energi sendiri, dan energi musuh.
// 4. Jika musuh sangat dekat, bot dodge atau ram hanya jika menguntungkan.
// ------------------------------------------------------------------

public class CodeCey : Bot
{
    private bool movingForward;
    private int moveDirection = 1;
    private int dodgeDirection = 1;
    private int scanCount = 0;

    static void Main(string[] args)
    {
        new CodeCey().Start();
    }

    CodeCey() : base(BotInfo.FromFile("CodeCey.json")) { }

    public override void Run()
    {
        BodyColor = Color.Blue;
        TracksColor = Color.Cyan;
        TurretColor = Color.Black;
        GunColor = Color.White;
        RadarColor = Color.Yellow;
        ScanColor = Color.Green;
        BulletColor = Color.Orange;

        movingForward = true;

        // Radar wajib terus bergerak supaya scan arc tidak nol.
        SetTurnRadarRight(double.PositiveInfinity);

        // Movement dasar mengikuti Crazy: aktif, panjang, dan zig-zag.
        while (IsRunning)
        {
            SetForward(40000 * moveDirection);
            movingForward = moveDirection == 1;

            SetTurnRight(80);
            WaitFor(new CodeCeyTurnCompleteCondition(this));

            SetTurnLeft(160);
            WaitFor(new CodeCeyTurnCompleteCondition(this));

            SetTurnRight(160);
            WaitFor(new CodeCeyTurnCompleteCondition(this));
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

        // Jangan pakai TurnGunRight blocking.
        // Pakai SetTurnGunRight supaya movement tetap jalan.
        SetTurnGunRight(gunBearing);

        double absGunBearing = Math.Abs(gunBearing);
        double firePower = ChooseFirePower(distance, e.Energy);

        // Greedy feasibility:
        // Tembak hanya kalau peluang kena cukup masuk akal.
        // Dekat boleh agak toleran, jauh harus lebih presisi.
        bool gunReadyForClose = distance < 180 && absGunBearing < 18;
        bool gunReadyForMid = distance < 450 && absGunBearing < 10;
        bool gunReadyForFar = distance < 700 && absGunBearing < 5;

        bool canShoot = Energy > firePower + 1.0;
        bool shouldShoot = canShoot && (gunReadyForClose || gunReadyForMid || gunReadyForFar);

        if (shouldShoot)
        {
            Fire(firePower);
        }

        // Kalau musuh terlalu dekat, jangan diam.
        // Ram hanya kalau musuh lemah dan energi kita jauh lebih unggul.
        if (distance < 90)
        {
            if (e.Energy <= 7 && Energy > e.Energy + 15)
            {
                TurnRight(bodyBearing);
                Forward(120);
            }
            else
            {
                QuickDodge(bodyBearing);
            }
        }
        else if (distance < 180 && scanCount % 3 == 0)
        {
            // Dodge ringan, tidak setiap scan.
            QuickDodge(bodyBearing);
        }
    }

    private double ChooseFirePower(double distance, double enemyEnergy)
    {
        double power;

        // Dekat: peluang kena tinggi, ambil bullet damage.
        if (distance < 90)
            power = 3.0;
        else if (distance < 160)
            power = 2.4;
        else if (distance < 260)
            power = 1.9;
        else if (distance < 420)
            power = 1.35;
        else if (distance < 650)
            power = 0.9;
        else
            power = 0.55;

        // Kalau musuh lemah, jangan overkill.
        if (enemyEnergy <= 3)
            power = Math.Min(power, 0.6);
        else if (enemyEnergy <= 7)
            power = Math.Min(power, 1.0);
        else if (enemyEnergy <= 12)
            power = Math.Min(power, 1.4);

        // Kalau energi kita rendah, hemat supaya tetap hidup.
        if (Energy < 12)
            power = Math.Min(power, 0.35);
        else if (Energy < 25)
            power = Math.Min(power, 0.75);
        else if (Energy < 40)
            power = Math.Min(power, 1.2);

        return Clamp(power, 0.35, 3.0);
    }

    private void QuickDodge(double enemyBearing)
    {
        // Dodge menyamping dari arah musuh.
        // Lebih aman daripada mundur lurus terus.
        if (enemyBearing >= 0)
            SetTurnLeft(70);
        else
            SetTurnRight(70);

        if (IsNearWall())
        {
            ReverseDirection();
            SetBack(180);
        }
        else
        {
            if (dodgeDirection > 0)
                SetForward(220);
            else
                SetBack(220);

            dodgeDirection *= -1;
        }
    }

    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Kalau kena peluru, ganti arah.
        ReverseDirection();

        double bulletBearing = CalcBearing(e.Bullet.Direction);

        if (bulletBearing >= 0)
            SetTurnLeft(70);
        else
            SetTurnRight(70);

        if (IsNearWall())
            SetBack(220);
        else
            SetForward(220);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        ReverseDirection();
        SetTurnRight(90);
    }

    public override void OnHitBot(HitBotEvent e)
    {
        double distance = DistanceTo(e.X, e.Y);
        double bearing = BearingTo(e.X, e.Y);
        double gunBearing = GunBearingTo(e.X, e.Y);

        SetTurnGunRight(gunBearing);

        // Kalau sangat dekat, peluang tembak kena tinggi.
        if (distance < 100 && Math.Abs(gunBearing) < 20 && Energy > 15)
        {
            Fire(Energy > 35 ? 2.0 : 1.0);
        }

        // Ram cuma kalau musuh lemah.
        if (distance < 80 && e.Energy <= 7 && Energy > e.Energy + 15)
        {
            TurnRight(bearing);
            Forward(120);
        }
        else
        {
            ReverseDirection();
        }
    }

    private void ReverseDirection()
    {
        moveDirection *= -1;

        if (movingForward)
        {
            SetBack(40000);
            movingForward = false;
        }
        else
        {
            SetForward(40000);
            movingForward = true;
        }
    }

    private bool IsNearWall()
    {
        const double margin = 90;
        return X < margin || Y < margin || X > ArenaWidth - margin || Y > ArenaHeight - margin;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}

public class CodeCeyTurnCompleteCondition : Condition
{
    private readonly Bot bot;

    public CodeCeyTurnCompleteCondition(Bot bot)
    {
        this.bot = bot;
    }

    public override bool Test()
    {
        return bot.TurnRemaining == 0;
    }
}