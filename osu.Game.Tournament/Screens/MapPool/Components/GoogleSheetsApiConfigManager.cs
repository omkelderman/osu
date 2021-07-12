// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Tournament.Screens.MapPool.Components
{
    public class GoogleSheetsApiConfigManager : IniConfigManager<GoogleSheetsApiConfig>
    {
        protected override string Filename => "google-sheets-api.ini";

        protected override void InitialiseDefaults()
        {
            SetDefault(GoogleSheetsApiConfig.SpreadsheetId, string.Empty);
            SetDefault(GoogleSheetsApiConfig.GoogleApiKey, string.Empty);
        }

        public GoogleSheetsApiConfigManager(Storage storage)
            : base(storage)
        {
        }
    }

    public enum GoogleSheetsApiConfig
    {
        SpreadsheetId,
        GoogleApiKey
    }
}
