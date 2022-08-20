// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Tournament.Models;

namespace osu.Game.Tournament.Components
{
    public class RoundDisplay : CompositeDrawable
    {
        public RoundDisplay(TournamentMatch match)
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new DrawableTournamentHeaderText(false)
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                        },
                        new OsuSpriteText
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Text = (match.Round.Value?.Name.Value ?? "Unknown Round").ToUpperInvariant(),
                            Font = EgtsFont.RedHatDisplay.With(size: 26, weight: FontWeight.Medium, italics: true)
                        },
                    }
                }
            };
        }
    }
}
