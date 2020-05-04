using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Utils : MonoBehaviour
{
    public static Utils Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            PlayerData = new DataStorage();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public DataStorage PlayerData;
    public TextMeshProUGUI GoldText;
    public Image GoldIcon;
    public TextMeshProUGUI WaveText;
    public Sprite[] ItemSprites;
    public SpawnArea[] SpawnAreas;
    public GameObject GangsterEnemy;
    public GameObject GamerEnemy;
    public GameObject ShotgunEnemy;
    private Weapon[] WeaponPrefabs;
    public static Sprite[] GoldIcons;
    public static GameObject[] Bullets;
    private static int livingEnemies;
    private static Canvas guiCanvas = null;
    public static Canvas GUICanvas
    {
        get
        {
            if (guiCanvas == null)
                guiCanvas = FindObjectOfType(typeof(Canvas)) as Canvas;

            if (guiCanvas == null)
            {
                var obj = new GameObject("Canvas");
                guiCanvas = obj.AddComponent<Canvas>();
            }
            return guiCanvas;
        }
    }

    void Start()
    {
        setItemInfos();

        object[] loadedCoins = Resources.LoadAll("Coins", typeof(Sprite));
        GoldIcons = new Sprite[loadedCoins.Length];
        for(int x = 0; x < loadedCoins.Length; x++)
        {
            GoldIcons[x] = (Sprite)loadedCoins[x];
        }

        object[] loadedBullets = Resources.LoadAll("Bullets", typeof(GameObject));
        Bullets = new GameObject[loadedBullets.Length];
        for(int x = 0; x < loadedBullets.Length; x++)
        {
            Bullets[x] = (GameObject)loadedBullets[x];
        }

        object[] loadedWeapons = Resources.LoadAll("Prefabs/Weapons", typeof(GameObject));
        WeaponPrefabs = new Weapon[loadedWeapons.Length];
        for(int x = 0; x < loadedWeapons.Length; x++)
        {
            WeaponPrefabs[x] = ((GameObject)loadedWeapons[x]).GetComponent<Weapon>();
        }

        object[] loadedItems = Resources.LoadAll("Items", typeof(Sprite));
        ItemSprites = new Sprite[loadedItems.Length];
        for(int x = 0; x < loadedItems.Length; x++)
        {
            ItemSprites[x] = (Sprite)loadedItems[x];
        }
    }

    public void SceneLoad()
    {
        if (GoldText == null) GoldText = GameObject.Find("GoldText").GetComponent<TextMeshProUGUI>();
        if (GoldIcon == null) GoldIcon = GameObject.Find("Gold Image").GetComponent<Image>();
        if (WaveText == null) WaveText = GameObject.Find("WaveText").GetComponent<TextMeshProUGUI>();
        foreach (SpawnArea s in SpawnAreas) DontDestroyOnLoad(s.gameObject);

        if (PlayerData.Left == null && PlayerData.Right == null)
        {
            PlayerData.Left = WeaponPrefabs.First(w => w.name == "BasicWeapon");
        }

        UpdateStats();
    }

    public void AddGold(float amount)
    {
        Instance.PlayerData.Gold += amount;
        GoldText.text = Instance.PlayerData.Gold.ToString();
        GoldIcon.sprite = GetTotalGoldIcon(Instance.PlayerData.Gold);
    }

    public static Vector3 SpriteObjectWorldSize(GameObject origin)
    {
        SpriteRenderer render = origin.GetComponent<SpriteRenderer>();
        Vector3 worldSize = render.sprite.rect.size / render.sprite.pixelsPerUnit * origin.transform.lossyScale;
        return worldSize;
    }

    public static Vector3 SpriteObjectScreenSize(GameObject origin)
    {
        Vector2 screenSize = 0.5f * SpriteObjectWorldSize(origin) / Camera.main.orthographicSize;
        screenSize.y *= Camera.main.aspect;
        return screenSize;
    }

    private static Vector3 guiSpriteObjectScreenPixelSize(GameObject origin, Vector3 worldSize)
    {
        return Camera.main.WorldToScreenPoint(origin.transform.position + worldSize) - Camera.main.WorldToScreenPoint(origin.transform.position - worldSize);
    }

    public static Vector3 GUISpriteObjectScreenPixelSize(GameObject origin)
    {
        return guiSpriteObjectScreenPixelSize(origin, SpriteObjectWorldSize(origin));
    }

    public static float GUIScaleWithSpriteObject(GameObject origin, float width)
    {
        return GUISpriteObjectScreenPixelSize(origin).x / width;
    }

    public static float HitNumberSize(float amount)
    {
        return 20f / (1 + 100 * (float)Math.Pow(2.71828f, -0.2f * amount)) + 32;
    }

    public static float HitNumberAlpha(float s)
    {
        return 255f / (1f + 10f * (float)Math.Pow(2.71828f, 20f * s - 3f));
    }

    public static float HitNumberRandomDirection()
    {
        return (UnityEngine.Random.value - 0.5f) * 6f;
    }

    public static string FormatHealth(HealthManager hpMngr)
    {
        StringBuilder sb = new StringBuilder();
        return sb.Append(hpMngr.Health).Append("/").Append(hpMngr.MaxHealth).ToString();
    }

    public static string FormatAmmo(Weapon weapon)
    {
        StringBuilder sb = new StringBuilder();
        return sb.Append(weapon.Ammo).Append("/").Append(weapon.MaxAmmo).ToString();
    }

    public static Sprite GetGoldIcon(float amount)
    {
        return GoldIcons.First(g => g.name == (Mathf.FloorToInt(Mathf.Min(GoldIcons.Length - 1, amount / 10)) + 1).ToString());
    }

    public static Sprite GetTotalGoldIcon(float amount)
    {
        return GoldIcons.First(g => g.name == (Mathf.FloorToInt(Mathf.Min(GoldIcons.Length - 1, amount / 100)) + 1).ToString());
    }

    public static GameObject GetBulletPrefab(BulletType type)
    {
        return Bullets.First(b => b.GetComponent<Bullet>().Type == type);
    }

    public static void SpawnEnemies()
    {
        foreach (SpawnArea area in Instance.SpawnAreas)
        {
            area.SpawnEnemies(new SpawnInstruction
                {
                    Gangsters = GangstersToSpawn(Instance.PlayerData.Wave),
                    Gamers = GamersToSpawn(Instance.PlayerData.Wave),
                    Shotguns = ShotgunsToSpawn(Instance.PlayerData.Wave),
                });
        }
    }

    static int GangstersToSpawn(float wave)
    {
        return Mathf.FloorToInt(5 / (1 + 0.9f * (float)Math.Pow(2.71828f, -0.8f * wave + 5)) + 1);
    }

    static int GamersToSpawn(float wave)
    {
        return Mathf.FloorToInt(5 / (1 + 0.9f * (float)Math.Pow(2.71828f, -0.3f * wave + 5)));
    }

    static int ShotgunsToSpawn(float wave)
    {
        return Mathf.FloorToInt(5 / (1 + 10f * (float)Math.Pow(2.71828f, -0.2f * wave + 1)));
    }

    public static void AddDeath()
    {
        livingEnemies--;
        if (livingEnemies <= 0)
        {
            Instance.NextWave();
        }
    }

    public static void AddEnemy()
    {
        livingEnemies++;
    }

    public void NextWave()
    {
        PlayerData.Wave++;
        BackMusic(Convert.ToInt32(PlayerData.Wave));
        WaveText.text = "Wave: " + PlayerData.Wave.ToString();
        if (PlayerData.Wave % 5 == 0)
        {
            SceneManager.LoadScene("Shop");
        }
        else
        {
            SpawnEnemies();
        }
    }

    public void ExitShop()
    {
        SceneManager.LoadScene("PlayerTestScene");
    }

    public void BackMusic(int waveNum)
    {
        int trackNum = waveNum % 5;
        if(trackNum == 1)
        {
            Camera.main.GetComponent<AudioSource>().clip = Resources.Load("Music/Wave 1&2", typeof(AudioClip)) as AudioClip;
            Camera.main.GetComponent<AudioSource>().Play();
        }
        else if(trackNum == 0)
        {
            Camera.main.GetComponent<AudioSource>().clip = Resources.Load("Music/Wave 5", typeof(AudioClip)) as AudioClip;
            Camera.main.GetComponent<AudioSource>().Play();
        }
    }

    public static Info.ItemInfo GetItemInfo(Sprite sprite)
    {
        return Instance.ItemInfos.First(ii => ii.Icon == sprite);
    }

    public static Info.ItemInfo GetItemInfo(string name)
    {
        return Instance.ItemInfos.First(ii => ii.Name == name);
    }

    public static Weapon GetWeapon(Sprite sprite)
    {
        return ShopHandler.weapons.First(wi => wi.GetComponent<Weapon>().Icon == sprite).GetComponent<Weapon>();
    }

    public static Weapon GetWeapon(string name)
    {
        return ShopHandler.weapons.First(wi => wi.GetComponent<Weapon>().Name == name).GetComponent<Weapon>();
    }

    public static string FormatColor(string text, string colorhex)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<color=#");
        sb.Append(colorhex);
        sb.Append(">");
        sb.Append(text);
        sb.Append("</color>");
        return sb.ToString();
    }

    public static string FormatGreen(string text)
    {
        return FormatColor(text, "0f0");
    }

    public static string FormatRed(string text)
    {
        return FormatColor(text, "f00");
    }

    public static string FormatPercent(float value)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Mathf.Abs(value * 100));
        sb.Append("%");
        return sb.ToString();
    }

    private static string[] exclusions = new string[] { "Name", "Description", "Icon", "Price" };
    private static string[] isPercent = new string[] { "ShotgunReload", "FlameReload", "SniperReload", "Speed", "Strength", "Dexterity", "EnemySlowness", "SniperShootingSpeed", "Money" };

    public static string AddSpacesToSentence(string text, bool preserveAcronyms = true)
    {
        if (string.IsNullOrWhiteSpace(text))
           return string.Empty;
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public static string GetItemStatText(Info.ItemInfo item)
    {
        StringBuilder sb = new StringBuilder();
        Type type = item.GetType();
        foreach (var field in type.GetFields().Where(f => f.IsPublic))
        {
            if (exclusions.Contains(field.Name))
                continue;

            float value = (float)field.GetValue(item);
            if (value == 0f)
                continue;

            sb.Append(AddSpacesToSentence(field.Name));
            sb.Append(": ");
            string toDisplay = isPercent.Contains(field.Name) ? FormatPercent(value) : value.ToString();
            if (value >= 0f)
                sb.Append(FormatGreen("+" + toDisplay));
            else
                sb.Append(FormatRed("-" + toDisplay));
            sb.Append("\n");
        }
        return sb.ToString();
    }

    public void UpdateStats()
    {
        PlayerData.Stats = new Info.ItemInfo
        {
            Speed = 1,
            Strength = 1,
            FlameReload = 1,
            CrystalRegen = 5,
            Money = 1,
            Dexterity = 0.5f,
        };
        foreach (Info.ItemInfo item in PlayerData.Items)
        {
            PlayerData.Stats += item;
        }
    }

    public Sprite ItemSpriteFromName(string name)
    {
        return ItemSprites.First(s => s.name == name);
    }

    public Info.ItemInfo[] ItemInfos;

    private void setItemInfos()
    {
        ItemInfos = new Info.ItemInfo[]
        {
            new Info.ItemInfo
            {
                Name = "Battery",
                Description = "The battery is charged, and ready for use. Maybe the reload time would go a little faster with that power.",
                Icon = ItemSpriteFromName("battery"),
                Price = 100,
                FlameReload = 0.05f,
                SniperReload = 0.05f,
                ShotgunReload = 0.05f,
                Money = -0.02f,
            },
            new Info.ItemInfo
            {
                Name = "Blue Core",
                Description = "This is a core filled with blue energy. That should be a nice boost for guns that use that color.",
                Icon = ItemSpriteFromName("blue_core"),
                Price = 60,
                BlueDamage = 2,
            },
            new Info.ItemInfo
            {
                Name = "Coffee",
                Description = "Want to get some energy? Coffee should do the trick.",
                Icon = ItemSpriteFromName("coffee"),
                Price = 70,
                Speed = 0.1f,
                Strength = -0.05f,
            },
            new Info.ItemInfo
            {
                Name = "Fuel Tank",
                Description = "So you don't think fire burns enough? Try using some of this. That should make a warm welcome.",
                Icon = ItemSpriteFromName("fuel_tank"),
                Price = 60,
                FireDamage = 0.5f,
                Dexterity = -0.05f,
            },
            new Info.ItemInfo
            {
                Name = "Gas Canister",
                Description = "Flamethrowers are fun. But they run out of gas too fast. Well... they used too.",
                Icon = ItemSpriteFromName("gas_canister"),
                Price = 40,
                FlameReload = 0.1f,
                Dexterity = -0.05f,
            },
            new Info.ItemInfo
            {
                Name = "Green Core",
                Description = "This is a core filled with green energy. That should be a nice boost for guns that use that color.",
                Icon = ItemSpriteFromName("green_core"),
                Price = 60,
                GreenDamage = 1,
            },
            new Info.ItemInfo
            {
                Name = "Jetpack",
                Description = "Guns can be heavy, but don't let that slow you down. A jetpack can carry that weight for you.",
                Icon = ItemSpriteFromName("jetpack"),
                Price = 80,
                Dexterity = 0.12f,
            },
            new Info.ItemInfo
            {
                Name = "Piggy Bank",
                Description = "Here comes the money! Here comes the money! MONEY! MONEY! MONEY!",
                Icon = ItemSpriteFromName("piggy_bank"),
                Price = 100,
                Money = 0.1f,
            },
            new Info.ItemInfo
            {
                Name = "Purple Core",
                Description = "This core is filled with purple energy. That's quite popular with snipers.",
                Icon = ItemSpriteFromName("purple_core"),
                Price = 60,
                PurpleDamage = 4,
            },
            new Info.ItemInfo
            {
                Name = "Red Core",
                Description = "This is a core filled with red energy. That should be a nice boost for guns that use that color.",
                Icon = ItemSpriteFromName("red_core"),
                Price = 60,
                PurpleDamage = 3,
            },
            new Info.ItemInfo
            {
                Name = "Regen Box",
                Description = "That crystal can't protect itself. But this might fix a few of its scratches",
                Icon = ItemSpriteFromName("regen_box"),
                Price = 30,
                CrystalRegen = 5,
            },
            new Info.ItemInfo
            {
                Name = "Shotgun Shells",
                Description = "Oh yeah. This is what makes the shotgun deadly.",
                Icon = ItemSpriteFromName("shotgun_shells"),
                Price = 70,
                ShotgunShells = 1,
            },
            new Info.ItemInfo
            {
                Name = "Hand Weight",
                Description = "Strength is important in combat, but this still weighs a lot.",
                Icon = ItemSpriteFromName("weight"),
                Price = 70,
                Strength = 0.1f,
                MaxHealth = 1,
                Speed = -0.1f,
            },
            new Info.ItemInfo
            {
                Name = "Wrist Watch",
                Description = "Never forget the time. With time on your wrist, its almost like everybody else is slow and easier to hit",
                Icon = ItemSpriteFromName("wrist_watch"),
                Price = 100,
                EnemySlowness = 0.05f,
                SniperShootingSpeed = 0.05f,
            },
        };
    }
}

