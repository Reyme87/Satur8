using AudioPlugSharp;

namespace Satur8.UI.VST
{
    public static class PluginEntryPoint
    {
        public static AudioPluginBase CreatePlugin()
        {
            return new SaturatorPlugin();
        }
    }
}
