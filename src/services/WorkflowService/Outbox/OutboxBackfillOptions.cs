namespace WorkflowService.Outbox;

public class OutboxBackfillOptions
{
    // Enable/disable the background one‑time backfill worker
    public bool Enabled { get; set; } = true;
    // Rows processed per batch
    public int BatchSize { get; set; } = 500;
    // Safety upper bound to avoid runaway (0 = unlimited)
    public int MaxTotal { get; set; } = 0;
}
