using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.AddControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackgroundJob()
        {
            Console.WriteLine("enter background job");
            BackgroundJob.Enqueue(() => Console.WriteLine("background job triggered"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            Console.WriteLine("enter scheduled job");
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            BackgroundJob.Schedule(() => Console.WriteLine("scheduled job triggered"), dateTimeOffset);
            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            Console.WriteLine("enter continuation job");
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            var job1 = BackgroundJob.Schedule(() => Console.WriteLine("start job 1"), dateTimeOffset);
            var job2 = BackgroundJob.ContinueJobWith(job1, () => Console.WriteLine("continue job 1"));
            var job3 = BackgroundJob.ContinueJobWith(job2, () => Console.WriteLine("continue job 2"));
            var job4 = BackgroundJob.ContinueJobWith(job3, () => Console.WriteLine("continue job 3"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurringJob()
        {
            Console.WriteLine("enter recurring job");
            RecurringJob.AddOrUpdate("RecurringJob1", () => Console.WriteLine("recurring job triggered"), "* * * * *");
            return Ok();
        }
    }

}
