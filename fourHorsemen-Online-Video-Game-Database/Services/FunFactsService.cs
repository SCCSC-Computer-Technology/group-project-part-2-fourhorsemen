namespace fourHorsemen_Online_Video_Game_Database.Services
{
    public class FunFactsService
    {
        private static readonly List<string> _facts = new()
        {
             "Origins & Odd Histories\n",
    "Super Mario Bros. 2: Was originally Doki Doki Panic in Japan.",
    "Final Fantasy’s Name: Named 'Final' because it was supposed to be Square’s last game.",
    "Pokémon Mascot Switch: Clefairy was almost the mascot before Pikachu took over.",
    "Portal Origins: Started as a student project called Narbacular Drop.",
    "Dragon King: Prototype of Smash Bros. was called Dragon King.",
    "Tomb Raider Gender Switch: Lara Croft was originally male.",
    "Animal Crossing N64: Originally made for the Nintendo 64 with a real-time clock.",
    "Crash Bandicoot: Designed to rival Mario and Sonic.",
    "Star Fox 2: Canceled after completion for marketing reasons.",
    "Pac-Man AI: Each ghost in Pac-Man has unique behavior.",
    "Tetris: Created in Soviet Russia.",
    "Doom Everywhere: Can run on calculators, printers—even a pregnancy test.",
    "Sonic’s Shoes: Inspired by Michael Jackson and Santa Claus.",
    "Mario’s Last Name: It’s Mario Mario.",
    "Cloud the Plumber: Cloud was originally designed as a plumber.",
    "Game Boy War Damage: One survived a Gulf War bombing and still worked.",
    "Konami Code: Used in over 100 games.",
    "Civ II Pacifist Win: Players found a way to win without violence.",
    "Sonic’s Egg Racer: Sonic was originally part of a game about eggs.",
    "Silent Hill Fog: Used to mask hardware limitations.",
    "Dreamcast’s OS: Ran on Windows CE.",
    "Microsoft x Nintendo: Microsoft once tried to buy Nintendo.",
    "PS2 Export Ban: PS2 was so powerful, it was restricted for military export.",
    "Crash Nickname: Developers called Crash 'Sonic's Ass Game'.",
    "Smash Bros. Almost Canceled: Low budget nearly killed the project.",
    "Wii Remotes in Surgery: Used in medical training via Trauma Center.\n\n",

       "Secrets, Easter Eggs & Hidden Content\n",
    "Arkham Asylum: Had a hidden sequel teaser room.",
    "GTA V Alien: Frozen alien found early in the game.",
    "Halo 3: Dev’s birthday message hidden inside.",
    "Minecraft Herobrine: Mythical figure rumored but never confirmed.",
    "Hitman Seal: A cheat lets you ride a seal.",
    "Mario 64 L is Real: A stone sign sparked decades of myths.",
    "Zelda Glitches: Ocarina of Time can be completed in minutes.",
    "MGS3 Aging Boss: You can let a boss die of old age.",
    "Sims Hidden Content: Features reapers, Satanic content, and more.",
    "Doom II Devil: Satan's face appears in the wall.",
    "Portal Cake: Features a cake recipe and eerie hints.",
    "Vault Boy: He's measuring radiation—not giving a thumbs up.",
    "NBA Jam Cheat: Game was rigged against the Bulls.",
    "Far Lands: A glitch world in Minecraft.",
    "Monster Dating: You can romance monsters in Stardew (mods).",
    "God in Halo: Appears briefly in the loading screen.",
    "Dreamcast Online: Had internet access before it was common.",
    "Mario’s Voice: Same actor also voices Wario.\n\n",

        "Famous Fails & Weird Risks\n",
    "E.T. Landfill: Millions of cartridges were buried in the desert.",
    "Nintendo & Sony: Their failed CD collaboration led to PlayStation.",
    "Mass Effect 3: Fan backlash led to a rewritten ending.",
    "Zelda CD-i: Widely considered the worst Zelda games.",
    "Kinect Dance: Star Wars Kinect had a dance mini-game.",
    "Kinect Clothing Fail: Struggled with reading dark clothes.",
    "FFVIII Spells: Used actual math formulas to scale spells.",
    "Sega Saturn Launch: Surprise launch confused retailers.",
    "X Æ A-12: Dev jokingly said they'd name their kid after a variable.",
    "Resident Evil Ghosts: Early builds had paranormal elements.",
    "FFXIV Relaunch: Game was so broken they had to restart it.",
    "Smash Bros. Voice: Lack of voice acting was due to budget.",
    "Aerith’s Death: Happened partly due to memory limitations.",
    "GTA Alien Again: Alien found dead in desert.",
    "Dreamcast Flop: Innovative but failed due to bad timing.\n\n",

     "Hardware & Tech Trivia\n",
    "PS2 was stronger than some military-grade computers.",
    "Game Boy survived a bombing and still worked.",
    "Dreamcast included built-in online support.",
    "GameCube was built with a carry handle.",
    "Wii MotionPlus added precision with a snap-on accessory.",
    "NES Zapper doesn’t work on modern screens.",
    "Doom even runs on toasters.",
    "GameCube startup had Easter egg chimes.",
    "SNES CD add-on eventually evolved into the PlayStation.",
    "Old arcade cabinets were often repurposed.\n\n",

          "Speedruns, Glitches & Alternate Playstyles\n",
    "Ocarina of Time was beaten in under 17 minutes.",
    "Half-Life was completed without using weapons.",
    "In MGS3, the aging boss can be defeated by waiting.",
    "Final Fantasy used real math for some spells.",
    "GTA speedruns include chaos and peaceful categories.",
    "Doom was completed without shooting any weapons.",
    "Civilization II was won without going to war.",
    "A Pokémon game was beaten with no battles.",
    "Someone walked for hours into Minecraft’s Far Lands.",
    "Dark Souls was beaten using a Rock Band guitar.\n\n",

        "Characters & Creators\n",
    "Mario’s full name is Mario Mario.",
    "Sonic’s shoes were inspired by Michael Jackson and Santa Claus.",
    "Bowser’s design was inspired by an ox from Japanese folklore.",
    "Lara Croft was originally designed as a male character.",
    "Charles Martinet voices both Mario and Wario.",
    "Aerith’s death in FF7 was due to memory constraints.",
    "Vault Boy is checking radiation, not giving a thumbs-up.",
    "John Romero’s head is hidden as a final boss in Doom.",
    "FF creator Hironobu Sakaguchi left after achieving success."

        };

        private static readonly Random _random = new();

        public static List<string> GetAllFacts()
        {
            return _facts;
        }

        public static string GetRandomFact()
        {
            int index = _random.Next(_facts.Count);
            return _facts[index];
        }
    }
}
