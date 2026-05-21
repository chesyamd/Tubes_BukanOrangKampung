using System;
using Robocode.TankRoyale.BotApi.Graphics;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Bot1 : Bot
{
    // Method utama untuk menjalankan bot
    static void Main(string[] args)
    {
        new Bot1().Start();
    }

    // Constructor bot, mengambil konfigurasi dari file Bot1.json
    Bot1() : base(BotInfo.FromFile("Bot1.json")) { }

    public override void Run()
    {
        // Mengatur warna bot agar mudah dikenali di arena
        BodyColor = Color.Pink;
        GunColor = Color.Black;
        RadarColor = Color.Cyan;

        // Selama bot masih hidup, bot akan terus berputar
        // Tujuannya agar radar/body bisa terus mencari posisi musuh
        while (IsRunning)
        {
            TurnRight(360); // Strategi greedy scan: terus mencari musuh di sekitar arena
        }
    }

    // Method ini akan otomatis dipanggil ketika bot berhasil memindai musuh
    public override void OnScannedBot(ScannedBotEvent e)
    {
        // Menghitung jarak bot kita ke musuh yang terdeteksi
        double distance = DistanceTo(e.X, e.Y);

        // Strategi greedy attack:
        // Bot langsung memilih power tembakan berdasarkan jarak musuh.
        // Semakin dekat musuh, semakin besar power peluru yang digunakan.
        if (distance < 150)
        {
            Fire(3);        // Musuh dekat, peluang kena besar, jadi pakai power tinggi
            Forward(20);    // Maju sedikit untuk memberi tekanan ke musuh
        }
        else if (distance < 400)
        {
            Fire(2);        // Musuh jarak sedang, pakai power sedang agar tetap efektif
            Forward(40);    // Maju untuk mendekati musuh dan menjaga tekanan
        }
        else
        {
            Fire(1);        // Musuh jauh, pakai power kecil agar energi tidak cepat habis
            Forward(60);    // Maju lebih jauh agar jarak ke musuh berkurang
        }

        // Strategi greedy movement:
        // Jika musuh terlalu dekat, bot mundur agar tidak mudah ditabrak atau ditembak dekat.
        if (distance < 100)
        {
            Back(80);       // Mundur untuk menjaga jarak aman
            TurnRight(45);  // Berbelok agar posisi bot tidak mudah diprediksi
        }
        else
        {
            // Jika jarak masih aman, arahkan gun ke musuh lalu maju sedikit
            TurnGunTo(e.X, e.Y);
            Forward(30);
        }
    }

    // Fungsi bantuan untuk mengarahkan gun ke posisi musuh
    private void TurnGunTo(double x, double y)
    {
        // Menghitung sudut yang dibutuhkan gun agar mengarah ke koordinat musuh
        double angle = GunBearingTo(x, y);

        // Memutar gun sesuai sudut yang sudah dihitung
        TurnGunRight(angle);
    }

    // Method ini dipanggil ketika bot bertabrakan dengan bot lain
    public override void OnHitBot(HitBotEvent e)
    {
        // Saat tabrakan, musuh pasti berada sangat dekat,
        // jadi bot langsung menembak dengan power besar
        Fire(3);

        // Setelah itu bot mundur agar tidak terus menempel dengan musuh
        Back(50);
    }

    // Method ini dipanggil ketika bot menabrak dinding arena
    public override void OnHitWall(HitWallEvent e)
    {
        // Bot mundur agar keluar dari area dinding
        Back(100);

        // Bot berbelok agar tidak menabrak dinding yang sama lagi
        TurnRight(90);
    }
}
