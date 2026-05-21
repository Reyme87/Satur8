using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satur8.Maths
{
    public class HarmonicSaturator
    {
        private readonly Clipper _clipper;

        public string ClipType { get; set; } = "Soft";
        public float Drive { get; set; } = 1.0f;
        public float EvenAmount { get; set; } = 0.5f;
        public float OddAmount { get; set; } = 1.0f;
        public float OutputGain { get; set; } = 1.0f;

        public float Bias
        {
            get => _clipper.Bias;
            set => _clipper.Bias = value;
        }

        public HarmonicSaturator(Clipper clipper)
        {
            _clipper = clipper;
        }

        private float ClipFunction(float x) => ClipType switch
        {
            "Hard" => _clipper.HardClip(x),
            "Tube" => _clipper.TanhClip(x),
            "Arctan" => _clipper.ArctanClip(x),
            _ => _clipper.SoftClip(x)
        };

        private float ClipperNorm() => ClipType switch
        {
            "Hard" => 1.0f / Math.Max(_clipper.HardThreshold, 1e-6f),
            "Tube" => 1.0f / (float)Math.Tanh(1.0),
            "Arctan" => 1.0f / (float)(2.0 / Math.PI * Math.Atan(Math.PI / 2.0)),
            _ => 1.5f
        };

        public float ProcessSample(float input)
        {
            float x = input * Drive;

            float fx = ClipFunction(x);
            float fnx = ClipFunction(-x);

            float evenPart = (fx + fnx) * 0.5f;

            float oddPart = (fx - fnx) * 0.5f;

            float norm = ClipperNorm() / Math.Max(Drive, 0.01f);

            float result = (evenPart * EvenAmount + oddPart * OddAmount) * norm;

            return result * OutputGain;
        }

        public void ProcessBuffer(float[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
                buffer[i] = ProcessSample(buffer[i]);
        }
    }
}
