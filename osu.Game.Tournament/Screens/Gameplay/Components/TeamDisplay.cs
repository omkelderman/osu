// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Models;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Tournament.Screens.Gameplay.Components
{
    public class TeamDisplay : DrawableTournamentTeam
    {
        private readonly TeamScore score;

        private readonly TeamName teamText;

        private readonly Bindable<string> teamName = new Bindable<string>("???");

        private bool showScore;

        public bool ShowScore
        {
            get => showScore;
            set
            {
                if (showScore == value)
                    return;

                showScore = value;

                if (IsLoaded)
                    updateDisplay();
            }
        }

        public TeamDisplay(TournamentTeam team, TeamColour colour, Bindable<int?> currentTeamScore, int pointsToWin)
            : base(team)
        {
            AutoSizeAxes = Axes.Both;

            bool flip = colour == TeamColour.Red;

            var anchor = flip ? Anchor.TopLeft : Anchor.TopRight;

            Flag.RelativeSizeAxes = Axes.None;
            Flag.Scale = new Vector2(0.8f);
            Flag.Origin = anchor;
            Flag.Anchor = anchor;

            Margin = new MarginPadding(20);

            InternalChild = new Container
            {
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(5),
                        Children = new Drawable[]
                        {
                            Flag,
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Origin = anchor,
                                Anchor = anchor,
                                Spacing = new Vector2(5),
                                Children = new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Horizontal,
                                        Spacing = new Vector2(5),
                                        Origin = anchor,
                                        Anchor = anchor,
                                        Children = new Drawable[]
                                        {
                                            new DrawableTeamHeader(colour, false)
                                            {
                                                Scale = new Vector2(0.75f),
                                                Origin = anchor,
                                                Anchor = anchor,
                                            },
                                            score = new TeamScore(currentTeamScore, colour, pointsToWin)
                                            {
                                                Origin = anchor,
                                                Anchor = anchor,
                                            }
                                        }
                                    },
                                    teamText = new TeamName(team)
                                    {
                                        Scale = new Vector2(0.5f),
                                        Origin = anchor,
                                        Anchor = anchor,
                                    },
                                }
                            },
                        }
                    },
                }
            };
        }

        private class TeamName : TournamentSpriteTextWithBackground
        {
            public TeamName(TournamentTeam team)
                : base(team?.FullName.Value ?? "???", Color4.Transparent, Color4.White)
            {
                Text.Font = Text.Font.With(weight: FontWeight.Black);
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            updateDisplay();
            FinishTransforms(true);

            if (Team != null)
                teamName.BindTo(Team.FullName);

            teamName.BindValueChanged(name => teamText.Text.Text = name.NewValue, true);
        }

        private void updateDisplay()
        {
            score.FadeTo(ShowScore ? 1 : 0, 200);
        }
    }
}
