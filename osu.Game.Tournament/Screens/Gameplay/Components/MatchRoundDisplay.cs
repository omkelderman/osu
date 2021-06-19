// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Models;
using osuTK.Graphics;

namespace osu.Game.Tournament.Screens.Gameplay.Components
{
    public class MatchRoundDisplay : TournamentSpriteTextWithBackground
    {
        private readonly Bindable<TournamentMatch> currentMatch = new Bindable<TournamentMatch>();

        public MatchRoundDisplay()
            : base(backgroundColour: Color4.Transparent, foregroundColour:Color4Extensions.FromHex("#01313c"))
        {
        }

        [BackgroundDependencyLoader]
        private void load(LadderInfo ladder)
        {
            currentMatch.BindValueChanged(matchChanged);
            currentMatch.BindTo(ladder.CurrentMatch);
        }

        private void matchChanged(ValueChangedEvent<TournamentMatch> match) =>
            Text.Text = match.NewValue.Round.Value?.Name.Value ?? "Unknown Round";
    }
}
