// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Tournament.Components
{
    public class TournamentSpriteTextWithBackground : CompositeDrawable
    {
        protected readonly TournamentSpriteText Text;
        protected readonly Box Background;

        public TournamentSpriteTextWithBackground(string text = "", ColourInfo? backgroundColour = null, ColourInfo? foregroundColour = null, FontUsage? font = null, float fontSize = 50)
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                Background = new Box
                {
                    Colour = backgroundColour ?? TournamentGame.ELEMENT_BACKGROUND_COLOUR,
                    RelativeSizeAxes = Axes.Both,
                },
                Text = new TournamentSpriteText
                {
                    Colour = foregroundColour ?? TournamentGame.ELEMENT_FOREGROUND_COLOUR,
                    Font = font?.With(size: fontSize) ?? OsuFont.Torus.With(weight: FontWeight.SemiBold, size: fontSize),
                    Padding = new MarginPadding { Left = 10, Right = 20 },
                    Text = text
                }
            };
        }
    }
}
