using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Satur8.Audio;
using Satur8.Maths;
using Satur8.Processors;
using System;
using System.Security.Claims;

namespace Satur8
{
    class Program
    {
        private static AudioProcessingChain _chain;
        private static WaveOutEvent _outputDevice;
        private static bool _running = true;

        public static void Main(string[] args)
        {
            string wavPath = @"C:\Users\Evan\Desktop\кокоджамбо\recorddft.wav";

            if (!System.IO.File.Exists(wavPath))
            {
                Console.WriteLine($"File not found: {wavPath}");
                Console.WriteLine("Generating test tone instead...");
                // Генератор тестового тона (синус 440 Гц, 5 секунд)
                var toneProvider = new SignalGenerator(44100, 1)
                {
                    Gain = 0.8,
                    Frequency = 440,
                    Type = SignalGeneratorType.Sin
                };
                // Ограничим 5 секундами
                var takeProvider = new TakeProvider(toneProvider, TimeSpan.FromSeconds(5));
                var clipper = new Clipper();
                _chain = new AudioProcessingChain(takeProvider, clipper);
            }
            else
            {
                var audioFile = new AudioFileReader(wavPath);
                var clipper = new Clipper();
                _chain = new AudioProcessingChain(audioFile, clipper);
            }

            // Настройка начальных параметров
            _chain.SetDynamics(thresholdDb: -12.0, ratio: 4.0, attackMs: 10.0, releaseMs: 100.0, makeupDb: 6.0);
            _chain.SetSaturation(evenAmount: 0.0f, oddAmount: 1.0f, drive: 1.0f, clipType: "Soft", bias: 0);

            // Воспроизведение
            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_chain);
            _outputDevice.Play();

            Console.WriteLine("Playing... Press 'p' to change params, 'q' to quit.");
            Console.WriteLine("Commands:");
            Console.WriteLine("  d: change dynamics (threshold, ratio)");
            Console.WriteLine("  s: change saturation (even, odd, drive)");
            Console.WriteLine("  t: change clip type (Hard/Soft)");
            Console.WriteLine("  q: quit");

            while (_running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).KeyChar;
                    switch (key)
                    {
                        case 'd':
                            ChangeDynamicsParams();
                            break;
                        case 's':
                            ChangeSaturationParams();
                            break;
                        case 't':
                            ChangeClipType();
                            break;
                        case 'q':
                            _running = false;
                            break;
                    }
                }
                Thread.Sleep(100);
            }

            _outputDevice.Stop();
            _outputDevice.Dispose();
            Console.WriteLine("Done.");

        }



        static void ChangeDynamicsParams()
        {
            Console.Write("Threshold (dB, e.g. -12): ");
            double thresh = double.Parse(Console.ReadLine());
            Console.Write("Ratio (>1 comp, <1 exp, e.g. 4): ");
            double ratio = double.Parse(Console.ReadLine());
            Console.Write("Attack (ms): ");
            double attack = double.Parse(Console.ReadLine());
            Console.Write("Release (ms): ");
            double release = double.Parse(Console.ReadLine());
            Console.Write("Makeup gain (dB): ");
            double makeup = double.Parse(Console.ReadLine());

            _chain.SetDynamics(thresh, ratio, attack, release, makeup);
            Console.WriteLine("Dynamics updated.");
        }

        static void ChangeSaturationParams()
        {
            Console.Write("Even amount (0..1): ");
            float even = float.Parse(Console.ReadLine());
            Console.Write("Odd amount (0..1): ");
            float odd = float.Parse(Console.ReadLine());
            Console.Write("Drive (0.5..3): ");
            float drive = float.Parse(Console.ReadLine());
            Console.Write("Bias (-0.5..0.5): ");
            float bias = float.Parse(Console.ReadLine());

            _chain.SetSaturation(even, odd, drive, "Soft", bias);
        }

        static void ChangeClipType()
        {
            Console.Write("Clip type (Hard/Soft): ");
            string type = Console.ReadLine();
            var sat = _chain.GetType().GetField("saturator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // Упростим: пересоздадим? Лучше добавить метод в AudioProcessingChain
            Console.WriteLine("Not fully implemented yet, but you can modify code.");
        }
    }
}

public class TakeProvider : ISampleProvider
{
    private ISampleProvider source;
    private int maxSamples;
    private int samplesRead = 0;

    public TakeProvider(ISampleProvider source, TimeSpan duration)
    {
        this.source = source;
        maxSamples = (int)(duration.TotalSeconds * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
    }

    public WaveFormat WaveFormat => source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        int remaining = maxSamples - samplesRead;
        if (remaining <= 0) return 0;
        int toRead = Math.Min(count, remaining);
        int read = source.Read(buffer, offset, toRead);
        samplesRead += read;
        return read;
    }
}