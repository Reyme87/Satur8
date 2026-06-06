using Satur8.Persistence.Misc;
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

        // Parameters
        private double _driveValue = 1.0;
        private double _outputGainValue = 1.0;
        private double _evenAmountValue = 0.5;
        private double _oddAmountValue = 1.0;
        private double _biasValue = 0.0;
        private int _clipTypeIndex = 0;
        private double _mixPercent = 100.0;
        private double _thresholdDb = -12.0;
        private double _ratioValue = 4.0;
        private bool _autoGainEnabled = false;

        // Panel
        private bool _showAccountPanel;

        // Auth state
        private bool _isLoggedIn;
        private string _loggedInAs = string.Empty;
        private string _authError = string.Empty;
        private bool _isLoading;

        public double DriveValue 
        { 
            get => _driveValue; 
            set => Set(ref _driveValue, value); 
        }

        public double OutputGainValue 
        { 
            get => _outputGainValue; 
            set => Set(ref _outputGainValue, value); 
        }

        public double EvenAmountValue 
        { 
            get => _evenAmountValue; 
            set => Set(ref _evenAmountValue, value); 
        }

        public double OddAmountValue 
        { 
            get => _oddAmountValue; 
            set => Set(ref _oddAmountValue, value); 
        }

        public double BiasValue 
        { 
            get => _biasValue; 
            set => Set(ref _biasValue, value); 
        }

        public int ClipTypeIndex 
        { 
            get => _clipTypeIndex; 
            set => Set(ref _clipTypeIndex, value); 
        }

        public double MixPercent 
        { 
            get => _mixPercent; 
            set => Set(ref _mixPercent, value);
        }

        public double ThresholdDb 
        { 
            get => _thresholdDb; 
            set => Set(ref _thresholdDb, value); 
        }

        public double RatioValue 
        { 
            get => _ratioValue; 
            set => Set(ref _ratioValue, value); 
        }

        public bool AutoGainEnabled 
        { 
            get => _autoGainEnabled; 
            set => Set(ref _autoGainEnabled, value); 
        }

        public bool ShowAccountPanel
        {
            get => _showAccountPanel;
            set
            {
                Set(ref _showAccountPanel, value);
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(ShowLoginForm)));
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(ShowProfilePanel)));
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                Set(ref _isLoggedIn, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowLoginForm)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowProfilePanel)));
            }
        }

        public string LoggedInAs
        {
            get => _loggedInAs;
            set => Set(ref _loggedInAs, value);
        }

        public string AuthError
        {
            get => _authError;
            set => Set(ref _authError, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        public bool ShowLoginForm => ShowAccountPanel && !IsLoggedIn;
        public bool ShowProfilePanel => ShowAccountPanel && IsLoggedIn;

        // Presets
        private List<PresetDto> _allPresets = new();
        private List<PresetDto> _filteredPresets = new();
        private PresetDto? _selectedPreset;
        private bool _showFavouritesOnly;
        private bool _showSavePanel;

        private string _newPresetName = "";
        private string _newPresetDesc = "";
        private string _newPresetCategory = "";
        private string _saveError = "";
        private List<string> _categories = new();

        public List<PresetDto> FilteredPresets
        {
            get => _filteredPresets;
            set => Set(ref _filteredPresets, value);
        }

        public PresetDto? SelectedPreset
        {
            get => _selectedPreset;
            set => Set(ref _selectedPreset, value);
        }

        public bool ShowFavouritesOnly
        {
            get => _showFavouritesOnly;
            set { Set(ref _showFavouritesOnly, value); ApplyFilter(); }
        }

        public bool ShowSavePanel
        {
            get => _showSavePanel;
            set => Set(ref _showSavePanel, value);
        }

        public string NewPresetName
        {
            get => _newPresetName;
            set => Set(ref _newPresetName, value);
        }

        public string NewPresetDesc
        {
            get => _newPresetDesc;
            set => Set(ref _newPresetDesc, value);
        }

        public string NewPresetCategory
        {
            get => _newPresetCategory;
            set => Set(ref _newPresetCategory, value);
        }

        public string SaveError
        {
            get => _saveError;
            set => Set(ref _saveError, value);
        }

        public List<string> Categories
        {
            get => _categories;
            set => Set(ref _categories, value);
        }

        public void SetPresets(List<PresetDto> presets)
        {
            _allPresets = presets;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredPresets = _showFavouritesOnly
                ? _allPresets.Where(p => p.IsFavourite).ToList()
                : _allPresets;
        }

        public void ApplyPreset(PresetDto preset)
        {
            DriveValue = preset.Parameters.Drive;
            OutputGainValue = preset.Parameters.Output;
            EvenAmountValue = preset.Parameters.Even;
            OddAmountValue = preset.Parameters.Odd;
            BiasValue = preset.Parameters.Bias;
            MixPercent = preset.Parameters.Mix;
            ThresholdDb = preset.Parameters.Threshold;
            RatioValue = preset.Parameters.Ratio;
            ClipTypeIndex = preset.Parameters.ClipType;
        }
    }
}
