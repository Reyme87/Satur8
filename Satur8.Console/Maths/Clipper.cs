namespace Satur8.Maths
{
    public class Clipper
    {
        public float HardThreshold { get; set; } = 0.8f;
        public  float Bias { get; set; } = 0.0f;

        public float HardClip(float signal)
        {
            float signalB = signal + Bias;
            float clipped = Math.Clamp(signalB, -HardThreshold, HardThreshold);

            return clipped;
        }

        public float SoftClip(float signal)
        {
            float signalB = signal + Bias;
            if (signal <= -1)
            {
                return -2.0f / 3.0f;
            }
            if (signal >= 1)
            {
                return 2.0f / 3.0f;
            }

            return (float)(signalB - Math.Pow(signalB, 3) / 3);
        }
    }
}
