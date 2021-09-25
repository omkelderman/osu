// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Game.Tournament
{
    public class EgtsConstants
    {
        public const float PARALLELOGRAM_ANGLE_DEGREES = 22.5f;
        public const float PARALLELOGRAM_ANGLE = MathF.PI * PARALLELOGRAM_ANGLE_DEGREES / 180;
        public static float ParallelogramAngleTanVale = MathF.Tan(PARALLELOGRAM_ANGLE);
    }
}
