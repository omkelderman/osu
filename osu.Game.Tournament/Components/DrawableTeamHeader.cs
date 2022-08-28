// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamHeader : TournamentSpriteTextWithBackground
    {
        public DrawableTeamHeader(TeamColour colour, bool doShear = true)
            : base(bgColour: TournamentGame.GetTeamColour(colour), textColour: TournamentGame.TEXT_COLOUR, doShear: doShear)
        {
            Text.Text = $"Team {colour}".ToUpperInvariant();
            Text.Scale = new Vector2(0.6f);
        }
    }
}
