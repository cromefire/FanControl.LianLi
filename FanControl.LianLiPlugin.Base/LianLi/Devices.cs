namespace FanControl.LianLiPlugin.Base.LianLi;

public class Devices
{
    private static readonly int[] VENDOR_IDS = { 0x0cf2 };
    private static readonly int[] PRODUCT_IDS = { 0x7750, 0xa100, 0xa101, 0xa102, 0xa103, 0xa104, 0xa105 };
    
    private List<FanController> _fancontrollers = [];

    public Devices(bool enableArgb)
    {
        // Scan for controllers

        var devices = Hid.Locate(VENDOR_IDS, PRODUCT_IDS);

        foreach (var device in devices)
        {
            _fancontrollers.Add(new FanController(device, enableArgb));
        }

    }

    // Dispose Fan Controllers
    public void Dispose()
    {
        foreach (var t in _fancontrollers)
        {
            t.Dispose();
        }
    }

    public bool FanControllers_Located()
    {
        return _fancontrollers.Count != 0;
    }

    public int FanControllers_Count()
    {
        return _fancontrollers.Count;
    }

    public float FanControllers_GetSpeed(int fancontrollerIndex, int fancontrollerChannel)
    {
        return _fancontrollers[fancontrollerIndex].GetSpeed(fancontrollerChannel);
    }

    public void FanControllers_SetSpeed(int fancontrollerIndex, int fancontrollerChannel, int speed)
    {
        _fancontrollers[fancontrollerIndex].SetSpeed(fancontrollerChannel, speed);
    }

}

internal class FanController
{
    private enum Type
    {
        SL,
        SLV2,
        SLI,
        AL,
        ALV2,
        Unknown
    }

    private readonly Type _type;
    private readonly HidDevice _device;

    public FanController(HidDevice device, bool enableArgb)
    {
        _device = device;
        switch (device.Pid)
        {
            case 0xa100:
            case 0x7750:
                _type = Type.SL;
                break;
            case 0xa101:
                _type = Type.AL;
                break;
            case 0xa102:
                _type = Type.SLI;
                break;
            case 0xa103:
            case 0xa105:
                _type = Type.SLV2;
                break;
            case 0xa104:
                _type = Type.ALV2;
                break;
            default:
                _type = Type.Unknown;
                break;
        }

        if (enableArgb) { SetArgb(); }
        for (var i = 0; i < 4; i++) { DisableRpmSync(i); }
    }


    public void SetSpeed(int fancontrollerChannel, int speed)
    {
        byte speedByte;

        if (speed == 0)
        {
            speedByte = 0x00; // Assuming 0x00 represents 0 RPM.
        }
        else
        {
            var speed_200_2100 = (byte)speed;
            speedByte = speed_200_2100;
        }

        // Send the command with the calculated speedByte
        _device.Write([224, (byte)(32 + fancontrollerChannel), 0, speedByte]);
    }

    public float GetSpeed(int fancontrollerChannel)
    {

        int offset = 1;
        byte[] buffer = new byte[65];

        switch (_type)
        {
            case Type.SL:
                offset = 1;
                buffer = _device.ReadInputReport(224, 65);
                break;
            case Type.SLV2:
                offset = 2;
                buffer = _device.ReadInputReport(224, 65);
                break;
            case Type.AL:
                offset = 1;
                buffer = _device.ReadInputReport(224, 65);
                break;
            case Type.ALV2:
                offset = 2;
                buffer = _device.ReadInputReport(224, 65);
                break;
            case Type.SLI:
                offset = 1;
                buffer = _device.ReadInputReport(224, 65);
                break;
        }

        return BitConverter.ToUInt16(buffer.Skip(offset + fancontrollerChannel * 2).Take(2).Reverse().ToArray(), 0);
    }

    public void Dispose()
    {
        _device.Dispose();
    }

    private void DisableRpmSync(int fancontrollerChannel)
    {
        var channelByte = (byte)(2 * fancontrollerChannel * 16);
        if (fancontrollerChannel == 0) { channelByte = 16; }

        switch (_type)
        {
            case Type.SL:
                _device.Write([224, 16, 49, channelByte]);
                break;
            case Type.SLV2:
                _device.Write([224, 16, 98, channelByte]);
                break;
            case Type.AL:
                _device.Write([224, 16, 66, channelByte]);
                break;
            case Type.ALV2:
            case Type.SLI:
                _device.Write([224, 16, 98, channelByte]);
                break;
        }
    }

    private void SetArgb()
    {
        switch (_type)
        {
            case Type.SL:
                _device.Write([224, 16, 48, 1, 0, 0, 0]);
                break;
            case Type.SLV2:
                _device.Write([224, 16, 97, 1, 0, 0, 0]);
                break;
            case Type.AL:
                _device.Write([224, 16, 65, 1, 0, 0, 0]);
                break;
            case Type.ALV2:
            case Type.SLI:
                _device.Write([224, 16, 97, 1, 0, 0, 0]);
                break;
        }
    }



}
