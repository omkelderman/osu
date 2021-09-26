// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Tournament.Models;
using osu.Game.Users.Drawables;
using osuTK;

namespace osu.Game.Tournament.Components
{
    public class DrawableTeamFlag : Container
    {
        private readonly TournamentTeam team;

        [UsedImplicitly]
        private Bindable<string> flag;

        private Sprite flagSprite;
        private UpdateableAvatar userAvatar;

        public DrawableTeamFlag(TournamentTeam team)
        {
            this.team = team;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            if (team == null) return;

            Size = new Vector2(75, 50);
            Masking = true;
            CornerRadius = 5;

            Children = new Drawable[]
            {
                flagSprite = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill
                },
                userAvatar = new UpdateableAvatar(isInteractive: false)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill
                }
            };

            void refreshFlag()
            {
                if (team.Players.Count == 1)
                {
                    var firstUser = team.Players[0];

                    if (userAvatar.User?.Id != firstUser.Id)
                    {
                        userAvatar.User = firstUser;
                    }

                    flagSprite.Alpha = 0;
                    userAvatar.Alpha = 1;
                }
                else
                {
                    flagSprite.Texture = textures.Get($@"Flags/{team.FlagName}");
                    flagSprite.Alpha = 1;
                    userAvatar.Alpha = 0;
                }
            }

            (flag = team.FlagName.GetBoundCopy()).BindValueChanged(_ => refreshFlag(), true);
        }
    }
}
