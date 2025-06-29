using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture.Sensor
{
    public class SensorGroup
    {
        public SensorGroup(string symbollink)
        {
            var hr = DirectN.Functions.MFCreateSensorGroup(symbollink, out var sensorGroup);
            
            hr = sensorGroup.GetSensorDeviceCount(out var count);
            
            for (int i=0; i< count; i++)
            {
                hr = sensorGroup.GetSensorDevice((uint)i, out var sensordevice);
                sensordevice.GetDeviceType(out var type);
            }
        }
    }
}
