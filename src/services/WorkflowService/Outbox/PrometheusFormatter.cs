using System.Text;
using System.Globalization;

namespace WorkflowService.Outbox;

public static class PrometheusFormatter
{
    public static string Format(OutboxMetricsSnapshot s)
    {
        var sb = new StringBuilder();
        AppendGauge(sb, "outbox_backlog_size", "Number of unprocessed outbox messages", s.BacklogSize);
        AppendGauge(sb, "outbox_backlog_failed_pending", "Unprocessed messages that have failed at least once (non-dead-letter)", s.FailedPending);
        AppendGauge(sb, "outbox_deadletter_unprocessed", "Unprocessed dead-letter messages", s.DeadLetterUnprocessed);
        AppendGauge(sb, "outbox_oldest_age_seconds", "Age in seconds of oldest unprocessed message", s.OldestAgeSeconds ?? 0);
        AppendCounter(sb, "outbox_messages_processed_total", "Total processed successfully", s.ProcessedTotal);
        AppendCounter(sb, "outbox_messages_failed_total", "Total transient failed attempts (non-terminal)", s.FailedTotal);
        AppendCounter(sb, "outbox_messages_giveup_total", "Total terminal give-up (non-dead-letter path)", s.GiveUpTotal);
        AppendCounter(sb, "outbox_messages_deadletter_total", "Total messages routed to dead-letter", s.DeadLetterTotal);
        AppendGauge(sb, "outbox_failure_ratio_window", "Failure ratio over rolling window", s.FailureRatioWindow);
        AppendGauge(sb, "outbox_throughput_per_minute_window", "Processed throughput per minute (rolling window)", s.ThroughputPerMinuteWindow);
        AppendGauge(sb, "outbox_failure_ratio_last_cycle", "Failure ratio last cycle", s.FailureRatioLastCycle);
        AppendGauge(sb, "outbox_processed_last_cycle", "Processed last cycle", s.ProcessedLastCycle);
        AppendGauge(sb, "outbox_failed_last_cycle", "Failed last cycle", s.FailedLastCycle);
        AppendGauge(sb, "outbox_giveup_last_cycle", "Give-up last cycle", s.GiveUpLastCycle);
        AppendGauge(sb, "outbox_deadletter_last_cycle", "Dead-letter last cycle", s.DeadLetterLastCycle);
        return sb.ToString();
    }

    private static void AppendGauge(StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} gauge");
        sb.Append(name).Append(' ').AppendLine(value.ToString(CultureInfo.InvariantCulture));
    }

    private static void AppendCounter(StringBuilder sb, string name, string help, long value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} counter");
        sb.Append(name).Append(' ').AppendLine(value.ToString(CultureInfo.InvariantCulture));
    }
}
