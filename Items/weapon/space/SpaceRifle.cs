using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TAPI;
using TAPI.UIKit;
using Terraria;

namespace LolHens.Items
{
    public class SpaceRifle : LolHensRayGun
    {
        public override void Init()
        {
            base.Init();

            bulletOffset.Y -= 2;
            bulletOrigin.X -= 2;
        }

        public override void DrawItemSlotItem(ref SpriteBatch sb, ref ItemSlot slot, ref Item item, ref Texture2D texture, ref Color color, ref float scale)
        {
            scale *= 1.4f;
        }

        public override int GetRayLength()
        {
            return 10;
        }

        public override void PostShootCustom(Player player, Projectile projectile, Vector2 position, Vector2 velocity, int projType, int damage, float knockback)
        {
            base.PostShootCustom(player, projectile, position, velocity, projType, damage, knockback);

            //CancelMana();
        }
    }
}