﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ElementalBoots.Items.Accessories.Wings.TerraWings
{
    [AutoloadEquip(EquipType.Wings)]
    class TerraWings: Wings
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            glowing = true;

            wingTimeMax = 300;

            constantAscend *= 4;
            ascentWhenRising *= 4;
            ascentWhenFalling *= 4;
            maxAscentSpeedMultiplier *= 2;
            maxHorizontalSpeedMultiplier *= 4;
            horizontalAccelerationMultiplier *= 4;
        }
    }
}
