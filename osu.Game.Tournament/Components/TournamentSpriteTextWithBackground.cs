// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

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
        public readonly TournamentSpriteText Text;

        protected readonly Box Background;

        public TournamentSpriteTextWithBackground(string text = "", ColourInfo? bgColour = null, ColourInfo? textColour = null, FontUsage? font = null, bool doShear = false)
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                Background = new Box
                {
                    Colour = bgColour ?? TournamentGame.ELEMENT_BACKGROUND_COLOUR,
                    RelativeSizeAxes = Axes.Both,
                },
                Text = new TournamentSpriteText
                {
                    Colour = textColour ?? TournamentGame.ELEMENT_FOREGROUND_COLOUR,
                    Font = font ?? EgtsFont.RedHatDisplay.With(weight: FontWeight.Medium, size: 50, italics: true),
                    Padding = new MarginPadding { Left = 10, Right = 20 },
                    Text = text
                }
            };

            if (!doShear) return;

            Background.Shear = EgtsConstants.ShearVector;

            // move the background box a bit to the right since the Shear thing extended it to the left
            float offset = (Text.Font.Size * EgtsConstants.ParallelogramAngleTanVale) / 4;
            Background.Margin = new MarginPadding { Left = offset };
        }
    }
}
