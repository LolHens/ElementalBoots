using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using LitJson;

using TAPI;
using Terraria;
using LolHens.Items;
using LolHens.Projectiles;
using LolHens.Buffs;
using LolHens.NPCs;

namespace LolHens
{
    public class LolHensBase : TAPI.ModBase
    {
        public static LolHensBase instance;

        public List<LolHensItem> items = new List<LolHensItem>();
        public List<LolHensProjectile> projectiles = new List<LolHensProjectile>();
        public List<LolHensNPC> npcs = new List<LolHensNPC>();

        public LolHensBase() : base() { instance = this; }

        public override void OnLoad()
        {
        }

        public override void OnAllModsLoaded()
        {
            NPCDef.byName["Vanilla:Illuminant Bat"].AddDrop(ItemDef.byName["LolHens:IlluminantPearl"], 0.0167f);
            NPCDef.byName["Vanilla:Illuminant Slime"].AddDrop(ItemDef.byName["LolHens:IlluminantPearl"], 0.0167f);
            NPCDef.byName["Vanilla:Eyezor"].AddDrop(ItemDef.byName["LolHens:BrokenHeroWings"], 0.004f);
            NPCDef.byName["Vanilla:King Slime"].AddDrop(ItemDef.byName["Vanilla:Gel"], 1, 40, 80);

            ItemDef.byName["Vanilla:Acorn"].MakeAmmo("Acorn");
            ItemDef.byName["Vanilla:Spiky Ball"].MakeAmmo("SpikyBall");
            ItemDef.byName["Vanilla:Cannonball"].MakeAmmo("Cannonball");

            ItemDef.byName["LolHens:TornadoInABottle"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.SKY, 0.3f, true);
            ItemDef.byName["LolHens:Slingshot"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.SURFACE, 0.1f, true);
            ItemDef.byName["Vanilla:Acorn"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.SURFACE, 0.2f, false, 10, 40);
            ItemDef.byName["LolHens:Magnet"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.UNDERGROUND, 0.1f, true);
            ItemDef.byName["LolHens:Extinguisher"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.UNDERGROUND, 0.1f, true);
            ItemDef.byName["LolHens:SpikyBallBlaster"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.CAVERN, 0.1f, true);
            //ItemDef.byName["LolHens:IceStorm"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.CAVERN, 0.1f, true);
            ItemDef.byName["LolHens:FrostSlimeWand"].MakeChestLoot(chestInfo => chestInfo.GetHeight() == ChestInfo.Height.CAVERN, 0.1f, true);
        }

        public override object OnModCall(TAPI.ModBase mod, params object[] args)
        {
            return base.OnModCall(mod, args);
        }
    }

    public static class LolHensBaseExtensions
    {
        public static void Resize<Key, Value>(this Dictionary<Key, Value> unused, ref Dictionary<Key, Value> dictionary, int newSize)
        {
            Dictionary<Key, Value> newDict = new Dictionary<Key, Value>(newSize);
            foreach (KeyValuePair<Key, Value> keyValPair in dictionary)
                newDict.Add(keyValPair.Key, keyValPair.Value);
            dictionary = newDict;
        }

        public static int Capacity<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            FieldInfo entriesField = source.GetType().GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
            if (entriesField == null) return 0;
            var entries = entriesField.GetValue(source) as Array;
            if (entries == null) return 0;
            return entries.Length;
        }


        public static void MakeChestLoot(this Item item, Func<ChestInfo, bool> filter, float chance = 1, bool rare = false, int numFrom = 1, int numTo = 1)
        {
            ChestInfo.Loot.Add(item, filter, chance, rare, numFrom, numTo);
        }

        public static void MakeAmmo(this Item item, String ammoName)
        {
            if (!ammoName.Contains(":")) ammoName = LolHensBase.instance.mod.InternalName + ":" + ammoName;
            if (!ItemDef.ammo.ContainsKey(ammoName)) ItemDef.ammo.Add(ammoName, 1000 + ItemDef.ammo.Count);
            item.ammo = ItemDef.ammo[ammoName];
        }

        public static void AddDrop(this NPC npc, Item item, float chance = 1, int numFrom = 1, int numTo = -1)
        {
            String drop = "{\"drop\":{\"item\":\"" + item.name + "\",";
            if (numTo == -1)
                drop = drop + "\"stack\":" + numFrom + ",";
            else
                drop = drop + "\"stack\":[" + numFrom + "," + numTo + "],";
            drop = drop + "\"chance\":" + chance.ToString().Replace(",", ".") + "}}";

            JsonData drops;
            if (NPCDef.npcDrops.ContainsKey(npc.type))
                drops = NPCDef.npcDrops[npc.type];
            else
                drops = new JsonData();
            drops.Add(JsonMapper.ToObject(drop)["drop"]);
            NPCDef.npcDrops[npc.type] = drops;
        }

        public static float Interpolate(this float[] array, float index, int[] translation = null)
        {
            int length = array.Length;
            if (translation == null)
            {
                translation = new int[array.Length];
                for (int i = 0; i < translation.Length; i++) translation[i] = i;
            }
            else
            {
                length = Math.Min(length, translation.Length);
            }
            int floor = (int)index;
            float interp = index - floor;
            float val1 = array[translation[floor % length]];
            float val2 = array[translation[(floor + 1) % length]];
            return val1 + interp * (val2 - val1);
        }

