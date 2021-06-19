// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Tournament
{
    public static class AgtsFont
    {
        public static FontUsage OswaldBold => GetFont(AgtsTypeFace.Oswald, weight: FontWeight.Bold);
        public static FontUsage OswaldMedium => GetFont(AgtsTypeFace.Oswald, weight: FontWeight.Medium);
        public static FontUsage MontserratMedium => GetFont(AgtsTypeFace.Montserrat, weight: FontWeight.Medium);
        public static FontUsage MontserratSemiBold => GetFont(AgtsTypeFace.Montserrat, weight: FontWeight.SemiBold);
        public static FontUsage MontserratBold => GetFont(AgtsTypeFace.Montserrat, weight: FontWeight.Bold);
        public static FontUsage MontserratBlack => GetFont(AgtsTypeFace.Montserrat, weight: FontWeight.Black);

        public static FontUsage GetFont(AgtsTypeFace typeface, float size = OsuFont.DEFAULT_FONT_SIZE, FontWeight weight = FontWeight.Bold)
            => new FontUsage(typeface.ToString(), size, weight.ToString());
    }

    public enum AgtsTypeFace
    {
        Montserrat,
        Oswald
    }
}
