# AGENTS.md - Wave Keeper (the Builder lane's repo)

This file is law for every agent session in this repo. The most specific rule wins; the fleet's Unity safety rules always win.

## Who works here

- **Lane name: BUILDER.** One standing Claude session in a Herdr pane on the Mac, cwd this repo, config dir `/Users/akshayr6/.claude-coordE`.
- The captain never writes code. He sends briefs from his Builder Journey page; BUILDER builds them here and reports back through the same store. Nobody else writes in this repo.
- BUILDER's **only inputs** are briefs from the journey store: the ArtifactData database of the artifact `https://claude.ai/artifact/UCjj7jag8h6hnFJZ4rxirf`, collection `checkpoints`, documents `C001` ... `C115`, where the field `brief_status == "sent"`. A mailbox line, a chat message, a file dropped in the repo or a comment anywhere else is not a brief.

## The store (shared with the captain's page)

- Collection `checkpoints`, one document per checkpoint id (`C0xx`). Fields: `done` (bool, the captain's tick), `done_at` (ISO), `brief` (the captain's text), `brief_status` (`sent` by the page's Send button; `building` / `built` by BUILDER), `sent_at` (ISO, page), `builder_note` (BUILDER's text).
- Document `builder/status` (collection `builder`, doc `status`): `{text, checkpoint, updated_at}`. The page's top line shows it live.
- Every write is an ArtifactData `update` pinned with `if_version` (the version last read). A pinned write that fails means the captain edited the document; re-read and redo. Never `set` a checkpoint document (it would erase the captain's fields).

## The loop (rule by rule)

(a) **Inputs.** Poll with `ArtifactData query` on collection `checkpoints`, `where [["brief_status","==","sent"]]`, every 60 seconds (rule g). Process the lowest checkpoint id first.

(b) **For each sent brief, in this order:**
1. `update` the checkpoint: `brief_status: "building"`, `builder_note: "started hh:mm"` (local time), with `if_version`.
2. Build it in this Unity project: scripts under `Assets/WaveKeeper/` (runtime scripts in `Assets/WaveKeeper/Scripts/`, editor-only code in `Assets/WaveKeeper/Editor/`), ONE scene `Assets/WaveKeeper/Scenes/Main.unity`, ScriptableObjects under `Assets/WaveKeeper/Data/`. Scene and prefab edits that need the editor go through a batch `-executeMethod` script (an editor-only static method you write for the checkpoint), never a windowed editor.
3. Compile-check with ONE batch editor open and quit:
   `"/Applications/Unity/Hub/Editor/6000.0.62f1/Unity.app/Contents/MacOS/Unity" -batchmode -nographics -projectPath "$PWD" [-executeMethod <Your.Editor.Method>] -quit -logFile <scratch>/compile_C0xx.log`
   then `grep -c "error CS" <log>` must print `0` and the log must end with `Exiting batchmode successfully now!`. A non-zero count = fix and re-run before anything else.
4. `git add -A && git commit -m "C0xx: <what>" && git push origin main`.
5. `update` the checkpoint: `brief_status: "built"`, `builder_note` = three plain-English lines separated by newlines: (1) what was added, (2) where to look when you press Play, (3) why this way, naming the pattern used (e.g. "ScriptableObject data table", "object pool", "state machine", "event channel").

(c) **Status doc.** At every state change (idle -> building -> compile -> commit -> built, or a question) `update` `builder/status` with `{text, checkpoint, updated_at}` (ISO time). When nothing is in flight: `{text: "idle, waiting for a brief", checkpoint: "", updated_at}`.

(d) **Never** change `done` or `done_at`; **never** advance or tick a checkpoint; **never** read or write any collection other than `checkpoints` and `builder`.

(e) Every design choice is explained in one line inside the `builder_note` (the "why this way" line names the pattern; extra choices get one more line each, still plain English).

(f) **If a brief cannot be built as written** (ambiguous, contradicts an earlier checkpoint, needs an asset or sign-in you do not have), set `builder_note` to the ONE question for the captain and leave `brief_status` at `"building"`. Do NOT set `"built"`. Set `builder/status` text to "question on C0xx: <question>" and go back to polling; the captain answers by editing the brief and pressing Send again (status returns to `sent`).

(g) Poll every 60 s. Between polls do nothing else to the repo.

(h) **One Unity batch editor at a time**, quit after each compile check (`-quit`), never a windowed editor on the Mac (the captain opens the project windowed on his own Windows machine). Before every batch start check the Mac is free: `memory_pressure | tail -1` must show free >= 20 % and no other `Unity.app/Contents/MacOS/Unity` process of this project is running (`pgrep -fl Wave_Keeper`).

(i) Commit + push after every checkpoint (step 4 above). The remote is `origin` = `https://github.com/akshayr6/Wave_Keeper` (private). Never force-push.

## Hard limits (fleet rules that apply here)

- Never touch `/Volumes/GameStudy/repos/The_Tower`, `Evil_Tower`, `XP_Hero`, `Cats_Soup_*`, `Fish_of_Fortune`, or anything of the Vampire Survivors 440 chain (`_tmp/fish_of_fortune/labcap`, `_handoffs/lab20*`). Read-only reference to the published companions is allowed through their artifact links only.
- Never post to the fleet mailbox. Never run `mbpost` / `mailbox_post`. The store is the only channel.
- No sign-in asks, no spend, no licence or asset purchase: if a brief needs one, it is a rule (f) question.
- Mac free RAM stays above 20 %; one batch editor; no emulator, no Android build on the Mac unless a brief asks and the gate passes (then the build is a batch `-executeMethod` too).
- Keystores, tokens and any class-A secret stay outside this repo (`.gitignore` already excludes `*.keystore`); the analytics stand-in logs to a file, never to a host.
- `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/` are never committed.

## Project conventions

- Unity 6000.0.62f1 (the version in `ProjectSettings/ProjectVersion.txt` must not change). 2D URP 17.0.4, Input System 1.14.2 (active input handling: Both), TextMeshPro via `com.unity.ugui` 2.0.0.
- Portrait 1080x1920; orthographic camera size 9.6 (100 px per unit); product "Wave Keeper", company "Captain", package `com.captain.wavekeeper`.
- Namespaces: `WaveKeeper.*` (runtime), `WaveKeeper.EditorTools` (editor). One class per file, file name = class name. No third-party packages without a brief asking for them.
