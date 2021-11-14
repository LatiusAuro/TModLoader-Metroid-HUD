using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI.Chat;
using Terraria.ModLoader;

namespace MetroidHealthBars
{
    class PlayerBars
    {
        internal static Texture2D healthFrame;
        internal static Texture2D healthFill;
        internal static Texture2D healthEnds;
        internal static Texture2D manaFrame;
        internal static Texture2D manaFill;
        internal static Texture2D manaEnd;

        public static void Initialise(Mod mod)
        {
            healthFrame = mod.GetTexture("UI/HPBarBase");
            healthFill = mod.GetTexture("UI/HPBarFill");
            healthEnds = mod.GetTexture("UI/HPBarEnds");
            manaFrame = mod.GetTexture("UI/MPBarBase");
            manaFill = mod.GetTexture("UI/MPBarFill");
            manaEnd = mod.GetTexture("UI/MPBarCap");
        }
    }
}
