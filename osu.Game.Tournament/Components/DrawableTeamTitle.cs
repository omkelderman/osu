// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics;
using osu.Game.Tournament.Models;
using osuTK.Graphics;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamTitle : TournamentSpriteTextWithBackground
    {
        private readonly TournamentTeam team;

        [UsedImplicitly]
        private Bindable<string> acronym;

        public DrawableTeamTitle(TournamentTeam team)
            : base(bgColour: Color4.Transparent, textColour: Color4.White)
        {
            this.team = team;
            Text.Font = Text.Font.With(weight: FontWeight.Black);
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            if (team == null) return;

            (acronym = team.Acronym.GetBoundCopy()).BindValueChanged(acronym => Text.Text = team?.FullName.Value ?? string.Empty, true);
        }
    }
}
