import React from "react";
import { useActiveTasksCount } from "../hooks/useActiveTasksCount";

/**
 * ActiveTasksBadge
 * Lightweight badge showing aggregate active task count with optional
 * breakdown tooltip. Pulses briefly when total changes.
 */
export interface ActiveTasksBadgeProps {
  showBreakdownTooltip?: boolean;
  className?: string;
  pulseOnChangeMs?: number;
  /**
   * If provided, renders a custom label before the number (default: "Tasks:")
   */
  label?: string;
}

export const ActiveTasksBadge: React.FC<ActiveTasksBadgeProps> = ({
  showBreakdownTooltip = true,
  className,
  pulseOnChangeMs = 600,
  label = "Tasks:"
}) => {
  const { data, loading, error } = useActiveTasksCount();
  const [pulse, setPulse] = React.useState(false);
  const prevTotalRef = React.useRef<number>(data.total);

  React.useEffect(() => {
    if (data.total !== prevTotalRef.current) {
      prevTotalRef.current = data.total;
      setPulse(true);
      const t = setTimeout(() => setPulse(false), pulseOnChangeMs);
      return () => clearTimeout(t);
    }
  }, [data.total, pulseOnChangeMs]);

  if (error) {
    return (
      <span
        className={className}
        style={baseStyle}
        title={error}
        data-testid="active-tasks-badge-error"
      >
        {label} --
      </span>
    );
  }

  if (loading) {
    return (
      <span
        className={className}
        style={baseStyle}
        title="Loading active tasks..."
        data-testid="active-tasks-badge-loading"
      >
        {label} …
      </span>
    );
  }

  const tooltip = showBreakdownTooltip
    ? `Available: ${data.available}
Assigned To Me: ${data.assignedToMe}
Assigned To My Roles: ${data.assignedToMyRoles}
Claimed: ${data.claimed}
In Progress: ${data.inProgress}
Overdue: ${data.overdue}
Failed: ${data.failed}`
    : undefined;

  return (
    <span
      className={`${className ?? ""} active-tasks-badge${pulse ? " pulse" : ""}`}
      style={baseStyle}
      title={tooltip}
      data-testid="active-tasks-badge"
    >
      <span style={{ opacity: 0.85 }}>{label}</span>
      <strong style={{ fontWeight: 600 }}>{data.total}</strong>
      <style>{pulseCss}</style>
    </span>
  );
};

const baseStyle: React.CSSProperties = {
  display: "inline-flex",
  alignItems: "center",
  gap: 6,
  padding: "2px 8px",
  borderRadius: 14,
  background: "var(--color-surface-alt, #f3f4f6)",
  fontSize: 12,
  fontWeight: 500,
  lineHeight: 1.3,
  fontFamily: "inherit",
  whiteSpace: "nowrap"
};

const pulseCss = `
.active-tasks-badge.pulse {
  animation: atb-pulse 0.6s ease;
}
@keyframes atb-pulse {
  0%   { box-shadow: 0 0 0 0 rgba(99,102,241,0.35); }
  70%  { box-shadow: 0 0 0 8px rgba(99,102,241,0); }
  100% { box-shadow: 0 0 0 0 rgba(99,102,241,0); }
}
`;

export default ActiveTasksBadge;
