🏆 Post-MVP Enhancements (Weeks 5+)
Workflow Engine

Parallel & Join nodes

Sub-workflows (call workflow B from workflow A)

Compensation/rollback (for sagas)

Pluggable step executors (register custom node types via config).

AI & Automation

AIProcessing node (prompt → AI service, output variables).

AIReview node (AI suggests, human confirms).

AI-powered decision gateways.

Cost tracking + provider adapters (OpenAI, Anthropic, Azure).

Builder UX

Rich property panel (prompt editing, API config).

Variable management (define workflow vars, types, validation).

Version diff viewer (see changes between v1, v2).

Preview/debug mode (simulate execution).

Ops & Integration

RabbitMQ integration for outbox → event-driven comms.

SignalR/WebSockets for live task updates in UI.

Metrics: avg workflow duration, SLA misses, stuck tasks.

Migration tool: move in-flight instances from v1 → v2 of workflow definition.
