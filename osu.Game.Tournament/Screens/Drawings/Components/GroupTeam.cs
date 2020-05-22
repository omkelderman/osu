// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Screens.Drawings.Components
{
    public class GroupTeam : DrawableTournamentTeam
    {
        private readonly FillFlowContainer innerContainer;

        public GroupTeam(TournamentTeam team)
            : base(team)
        {
            Width = 132;
            AutoSizeAxes = Axes.Y;

            Flag.Anchor = Anchor.TopCentre;
            Flag.Origin = Anchor.TopCentre;

            AcronymText.Anchor = Anchor.TopCentre;
            AcronymText.Origin = Anchor.TopCentre;
            AcronymText.Text = team.Acronym.Value;
            AcronymText.Font = OsuFont.Torus.With(weight: FontWeight.Bold, size: 14);
            AcronymText.Margin = new MarginPadding(3f);

            InternalChildren = new Drawable[]
            {
                innerContainer = new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,

                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,

                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(0, 0),

                    Children = new Drawable[]
                    {
                        Flag,
                        AcronymText
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            innerContainer.ScaleTo(1.5f);
            innerContainer.ScaleTo(1f, 200);
        }
    }
}