public class DataStorage
{
    public List<Info.ItemInfo> Items = new List<Info.ItemInfo>();
    public float Health = 100;
    public float MaxHealth = 100;
    public float Gold = 300;
    public float CrystalHealth = 1000;
    public float Wave = 0;
    public Weapon Left;
    public Weapon Right;
    public Info.ItemInfo Stats = new Info.ItemInfo();
}

namespace Info
{
    public class ItemInfo
    {
        public float Speed = 0;
        public float Dexterity = 0;
        public float MaxHealth = 0;
        public float Strength = 0;
        public float FireDamage = 0;
        public float BlueDamage = 0;
        public float RedDamage = 0;
        public float GreenDamage = 0;
        public float PurpleDamage = 0;
        public float FlameReload = 0;
        public float DashReload = 0;
        public float SniperReload = 0;
        public float SniperShootingSpeed = 0;
        public float ShotgunReload = 0;
        public float ShotgunShells = 0;
        public float Money = 0;
        public float CrystalRegen = 0;
        public float DashLength = 0;
        public float EnemySlowness = 0;
        public string Name = "";
        public string Description = "";
        public Sprite Icon;
        public float Price = 0;
        public static ItemInfo operator +(ItemInfo lhs, ItemInfo rhs)
        {
            return new ItemInfo
            {
                Speed = lhs.Speed + rhs.Speed,
                Dexterity = lhs.Dexterity + rhs.Dexterity,
                Strength = lhs.Strength + rhs.Strength,
                FireDamage = lhs.FireDamage + rhs.FireDamage,
                BlueDamage = lhs.BlueDamage + rhs.BlueDamage,
                RedDamage = lhs.RedDamage + rhs.RedDamage,
                GreenDamage = lhs.GreenDamage + rhs.GreenDamage,
                PurpleDamage = lhs.PurpleDamage + rhs.PurpleDamage,
                FlameReload = lhs.FlameReload + rhs.FlameReload,
                DashReload = lhs.DashReload + rhs.DashReload,
                SniperReload = lhs.SniperReload + rhs.SniperReload,
                ShotgunReload = lhs.ShotgunReload + rhs.ShotgunReload,
                ShotgunShells = lhs.ShotgunShells + rhs.ShotgunShells,
                Money = lhs.Money + rhs.Money,
                CrystalRegen = lhs.CrystalRegen + rhs.CrystalRegen,
                DashLength = lhs.DashLength + rhs.DashLength,
                EnemySlowness = lhs.EnemySlowness + rhs.EnemySlowness,
            };
        }
    }

    public class WeaponInfo : MonoBehaviour
    {
        public Sprite Icon;
        public string Name = "";
        public string Description = "";
        public bool Continuous = false;
        [Range(1f, 10f)]
        public int BulletsInVolley = 1;
        [Range(0f, 10f)]
        public float ReloadTime = 1f;
        [Range(0f, 2f)]
        public float TimeBetweenBullets = 0f;
        [Range(0f, 360f)]
        public float BulletSpreadAngle = 0f;
        [Range(0f, 10f)]
        public float ShootingCooldown = 1f;
        public float MaxAmmo;
        public float Price = 0f;
    }
}

public class SpawnInstruction
{
    public int Total { get => Gangsters + Gamers; }
    public int Gangsters = 0;
    public int Gamers = 0;
    public int Shotguns = 0;
}


