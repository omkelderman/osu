// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.IO;
using osu.Game.Tournament.IPC;
using osu.Game.Tournament.Models;
using osu.Game.Tournament.Screens.Gameplay.Components;
using osu.Game.Tournament.Screens.MapPool;
using osu.Game.Tournament.Screens.TeamWin;
using osuTK.Graphics;

namespace osu.Game.Tournament.Screens.Gameplay
{
    public class GameplayScreen : BeatmapInfoScreen, IProvideVideo
    {
        private readonly BindableBool warmup = new BindableBool();

        public readonly Bindable<TourneyState> State = new Bindable<TourneyState>();
        private OsuButton warmupButton;
        private MatchIPCInfo ipc;

        [Resolved(canBeNull: true)]
        private TournamentSceneManager sceneManager { get; set; }

        [Resolved]
        private TournamentMatchChatDisplay chat { get; set; }

        private Drawable chroma;

        private Container<TourneyVideo> videosContainer;
        private TourneyVideo fallbackGameplayVideo;
        private readonly List<(string, TourneyVideo)> modSpecificGameplayVideos = new List<(string, TourneyVideo)>();

        [BackgroundDependencyLoader]
        private void load(LadderInfo ladder, MatchIPCInfo ipc, TournamentStorage storage)
        {
            this.ipc = ipc;

            var videos = new List<TourneyVideo>();

            // look for all video files that start with "gameplay-"
            if (storage.ExistsDirectory("videos"))
            {
                foreach (var name in storage.GetFiles("videos", "gameplay-*").Select(Path.GetFileNameWithoutExtension))
                {
                    var vid = new TourneyVideo(name)
                    {
                        Name = name,
                        Loop = true,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    };
                    videos.Add(vid);
                    modSpecificGameplayVideos.Add((name.Substring("gameplay-".Length), vid));
                }
            }

            fallbackGameplayVideo = new TourneyVideo("gameplay")
            {
                Name = "gameplay",
                Loop = true,
                RelativeSizeAxes = Axes.Both,
            };
            videos.Add(fallbackGameplayVideo);

            AddRangeInternal(new Drawable[]
            {
                videosContainer = new Container<TourneyVideo>
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = videos
                },
                header = new MatchHeader
                {
                    ShowLogo = false
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Y = 110,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new[]
                    {
                        chroma = new Container
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Height = 512,
                            Children = new Drawable[]
                            {
                                new ChromaArea
                                {
                                    Name = "Left chroma",
                                    RelativeSizeAxes = Axes.Both,
                                    Width = 0.5f,
                                },
                                new ChromaArea
                                {
                                    Name = "Right chroma",
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    Width = 0.5f,
                                }
                            }
                        },
                    }
                },
                scoreDisplay = new TournamentMatchScoreDisplay
                {
                    Y = -147,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.TopCentre,
                },
                new ControlPanel
                {
                    Children = new Drawable[]
                    {
                        warmupButton = new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Toggle warmup",
                            Action = () => warmup.Toggle()
                        },
                        new TourneyButton
                        {
                            RelativeSizeAxes = Axes.X,
                            Text = "Toggle chat",
                            Action = () => { State.Value = State.Value == TourneyState.Idle ? TourneyState.Playing : TourneyState.Idle; }
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Chroma width",
                            Current = LadderInfo.ChromaKeyWidth,
                            KeyboardStep = 1,
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Players per team",
                            Current = LadderInfo.PlayersPerTeam,
                            KeyboardStep = 1,
                        }
                    }
                }
            });

            ladder.ChromaKeyWidth.BindValueChanged(width => chroma.Width = width.NewValue, true);

            warmup.BindValueChanged(w =>
            {
                warmupButton.Alpha = !w.NewValue ? 0.5f : 1;
                header.ShowScores = !w.NewValue;
            }, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            State.BindTo(ipc.State);
            State.BindValueChanged(stateChanged, true);
            ipc.Beatmap.BindValueChanged(beatmapChanged, true);
        }

        protected override void CurrentMatchChanged(ValueChangedEvent<TournamentMatch> match)
        {
            base.CurrentMatchChanged(match);

            if (match.NewValue == null)
                return;

            warmup.Value = match.NewValue.Team1Score.Value + match.NewValue.Team2Score.Value == 0;
            scheduledOperation?.Cancel();
        }

        private void beatmapChanged(ValueChangedEvent<BeatmapInfo> beatmap)
        {
            if (beatmap.NewValue == null || CurrentMatch.Value == null)
                return;

            var currentSelectedBeatmapMods = CurrentMatch.Value.Round.Value.Beatmaps.FirstOrDefault(b => b.ID == beatmap.NewValue.OnlineBeatmapID)?.Mods;
            showVideoForModsAndHideAllElse(currentSelectedBeatmapMods);
        }

        private ScheduledDelegate hideVideosScheduleOperation;

        private void showVideoForModsAndHideAllElse(string currentSelectedBeatmapMods)
        {
            hideVideosScheduleOperation?.Cancel();
            var videosThatShouldBeHidden = new List<TourneyVideo>();
            TourneyVideo videoToShow = null;

            foreach (var (mods, video) in modSpecificGameplayVideos)
            {
                if (mods.Equals(currentSelectedBeatmapMods, StringComparison.OrdinalIgnoreCase))
                {
                    videoToShow = video;
                }
                else
                {
                    videosThatShouldBeHidden.Add(video);
                }
            }

            if (videoToShow == null)
            {
                videoToShow = fallbackGameplayVideo;
            }
            else
            {
                videosThatShouldBeHidden.Add(fallbackGameplayVideo);
            }

            // put video to show on top, and fade it in
            videosContainer.ChangeChildDepth(videoToShow, (float)-Clock.CurrentTime);
            videoToShow.FadeIn(FADE_DELAY);

            // after the fade, hide all others
            hideVideosScheduleOperation = Scheduler.AddDelayed(() =>
            {
                foreach (var video in videosThatShouldBeHidden)
                {
                    video.FadeOut();
                }
            }, FADE_DELAY);
        }

        private ScheduledDelegate scheduledOperation;
        private TournamentMatchScoreDisplay scoreDisplay;

        private TourneyState lastState;
        private MatchHeader header;

        private void stateChanged(ValueChangedEvent<TourneyState> state)
        {
            try
            {
                if (state.NewValue == TourneyState.Ranking)
                {
                    if (warmup.Value || CurrentMatch.Value == null) return;

                    if (ipc.Score1.Value > ipc.Score2.Value)
                        CurrentMatch.Value.Team1Score.Value++;
                    else
                        CurrentMatch.Value.Team2Score.Value++;
                }

                scheduledOperation?.Cancel();

                void expand()
                {
                    chat?.Contract();

                    using (BeginDelayedSequence(300))
                    {
                        scoreDisplay.FadeIn(100);
                        SongBar.Expanded = true;
                    }
                }

                void contract()
                {
                    SongBar.Expanded = false;
                    scoreDisplay.FadeOut(100);
                    using (chat?.BeginDelayedSequence(500))
                        chat?.Expand();
                }

                switch (state.NewValue)
                {
                    case TourneyState.Idle:
                        contract();

                        const float delay_before_progression = 4000;

                        // if we've returned to idle and the last screen was ranking
                        // we should automatically proceed after a short delay
                        if (lastState == TourneyState.Ranking && !warmup.Value)
                        {
                            if (CurrentMatch.Value?.Completed.Value == true)
                                scheduledOperation = Scheduler.AddDelayed(() => { sceneManager?.SetScreen(typeof(TeamWinScreen)); }, delay_before_progression);
                            else if (CurrentMatch.Value?.Completed.Value == false)
                                scheduledOperation = Scheduler.AddDelayed(() => { sceneManager?.SetScreen(typeof(MapPoolScreen)); }, delay_before_progression);
                        }

                        break;

                    case TourneyState.Ranking:
                        scheduledOperation = Scheduler.AddDelayed(contract, 10000);
                        break;

                    default:
                        chat.Contract();
                        expand();
                        break;
                }
            }
            finally
            {
                lastState = state.NewValue;
            }
        }

        private class ChromaArea : CompositeDrawable
        {
            [Resolved]
            private LadderInfo ladder { get; set; }

            [BackgroundDependencyLoader]
            private void load()
            {
                // chroma key area for stable gameplay
                Colour = new Color4(0, 255, 0, 255);

                ladder.PlayersPerTeam.BindValueChanged(performLayout, true);
            }

            private void performLayout(ValueChangedEvent<int> playerCount)
            {
                switch (playerCount.NewValue)
                {
                    case 3:
                        InternalChildren = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.5f,
                                Height = 0.5f,
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Height = 0.5f,
                            },
                        };
                        break;

                    default:
                        InternalChild = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                        };
                        break;
                }
            }
        }
    }
}
