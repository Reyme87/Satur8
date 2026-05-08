using NAudio.Wave;
using Satur8.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satur8.Processors
{
    public class AudioProcessingChain : ISampleProvider
    {
        private ISampleProvider _source;
        private DynamicsProcessor _dynamics;
        private HarmonicSaturator _saturator;

        public AudioProcessingChain(ISampleProvider source, Clipper clipper)
        {
            _source = source;
            _dynamics = new DynamicsProcessor(source.WaveFormat.SampleRate);
            _saturator = new HarmonicSaturator(clipper);
        }

        public WaveFormat WaveFormat => _source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);
            if (samplesRead == 0)
            {
                return 0;
            }

            _dynamics.ProcessBuffer(buffer, offset, samplesRead);
            _saturator.ProcessBuffer(buffer, offset, samplesRead);

            return samplesRead;
        }

        public void SetDynamics(double thresholdDb, double ratio, double attackMs, double releaseMs, double makeupDb)
        {
            _dynamics.ThresholdDb = thresholdDb;
            _dynamics.Ratio = ratio;
            _dynamics.AttackMs = attackMs;
            _dynamics.ReleaseMs = releaseMs;
            _dynamics.MakeupGainDb = makeupDb;
            _dynamics.UpdateTimeConstants();
        }

        public void SetSaturation(float evenAmount, float oddAmount, float drive, string clipType, float bias)
        {
            _saturator.EvenAmount = evenAmount;
            _saturator.OddAmount = oddAmount;
            _saturator.Drive = drive;
            _saturator.Bias = bias;
            _saturator.SetClipType(clipType);
        }
    }
}
