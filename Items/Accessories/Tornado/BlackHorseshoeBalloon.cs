﻿using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ElementalBoots.Items.Accessories.Tornado
{
    [AutoloadEquip(EquipType.Balloon)]
    class BlackHorseshoeBalloon : TornadoInABalloon
    {
        public override void SetDefaults()
        {
            item.maxStack = 1;
            item.value = 1*Value.GOLD;
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "TornadoInABalloon");
            recipe.AddIngredient(ItemID.LuckyHorseshoe);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}