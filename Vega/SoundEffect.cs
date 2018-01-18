#if SOUND_ENGINE

using System;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Vega
{
    internal class Pool : IDisposable
    {
        private int[] Sources;
        private int N;

        public Pool()
        {
            this.N = 0;
            this.Sources = new int[10];
            AL.GenSources(this.Sources);
        }

        public int Next()
        {
            int ret = this.Sources[this.N];
            this.N++;
            if (this.N >= this.Sources.Length)
                this.N = 0;
            return ret;
        }

        public void Dispose()
        {
            foreach (int source in this.Sources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }
        }
    }

    public class SoundEffect
        : IDisposable
    {
        private static Pool SourcePool = new Pool();

        public bool IsDisposed { get; private set; }
        private int Buffer;
        private int LastPlayed;

        public SoundEffect(string path)
        {
            this.IsDisposed = false;
            this.Buffer = AL.GenBuffer();
            byte[] soundData = LoadWave(new FileStream(path, FileMode.Open, FileAccess.Read),
                out int numChannels, out int bitsPerSample, out int sampleRate);
            AL.BufferData(this.Buffer, GetSoundFormat(numChannels, bitsPerSample), soundData, soundData.Length, sampleRate);
        }

        public void Play()
        {
            if (this.IsDisposed)
                throw new InvalidOperationException("Tried to play sound, but it is disposed");
            if (Main.Self.Config.MuteEffect == 1)
                return;
            if (this.LastPlayed == Main.Self.TotalFrameCount)
                return;
            this.LastPlayed = Main.Self.TotalFrameCount;
            int source = SourcePool.Next();
            AL.SourceStop(source);
            AL.Source(source, ALSourcei.Buffer, this.Buffer);
            AL.SourcePlay(source);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            AL.DeleteBuffer(this.Buffer);
            this.IsDisposed = true;
        }

        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1:
                    return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2:
                    return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default:
                    throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        private static byte[] LoadWave(Stream stream, out int numChannels, out int bitsPerSample, out int sampleRate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                if (new string(reader.ReadChars(4)) != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riffChunckSize = reader.ReadInt32();

                if (new string(reader.ReadChars(4)) != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                if (new string(reader.ReadChars(4)) != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");
                // format_chunk_size
                reader.ReadInt32();
                // audio_format
                reader.ReadInt16();
                numChannels = reader.ReadInt16();
                sampleRate = reader.ReadInt32();
                // byte_rate
                reader.ReadInt32();
                // block_align
                reader.ReadInt16();
                bitsPerSample = reader.ReadInt16();

                if (new string(reader.ReadChars(4)) != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                // data_chunk_size
                reader.ReadInt32();

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }
    }
}
#else
using System;

namespace Vega
{

    public class SoundEffect
        : IDisposable
    {
        public bool IsDisposed { get; private set; }
        private int LastPlayed;

        public SoundEffect()
        {
            this.IsDisposed = false;
        }

        public void Play()
        {
            if (this.IsDisposed)
                throw new InvalidOperationException("Tried to play sound, but it is disposed");
            if (this.LastPlayed == Main.Self.TotalFrameCount)
                return;
            this.LastPlayed = Main.Self.TotalFrameCount;
        }

        public void Dispose()
        {
        }
    }
}
#endif