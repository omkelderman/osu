// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
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
            OsuSpriteText textName;
            OsuSpriteText textDescription;

            AutoSizeAxes = Axes.Both;
            InternalChild = new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    textDescription = new OsuSpriteText
                    {
                        Colour = TournamentGame.TEXT_COLOUR,
                        Font = EgtsFont.RedHatDisplay.With(italics: true),
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre
                    },
                    textName = new OsuSpriteText
                    {
                        Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Bold, italics: true),
                        Colour = TournamentGame.TEXT_COLOUR,
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre
                    },
                }
            };

            name = round.Name.GetBoundCopy();
            name.BindValueChanged(_ => textName.Text = ((losers ? "Losers " : "") + round.Name).ToUpperInvariant(), true);

            description = round.Description.GetBoundCopy();
            description.BindValueChanged(_ => textDescription.Text = round.Description.Value?.ToUpperInvariant(), true);
        }
    }
}
