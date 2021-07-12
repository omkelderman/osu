// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NuGet.Packaging;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.IPC;
using osu.Game.Tournament.Models;
using osu.Game.Tournament.Screens.Gameplay;
using osu.Game.Tournament.Screens.Gameplay.Components;
using osu.Game.Tournament.Screens.MapPool.Components;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Tournament.Screens.MapPool
{
    public class MapPoolScreen : TournamentScreen
    {
        private readonly FillFlowContainer<FillFlowContainer<TournamentBeatmapPanel>> mapFlows;

        private readonly Bindable<TournamentMatch> currentMatch = new Bindable<TournamentMatch>();

        [Resolved(canBeNull: true)]
        private TournamentSceneManager sceneManager { get; set; }

        private TeamColour pickColour;
        private ChoiceType pickType;

        private readonly OsuButton buttonRedBan;
        private readonly OsuButton buttonBlueBan;
        private readonly OsuButton buttonRedPick;
        private readonly OsuButton buttonBluePick;

        private readonly ControlPanel controlPanel;

        private string spreadsheetId;
        private string googleApiKey;
        private TextFlowContainer loadBansPicksFromSheetStatus;
        private OsuNumberBox matchIdFromSheet;

        public MapPoolScreen()
        {
            InternalChildren = new Drawable[]
            {
                new TourneyVideo("mappool")
                {
                    Loop = true,
                    RelativeSizeAxes = Axes.Both,
                },
                new MatchHeader(),
                mapFlows = new FillFlowContainer<FillFlowContainer<TournamentBeatmapPanel>>
                {
                    Y = 160,
                    Spacing = new Vector2(10, 10),
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                },
                controlPanel = new ControlPanel
                {
                    Children = new Drawable[]
                    {
                        new TournamentSpriteText
                        {
                            Text = "Current Mode"
                        },
                        buttonRedBan = new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Red Ban",
                            Action = () => setMode(TeamColour.Red, ChoiceType.Ban)
                        },
                        buttonBlueBan = new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Blue Ban",
                            Action = () => setMode(TeamColour.Blue, ChoiceType.Ban)
                        },
                        buttonRedPick = new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Red Pick",
                            Action = () => setMode(TeamColour.Red, ChoiceType.Pick)
                        },
                        buttonBluePick = new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Blue Pick",
                            Action = () => setMode(TeamColour.Blue, ChoiceType.Pick)
                        },
                        new ControlPanel.Spacer(),
                        new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Reset",
                            Action = reset
                        },
                        new ControlPanel.Spacer(),
                    }
                }
            };
        }

        private void loadGoogleApiButtons()
        {
            controlPanel.AddRange(new Drawable[]
            {
                new TournamentSpriteText
                {
                    Text = "Match ID from sheet"
                },
                matchIdFromSheet = new OsuNumberBox
                {
                    RelativeSizeAxes = Axes.X
                },
                new TourneyButton
                {
                    RelativeSizeAxes = Axes.X,
                    Text = "Load from sheet",
                    Action = loadDataFromGoogleSheet
                },
                loadBansPicksFromSheetStatus = new TextFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                }
            });
        }

        private async void loadDataFromGoogleSheet()
        {
            if (string.IsNullOrEmpty(spreadsheetId) || string.IsNullOrEmpty(googleApiKey)) return;

            var sheetsService = new SheetsService(new BaseClientService.Initializer { ApiKey = googleApiKey });

            if (matchIdFromSheet.Text == string.Empty)
            {
                loadBansPicksFromSheetStatus.Text = string.Empty;
                return;
            }

            var p1Expected = currentMatch.Value?.Team1.Value?.FullName.Value;
            var p2Expected = currentMatch.Value?.Team2.Value?.FullName.Value;

            if (string.IsNullOrEmpty(p1Expected) || string.IsNullOrEmpty(p2Expected))
            {
                return;
            }

            int matchId = int.Parse(matchIdFromSheet.Text);
            loadBansPicksFromSheetStatus.Text = $"Polling {matchId}...";
            var (message, list, scores) = await fetchDataFromSheet(sheetsService, spreadsheetId, matchId, p1Expected, p2Expected).ConfigureAwait(false);
            Schedule(() =>
            {
                loadBansPicksFromSheetStatus.Text = message;
                processNewListFromSheet(list);
                processScoresFromSheet(scores);
            });
        }

        private void processNewListFromSheet(IList<BeatmapChoice> list)
        {
            if (list == null) return;

            if (list.Count < currentMatch.Value.PicksBans.Count)
            {
                // given list is shorter then current list, just remove everything and re-add everything
                currentMatch.Value.PicksBans.Clear();
                currentMatch.Value.PicksBans.AddRange(list);
            }
            else
            {
                for (var i = 0; i < currentMatch.Value.PicksBans.Count; ++i)
                {
                    var pickBan = currentMatch.Value.PicksBans[i];
                    var first = list[0]; // cannot throw, we just checked that list is always larger or equal to currentMatch.Value.PicksBans in size

                    if (first.BeatmapID == pickBan.BeatmapID)
                    {
                        // ok we gud, so far we're following the normal list
                        list.RemoveAt(0);
                    }
                    else
                    {
                        // ok from this point on we've deviated from the provided list
                        // clear everything else that comes next to it
                        for (int j = i; j < currentMatch.Value.PicksBans.Count; ++j)
                        {
                            currentMatch.Value.PicksBans.RemoveAt(j);
                        }

                        break;
                    }
                }

                // either we've reached the end of currentMatch.Value.PicksBans normally, or we cleared everything else
                // now load everything that still sits in list
                currentMatch.Value.PicksBans.AddRange(list);
            }

            setNextMode();
        }

        private void processScoresFromSheet((int p1Score, int p2Score)? scores)
        {
            if (scores == null) return;

            currentMatch.Value.Team1Score.Value = scores.Value.p1Score;
            currentMatch.Value.Team2Score.Value = scores.Value.p2Score;
        }

        private static async Task<(string message, List<BeatmapChoice> list, (int p1Score, int p2Score)?)> fetchDataFromSheet(
            SheetsService sheetsService, string spreadsheetId, int matchId, string p1Expected, string p2Expected)
        {
            try
            {
                var p1NameRange = $"'{matchId}'!B2";
                var p2NameRange = $"'{matchId}'!D2";
                var p1ScoreRange = $"'{matchId}'!B3";
                var p2ScoreRange = $"'{matchId}'!D3";

                var p1P2 = await doSheetGetReq(sheetsService, spreadsheetId, p1NameRange, p2NameRange, p1ScoreRange, p2ScoreRange);
                var p1 = p1P2.ValueRanges[0].Values[0][0] as string;
                var p2 = p1P2.ValueRanges[1].Values[0][0] as string;

                if (string.IsNullOrEmpty(p1) || string.IsNullOrEmpty(p2) || p1 != p1Expected || p2 != p2Expected)
                {
                    return ($"player names not found on the sheet. Found {p1} vs {p2}. Correct Match ID given?", null, null);
                }

                var p1Score = Convert.ToInt32(p1P2.ValueRanges[2].Values[0][0]);
                var p2Score = Convert.ToInt32(p1P2.ValueRanges[3].Values[0][0]);

                var bansRange = $"'{matchId}'!B11:D12";
                var picksRange = $"'{matchId}'!B15:C30";
                var mapPoolRange = $"'{matchId}'!J4:K30";

                var bansPicks = await doSheetGetReq(sheetsService, spreadsheetId, bansRange, picksRange, mapPoolRange);
                var (firstBanPlayer, p1Ban1, p1Ban2) = getBans(bansPicks.ValueRanges[0].Values[0]);
                var (secondBanPlayer, p2Ban1, p2Ban2) = getBans(bansPicks.ValueRanges[0].Values[1]);
                var mapPool = parseMapPool(bansPicks.ValueRanges[2]);

                if (firstBanPlayer != p1 || secondBanPlayer != p2)
                {
                    // ????
                    return ("sheet is borked or this program is borked, could not figure out who banned what", null, (p1Score, p2Score));
                }

                (BeatmapChoice ban1, BeatmapChoice ban2)? processBan(string p1Ban, string p2Ban)
                {
                    var p1Empty = string.IsNullOrEmpty(p1Ban);
                    var p2Empty = string.IsNullOrEmpty(p2Ban);

                    if (p1Empty && p2Empty)
                    {
                        // ban phase didnt happen
                        return (null, null);
                    }

                    BeatmapChoice ban1;

                    if (p1Empty)
                    {
                        ban1 = null;
                    }
                    else if (mapPool.TryGetValue(p1Ban, out var p1Id))
                    {
                        ban1 = new BeatmapChoice
                        {
                            BeatmapID = p1Id,
                            Team = TeamColour.Red,
                            Type = ChoiceType.Ban
                        };
                    }
                    else
                    {
                        // map not found, error
                        return null;
                    }

                    BeatmapChoice ban2;

                    if (p2Empty)
                    {
                        ban2 = null;
                    }
                    else if (mapPool.TryGetValue(p2Ban, out var p2Id))
                    {
                        ban2 = new BeatmapChoice
                        {
                            BeatmapID = p2Id,
                            Team = TeamColour.Blue,
                            Type = ChoiceType.Ban
                        };
                    }
                    else
                    {
                        // map not found, error
                        return null;
                    }

                    return (ban1, ban2);
                }

                var bans1 = processBan(p1Ban1, p2Ban1);
                var bans2 = processBan(p1Ban2, p2Ban2);

                if (bans1 == null || bans2 == null)
                {
                    return ("error parsing bans", null, (p1Score, p2Score));
                }

                var bans = new[] { bans1.Value.ban1, bans1.Value.ban2, bans2.Value.ban1, bans2.Value.ban2 }.Where(x => x != null).ToList();

                var picks = parsePicks(bansPicks.ValueRanges[1].Values, mapPool, p1, p2);

                if (picks == null)
                {
                    return ("error parsing picks", null, (p1Score, p2Score));
                }

                if (picks.Count != 0) return ($"{p1} vs {p2} loaded all bans and picks from sheet!", bans.Concat(picks).ToList(), (p1Score, p2Score));

                var message = bans.Count == 0 ? $"{p1} vs {p2} is still empty on the sheet!" : $"{p1} vs {p2} loaded all bans (no picks) from sheet!";
                return (message, bans, (p1Score, p2Score));
            }
            catch (Exception ex)
            {
                return (ex.ToString(), null, null);
            }
        }

        private static List<BeatmapChoice> parsePicks(IList<IList<object>> rows, IReadOnlyDictionary<string, int> mapPool, string p1, string p2)
        {
            if (rows == null)
            {
                return new List<BeatmapChoice>();
            }

            var list = new List<BeatmapChoice>();
            var prevColour = TeamColour.Red;

            foreach (var row in rows)
            {
                var player = row[0] as string;
                var map = row.Count > 1 ? row[1] as string : null;

                if (string.IsNullOrEmpty(player) || string.IsNullOrEmpty(map))
                {
                    continue;
                }

                if (!mapPool.TryGetValue(map, out var mapId))
                {
                    return null;
                }

                TeamColour col;

                if (player == "Tiebreaker")
                {
                    col = prevColour == TeamColour.Red ? TeamColour.Blue : TeamColour.Red;
                }
                else if (player == p1)
                {
                    col = TeamColour.Red;
                }
                else if (player == p2)
                {
                    col = TeamColour.Blue;
                }
                else
                {
                    return null;
                }

                prevColour = col;
                list.Add(new BeatmapChoice
                {
                    BeatmapID = mapId,
                    Team = col,
                    Type = ChoiceType.Pick
                });
            }

            return list;
        }

        private static readonly Regex map_command_map_id_regex = new Regex("^!mp map (?<mapId>\\d+)", RegexOptions.Compiled);

        private static Dictionary<string, int> parseMapPool(ValueRange valueRange)
        {
            var dict = new Dictionary<string, int>();

            foreach (var row in valueRange.Values)
            {
                var map = row[0] as string;
                var mapCommand = row[1] as string;

                if (string.IsNullOrEmpty(map) || string.IsNullOrEmpty(mapCommand))
                {
                    continue;
                }

                var match = map_command_map_id_regex.Match(mapCommand);
                if (!match.Success) continue;

                var mapId = int.Parse(match.Groups["mapId"].Value);
                dict[map] = mapId;
            }

            return dict;
        }

        private static (string firstBanPlayer, string ban1, string ban2) getBans(IList<object> row)
        {
            var player = row[0] as string;
            var ban1 = row.Count > 1 ? row[1] as string : null;
            var ban2 = row.Count > 2 ? row[2] as string : null;
            return (player, ban1, ban2);
        }

        private static ConfiguredTaskAwaitable<BatchGetValuesResponse> doSheetGetReq(SheetsService sheetsService, string spreadsheetId, params string[] ranges)
        {
            var req = sheetsService.Spreadsheets.Values.BatchGet(spreadsheetId);
            req.Ranges = ranges;
            req.ValueRenderOption = SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            return req.ExecuteAsync().ConfigureAwait(false);
        }

        [BackgroundDependencyLoader]
        private void load(MatchIPCInfo ipc, Storage storage)
        {
            currentMatch.BindValueChanged(matchChanged);
            currentMatch.BindTo(LadderInfo.CurrentMatch);

            ipc.Beatmap.BindValueChanged(beatmapChanged);
            var googleSheetsApiConfigManager = new GoogleSheetsApiConfigManager(storage);

            spreadsheetId = googleSheetsApiConfigManager.Get<string>(GoogleSheetsApiConfig.SpreadsheetId);
            googleApiKey = googleSheetsApiConfigManager.Get<string>(GoogleSheetsApiConfig.GoogleApiKey);

            if (!string.IsNullOrEmpty(spreadsheetId) && !string.IsNullOrEmpty(googleApiKey))
            {
                loadGoogleApiButtons();
            }
        }

        private void beatmapChanged(ValueChangedEvent<BeatmapInfo> beatmap)
        {
            if (currentMatch.Value == null || currentMatch.Value.PicksBans.Count(p => p.Type == ChoiceType.Ban) < 2)
                return;

            // if bans have already been placed, beatmap changes result in a selection being made autoamtically
            if (beatmap.NewValue.OnlineBeatmapID != null)
                addForBeatmap(beatmap.NewValue.OnlineBeatmapID.Value);
        }

        private void setMode(TeamColour colour, ChoiceType choiceType)
        {
            pickColour = colour;
            pickType = choiceType;

            static Color4 setColour(bool active) => active ? Color4.White : Color4.Gray;

            buttonRedBan.Colour = setColour(pickColour == TeamColour.Red && pickType == ChoiceType.Ban);
            buttonBlueBan.Colour = setColour(pickColour == TeamColour.Blue && pickType == ChoiceType.Ban);
            buttonRedPick.Colour = setColour(pickColour == TeamColour.Red && pickType == ChoiceType.Pick);
            buttonBluePick.Colour = setColour(pickColour == TeamColour.Blue && pickType == ChoiceType.Pick);
        }

        private void setNextMode()
        {
            const TeamColour roll_winner = TeamColour.Red; //todo: draw from match

            var nextColour = (currentMatch.Value.PicksBans.LastOrDefault()?.Team ?? roll_winner) == TeamColour.Red ? TeamColour.Blue : TeamColour.Red;

            if (pickType == ChoiceType.Ban && currentMatch.Value.PicksBans.Count(p => p.Type == ChoiceType.Ban) >= 2)
                setMode(pickColour, ChoiceType.Pick);
            else
                setMode(nextColour, currentMatch.Value.PicksBans.Count(p => p.Type == ChoiceType.Ban) >= 2 ? ChoiceType.Pick : ChoiceType.Ban);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            var maps = mapFlows.Select(f => f.FirstOrDefault(m => m.ReceivePositionalInputAt(e.ScreenSpaceMousePosition)));
            var map = maps.FirstOrDefault(m => m != null);

            if (map != null)
            {
                if (e.Button == MouseButton.Left && map.Beatmap.OnlineBeatmapID != null)
                    addForBeatmap(map.Beatmap.OnlineBeatmapID.Value);
                else
                {
                    var existing = currentMatch.Value.PicksBans.FirstOrDefault(p => p.BeatmapID == map.Beatmap.OnlineBeatmapID);

                    if (existing != null)
                    {
                        currentMatch.Value.PicksBans.Remove(existing);
                        setNextMode();
                    }
                }

                return true;
            }

            return base.OnMouseDown(e);
        }

        private void reset()
        {
            currentMatch.Value.PicksBans.Clear();
            setNextMode();
        }

        private ScheduledDelegate scheduledChange;

        private void addForBeatmap(int beatmapId)
        {
            if (currentMatch.Value == null)
                return;

            if (currentMatch.Value.Round.Value.Beatmaps.All(b => b.BeatmapInfo.OnlineBeatmapID != beatmapId))
                // don't attempt to add if the beatmap isn't in our pool
                return;

            if (currentMatch.Value.PicksBans.Any(p => p.BeatmapID == beatmapId))
                // don't attempt to add if already exists.
                return;

            currentMatch.Value.PicksBans.Add(new BeatmapChoice
            {
                Team = pickColour,
                Type = pickType,
                BeatmapID = beatmapId
            });

            setNextMode();

            if (pickType == ChoiceType.Pick && currentMatch.Value.PicksBans.Any(i => i.Type == ChoiceType.Pick))
            {
                scheduledChange?.Cancel();
                scheduledChange = Scheduler.AddDelayed(() => { sceneManager?.SetScreen(typeof(GameplayScreen)); }, 10000);
            }
        }

        private void matchChanged(ValueChangedEvent<TournamentMatch> match)
        {
            if (matchIdFromSheet != null) matchIdFromSheet.Text = string.Empty;
            if (loadBansPicksFromSheetStatus != null) loadBansPicksFromSheetStatus.Text = string.Empty;

            mapFlows.Clear();

            int totalRows = 0;

            if (match.NewValue.Round.Value != null)
            {
                FillFlowContainer<TournamentBeatmapPanel> currentFlow = null;
                string currentMod = null;

                int flowCount = 0;

                foreach (var b in match.NewValue.Round.Value.Beatmaps)
                {
                    if (currentFlow == null || currentMod != b.Mods)
                    {
                        mapFlows.Add(currentFlow = new FillFlowContainer<TournamentBeatmapPanel>
                        {
                            Spacing = new Vector2(10, 5),
                            Direction = FillDirection.Full,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y
                        });

                        currentMod = b.Mods;

                        totalRows++;
                        flowCount = 0;
                    }

                    if (++flowCount > 2)
                    {
                        totalRows++;
                        flowCount = 1;
                    }

                    currentFlow.Add(new TournamentBeatmapPanel(b.BeatmapInfo, b.Mods)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Height = 42,
                    });
                }
            }

            mapFlows.Padding = new MarginPadding(5)
            {
                // remove horizontal padding to increase flow width to 3 panels
                Horizontal = totalRows > 9 ? 0 : 100
            };
        }
    }
}
