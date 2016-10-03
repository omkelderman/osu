//Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
//Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.GameModes.Testing;
using osu.Game.GameModes.Menu;
using osu.Game.Graphics;

namespace osu.Desktop.Tests
{
    class TestCaseMenuButtonSystem : TestCase
    {
        // adding some FontAwesome-icons to name to force-load the FontAwesome textures early
        public override string Name => @"ButtonSystem " + ((char)FontAwesome.gear) + ((char)FontAwesome.user) + ((char)FontAwesome.users) + ((char)FontAwesome.terminal);
        public override string Description => @"Main menu button system";

        public override void Reset()
        {
            base.Reset();

            Add(new ButtonSystem());
        }
    }
}
