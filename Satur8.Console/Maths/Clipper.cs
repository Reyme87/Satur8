namespace Satur8.Maths
{
    public class Clipper
    {
        public float HardThreshold { get; set; } = 1.0f;
        public float Bias { get; set; } = 0.0f;

        public float HardClip(float signal)
        {
            return Math.Clamp(signal + Bias, -HardThreshold, HardThreshold);
        }

        public float SoftClip(float signal)
        {
            float s = signal + Bias;
            if (s >= 1.0f) return 2.0f / 3.0f;
            if (s <= -1.0f) return -2.0f / 3.0f;
            return s - (s * s * s) / 3.0f;
        }

        public float TanhClip(float signal)
        {
            return (float)Math.Tanh(signal + Bias);
        }

        public float ArctanClip(float signal)
        {
            return (float)(2.0 / Math.PI * Math.Atan((signal + Bias) * Math.PI / 2.0));
        }
    }
}
