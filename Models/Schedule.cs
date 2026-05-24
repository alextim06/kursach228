using System;

public class Schedule
{
    public int schedule_id { get; set; }
    public int group_id { get; set; }
    public DateTime lesson_date { get; set; }
    public TimeSpan lesson_time { get; set; }
    public string topic { get; set; }
}