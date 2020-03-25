// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Input.Bindings;
using osu.Game.Graphics.Cursor;
using osu.Game.Input.Bindings;
using osu.Game.Tournament.Models;
using osu.Game.Tournament.Screens.Drawings;
using osu.Game.Tournament.Screens.Gameplay;
using osu.Game.Tournament.Screens.Ladder;
using osu.Game.Tournament.Screens.MapPool;
using osu.Game.Tournament.Screens.Schedule;
using osu.Game.Tournament.Screens.Showcase;
using osu.Game.Tournament.Screens.TeamIntro;
using osu.Game.Tournament.Screens.TeamWin;
using osuTK.Graphics;

namespace osu.Game.Tournament
{
    public class TournamentGame : TournamentGameBase, IKeyBindingHandler<GlobalAction>
    {
        public static ColourInfo GetTeamColour(TeamColour teamColour) => teamColour == TeamColour.Red ? COLOUR_RED : COLOUR_BLUE;

        public static readonly Color4 COLOUR_RED = Color4Extensions.FromHex("#AA1414");
        public static readonly Color4 COLOUR_BLUE = Color4Extensions.FromHex("#1462AA");

        public static readonly Color4 ELEMENT_BACKGROUND_COLOUR = Color4Extensions.FromHex("#fff");
        public static readonly Color4 ELEMENT_FOREGROUND_COLOUR = Color4Extensions.FromHex("#000");

        public static readonly Color4 TEXT_COLOUR = Color4Extensions.FromHex("#fff");

        private static readonly Dictionary<GlobalAction, Type> hot_keys_to_screen_type_dict = new Dictionary<GlobalAction, Type>
        {
            { GlobalAction.SelectTourneySchedule, typeof(ScheduleScreen) },
            { GlobalAction.SelectTourneyBracket, typeof(LadderScreen) },
            { GlobalAction.SelectTourneyTeamIntro, typeof(TeamIntroScreen) },
            { GlobalAction.SelectTourneySeeding, typeof(SeedingScreen) },
            { GlobalAction.SelectTourneyMapPool, typeof(MapPoolScreen) },
            { GlobalAction.SelectTourneyGamePlay, typeof(GameplayScreen) },
            { GlobalAction.SelectTourneyWin, typeof(TeamWinScreen) },
            { GlobalAction.SelectTourneyDrawings, typeof(DrawingsScreen) },
            { GlobalAction.SelectTourneyShowcase, typeof(ShowcaseScreen) },
        };

        private TournamentSceneManager tournamentSceneManager;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(new OsuContextMenuContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = tournamentSceneManager = new TournamentSceneManager()
            });

            // we don't want to show the menu cursor as it would appear on stream output.
            MenuCursorContainer.Cursor.Alpha = 0;
        }

        public bool OnPressed(GlobalAction action)
        {
            if (tournamentSceneManager == null) return false;

            if (hot_keys_to_screen_type_dict.TryGetValue(action, out var type))
            {
                tournamentSceneManager.SetScreen(type);
                return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
            // no-op
        }
    }
}
