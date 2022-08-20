// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Graphics;
using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamHeader : TournamentSpriteTextWithBackground
    {
        public DrawableTeamHeader(TeamColour colour, bool doShear = true)
            : base(bgColour: TournamentGame.GetTeamColour(colour), textColour: TournamentGame.TEXT_COLOUR)
        {
            Text.Text = $"Team {colour}".ToUpperInvariant();
            Text.Scale = new Vector2(0.6f);

            if (!doShear) return;

            Background.Shear = EgtsConstants.ShearVector;

            // move the background box a bit to the right since the Shear thing extended it to the left
            float offset = (Text.Font.Size * EgtsConstants.ParallelogramAngleTanVale) / 4;
            Background.Margin = new MarginPadding { Left = offset };
        }
    }
}
