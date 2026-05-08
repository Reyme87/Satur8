namespace Satur8.Audio
{
    public class TestOscillator
    {
        private double phase = 0.0;
        private double sampleRate;

        /// <summary>
        /// Частота дискретизации (Гц)
        /// </summary>
        public double SampleRate
        {
            get => sampleRate;
            set => sampleRate = value > 0 ? value : 44100.0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sampleRate">Частота дискретизации (по умолчанию 44100 Гц)</param>
        public TestOscillator(double sampleRate = 44100.0)
        {
            this.sampleRate = sampleRate;
        }

        /// <summary>
        /// Сброс фазы генератора
        /// </summary>
        public void Reset()
        {
            phase = 0.0;
        }

        /// <summary>
        /// Генерация синусоидального сигнала
        /// </summary>
        /// <param name="frequency">Частота в Гц</param>
        /// <param name="amplitude">Амплитуда (0-1)</param>
        /// <returns>Следующий семпл</returns>
        public float GetSine(float frequency, float amplitude = 1.0f)
        {
            float value = (float)(Math.Sin(2.0 * Math.PI * frequency * phase / sampleRate) * amplitude);
            phase += 1.0;
            return value;
        }

        /// <summary>
        /// Создает массив семплов синусоиды
        /// </summary>
        /// <param name="numSamples">Количество семплов</param>
        /// <param name="frequency">Частота в Гц</param>
        /// <param name="amplitude">Амплитуда</param>
        /// <returns>Массив float с синусоидой</returns>
        public float[] GenerateSineWave(int numSamples, float frequency, float amplitude = 1.0f)
        {
            float[] buffer = new float[numSamples];
            Reset();

            for (int i = 0; i < numSamples; i++)
            {
                buffer[i] = GetSine(frequency, amplitude);
            }

            return buffer;
        }
    }
}
