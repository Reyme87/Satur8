public class PhysicalSaturator
{
    // Параметры асимметричной нелинейности
    public float Asymmetry { get; set; } = 0.0f;   // -1..1, смещение кривой
    public float Drive { get; set; } = 1.0f;
    public float OutputGain { get; set; } = 1.0f;

    // Параметры динамики (зависят от огибающей)
    public float EnvelopeSensitivity { get; set; } = 0.0f; // 0..1
    private float envelope = 0.0f;
    private float envelopeAttack = 0.01f;   // время атаки в секундах
    private float envelopeRelease = 0.1f;
    private float sampleRate = 44100.0f;

    // Параметры гистерезиса (эффект памяти)
    public float Hysteresis { get; set; } = 0.0f;  // 0..1
    private float lastOutput = 0.0f;

    public PhysicalSaturator(float sampleRate = 44100.0f)
    {
        this.sampleRate = sampleRate;
    }

    // Основная нелинейная функция (рациональная)
    private float RationalNonlinearity(float x, float asym)
    {
        // Асимметрия: сдвигаем вход и меняем коэффициенты для +/- частей
        float xShifted = x + asym * 0.5f;
        float sign = Math.Sign(xShifted);
        float absX = Math.Abs(xShifted);

        // Коэффициенты (можно вынести в параметры)
        float a1 = 1.2f, a3 = 0.4f;   // числитель
        float b1 = 0.3f, b2 = 0.2f;   // знаменатель

        float numerator = a1 * xShifted + a3 * xShifted * xShifted * xShifted;
        float denominator = 1.0f + b1 * absX + b2 * xShifted * xShifted;

        return numerator / denominator;
    }

    // Динамическое обновление огибающей
    private void UpdateEnvelope(float input)
    {
        float absInput = Math.Abs(input);
        if (absInput > envelope)
            envelope += (absInput - envelope) * envelopeAttack;
        else
            envelope += (absInput - envelope) * envelopeRelease;
    }

    public float ProcessSample(float input)
    {
        // 1. Предфильтрация (простой ФВЧ для удаления постоянной составляющей)
        //    можно добавить позже

        // 2. Динамическая огибающая (влияет на Drive)
        UpdateEnvelope(input);
        float dynamicDrive = Drive * (1.0f + EnvelopeSensitivity * envelope);

        // 3. Применяем драйв
        float driven = input * dynamicDrive;

        // 4. Асимметричная нелинейность
        float saturated = RationalNonlinearity(driven, Asymmetry);

        // 5. Гистерезис (простой фильтр 1-го порядка, зависит от прошлого значения)
        if (Hysteresis > 0.0f)
        {
            float alpha = 0.9f; // глубина гистерезиса
            saturated = saturated * alpha + lastOutput * (1.0f - alpha);
            lastOutput = saturated;
        }

        // 6. Постфильтрация (можно добавить ФНЧ для среза жестких гармоник)

        return saturated * OutputGain;
    }

    // Обработка буфера
    public void ProcessBuffer(float[] buffer, int offset, int count)
    {
        for (int i = offset; i < offset + count; i++)
            buffer[i] = ProcessSample(buffer[i]);
    }

    // Установка времени атаки/релиза (в миллисекундах)
    public void SetEnvelopeTimes(float attackMs, float releaseMs)
    {
        envelopeAttack = 1.0f - (float)Math.Exp(-1.0 / (attackMs * 0.001 * sampleRate));
        envelopeRelease = 1.0f - (float)Math.Exp(-1.0 / (releaseMs * 0.001 * sampleRate));
    }
}