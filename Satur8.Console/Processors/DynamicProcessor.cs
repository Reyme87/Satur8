using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satur8.Processors
{
    public class DynamicsProcessor
    {
        // Параметры
        public double ThresholdDb { get; set; } = -12.0;   // порог в dB
        public double Ratio { get; set; } = 4.0;          // >1 компрессия, <1 экспансия, =1 нейтрально
        public double AttackMs { get; set; } = 5.0;       // миллисекунды
        public double ReleaseMs { get; set; } = 100.0;
        public double MakeupGainDb { get; set; } = 0.0;

        // Коэффициенты фильтров огибающей (рассчитываются из времени атаки/релиза)
        private double attackCoeff;
        private double releaseCoeff;
        private double envelope = 0.0;   // текущая огибающая (в dB или линейная)

        private double sampleRate;

        public DynamicsProcessor(double sampleRate = 44100.0)
        {
            this.sampleRate = sampleRate;
            UpdateTimeConstants();
        }

        // Пересчёт коэффициентов атаки/релиза при изменении параметров
        public void UpdateTimeConstants()
        {
            attackCoeff = TimeConstantToCoeff(AttackMs);
            releaseCoeff = TimeConstantToCoeff(ReleaseMs);
        }

        private double TimeConstantToCoeff(double ms)
        {
            if (ms <= 0) return 1.0;
            double tau = ms * 0.001; // секунды
            return 1.0 - Math.Exp(-1.0 / (tau * sampleRate));
        }

        // Преобразование уровня в dB (с защитой от нуля)
        private double LinearToDb(double linear)
        {
            return 20.0 * Math.Log10(Math.Max(linear, 1e-9));
        }

        private double DbToLinear(double db)
        {
            return Math.Pow(10.0, db / 20.0);
        }

        /// <summary>
        /// Вычисляет необходимое усиление (gain reduction) в dB на основе текущего уровня.
        /// </summary>
        private double ComputeGainDb(double inputLevelDb)
        {
            // Разница между уровнем и порогом
            double over = inputLevelDb - ThresholdDb;
            if (over <= 0.0) return 0.0; // ниже порога — не трогаем

            // Компрессия или экспансия
            // Компрессия: выходной уровень = порог + over / ratio
            // Экспансия: выходной уровень = порог + over * ratio (при ratio<1)
            double outputLevelDb = ThresholdDb + over / Ratio;
            return outputLevelDb - inputLevelDb; // отрицательное значение для компрессии
        }

        /// <summary>
        /// Обработка одного семпла
        /// </summary>
        public float ProcessSample(float input)
        {
            // 1. Извлекаем огибающую (используем RMS или пик; для простоты — абсолютное значение)
            double absInput = Math.Abs(input);

            // 2. Обновляем огибающую (фильтр атаки/релиза)
            if (absInput > envelope)
                envelope += attackCoeff * (absInput - envelope);
            else
                envelope += releaseCoeff * (absInput - envelope);

            // 3. Преобразуем огибающую в dB
            double envelopeDb = LinearToDb(envelope);

            // 4. Вычисляем gain reduction в dB
            double gainReductionDb = ComputeGainDb(envelopeDb);

            // 5. Применяем makeup gain
            double totalGainDb = gainReductionDb + MakeupGainDb;
            double totalGainLinear = DbToLinear(totalGainDb);

            // 6. Умножаем входной сигнал на усиление
            return (float)(input * totalGainLinear);
        }

        /// <summary>
        /// Обработка буфера
        /// </summary>
        public void ProcessBuffer(float[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
                buffer[i] = ProcessSample(buffer[i]);
        }

        // Сброс состояния огибающей
        public void Reset()
        {
            envelope = 0.0;
        }
    }
}
