# Document Generation (DocGen) Service – MVP Design Document (Updated)

## 1. Executive Summary
The **DocGen Service** is a core sibling to Workflow, Authentication, and User Management in the **SMART Works Core Platform**. It provides centralized, multi-tenant **document generation and template management** capabilities that power every SMART Works product pod: Contract Management, Legal Case Management, SMART Proposals, and Internal Audit.  

The MVP delivers two key pillars:
1. **Template Studio** – An in-app authoring and governance experience using Syncfusion DocumentEditor (track changes, comments, versioning).  
2. **Document Generation** – Reliable DOCX and PDF generation from structured templates, integrated directly into automated workflows.  

---

## 2. Concept Overview
- **What it is**: A dedicated microservice for document template management and generation.  
- **Why it matters**: Many SMART Works pods require consistent, governed document output. DocGen ensures **standardization, compliance, and automation**.  
- **MVP scope**:  
  - Author and edit templates directly inside SMART Works using Syncfusion DocumentEditor.  
  - Maintain version history with track changes and publishing rules.  
  - Generate DOCX and PDF from published templates.  
  - Integrate document generation as a **Workflow node**, enabling end-to-end automation.  

---

## 3. MVP Features

### A. Template Management (Template Studio)
- **Create/Edit Templates In-App**: Use Syncfusion DocumentEditor for rich authoring.  
- **Track Changes & Comments**: Review and approve/reject edits before publish.  
- **Version Control**: Draft vs. Published states; immutable versions once published.  
- **Field Discovery**: Detect placeholders (`{{client.name}}`, loops/conditions).  
- **Metadata**: Title, tags, owner, merge dialect, description.  
- **Import/Export**: Upload DOCX into Studio; export DOCX for external review.  

### B. Document Generation
- **Input**: Inline JSON payloads (MVP scope).  
- **Output**: DOCX and PDF artifacts.  
- **Storage**: Tenant-scoped artifact repository (S3-compatible or local).  
- **Access**: Download via signed URL or preview in-app.  

### C. Workflow Integration
- **Generate Document Node**: Submit job, wait for `document.generated` event.  
- **Event-First Design**: `render.requested`, `document.generated`, `document.failed`.  
- **Track Changes Governance**: Templates must have no pending revisions before publishing; ensures workflows always use approved language.  

### D. User Interface (MUI Premium + Syncfusion)
- **Templates Table**: MUI DataGridPremium → searchable, filterable list of templates/versions.  
- **Template Studio**: Syncfusion DocumentEditor in center; left metadata panel; right variable inspector.  
- **Version History**: Compare versions, rollback, see publishing trail.  
- **Jobs Table**: DataGridPremium for job monitoring; actions (retry, cancel, download).  
- **Preview Panel**: Syncfusion PDF Viewer for PDFs; DocumentEditor in read-only mode for DOCX.  

---

## 4. Benefits for Clients

### Efficiency
- Author and manage templates directly in-app → no external Word dependency.  
- Automated generation saves time vs. manual drafting.  

### Compliance & Governance
- Track changes and comments provide auditable template evolution.  
- Immutable versioning ensures consistent use of approved language.  

### Integration
- Templates seamlessly tie into workflows (e.g., Contract Approval → Generate Contract → E-Sign).  
- One DocGen service supports all SMART Works pods.  

### Multi-Tenant Security
- Tenant-scoped storage; strict RBAC (`docgen.read|write|publish|admin`).  
- Track who authored, reviewed, and published templates.  

### Future-Ready
- MVP lays groundwork for:  
  - Data connectors (REST, SQL, GraphQL).  
  - Clause libraries and AI-assisted drafting.  
  - Support for Excel (XLSX) and PowerPoint (PPTX).  
  - E-signature integration.  

---

## 5. High-Level Architecture

### Position in Core Platform
- DocGen is a **sibling microservice** to Workflow, Auth, and User.  
- Exposed via API Gateway (`/docgen/...`).  
- Consumes/produces events over RabbitMQ for Workflow integration.  

### Data Flow
1. Author template in Template Studio (Syncfusion DocumentEditor).  
2. Save as Draft → stored as **SFDT + DOCX + HTML**.  
3. Publish template → validated (no pending track changes) → immutable version created.  
4. Workflow or user submits job → Render Orchestrator enqueues request.  
5. Renderer Worker merges JSON → produces artifact (DOCX/PDF).  
6. Artifact stored; URL returned; `document.generated` event emitted.  

---

## 6. MVP Deliverables

- **Backend**:  
  - DocGenService with Templates + Jobs APIs.  
  - Renderers: HTML→PDF, DOCX→DOCX.  
  - SFDT↔DOCX conversion endpoints.  
  - Event emission (`document.generated`, `document.failed`).  
- **Frontend**:  
  - Templates management UI (TemplatesTable, TemplateStudio, VersionHistory).  
  - Job submission + monitoring (JobsTable, JobDetailsDrawer).  
  - Artifact previews (Syncfusion PDF Viewer, DocumentEditor read-only).  
- **Integration**:  
  - Workflow “Generate Document” node.  
  - End-to-end demo flow: Contract approval → Generate document → Resume workflow.  
- **Admin / Security**:  
  - RBAC enforcement; tenant-scoped storage; signed URLs.  
  - Audit trail on edits, comments, publish, generate.  

---

## 7. Client Demonstration Scenarios

- **Contract Management**: Create/edit a contract cover page in Template Studio; publish v1; Workflow generates PDF; ready for e-sign.  
- **Legal Case Management**: Draft motion brief with track changes; publish after review; Workflow generates court-ready DOCX.  
- **Proposals**: Import RFP response template; edit in-app; Workflow generates batch proposals with JSON line items.  
- **Internal Audit**: Author executive summary template; Workflow generates audit report PDF with tenant branding.  

---

## 8. Roadmap Beyond MVP

- **Phase 2**: Data connectors (REST, SQL); e-signature integrations (Adobe, DocuSign).  
- **Phase 3**: Clause library; brand packs; AI-assisted drafting (suggestions, rewrites).  
- **Phase 4**: Support XLSX and PPTX generation.  
- **Phase 5**: Collaborative run-time editing and redlining of generated documents.  

---

## 9. Acceptance Criteria

- Author and edit templates in Template Studio with track changes enabled.  
- Publish immutable version after accepting/rejecting all revisions.  
- Submit job with JSON payload; artifact generated (DOCX/PDF) and stored.  
- Preview artifacts in browser (PDF Viewer, DocumentEditor).  
- Workflow can submit a Generate Document job and resume on `document.generated`.  
- UI can round-trip: Draft → Edit → Save → Publish → Generate → Preview.  

---

## 10. Summary

The **DocGen MVP** equips SMART Works with a powerful **Template Studio + Document Generator**. Clients benefit from immediate value—**authoring, governance, and automation**—while the platform gains a reusable, extensible service that underpins every product pod.  

DocGen, as a sibling to Workflow, ensures that document-driven processes (contracts, cases, proposals, audits) can be automated end-to-end while maintaining compliance, brand consistency, and user efficiency.
