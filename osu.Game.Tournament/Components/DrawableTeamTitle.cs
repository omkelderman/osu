// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Game.Tournament.Models;
using osuTK.Graphics;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamTitle : TournamentSpriteTextWithBackground
    {
        private readonly TournamentTeam team;

        [UsedImplicitly]
        private Bindable<string> acronym;

        public DrawableTeamTitle(TournamentTeam team, TeamColour colour)
            : base(backgroundColour: Color4.Transparent, foregroundColour: TournamentGame.GetTeamColour(colour))
        {
            this.team = team;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            if (team == null) return;

            (acronym = team.Acronym.GetBoundCopy()).BindValueChanged(acronym => Text.Text = team?.FullName.Value ?? string.Empty, true);
        }
    }
}
