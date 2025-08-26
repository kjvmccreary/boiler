# Document Generation (DocGen) Service MVP Implementation Checklist (Updated with Template Studio)

This checklist reflects the MVP Phase for the **DocGen Service**, a sibling microservice to Workflow, User, and Auth in the SMART Works Core Platform. 
The goal is to enable basic document generation (HTML/Markdown → PDF and DOCX), **plus in-app template authoring** with Syncfusion DocumentEditor (track changes, comments, versioning). 
This makes DocGen a first-class **Template Studio**, not just an upload API. 
The checklist also notes where **MUI Premium** and **Syncfusion** controls are used.

---

## 0) Prerequisites

☐ Ensure PostgreSQL, Redis, and RabbitMQ containers are running.

☐ Verify API Gateway is routing AuthService and UserService correctly.

☐ Confirm RBAC seed is loaded with `docgen.read`, `docgen.write`, `docgen.admin` and JWT auth works end-to-end.

☐ Create storage bucket/folder for generated artifacts (S3-compatible or local volume).

☐ Ensure Syncfusion license key is registered in frontend and backend services.

---

## 1) Backend Service – src/services/DocGenService/

☐ Create project files: DocGenService.csproj, Program.cs, appsettings.json, appsettings.Development.json.

☐ Add Controllers/: TemplatesController (upload/list/get/version/fields, draft/publish), JobsController (submit/status/artifact), AdminController (retry/cancel).

☐ Add Domain/: Template (metadata, tags, variables), TemplateVersion (status Draft|Published), DocumentJob, DocumentArtifact, Enums (JobStatus, OutputFormat).

☐ Add Persistence/: DocGenDbContext.cs with EF mappings for templates, versions, jobs; indexes (TenantId, status, createdAt).

☐ Add Renderers/: HtmlToPdfRenderer, DocxRenderer (OpenXML SDK).

☐ Add Services/: TemplateService (handles both DOCX and SFDT), JobService, RenderOrchestrator (enqueue job → worker → store artifact).

☐ Add Background/: JobWorker (consume queue, execute renderer, persist artifact).

☐ Add Converters/: DOCX↔SFDT (Syncfusion server libraries if required).

☐ Add Security/: Policies.cs with docgen.read, docgen.write, docgen.admin.

☐ Add Health check endpoint.

---

## 2) Shared Libraries

☐ Add DTOs: TemplateDto, TemplateVersionDto, UploadTemplateRequestDto, SubmitJobRequestDto, JobStatusDto, ArtifactDto.

☐ Add Contracts: IDocGenEvents (document.generated, document.failed).

☐ Update RBAC seed with docgen.* permissions.

☐ Update shared event contracts with `render.requested`, `document.generated`, `document.failed`.

---

## 3) API Gateway – src/services/ApiGateway/ocelot.json

☐ Add routes: /docgen/templates/* → DocGenService:5004.

☐ Add routes: /docgen/jobs/* → DocGenService:5004.

---

## 4) Docker – docker/services/DocGenService.Dockerfile

☐ Create Dockerfile for DocGenService.

☐ Add service to docker-compose.yml (map ports 5004 and 7004; set DB/JWT/Redis/storage env).

☐ Add healthcheck.

---

## 5) Frontend – Template Studio & Jobs (React + TypeScript + MUI Premium + Syncfusion)

☐ Create src/services/docGenService.ts (REST wrappers for templates, jobs).

☐ Create TemplatesTable.tsx (MUI DataGridPremium → list templates, versions, status, owner).

☐ Create TemplateStudio.tsx:  
   - Center: **Syncfusion DocumentEditor** (track changes, comments enabled).  
   - Left: Template metadata panel (tags, owner, merge dialect).  
   - Right: Variables inspector (list of tokens; insertable into editor).  
   - Top actions: Save Draft, Preview Merge (sample JSON), **Publish vX.Y.Z**.

☐ Create VersionHistory.tsx (MUI DataGridPremium → versions list with compare, rollback).

☐ Create TemplateUploadDialog.tsx (MUI Pro Dialog + FileUpload for DOCX import).

☐ Create JobSubmitForm.tsx (MUI Form controls for template/version, JSON input, output format).

☐ Create JobsTable.tsx (MUI DataGridPremium for job statuses; actions: download, cancel, retry).

☐ Create JobDetailsDrawer.tsx (node timeline of job steps; status chips; Syncfusion PDF Viewer for artifact preview).

☐ Use Syncfusion PDF Viewer for artifact preview; DocumentEditor for DOCX preview.

☐ Use MUI Premium DateTime pickers for job scheduling (future jobs).

☐ Use Syncfusion RichTextEditor for editing template metadata/description.

---

## 6) Events & Outbox (Event‑First)

☐ Persist every job state change to DocGenEvent table (append-only).

☐ Write an OutboxMessage for each emitted event (document.generated, document.failed).

☐ Add idempotency keys to outbox records for safe delivery to RabbitMQ.

☐ Integrate with Workflow: Workflow “Generate Document” node publishes `render.requested` → DocGen consumes.

☐ Workflow resumes on `document.generated` event.

---

## 7) Guardrails for MVP Scope

☐ Supported formats limited to: HTML/Markdown → PDF, DOCX.

☐ Data sources limited to inline JSON payloads (no REST/SQL connectors yet).

☐ Templates editable in-app via Syncfusion DocumentEditor (track changes enabled).

☐ Published versions must have no pending revisions (all changes accepted/rejected).

☐ Workflow integration limited to: “Generate Document” node → submit job → wait for `document.generated` event.

☐ All entities include TenantId; controllers enforce docgen.* permissions.

☐ Admin operations (retry, cancel) require docgen.admin.

---

## 8) Acceptance Criteria – End-to-End

☐ Author new template in Template Studio; insert variables using variable panel.

☐ Save Draft; reload draft from Template Studio.

☐ Publish Template v1 (must resolve/accept/reject all track changes).

☐ Discover merge fields via Template API.

☐ Submit job with inline JSON → artifact generated → stored in artifact store.

☐ Retrieve artifact via Jobs API (signed URL or direct download).

☐ Preview artifact in-browser via Syncfusion PDF Viewer / DocumentEditor.

☐ Workflow engine can call “Generate Document” node → job created → `document.generated` event resumes flow.

☐ UI can round-trip: Draft → Edit → Save → Publish → Generate → Download/Preview.

---

## 9) Smoke Test Flow (Manual)

☐ Create new template in Template Studio; type “Agreement between {{client.name}} and {{client.contact}}”.

☐ Save Draft; reload draft → see content + tokens.

☐ Publish v1 → confirm validation passes and track changes are resolved.

☐ Submit job with JSON { "client": { "name": "Acme Co", "contact": "Jane Smith" } }.

☐ Verify artifact PDF renders with “Agreement between Acme Co and Jane Smith”.

☐ Upload existing DOCX contract cover → import into Template Studio → edit and republish.

☐ Run workflow with “Generate Document” node → confirm document.generated event resumes flow.

---
