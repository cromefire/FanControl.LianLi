using FanControl.Plugins;

namespace FanControl.LianLiPlugin.Base;

public class LianLiPluginBase(bool enableArgb)
{
    private LianLi.Devices _devices;
    private bool _initialized;

    public void Close()
    {
        // Close all Lian Li Devices
        _devices.Dispose();
    }

    public void Initialize()
    {
        _devices = new LianLi.Devices(enableArgb);
        _initialized = _devices.FanControllers_Located();
    }

    public void Load(IPluginSensorsContainer _container)
    {
        if(_initialized)
        {
            /** Load Fan Controllers **/
            // Each Controller consists of 4 Channels - Configure a sensor for each  
            var fanControlSensorsCount = _devices.FanControllers_Count() * 4;
            for (var i = 0; i < fanControlSensorsCount; i++)
            {
                var controllerIndex = i / 4;
                var channelIndex = i % 4;
                _container.ControlSensors.Add(new ControlSensors(_devices, controllerIndex, channelIndex));
                _container.FanSensors.Add(new Sensors(_devices, controllerIndex, channelIndex));
            }

        }
    }
}