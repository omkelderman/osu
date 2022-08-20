// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Models;
using osu.Game.Tournament.Screens.Ladder.Components;
using osuTK;

namespace osu.Game.Tournament.Screens.Schedule
{
    public class ScheduleScreen : TournamentScreen
    {
        private readonly Bindable<TournamentMatch> currentMatch = new Bindable<TournamentMatch>();
        private Container mainContainer;
        private LadderInfo ladder;

        [BackgroundDependencyLoader]
        private void load(LadderInfo ladder)
        {
            this.ladder = ladder;

            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new TourneyVideo("schedule")
                {
                    RelativeSizeAxes = Axes.Both,
                    Loop = true,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(100) { Bottom = 50 },
                    Children = new Drawable[]
                    {
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(),
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new FillFlowContainer<OsuSpriteText>
                                    {
                                        Margin = new MarginPadding { Top = 75, Left = 60 },
                                        Padding = new MarginPadding { Left = 10, Right = 20 },
                                        Direction = FillDirection.Horizontal,
                                        AutoSizeAxes = Axes.Both,
                                        Spacing = new Vector2(2),
                                        Children = new[]
                                        {
                                            new OsuSpriteText
                                            {
                                                Text = "Expert Global Taiko Showdown ".ToUpperInvariant(),
                                                Colour = Color4Extensions.FromHex("#fff"),
                                                Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Black, size: 30, italics: true),
                                            },
                                            new OsuSpriteText
                                            {
                                                Text = "2021",
                                                Colour = Color4Extensions.FromHex("#E70991"),
                                                Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Black, size: 30, italics: true),
                                            },
                                        }
                                    }
                                },
                                new Drawable[]
                                {
                                    mainContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                    }
                                }
                            }
                        }
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            currentMatch.BindTo(ladder.CurrentMatch);
            currentMatch.BindValueChanged(matchChanged, true);
        }

        private void matchChanged(ValueChangedEvent<TournamentMatch> match)
        {
            var upcoming = ladder.Matches.Where(p => !p.Completed.Value && p.Team1.Value != null && p.Team2.Value != null && Math.Abs(p.Date.Value.DayOfYear - DateTimeOffset.UtcNow.DayOfYear) < 4);
            var conditionals = ladder
                               .Matches.Where(p => !p.Completed.Value && (p.Team1.Value == null || p.Team2.Value == null) && Math.Abs(p.Date.Value.DayOfYear - DateTimeOffset.UtcNow.DayOfYear) < 4)
                               .SelectMany(m => m.ConditionalMatches.Where(cp => m.Acronyms.TrueForAll(a => cp.Acronyms.Contains(a))));

            upcoming = upcoming.Concat(conditionals);
            upcoming = upcoming.OrderBy(p => p.Date.Value).Take(8);

            ScheduleContainer comingUpNext;

            mainContainer.Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = 0.74f,
                        Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Children = new Drawable[]
                            {
                                new ScheduleContainer("recent matches", colour: Color4Extensions.FromHex("#FFC53B"))
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Width = 0.4f,
                                    ChildrenEnumerable = ladder.Matches
                                                               .Where(p => p.Completed.Value && p.Team1.Value != null && p.Team2.Value != null
                                                                           && Math.Abs(p.Date.Value.DayOfYear - DateTimeOffset.UtcNow.DayOfYear) < 4)
                                                               .OrderByDescending(p => p.Date.Value)
                                                               .Take(8)
                                                               .Select(p => new ScheduleMatch(p))
                                },
                                new ScheduleContainer("upcoming matches", colour: Color4Extensions.FromHex("#FFC53B"))
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Width = 0.6f,
                                    ChildrenEnumerable = upcoming.Select(p => new ScheduleMatch(p))
                                },
                            }
                        }
                    },
                    comingUpNext = new ScheduleContainer("coming up next", FontWeight.Black)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = 0.25f,
                    }
                }
            };

            if (match.NewValue != null)
            {
                comingUpNext.Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(30),
                    Children = new Drawable[]
                    {
                        new ScheduleMatch(match.NewValue, false)
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = match.NewValue.Round.Value?.Name.Value?.ToUpperInvariant(),
                            Padding = new MarginPadding { Left = 10, Right = 20 },
                            Colour = Color4Extensions.FromHex("#fff"),
                            Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Black, size: 25, italics: true),
                        },
                        new FillFlowContainer<OsuSpriteText>
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Direction = FillDirection.Horizontal,
                            AutoSizeAxes = Axes.Both,
                            Spacing = new Vector2(2),
                            Children = new[]
                            {
                                new OsuSpriteText
                                {
                                    Text = match.NewValue.Team1.Value?.FullName.Value,
                                    Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Bold, size: 24, italics: true)
                                },
                                new OsuSpriteText
                                {
                                    Text = " vs ",
                                    Colour = Color4Extensions.FromHex("#ffc43d"),
                                    Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Bold, size: 24, italics: true)
                                },
                                new OsuSpriteText
                                {
                                    Text = match.NewValue.Team2.Value?.FullName.Value,
                                    Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Bold, size: 24, italics: true)
                                },
                            },
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new ScheduleMatchDate(match.NewValue.Date.Value)
                                {
                                    Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Regular, size: 24, italics: true)
                                }
                            }
                        },
                    }
                };
            }
        }

        public class ScheduleMatch : DrawableTournamentMatch
        {
            public ScheduleMatch(TournamentMatch match, bool showTimestamp = true)
                : base(match)
            {
                Flow.Direction = FillDirection.Horizontal;

                Scale = new Vector2(0.8f);

                CurrentMatchSelectionBox.Scale = new Vector2(1.02f, 1.15f);

                bool conditional = match is ConditionalTournamentMatch;

                if (conditional)
                    Colour = OsuColour.Gray(0.5f);

                if (showTimestamp)
                {
                    AddInternal(new DrawableDate(Match.Date.Value)
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopLeft,
                        Colour = OsuColour.Gray(0.85f),
                        Alpha = conditional ? 0.6f : 1,
                        Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Medium),
                        Margin = new MarginPadding { Horizontal = 10, Vertical = 5 },
                    });
                    AddInternal(new OsuSpriteText
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomLeft,
                        Colour = OsuColour.Gray(0.85f),
                        Alpha = conditional ? 0.6f : 1,
                        Font = EgtsFont.RedHatDisplay.With(weight: FontWeight.Medium),
                        Margin = new MarginPadding { Horizontal = 10, Vertical = 5 },
                        Text = match.Date.Value.ToUniversalTime().ToString("HH:mm UTC") + (conditional ? " (conditional)" : "")
                    });
                }
            }
        }

        public class ScheduleMatchDate : DrawableDate
        {
            public ScheduleMatchDate(DateTimeOffset date, float textSize = OsuFont.DEFAULT_FONT_SIZE, bool italic = true)
                : base(date, textSize, italic)
            {
            }

            protected override string Format() => Date < DateTimeOffset.Now
                ? $"Started {base.Format()}"
                : $"Starting {base.Format()}";
        }

        public class ScheduleContainer : Container
        {
            protected override Container<Drawable> Content => content;

            private readonly FillFlowContainer content;

            public ScheduleContainer(string title, FontWeight weight = FontWeight.Bold, ColourInfo? colour = null)
            {
                Padding = new MarginPadding { Left = 60, Top = 10 };
                InternalChildren = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            new OsuSpriteText
                            {
                                Text = title.ToUpperInvariant(),
                                Padding = new MarginPadding { Left = 10, Right = 20 },
                                Colour = colour ?? Color4Extensions.FromHex("#fff"),
                                Font = EgtsFont.RedHatDisplay.With(weight: weight, size: 25, italics: true),
                            },
                            content = new FillFlowContainer
                            {
                                Direction = FillDirection.Vertical,
                                RelativeSizeAxes = Axes.Both,
                                Margin = new MarginPadding(10)
                            },
                        }
                    },
                };
            }
        }
    }
}
