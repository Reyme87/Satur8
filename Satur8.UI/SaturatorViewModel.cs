using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Satur8.UI
{
    public class SaturatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private double _driveValue = 1.0;
        public double DriveValue { get => _driveValue; set => Set(ref _driveValue, value); }

        private double _outputGainValue = 1.0;
        public double OutputGainValue { get => _outputGainValue; set => Set(ref _outputGainValue, value); }

        private double _evenAmountValue = 0.5;
        public double EvenAmountValue { get => _evenAmountValue; set => Set(ref _evenAmountValue, value); }

        private double _oddAmountValue = 1.0;
        public double OddAmountValue { get => _oddAmountValue; set => Set(ref _oddAmountValue, value); }

        private double _biasValue = 0.0;
        public double BiasValue { get => _biasValue; set => Set(ref _biasValue, value); }

        private int _clipTypeIndex = 0;
        public int ClipTypeIndex { get => _clipTypeIndex; set => Set(ref _clipTypeIndex, value); }

        private double _mixPercent = 100.0;
        public double MixPercent { get => _mixPercent; set => Set(ref _mixPercent, value); }

        private double _thresholdDb = -12.0;
        public double ThresholdDb { get => _thresholdDb; set => Set(ref _thresholdDb, value); }

        private double _ratioValue = 4.0;
        public double RatioValue { get => _ratioValue; set => Set(ref _ratioValue, value); }

        private bool _autoGainEnabled = false;
        public bool AutoGainEnabled { get => _autoGainEnabled; set => Set(ref _autoGainEnabled, value); }
    }
}
