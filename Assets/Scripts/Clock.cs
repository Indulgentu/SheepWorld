using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Clock
    {
        private DateTime now;
        private TimeSpan timeNow;
        private TimeSpan gameTime;
        private int minutesPerDay; //Realtime minutes per game-day (1440 would be realtime)

        public Clock(int minPerDay)
        {
            minutesPerDay = minPerDay;
        }

        public System.TimeSpan GetTime()
        {
            now = System.DateTime.Now;
            timeNow = now.TimeOfDay;
            double hours = timeNow.TotalMinutes % minutesPerDay;
            double minutes = (hours % 1) * 60;
            double seconds = (minutes % 1) * 60;
            gameTime = new TimeSpan((int)hours, (int)minutes, (int)seconds);

            return gameTime;
        }
    }
}
