// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Tournament.Models;
using osuTK.Graphics;

namespace osu.Game.Tournament.Components
{
    public class RoundDisplay : CompositeDrawable
    {
        public RoundDisplay(TournamentMatch match, ColourInfo? textColour = null)
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
                        new TournamentSpriteText
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Colour = textColour ?? Color4.White,
                            Text = match.Round.Value?.Name.Value ?? "Unknown Round",
                            Font = OsuFont.Torus.With(size: 26, weight: FontWeight.SemiBold)
                        },
                    }
                }
            };
        }
    }
}
