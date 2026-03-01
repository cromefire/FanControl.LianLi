using FanControl.Plugins;

namespace FanControl.LianLiPlugin.Base;

public class ControlSensors : IPluginControlSensor2
{
    private LianLi.Devices _devices;
    private readonly int _controllerIndex;
    private readonly int _channelIndex;
    private float? _val;

    public ControlSensors(LianLi.Devices devices, int controllerIndex, int channelIndex) {
        _devices = devices;
        _controllerIndex = controllerIndex;
        _channelIndex = channelIndex;
    }

    public float? Value { get; private set; }
    public string Name => $"Uni Controller #{_controllerIndex + 1} Ch. {_channelIndex + 1}";

    public string Id => $"Controller_{_controllerIndex} _Channel_{_channelIndex}";
    public string PairedFanSensorId => $"Controller_{_controllerIndex} _Channel_{_channelIndex}";

    public void Update() => Value = _val;

    public void Set(float val)
    {
        if (_val == null || !(Math.Abs(_val.Value - val) <= 0.0001))
        {
            _devices.FanControllers_SetSpeed(_controllerIndex, _channelIndex, (int)val);
            _val = val;
        }
    }

    public void Reset()
    {
           
    }
}

public class Sensors: IPluginSensor
{

    private LianLi.Devices _devices;
    private readonly int _controllerIndex;
    private readonly int _channelIndex;

    public Sensors(LianLi.Devices devices, int controllerIndex, int channelIndex)
    {
        _devices = devices;
        _controllerIndex = controllerIndex;
        _channelIndex = channelIndex;
    }

    public float? Value { get; private set; }

    public string Name => $"Uni Controller #{_controllerIndex + 1} Ch. {_channelIndex + 1}";

    public string Id => $"Controller_{_controllerIndex} _Channel_{_channelIndex}";

    public void Update() => Value = _devices.FanControllers_GetSpeed(_controllerIndex, _channelIndex);

}
