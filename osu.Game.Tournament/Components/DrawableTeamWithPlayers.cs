// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamWithPlayers : CompositeDrawable
    {
        public DrawableTeamWithPlayers(TournamentTeam team, TeamColour colour)
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(30),
                    Children = new Drawable[]
                    {
                        new DrawableTeamTitleWithHeader(team, colour),
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Padding = new MarginPadding { Left = 10 },
                            Spacing = new Vector2(30),
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    Direction = FillDirection.Vertical,
                                    AutoSizeAxes = Axes.Both,
                                    ChildrenEnumerable = team?.Players.Select(createPlayerText).Take(5) ?? Enumerable.Empty<Drawable>()
                                },
                                new FillFlowContainer
                                {
                                    Direction = FillDirection.Vertical,
                                    AutoSizeAxes = Axes.Both,
                                    ChildrenEnumerable = team?.Players.Select(createPlayerText).Skip(5) ?? Enumerable.Empty<Drawable>()
                                },
                            }
                        },
                    }
                },
            };

            static OsuSpriteText createPlayerText(TournamentUser p) =>
                new OsuSpriteText
                {
                    Text = p.Username,
                    Font = EgtsFont.RedHatDisplay.With(size: 24, weight: FontWeight.Bold, italics: true),
                    Colour = EgtsConstants.TextColor
                };
        }
    }
}
