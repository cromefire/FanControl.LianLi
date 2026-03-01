using HidSharp;

namespace FanControl.LianLiPlugin.Base.LianLi;

internal class Hid
{

    public static List<HidDevice> Locate(int[] vendorIds, int[] productIds)
    {

        var devices = new List<HidDevice>();
        var locatedDevices = DeviceList.Local.GetHidDevices().ToArray();

        foreach (HidSharp.HidDevice device in locatedDevices)
        {

            int vid;
            int pid;
            int.TryParse(GetIdentifierPart("vid_", device.DevicePath), System.Globalization.NumberStyles.HexNumber, null, out vid);
            int.TryParse(GetIdentifierPart("pid_", device.DevicePath), System.Globalization.NumberStyles.HexNumber, null, out pid);

            if (Enumerable.Contains(vendorIds, vid) && Enumerable.Contains(productIds, pid))
            {
                devices.Add(new HidDevice(pid,
                    device
                ));
            }
        }

        return devices;

    }

    private static string GetIdentifierPart(string identifier, string deviceId)
    {
        var vidIndex = deviceId.IndexOf(identifier, StringComparison.Ordinal);
        var startingAtVid = deviceId.Substring(vidIndex + 4);
        return startingAtVid.Substring(0, 4);
    }

}

internal class HidDevice
{
    public int Pid;
    private readonly HidStream _stream;

    public HidDevice(int pid, HidSharp.HidDevice device)
    {
        Pid = pid;

        if(device.TryOpen(out _stream))
        {
            device.GetReportDescriptor();
        }
    }

    public byte[] ReadInputReport(byte reportId, int bufferLength)
    {
        byte[] buffer = new byte[bufferLength];
        buffer[0] = reportId;
        _stream.GetInputReport(buffer);

        return buffer;
    }

    public void Write(byte[] buffer)
    {
        if (!_stream.CanWrite) { return; }
        _stream.Write(buffer);
    }

    public void Dispose()
    {
        if (_stream != null) { _stream.Dispose(); }
    }

}
