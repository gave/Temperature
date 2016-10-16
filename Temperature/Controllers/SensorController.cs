using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Temperature.Models;

namespace Temperature.Controllers
{
    [RoutePrefix("api/sensor")]
    public class SensorController : ApiController
    {
        [Route("{id:int}")]
        public IHttpActionResult GetActualTemperature(int id)
        {
            var tp = new TemperatureServerLib.TempMeterServer();

            var dev = tp.GetDeviceInfoByID((uint)id, 1);

            DateTime dt = new DateTime();
            float fl = 0;

            if (dev != null)
            {
                dev.GetMostRecentLog(out dt, out fl);
            }
            else
            {
                return NotFound();
            }

            return Ok(new Sensor { TimeValue = dt, Value = fl });
        }

        [Route("lastday/{id:int}")]
        public IEnumerable<Sensor> GetLastDayTemperature(int id)
        {
            var tp = new TemperatureServerLib.TempMeterServer();

            var dev = tp.GetDeviceInfoByID((uint)id, 1);

            List<Sensor> sensors = new List<Sensor>();

            if (dev != null)
            {
                dynamic dates;
                dynamic values;
                var grain = DateTime.MinValue;

                dev.GetLog(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, grain.AddMinutes(6), out dates, out values);
                int i = 0;

                foreach (var d in dates)
                {
                    sensors.Add(new Sensor { TimeValue = DateTime.FromOADate(d), Value = values[i++] });
                }
            }

            return sensors;
        }
    }
}
