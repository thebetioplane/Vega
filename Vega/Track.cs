using System;
using System.Collections.Generic;
using System.IO;
using ManagedBass;

namespace Vega
{
    public enum SongPlayerStatus { Stopped, Playing, Paused }
    public class Track
    {
        public string Title { get; set; }
        public string Artist { get; set; }

        public string SongFileName { get; set; }
        public List<TimingPoint> Timing;
        public string Directory { get; set; }
        public string SongFullPath => Path.Combine(this.Directory, this.SongFileName);
        private IniFile Meta;
        public Track(string directory)
        {
            this.Directory = directory;
            string metaPath = Path.Combine(this.Directory, "meta");
            if (! File.Exists(metaPath))
                throw new FileNotFoundException("Track has no meta file!", metaPath);
            this.Meta = new IniFile(metaPath);
            this.Meta.ReadFile();
            this.Timing = new List<TimingPoint>();
            this.Title = this.Meta["title"] ?? "unknown";
            this.Artist = this.Meta["artist"] ?? "unknown";
            this.SongFileName = this.Meta["song"];
            this.Meta.WriteFile();
            if (this.SongFileName == null)
                throw new FileNotFoundException("There is no song file");
            if (! File.Exists(this.SongFullPath))
                throw new FileNotFoundException("Cannot find song file", this.SongFileName);
            if (this.Meta["bpm1"] == null)
            {
                this.Timing.Add(new TimingPoint(60.0f, 0));
            }
            else
            {
                for (int i = 1; ; ++i)
                {
                    string value = this.Meta["bpm" + i.ToString()];
                    if (value == null)
                        break;
                    this.Timing.Add(new TimingPoint(value));
                }
            }
            this.Timing.Sort();
        }
        public GameModePlay.Level GetLevel(int n)
        {
            return new GameModePlay.Level(this);
        }
        private static int SPStream;
        private static SongPlayerStatus SPStatus;
        public static SongPlayerStatus GetSPStatus()
        {
            return SPStatus;
        }
        private static Track SPCurrentTrack = null;
        public static Track GetSPCurrentTrack()
        {
            return SPCurrentTrack;
        }
        static Track()
        {
            try
            {
                Bass.Init();
            }
            catch (DllNotFoundException)
            {
                throw new Exception("You are missing bass.dll. If you are on windows, the updater should get it for you. If you are on linux, then you need to download `libbass.so` for your platform (32 bit or 64 bit) from un4seen.com and put it in your /usr/lib.");
            }
            SPStream = 0;
            SPStatus = SongPlayerStatus.Stopped;
        }
        public void Load()
        {
            this.Unload();
            SPStream = Bass.CreateStream(this.SongFullPath);
            if (SPStream == 0)
                throw new Exception("bass.dll could not create stream");
            SPCurrentTrack = this;
        }
        public void Unload()
        {
            if (SPStatus == SongPlayerStatus.Stopped)
                return;
            Bass.StreamFree(SPStream);
            SPStream = 0;
            SPStatus = SongPlayerStatus.Stopped;
        }
        public void Play()
        {
            if (SPStatus == SongPlayerStatus.Playing)
                return;
            if (SPStream == 0)
                this.Load();
            Bass.ChannelPlay(SPStream);
            SPStatus = SongPlayerStatus.Playing;
        }
        public void Pause()
        {
            if (SPStatus == SongPlayerStatus.Paused)
                return;
            if (SPStream == 0)
                throw new InvalidOperationException("Stream not loaded");
            Bass.ChannelPause(SPStream);
            SPStatus = SongPlayerStatus.Paused;
        }
        public double GetSeconds()
        {
            return Bass.ChannelBytes2Seconds(SPStream, Bass.ChannelGetPosition(SPStream));
        }
    }
}