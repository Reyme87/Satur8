using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satur8.Maths
{
    public class HarmonicSaturator
    {
        private Clipper _clipper;

        /// <summary>
        /// Выбор типа клиппинга: "Hard" или "Soft"
        /// </summary>
        public string ClipType { get; set; } = "Soft";

        /// <summary>
        /// Коэффициент для чётной составляющей (0..2)
        /// </summary>
        public float EvenAmount { get; set; } = 0.5f;

        /// <summary>
        /// Коэффициент для нечётной составляющей (0..2)
        /// </summary>
        public float OddAmount { get; set; } = 1.0f;

        /// <summary>
        /// Драйв (усиление перед клиппингом)
        /// </summary>
        public float Drive { get; set; } = 1.0f;

        /// <summary>
        /// Выходное усиление
        /// </summary>
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

        public void SetClipType(string type)
        {
            ClipType = type;
        }

        private float ClipFunction(float x)
        {
            if (ClipType == "Hard")
                return _clipper.HardClip(x);
            else
                return _clipper.SoftClip(x);
        }

        /// <summary>
        /// Обработка одного семпла
        /// </summary>
        public float ProcessSample(float input)
        {
            float x = input * Drive;
            float fx = ClipFunction(x);
            float f_minus_x = ClipFunction(-x);

            // Чётная часть
            float evenPart = (fx + f_minus_x) * 0.5f;
            // Нечётная часть
            float oddPart = (fx - f_minus_x) * 0.5f;

            float result = evenPart * EvenAmount + oddPart * OddAmount;
            return result * OutputGain;
        }

        /// <summary>
        /// Обработка буфера
        /// </summary>
        public void ProcessBuffer(float[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
                buffer[i] = ProcessSample(buffer[i]);
        }
    }
}
