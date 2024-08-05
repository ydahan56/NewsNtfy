using FluentScheduler;

namespace Agent.Jobs
{
    public class MonitorJob : Registry
    {
        public MonitorJob(IJob website, int minutes)
        {
            if (website is null)
                throw new ArgumentNullException(nameof(website));

            if (minutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(minutes));

            this.Schedule(website).NonReentrant().ToRunNow().AndEvery(minutes).Minutes();
        }
    }
}
