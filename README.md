# Tubes_BukanOrangKampung

<<<<<<< HEAD
Repository Tugas Besar IF25-21013 Strategi Algoritma: pemanfaatan algoritma greedy untuk bot Robocode Tank Royale.
=======
Repository Tugas Besar IF25-21013 Strategi Algoritma: pemanfaatan algoritma greedy untuk membuat bot Robocode Tank Royale.
>>>>>>> 4e6a780599d70787268a051d127fc2f2fd4f2bef

## Struktur Repository

```text
Tubes_BukanOrangKampung/
├── src/
│   ├── main-bot/
<<<<<<< HEAD
│   │   └── Bot1/
│   └── alternative-bots/
│       ├── alt-bot-1/CodeCey/
│       ├── alt-bot-2/AdaptiveBot/
=======
│   │   └── FIX/
│   └── alternative-bots/
│       ├── alt-bot-1/AdaptiveBot/
│       ├── alt-bot-2/Bot1/
>>>>>>> 4e6a780599d70787268a051d127fc2f2fd4f2bef
│       └── alt-bot-3/Bot2/
├── doc/
│   └── README.md
├── build-all.sh
├── build-all.cmd
└── README.md
```

## Bot Utama

<<<<<<< HEAD
### Bot1
Strategi greedy utama: **Greedy Aggressive Distance Shooter**.

Heuristic:
1. Jika jarak musuh dekat, gunakan power peluru besar.
2. Jika jarak musuh sedang, gunakan power sedang.
3. Jika jarak musuh jauh, gunakan power kecil.
4. Jika musuh sangat dekat, bot mundur dan berbelok untuk mengurangi risiko tabrakan.
5. Jika menabrak wall, bot mundur dan berbelok.

## Bot Alternatif

### alt-bot-1: CodeCey
Heuristic utama: movement aktif, radar scanning, firing berdasarkan peluang hit, dan dodge ketika musuh dekat.

### alt-bot-2: AdaptiveBot
Heuristic utama: survival melalui adaptive movement dan wall avoidance.
=======
### FIX
Strategi greedy utama: **Adaptive Bot1 Greedy Radar**.

Heuristic:
1. Jika bot dekat wall, bot langsung bergerak ke tengah arena.
2. Jika musuh terdeteksi, gun diarahkan ke musuh dan bot langsung menembak.
3. Power peluru dipilih berdasarkan jarak musuh, energi musuh, energi bot, dan jumlah musuh tersisa.
4. Jika musuh dekat, bot melakukan dodge agar tidak tertabrak.
5. Jika terkena peluru, bot mengubah arah dan bergerak untuk menghindari tembakan lanjutan.

Tujuan strategi ini adalah mengoptimalkan skor dari bullet damage, survival score, dan peluang finishing tanpa terlalu boros energi.

## Bot Alternatif

### alt-bot-1: AdaptiveBot
Heuristic utama: mengutamakan survival dengan wall avoidance dan movement acak.

### alt-bot-2: Bot1
Heuristic utama: menyerang agresif berdasarkan jarak musuh.
>>>>>>> 4e6a780599d70787268a051d127fc2f2fd4f2bef

### alt-bot-3: Bot2
Heuristic utama: strafe movement setelah menembak dan fire power dinamis berdasarkan jarak.

## Requirement

- .NET 10.0
- Robocode Tank Royale Bot API `0.41.0`
- Microsoft.Extensions.Configuration.Binder `10.0.0`
<<<<<<< HEAD
=======
- Robocode Tank Royale engine dari starter pack tugas besar
>>>>>>> 4e6a780599d70787268a051d127fc2f2fd4f2bef

## Build Semua Bot

Mac/Linux:

```bash
chmod +x build-all.sh
./build-all.sh
```

Windows:

```cmd
build-all.cmd
```

## Run Bot Utama

```bash
<<<<<<< HEAD
cd src/main-bot/Bot1
chmod +x Bot1.sh
./Bot1.sh
=======
cd src/main-bot/FIX
chmod +x FIX.sh
./FIX.sh
```

Atau manual:

```bash
cd src/main-bot/FIX
dotnet build
dotnet run --no-build
```

## Run Bot Alternatif

Contoh:

```bash
cd src/alternative-bots/alt-bot-1/AdaptiveBot
dotnet build
dotnet run --no-build
>>>>>>> 4e6a780599d70787268a051d127fc2f2fd4f2bef
```

## Author

Kelompok: **BukanOrangKampung**

Anggota:
<<<<<<< HEAD
1. Cey
2. TODO: isi nama anggota
3. TODO: isi nama anggota
=======
1. M.Rifat Syauki (124140138)
2. Pina Ramanda (124140170)
3. Chesya Margaretha Deto (124140098)
   
>>>>>>> 4e6a780599d70787268a051d127fc2f2fd4f2bef
