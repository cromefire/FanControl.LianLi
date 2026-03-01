using FanControl.LianLiPlugin.Base;
using FanControl.Plugins;

namespace FanControl.LianLiPlugin;

public class LianLiPlugin : IPlugin
{
    private LianLiPluginBase pluginBase = new(true);
    public string Name => "Lian Li ARGB";

    public void Close()
    {
        pluginBase.Close();
    }

    public void Initialize()
    {
        pluginBase.Initialize();
    }

    public void Load(IPluginSensorsContainer _container)
    {
        pluginBase.Load(_container);
    }
}