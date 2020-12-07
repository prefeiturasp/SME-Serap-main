using System;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling
{
    public class SchedulingConfigAttribute : Attribute
    {
        public string Cron { get; set; }
    }
}