// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Components
{
    public partial class DrawableTeamFlag : Container
    {
        private readonly TournamentTeam team;

        [UsedImplicitly]
        private Bindable<string> flag;

        private Sprite flagSprite;

        public DrawableTeamFlag(TournamentTeam team)
        {
            this.team = team;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, LargeTextureStore largeTextures)
        {
            if (team == null) return;

            if (largeTextures == null) throw new ArgumentNullException(nameof(largeTextures));

            Size = new Vector2(75, 54);
            Masking = true;
            CornerRadius = 5;
            Child = flagSprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill
            };

            (flag = team.FlagName.GetBoundCopy()).BindValueChanged(_ =>
            {
                if (isUserId(team.FlagName.Value, out int userId))
                {
                    // this is super omega ugly and will freeze the client while its loading, but that should only happen at startup anyway so whatever
                    var avatarTex = largeTextures.Get($@"https://a.ppy.sh/{userId}") ?? largeTextures.Get(@"Online/avatar-guest");
                    flagSprite.Texture = avatarTex;
                    flagSprite.FillMode = FillMode.Fit;
                    return;
                }

                flagSprite.Texture = textures.Get($@"Flags/{team.FlagName}");
                flagSprite.FillMode = FillMode.Fill;
            }, true);
        }

        private static bool isUserId(string str, out int userId)
        {
            userId = default;
            if (string.IsNullOrEmpty(str)) return false;

            if (str.Any(c => c is < '0' or > '9')) return false;

            userId = int.Parse(str);
            return userId != 0;
        }
    }
}
