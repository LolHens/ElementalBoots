﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using TAPI;
using TAPI.UIKit;

namespace LolHens.Items
{
    public class SpikyBallBlaster : LolHensGun
    {
        public override void Init()
        {
            base.Init();
            bulletOffset = 60;
            bulletOrigin.Y -= 10;
        }

        public override void DrawItemSlotItem(ref SpriteBatch sb, ref ItemSlot slot, ref Item item, ref Texture2D texture, ref Color color, ref float scale)
        {
            scale *= 1.4f;
        }
    }
}