        public static LolHensItem AsLolHensItem(this Item item)
        {
            if (item != null)
            {
                ModItem modItem = item.GetSubClass<ModItem>();
                if (modItem != null && modItem is LolHensItem) return modItem as LolHensItem;
            }
            return null;
        }

        public static LolHensItem GetWingsItem(this Player player)
        {
            if (player.wings != 0) foreach (LolHensItem item in LolHensBase.instance.items) if (item.item.wingSlot == player.wings) return item;
            return null;
        }

        public static int AddBuff(this CodableEntity entity, string name, int time, CodableEntity trigger, bool quiet = true, bool resetTimer = true)
        {
            if (!name.Contains(":")) name = "Vanilla:" + name;
            return AddBuff(entity, BuffDef.byName[name], time, trigger, quiet, resetTimer);
        }

        public static int AddBuff(this CodableEntity entity, int type, int time, CodableEntity trigger, bool quiet = true, bool resetTimer = true)
        {
            int ret = -1;
            if (!resetTimer && entity.HasBuff(type)) return ret;
            LolHensBuff.lastTrigger = trigger;
            if (entity is Player) ret = (entity as Player).AddBuff(type, time, quiet);
            else if (entity is NPC) ret = (entity as NPC).AddBuff(type, time, quiet);
            LolHensBuff.lastTrigger = null;
            return ret;
        }

        public static LolHensProjectile AsLolHensProjectile(this Projectile projectile)
        {
            if (projectile != null)
            {
                ModProjectile modProjectile = projectile.GetSubClass<ModProjectile>();
                if (modProjectile != null && modProjectile is LolHensProjectile) return modProjectile as LolHensProjectile;
            }
            return null;
        }

        public static LolHensProjectile New(this Projectile projectile, float x, float y, float speedX, float speedY, int damage, float knockBack, int owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            return Main.projectile[Projectile.NewProjectile(x, y, speedX, speedY, projectile.type, damage, knockBack, owner, ai0, ai1)].AsLolHensProjectile();
        }

        public static void UseAmmo(this Player player)
        {
            if (player.inventory[player.selectedItem].useAmmo == 0) return;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ammo == player.inventory[player.selectedItem].useAmmo && player.inventory[i].stack > 0)
                {
                    player.inventory[i].stack--;
                    if (player.inventory[i].stack == 0) player.inventory[i].SetDefaults(0);
                    return;
                }
            }
        }

        public static bool isEnemy(this CodableEntity entity)
        { // TODO WIP
            if (!entity.active) return false;
            if (entity is NPC)
            {
                NPC npc = entity as NPC;
                if (npc.dontTakeDamage) return false;
                if (npc.friendly) return false;
            }
            else if (entity is Player)
            {
                Player player = entity as Player;
                return false;
            }
            return false;
        }

        public static LolHensNPC AsLolHensNPC(this NPC npc)
        {
            if (npc != null)
            {
                ModNPC modNPC = npc.GetSubClass<ModNPC>();
                if (modNPC != null && modNPC is LolHensNPC) return modNPC as LolHensNPC;
            }
            return null;
        }

        public static int BuffIndex(this CodableEntity entity, int buff)
        {
            if (entity is Player)
            {
                Player player = entity as Player;
                for (int i = 0; i < player.buffType.Length; i++) if (player.buffType[i] == buff) return i;
            }
            else if (entity is NPC)
            {
                NPC npc = entity as NPC;
                for (int i = 0; i < npc.buffType.Length; i++) if (npc.buffType[i] == buff) return i;
            }
            return -1;
        }

        public static bool HasBuff(this CodableEntity entity, int buff)
        {
            return BuffIndex(entity, buff) > -1;
        }

        public static void AddPet(this Player player, NPC npc)
        {
            LolHensPet pet = npc.AsLolHensNPC() as LolHensPet;
            if (pet != null) player.AddBuff(pet.petBuff, 100);
        }

        public static void SetFrameGun(this Player player, Item item)
        {
            player.bodyFrame = SetFrameGun(player.bodyFrame, player.itemRotation, player.direction, player.gravDir, item);
        }

        /*
         * AUTHOR: Grox the Great
         * Change the arm frame in the same way that useStyle 5 would.
         */
        public static Rectangle SetFrameGun(Rectangle bodyFrame, float itemRotation, int direction, float gravDir, Item item)
        {
            float FacingRotation = itemRotation * direction;
            bodyFrame.Y = bodyFrame.Height * 3;
            if (FacingRotation < -0.75f)
            {
                bodyFrame.Y = bodyFrame.Height * 2;
                if (gravDir == -1.0f) bodyFrame.Y = bodyFrame.Height * 4;
            }
            if (FacingRotation > 0.6f)
            {
                bodyFrame.Y = bodyFrame.Height * 4;
                if (gravDir == -1.0f) bodyFrame.Y = bodyFrame.Height * 2;
            }
            return bodyFrame;
        }
    }
}