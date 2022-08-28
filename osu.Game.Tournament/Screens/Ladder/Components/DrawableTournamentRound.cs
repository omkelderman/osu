// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Models;

namespace osu.Game.Tournament.Screens.Ladder.Components
{
    public class DrawableTournamentRound : CompositeDrawable
    {
        [UsedImplicitly]
        private readonly Bindable<string> name;

        [UsedImplicitly]
        private readonly Bindable<string> description;

        public DrawableTournamentRound(TournamentRound round, bool losers = false)
        {
            TournamentSpriteTextWithBackground textName;
            TournamentSpriteTextWithBackground textDescription;

            AutoSizeAxes = Axes.Both;
            InternalChild = new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    textDescription = new TournamentSpriteTextWithBackground(textColour: EgtsConstants.TextColor, font: EgtsFont.RedHatDisplay.With(italics: true), doShear: true)
                    {
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre
                    },
                    textName = new TournamentSpriteTextWithBackground(textColour: EgtsConstants.TextColor, font: EgtsFont.RedHatDisplay.With(weight: FontWeight.Bold, italics: true), doShear: true)
                    {
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre
                    }
                }
            };

            name = round.Name.GetBoundCopy();
            name.BindValueChanged(_ => textName.Text.Text = ((losers ? "Losers " : "") + round.Name).ToUpperInvariant(), true);

            description = round.Description.GetBoundCopy();
            description.BindValueChanged(_ => textDescription.Text.Text = round.Description.Value?.ToUpperInvariant(), true);
        }
    }
}
