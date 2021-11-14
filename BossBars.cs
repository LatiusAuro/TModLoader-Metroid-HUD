using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI.Chat;
using Terraria.ModLoader;

//Credit must be given to the creators of YABHB, as I was able to learn from their code how to make a Boss Health Bar.

namespace MetroidHealthBars
{
    public class BossBars
    {
        public enum DisplayType
        {
            Solo, // Uses npc.life and npc.realLife
            Group, // Counts all NPCs of a typelist and uses collective life vs lifeMax
            Phase, // Checks for NPCs in the typelist in descending order, using lifeMax as a total.
            Off, // Just don't show
        }

        public DisplayType DisplayMode = DisplayType.Solo; //Defaults to single boss
        
        /// All NPC types collected in this health bar, see DisplayType.Group
        internal int[] groupNPCType = null;
        internal int groupNPCLifeMax = 0;
        internal bool groupNPCLIfeMaxRecordedOnExpert = false;

        /// A group NPC has drawn this bar already? Also used to count.
        internal ushort groupShowCount = 0;

        internal static Texture2D bossFrame;
        internal static Texture2D bossFill;
        internal static Texture2D bossEnds;

        public static void Initialise(Mod mod) {
            bossFrame = mod.GetTexture("UI/BossBarBase");
            bossFill = mod.GetTexture("UI/HPBarFill");
            bossEnds = mod.GetTexture("UI/HPBarEnds");
        }

        protected virtual string GetBossDisplayNameNPC(NPC npc)
        {
            return npc.GivenOrTypeName;
        }

        public int DrawBossHealthBar(SpriteBatch spriteBatch, int XRight, int yTop, float Alpha, int life, int lifeMax, NPC npc) {
            if (groupShowCount > 0 &&
                (DisplayMode == DisplayType.Group || DisplayMode == DisplayType.Phase))
            {
                groupShowCount++;
                return yTop; // We don't show multiple NPC healthbars for a collective boss. ever.
            }

            string displayName = "";
            //ManageMultipleNPCVars(ref life, ref lifeMax, ref displayName);
            //ShowHealthBarLifeOverride(npc, ref life, ref lifeMax);
            float percentage = (life / lifeMax) * 100.0f;

            Color framecolor = new Color(1f, 1f, 1f);
            //Color barcolor = new Color(1f, 0.925f, 0.5f);
            //Color barends = new Color(0.5f, 0.4625f, 0.25f);
            Color blackcolor = new Color(0f, 0f, 0f);

            framecolor *= Alpha;
            //barcolor *= Alpha;
            //barends *= Alpha;
            blackcolor *= -Alpha;

            Texture2D frame = bossFrame;
            Texture2D fill = bossFill;
            Texture2D ends = bossEnds;

            Vector2 FrameTopLeft = new Vector2(XRight - frame.Width, yTop);
            Vector2 BarTopLeft = new Vector2(XRight + 34 - frame.Width, yTop + 20);

            drawBossBarFrame(spriteBatch, framecolor, frame, FrameTopLeft);
            //drawBossHBarFill(spriteBatch, percentage, framecolor, fill, ends, BarTopLeft);
            //drawBossBarPercentage(spriteBatch, percentage, framecolor, XRight, yTop);

            if (DisplayMode != DisplayType.Group)
            {
                displayName = GetBossDisplayNameNPC(npc);
            }

            string text = string.Concat(displayName, " - Max HP: ", lifeMax);
            /*
            // Draw border
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    DynamicSpriteFontExtensionMethods.DrawString(
                        spriteBatch,
                        Main.fontMouseText,
                        text,
                        position + new Vector2(x, y),
                        blackColour, 0f,
                        origin,
                        1.1f, SpriteEffects.None, 0f);
                }
            }

            // Main text
            DynamicSpriteFontExtensionMethods.DrawString(
                spriteBatch,
                Main.fontMouseText,
                text,
                position,
                frameColour, 0f,
                origin,
                1.1f, SpriteEffects.None, 0f);
             */

            return yTop;
        }

        private void ManageMultipleNPCVars(ref int life, ref int lifeMax, ref string displayName)
        {
            if (DisplayMode == DisplayType.Group && groupNPCType != null)
            {
                life = 0; lifeMax = 0;

                // Reset when the life max would change (pretty much only during switching to expert mode)
                if (groupNPCLIfeMaxRecordedOnExpert != Main.expertMode)
                {
                    groupNPCLIfeMaxRecordedOnExpert = Main.expertMode;
                    groupNPCLifeMax = 0;
                }

                // First run, only include active
                foreach (int type in groupNPCType)
                {
                    foreach (NPC n in Main.npc)
                    {
                        if (n.type == type)
                        {
                            // Get the names in order of priority
                            if (displayName == "")
                            {
                                displayName = GetBossDisplayNameNPC(n);
                            }
                            if (!n.active) continue;
                            life += n.life;
                            lifeMax += n.lifeMax;
                        }
                    }
                }
                // Get the highest recorded value
                if (groupNPCLifeMax < lifeMax) groupNPCLifeMax = lifeMax;
                lifeMax = groupNPCLifeMax;

                // Set to true to prevent further draws of the same thing this frame (see BossDisplayInfo)
                groupShowCount++;
            }
        }

        private int drawBossHBarFill(SpriteBatch spriteBatch, float percentage, Color barcolor, Texture2D fill, Texture2D ends, Vector2 BarTopLeft)
        {
            int realLength = (int) (percentage * ends.Width);
            if (realLength <= 0) {
                return 0;
            }
            float fillLength = (percentage - 2.0f);
            if (fillLength <= 0) {
                //Draw just the ends
                /*
                 spriteBatch.Draw(
                        ends,
                        BarTopLeft,
                        new Rectangle(0, 0, decoOffset - 1, ends.Height),
                        barcolor,
                        0f,
                        Vector2.Zero,
                        new Vector2(percentage, 1f),
                        SpriteEffects.None,
                        0f);
                 */

            } else {
                //Draw the ends and then the rest of the bar
                /*
                 spriteBatch.Draw(
                        ends,
                        BarTopLeft,
                        new Rectangle(0, 0, decoOffset - 1, ends.Height),
                        barcolor,
                        0f,
                        Vector2.Zero,
                        new Vector2(1f, 1f),
                        SpriteEffects.None,
                        0f);
                    spriteBatch.Draw(
                        fill,
                        BarTopLeft + new Vector2(ends.Width, 0f),
                        new Rectangle(0, 0, decoOffset - 1, fill.Height),
                        barcolor,
                        0f,
                        Vector2.Zero,
                        new Vector2(fillLength, 1f),
                        SpriteEffects.None,
                        0f);
                    spriteBatch.Draw(
                        ends,
                        BarTopLeft + new Vector2(ends.Width + (fill.Width * fillLength), 0f),
                        new Rectangle(0, 0, decoOffset - 1, ends.Height),
                        barcolor,
                        0f,
                        Vector2.Zero,
                        new Vector2(1f, 1f),
                        SpriteEffects.None,
                        0f);
                 */
            }
            return realLength;
        }

        private static void drawBossBarFrame(SpriteBatch spriteBatch, Color framecolor, Texture2D frame, Vector2 FrameTopLeft) {
            spriteBatch.Draw(
                frame,
                FrameTopLeft,
                framecolor
                );
        }
    }
}
