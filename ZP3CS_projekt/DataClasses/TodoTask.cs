using System;
using System.Collections.Generic;
using System.Text;

namespace ZP3CS_projekt.DataClasses
{
    public class TodoTask
    {
        private static int _instanceCounter = 0;
        private static int _maxProgressValue = 5;
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? Finished { get; set; }
        public TimeSpan? DeadlineTime { get; set; }
        public int ProgressValue { get; set; }
        public int MaxProgressValue { get { return _maxProgressValue; } }

        public TodoTask(string descr, DateTime? deadline, TimeSpan? deadline_time)
        {
            ID = ++_instanceCounter;
            Description = descr;
            Deadline = deadline;
            DeadlineTime = deadline_time;
            ProgressValue = 0;
        }
        public TodoTask UpdateTodoTask(string descr, DateTime? deadline, TimeSpan? deadline_time, int taskProgress)
        {
            Description = descr;
            Deadline = deadline;
            DeadlineTime = deadline_time;
            ProgressValue = taskProgress;
            return this;
        }

        public void MarkAsFinished()
        {
            Finished = DateTime.Now;
            ProgressValue = MaxProgressValue;
        }
        
        public void ProgressIncr()
        {
            if(ProgressValue != MaxProgressValue)
            {
                ProgressValue++;
            }            
        }

        public void ProgressDecr()
        {
            if(ProgressValue > 0)
            {
                ProgressValue--;
            }            
        }
    }
}
