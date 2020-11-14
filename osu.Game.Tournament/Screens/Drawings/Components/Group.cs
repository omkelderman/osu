﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Tournament.Models;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Tournament.Screens.Drawings.Components
{
    public class Group : Container
    {
        private const int height = 128;

        private const int group_team_spacing_x = 6;
        private const int group_team_margin_x = 7;
        private const int group_team_count_x = 4;
        private const int width = (group_team_count_x * GroupTeam.WIDTH) + ((group_team_count_x - 1) * group_team_spacing_x) + (2 * group_team_margin_x);
        public readonly string GroupName;

        public int TeamsCount { get; private set; }

        private readonly FlowContainer<GroupTeam> teams;

        private readonly List<GroupTeam> allTeams = new List<GroupTeam>();

        public Group(string name)
        {
            GroupName = name;

            Size = new Vector2(width, height);

            Masking = true;
            CornerRadius = 4;

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(54, 54, 54, 255)
                },
                // Group name
                new TournamentSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,

                    Position = new Vector2(0, 7f),

                    Text = $"TEAM {name.ToUpperInvariant()}",
                    Font = OsuFont.Torus.With(weight: FontWeight.Bold, size: 8),
                    Colour = new Color4(255, 204, 34, 255),
                },
                teams = new FillFlowContainer<GroupTeam>
                {
                    RelativeSizeAxes = Axes.Both,

                    Spacing = new Vector2(group_team_spacing_x, 22),

                    Margin = new MarginPadding
                    {
                        Top = 21f,
                        Bottom = 7f,
                        Left = group_team_margin_x,
                        Right = group_team_margin_x
                    }
                }
            };
        }

        public void AddTeam(TournamentTeam team)
        {
            GroupTeam gt = new GroupTeam(team);

            if (TeamsCount < 8)
            {
                teams.Add(gt);
                allTeams.Add(gt);

                TeamsCount++;
            }
        }

        public bool ContainsTeam(string fullName)
        {
            return allTeams.Any(t => t.Team.FullName.Value == fullName);
        }

        public bool RemoveTeam(TournamentTeam team)
        {
            allTeams.RemoveAll(gt => gt.Team == team);

            if (teams.RemoveAll(gt => gt.Team == team) > 0)
            {
                TeamsCount--;
                return true;
            }

            return false;
        }

        public void ClearTeams()
        {
            allTeams.Clear();
            teams.Clear();

            TeamsCount = 0;
        }

        public string GetStringRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            foreach (GroupTeam gt in allTeams)
                sb.AppendLine(gt.Team.FullName.Value);
            return sb.ToString();
        }
    }
}
