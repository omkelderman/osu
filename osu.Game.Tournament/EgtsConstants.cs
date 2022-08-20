// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics.Colour;
using osuTK;

namespace osu.Game.Tournament
{
    public static class EgtsConstants
    {
        public const float PARALLELOGRAM_ANGLE_DEGREES = 22.5f;
        public const float PARALLELOGRAM_ANGLE = MathF.PI * PARALLELOGRAM_ANGLE_DEGREES / 180;
        public static float ParallelogramAngleTanVale = MathF.Tan(PARALLELOGRAM_ANGLE);
        public static Vector2 ShearVector = new Vector2(ParallelogramAngleTanVale, 0);
        public static ColourInfo TextColor = Color4Extensions.FromHex("#1c1c1c");
    }
}
