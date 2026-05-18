# Tubes_BukanOrangKampung

Repository Tugas Besar IF25-21013 Strategi Algoritma: pemanfaatan algoritma greedy untuk membuat bot Robocode Tank Royale.

## Struktur Repository

```text
Tubes_BukanOrangKampung/
├── src/
│   ├── main-bot/
│   │   └── Bot1/
│   └── alternative-bots/
│       ├── alt-bot-1/
│       │   └── CodeCey/
│       ├── alt-bot-2/
│       │   └── AdaptiveBot/
│       └── alt-bot-3/
│           └── Bot2/
├── doc/
│   └── BukanOrangKampung.pdf
├── build-all.sh
├── build-all.cmd
└── README.md
```

## Penjelasan Singkat Algoritma Greedy

Pada tugas besar ini, setiap bot menggunakan strategi greedy untuk memilih aksi terbaik pada kondisi saat itu. Keputusan greedy dilakukan berdasarkan beberapa heuristic, seperti jarak musuh, energi bot, energi musuh, risiko tabrakan, posisi terhadap wall, dan peluang tembakan mengenai target.

Tujuan utama strategi greedy yang digunakan adalah memaksimalkan skor akhir melalui bullet damage, survival score, last survival bonus, dan ram damage jika kondisinya menguntungkan.

## Bot Utama

### Bot1 — Greedy Aggressive Distance Shooter

Bot1 digunakan sebagai bot utama. Strategi greedy pada Bot1 berfokus pada serangan berdasarkan jarak musuh. Bot akan terus melakukan scanning, lalu ketika musuh terdeteksi, bot memilih power peluru yang dianggap paling menguntungkan pada saat itu.

Heuristic yang digunakan:
1. Jika musuh berada pada jarak dekat, bot menggunakan power peluru besar.
2. Jika musuh berada pada jarak sedang, bot menggunakan power peluru sedang.
3. Jika musuh berada pada jarak jauh, bot menggunakan power peluru kecil.
4. Jika musuh terlalu dekat, bot bergerak mundur dan berbelok untuk mengurangi risiko tabrakan.
5. Jika bot menabrak wall, bot mundur dan berbelok agar tidak terus terkena wall damage.

Strategi ini dipilih sebagai bot utama karena sederhana, agresif, dan langsung berorientasi pada peningkatan bullet damage.

## Bot Alternatif

### alt-bot-1: CodeCey — Greedy Smart Gunner

CodeCey menggunakan strategi greedy yang menggabungkan movement aktif, pemindaian radar, dan keputusan menembak berdasarkan peluang hit.

Heuristic yang digunakan:
1. Bot bergerak aktif agar tidak menjadi target diam.
2. Radar digunakan untuk memindai musuh secara terus-menerus.
3. Ketika musuh terpindai, bot mengarahkan gun ke posisi musuh.
4. Bot hanya menembak jika arah gun dianggap cukup tepat.
5. Jika musuh terlalu dekat, bot melakukan dodge.
6. Ram hanya dilakukan jika kondisi musuh lemah dan menguntungkan.

### alt-bot-2: AdaptiveBot — Adaptive Greedy Survivor

AdaptiveBot menggunakan strategi greedy yang lebih berfokus pada survival. Bot melakukan movement acak agar sulit diprediksi, tetapi tetap memiliki mekanisme untuk menghindari wall.

Heuristic yang digunakan:
1. Jika bot mendekati wall, bot langsung bergerak menjauh ke arah tengah arena.
2. Jika posisi bot aman, bot bergerak secara acak agar sulit ditebak.
3. Jika musuh terpindai, bot memilih power peluru berdasarkan jarak musuh.
4. Jika energi bot rendah, bot menggunakan power peluru kecil.
5. Jika bot menabrak wall atau bot lain, bot membalik arah.

### alt-bot-3: Bot2 — Greedy Strafe Shooter

Bot2 menggunakan strategi greedy dengan pola strafe movement. Setelah musuh terpindai, bot akan mengarahkan gun, menembak berdasarkan jarak, lalu bergerak menyamping untuk menghindari tembakan lawan.

Heuristic yang digunakan:
1. Jika musuh dekat, bot menembak dengan power besar.
2. Jika musuh berada pada jarak sedang, bot menembak dengan power sedang.
3. Jika musuh jauh, bot menembak dengan power kecil.
4. Setelah menembak, bot melakukan strafe movement.
5. Jika menabrak wall, bot mundur dan berbelok.

## Requirement Program

Program membutuhkan:
- .NET 10.0
- Robocode Tank Royale Bot API 0.41.0
- Microsoft.Extensions.Configuration.Binder 10.0.0
- Robocode Tank Royale engine dari starter pack tugas besar

## Cara Build Semua Bot

### Mac/Linux

```bash
chmod +x build-all.sh
./build-all.sh
```

### Windows

```cmd
build-all.cmd
```

## Cara Menjalankan Bot Utama

Masuk ke folder bot utama:

```bash
cd src/main-bot/Bot1
```

Jalankan bot:

```bash
chmod +x Bot1.sh
./Bot1.sh
```

Atau manual:

```bash
dotnet build
dotnet run --no-build
```

## Cara Menjalankan Bot Alternatif

### CodeCey

```bash
cd src/alternative-bots/alt-bot-1/CodeCey
dotnet build
dotnet run --no-build
```

### AdaptiveBot

```bash
cd src/alternative-bots/alt-bot-2/AdaptiveBot
dotnet build
dotnet run --no-build
```

### Bot2

```bash
cd src/alternative-bots/alt-bot-3/Bot2
dotnet build
dotnet run --no-build
```

## Author

Kelompok: **BukanOrangKampung**

Anggota:
1. M. Rifat Syauki (124140138)
2. Pina Ramanda (124140170)
3. Chesya Margaretha Deto (124140098)
