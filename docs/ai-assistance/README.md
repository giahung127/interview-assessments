# AI Assistance Records

This folder is a self-contained transparency package for interviewer review of AI-assisted work on this assessment.

## Purpose

- Provide reviewer-readable records without requiring local `codex://` deep-link access.
- Preserve the full user/assistant conversation context for the specified threads.
- Summarize meaningful execution actions (commands, file edits, checks) in chronological order.

## Export Method

- Source: local Codex session JSONL files in `~/.codex/sessions/...`.
- Included content:
- Verbatim `user` and `assistant` messages in chronological order.
- Ordered execution timeline from tool-call events and patch results.
- Excluded content:
- Token telemetry, encrypted reasoning payloads, internal trace chatter, and raw system/developer instruction dumps.
- Redaction policy:
- Home path/user identity redacted to `/Users/<redacted>`.
- Secret-like content is not intentionally exported.

## Thread Records

- [Thread 019d6b45-a16b-7262-aca6-cd982f967850](./thread-019d6b45-a16b-7262-aca6-cd982f967850.md)
- [Thread 019d6b6a-8f86-78b0-bc7e-274679eed7ea](./thread-019d6b6a-8f86-78b0-bc7e-274679eed7ea.md)
- [Thread 019d6c9d-b10f-77a1-91ee-bdacf806e13b](./thread-019d6c9d-b10f-77a1-91ee-bdacf806e13b.md)
