// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamHeader : TournamentSpriteTextWithBackground
    {
        public DrawableTeamHeader(TeamColour colour)
        {
            Text.Colour = TournamentGame.TEXT_COLOUR;
            Text.Text = $"Team {colour}".ToUpperInvariant();
            Text.Font = Text.Font.With(weight: FontWeight.Medium, italics: true);
            Text.Scale = new Vector2(0.6f);

            Background.Colour = TournamentGame.GetTeamColour(colour);
            Background.Shear = new Vector2(EgtsConstants.ParallelogramAngleTanVale, 0);

            // move the background box a bit to the right since the Shear thing extended it to the left
            var offset = (Text.Font.Size * EgtsConstants.ParallelogramAngleTanVale) / 4;
            Background.Margin = new MarginPadding { Left = offset };
        }
    }
}
