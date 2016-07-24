﻿using Terraria;
using Terraria.ModLoader;

namespace ElementalBoots.Items
{
    public abstract class MItem: ModItem, IEquipped
    {
        public bool Equipped { get; internal set; }

        public sealed override void UpdateAccessory(Player player, bool hideVisual)
        {
            var mPlayer = player.GetModPlayer<MPlayer>(mod);

            mPlayer.UpdateEquipped(this);

            UpdateAccessory2(player, hideVisual);
        }

        public virtual void UpdateAccessory2(Player player, bool hideVisual)
        {
        }

        public virtual void OnEquip(Player player)
        {
            Equipped = true;
        }

        public virtual void OnUnEquip(Player player)
        {
            Equipped = false;
        }

        private long _lastEquippedTime;

        public void SetLastEquippedTime(long time)
        {
            _lastEquippedTime = time;
        }

        public long GetLastEquippedTime()
        {
            return _lastEquippedTime;
        }
    }
}