using AudioPlugSharp;
using AudioPlugSharpWPF;
using Satur8.Maths;
using Satur8.Persistence.Services;
using Satur8.Processors;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Satur8.UI.VST
{
    public class SaturatorPlugin : AudioPluginWPF
    {
        private SaturatorViewModel? _viewModel;

        private Clipper _clipper = null!;
        private HarmonicSaturator _saturator = null!;
        private DynamicsProcessor _dynamics = null!;

        private FloatAudioIOPort _stereoInput = null!;
        private FloatAudioIOPort _stereoOutput = null!;

        private AudioPluginParameter _driveParam = null!;
        private AudioPluginParameter _evenParam = null!;
        private AudioPluginParameter _oddParam = null!;
        private AudioPluginParameter _biasParam = null!;
        private AudioPluginParameter _outputParam = null!;
        private AudioPluginParameter _mixParam = null!;
        private AudioPluginParameter _thresholdParam = null!;
        private AudioPluginParameter _ratioParam = null!;

        public SaturatorPlugin()
        {
            Company = "ReyMusic";
            Website = "https://github.com/Reyme87";
            Contact = "support@your-email.com";
            PluginName = "Satur8";
            PluginCategory = "Fx";
            PluginVersion = "1.0.0";
            PluginID = GuidToPluginId(new Guid("9D08828B-FC48-400D-B450-B03B36B54731"));
            SampleFormatsSupported = EAudioBitsPerSample.Bits32;

            HasUserInterface = true;
            EditorWidth = 640;
            EditorHeight = 420;
        }

        public override void Initialize()
        {
            base.Initialize();

            _stereoInput = new FloatAudioIOPort("Stereo Input", EAudioChannelConfiguration.Stereo);
            _stereoOutput = new FloatAudioIOPort("Stereo Output", EAudioChannelConfiguration.Stereo);
            InputPorts = [_stereoInput];
            OutputPorts = [_stereoOutput];

            _clipper = new Clipper();
            _saturator = new HarmonicSaturator(_clipper);
            _dynamics = new DynamicsProcessor(44100);

            _driveParam = new AudioPluginParameter
            {
                ID = "Drive",
                Name = "Drive",
                MinValue = 0.5,
                MaxValue = 5.0,
                DefaultValue = 1.0,
                ValueFormat = "{0:0.0} dB"
            };
            AddParameter(_driveParam);

            _evenParam = new AudioPluginParameter
            {
                ID = "Even",
                Name = "Even",
                MinValue = 0.0,
                MaxValue = 1.0,
                DefaultValue = 0.5,
                ValueFormat = "{0:0.00}"
            };
            AddParameter(_evenParam);

            _oddParam = new AudioPluginParameter
            {
                ID = "Odd",
                Name = "Odd",
                MinValue = 0.0,
                MaxValue = 1.0,
                DefaultValue = 1.0,
                ValueFormat = "{0:0.00}"
            };
            AddParameter(_oddParam);

            _biasParam = new AudioPluginParameter
            {
                ID = "Bias",
                Name = "Bias",
                MinValue = -0.5,
                MaxValue = 0.5,
                DefaultValue = 0.0,
                ValueFormat = "{0:0.00}"
            };
            AddParameter(_biasParam);

            _outputParam = new AudioPluginParameter
            {
                ID = "Output",
                Name = "Output",
                MinValue = -20.0,
                MaxValue = 20.0,
                DefaultValue = 0.0,
                ValueFormat = "{0:0.0} dB"
            };
            AddParameter(_outputParam);

            _mixParam = new AudioPluginParameter
            {
                ID = "Mix",
                Name = "Mix",
                MinValue = 0.0,
                MaxValue = 100.0,
                DefaultValue = 100.0,
                ValueFormat = "{0:0.0} %"
            };
            AddParameter(_mixParam);

            _thresholdParam = new AudioPluginParameter
            {
                ID = "Threshold",
                Name = "Threshold",
                MinValue = -30.0,
                MaxValue = 0.0,
                DefaultValue = -12.0,
                ValueFormat = "{0:0.0} dB"
            };
            AddParameter(_thresholdParam);

            _ratioParam = new AudioPluginParameter
            {
                ID = "Ratio",
                Name = "Ratio",
                MinValue = 1.0,
                MaxValue = 10.0,
                DefaultValue = 4.0,
                ValueFormat = "{0:0.0}"
            };
            AddParameter(_ratioParam);

            try
            {
                Log("PluginService.Initialize start");
                PluginService.Initialize();
                Log("PluginService.Initialize OK");
            }
            catch (Exception ex)
            {
                Log($"PluginService FAILED: {ex.GetType().Name}: {ex.Message}");
            }
        }

        public override void Process()
        {
            base.Process();
            Host.ProcessAllEvents();

            float drive = (float)_driveParam.ProcessValue;
            float even = (float)_evenParam.ProcessValue;
            float odd = (float)_oddParam.ProcessValue;
            float bias = (float)_biasParam.ProcessValue;
            float outputDb = (float)_outputParam.ProcessValue;
            float mixPercent = (float)_mixParam.ProcessValue;
            float thresholdDb = (float)_thresholdParam.ProcessValue;
            float ratio = (float)_ratioParam.ProcessValue;

            float mix = mixPercent / 100f;
            float outputLinear = (float)AudioPluginParameter.DBToLinear(outputDb);

            _saturator.Drive = drive;
            _saturator.EvenAmount = even;
            _saturator.OddAmount = odd;
            _saturator.Bias = bias;
            _saturator.OutputGain = outputLinear;

            _dynamics.ThresholdDb = thresholdDb;
            _dynamics.Ratio = ratio;
            _dynamics.UpdateTimeConstants();

            Span<float> leftIn = _stereoInput.GetAudioBuffer(0);
            Span<float> rightIn = _stereoInput.GetAudioBuffer(1);
            Span<float> leftOut = _stereoOutput.GetAudioBuffer(0);
            Span<float> rightOut = _stereoOutput.GetAudioBuffer(1);

            for (int i = 0; i < leftIn.Length; i++)
            {
                float ls = leftIn[i];
                float lp = _saturator.ProcessSample(_dynamics.ProcessSample(ls));
                leftOut[i] = ls + (lp - ls) * mix;

                float rs = rightIn[i];
                float rp = _saturator.ProcessSample(_dynamics.ProcessSample(rs));
                rightOut[i] = rs + (rp - rs) * mix;
            }
        }

        public static Guid GetClassId()
        {
            return new Guid(0x9E3B7A1F, 0x5C8D, 0x4E2F,
                0x9E, 0x3B, 0x7A, 0x1F, 0x5C, 0x8D, 0x4E, 0x2F);
        }

        public static ulong GuidToPluginId(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            return BitConverter.ToUInt64(bytes, 0);
        }

        public override UserControl GetEditorView()
        {
            Log("GetEditorView start");

            try
            {
                if (Application.Current == null)
                {
                    Log("Creating Application...");
                    new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                    Log("Application OK");
                }

                Log("Creating SaturatorView...");
                var view = new SaturatorView();
                Log("SaturatorView OK");

                _viewModel = view.DataContext as SaturatorViewModel;
                Log($"ViewModel: {(_viewModel == null ? "NULL" : "OK")}");

                if (_viewModel != null)
                {
                    _viewModel.DriveValue = _driveParam.ProcessValue;
                    _viewModel.EvenAmountValue = _evenParam.ProcessValue;
                    _viewModel.OddAmountValue = _oddParam.ProcessValue;
                    _viewModel.BiasValue = _biasParam.ProcessValue;
                    _viewModel.OutputGainValue = _outputParam.ProcessValue;
                    _viewModel.MixPercent = _mixParam.ProcessValue;
                    _viewModel.ThresholdDb = _thresholdParam.ProcessValue;
                    _viewModel.RatioValue = _ratioParam.ProcessValue;

                    _viewModel.PropertyChanged += (s, e) =>
                    {
                        if (_viewModel == null) return;
                        switch (e.PropertyName)
                        {
                            case nameof(SaturatorViewModel.DriveValue):
                                _driveParam.ProcessValue = _viewModel.DriveValue; break;
                            case nameof(SaturatorViewModel.EvenAmountValue):
                                _evenParam.ProcessValue = _viewModel.EvenAmountValue; break;
                            case nameof(SaturatorViewModel.OddAmountValue):
                                _oddParam.ProcessValue = _viewModel.OddAmountValue; break;
                            case nameof(SaturatorViewModel.BiasValue):
                                _biasParam.ProcessValue = _viewModel.BiasValue; break;
                            case nameof(SaturatorViewModel.OutputGainValue):
                                _outputParam.ProcessValue = _viewModel.OutputGainValue; break;
                            case nameof(SaturatorViewModel.MixPercent):
                                _mixParam.ProcessValue = _viewModel.MixPercent; break;
                            case nameof(SaturatorViewModel.ThresholdDb):
                                _thresholdParam.ProcessValue = _viewModel.ThresholdDb; break;
                            case nameof(SaturatorViewModel.RatioValue):
                                _ratioParam.ProcessValue = _viewModel.RatioValue; break;
                        }
                    };
                    Log("Bindings OK");
                }

                Log("GetEditorView end OK");
                return view;
            }
            catch (Exception ex)
            {
                Log($"CRASH: {ex.GetType().Name}: {ex.Message}");
                Log($"Inner: {ex.InnerException?.Message}");
                Log($"Stack: {ex.StackTrace}");
                return new UserControl();
            }
        }

        private static void Log(string message)
        {
            try
            {
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "satur8_log.txt");
                File.AppendAllText(path, $"{DateTime.Now:HH:mm:ss.fff} {message}\n");
            }
            catch { }
        }
    }
}