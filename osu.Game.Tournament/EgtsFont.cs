// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Tournament
{
    public static class EgtsFont
    {
        public static FontUsage RedHatDisplay => GetFont();

        public static FontUsage GetFont(float size = OsuFont.DEFAULT_FONT_SIZE, FontWeight weight = FontWeight.Regular, bool italics = false)
            => new FontUsage("RedHatDisplay", size, weight.ToString(), italics);
    }
}
