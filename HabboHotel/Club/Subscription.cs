using Moon;
using Moon.Core;

namespace Plus.HabboHotel.Club
{
    public class Subscription
    {
        private readonly string Caption;
        private int TimeExpire;

        internal string SubscriptionId
        {
            get
            {
                return this.Caption;
            }
        }

        internal int ExpireTime
        {
            get
            {
                return this.TimeExpire;
            }
        }

        internal Subscription(string Caption, int TimeExpire)
        {
            this.Caption = Caption;
            this.TimeExpire = TimeExpire;
        }

        internal bool IsValid()
        {
            return this.TimeExpire > MoonEnvironment.GetUnixTimestamp();
        }

        internal void SetEndTime(int time)
        {
            this.TimeExpire = time;
        }

        internal void ExtendSubscription(int Time)
        {
            try
            {
                this.TimeExpire = (this.TimeExpire + Time);
            }
            catch
            {
                Logging.LogException("T: " + this.TimeExpire + "." + Time);
            }
        }
    }
}
