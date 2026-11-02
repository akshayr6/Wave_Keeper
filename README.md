# Wave Keeper

A portrait (1080x1920) 2D tower-defence game for Android, built in **Unity 6000.0.62f1** with the 2D URP. It is the build-along project of the captain's **Builder Journey** (https://claude.ai/artifact/UCjj7jag8h6hnFJZ4rxirf): 14 roadmap steps, each sent as a brief to the Builder lane and built here, checkpoint by checkpoint, so the captain can open the result, press Play and judge it against the roadmap's "done when" lines.

## What is in the repo

- `Assets/WaveKeeper/Scenes/Main.unity` - the one scene; everything is reachable from it.
- `Assets/WaveKeeper/Scripts/` - runtime code (namespace `WaveKeeper.*`).
- `Assets/WaveKeeper/Data/` - ScriptableObject tables (waves, towers, upgrades ...).
- `Assets/WaveKeeper/Editor/` - editor-only tools, including `WaveKeeperSetup.cs` (the one-shot project setup) and the batch scene builders the Builder writes per checkpoint.
- `Packages/manifest.json` - URP 17.0.4, 2D Sprite, 2D Animation 10.2.1, Input System 1.14.2, uGUI 2.0.0 (TextMeshPro lives inside it in Unity 6).
- `AGENTS.md` - the Builder lane's rules (read it before changing anything).

`Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/` are generated and not committed; Unity rebuilds them on first open.

## How the captain opens it on Windows

1. Clone: `git clone https://github.com/akshayr6/Wave_Keeper.git` (private repo; your GitHub login already has access).
2. Unity Hub -> **Add** -> **Add project from disk** -> choose the cloned folder.
3. The editor version **must be 6000.0.62f1** (the Hub shows it next to the project; install that exact version under Installs if it is missing). Opening with another version re-imports and changes `ProjectVersion.txt`, which the Builder's checks then fail.
4. First open takes a few minutes (Library build). Then open `Assets/WaveKeeper/Scenes/Main.unity` and press Play. The Game view aspect should be set to **Portrait 1080x1920** (the dropdown above the Game view).
5. After each checkpoint the Builder pushes a commit named `C0xx: <what>`; `git pull` and Unity picks the changes up automatically.

## How the loop works

1. On a build checkpoint of the Journey page you edit the starter brief and press **Send**. The page stores it in its shared database (collection `checkpoints`, doc `C0xx`, `brief_status = sent`).
2. The Builder lane (a Claude session on the Mac, pane name BUILDER) polls that database every 60 s. When it sees `sent`, it marks the checkpoint `building` with a "started hh:mm" note and the page's top line shows what it is doing.
3. It writes the code and scene changes here, compiles them with a headless Unity (no window), commits and pushes `C0xx: <what>`.
4. It marks the checkpoint `built` and writes a three-line note: what was added, where to look when you press Play, why this way (the pattern's name). You read the note on the page, pull, press Play, and tick the checkpoint yourself - the Builder never ticks it.
5. If a brief cannot be built as written, the note becomes ONE question and the status stays `building`. Edit the brief to answer and press Send again.

The Builder never changes your ticks, never posts anywhere else, never asks you to sign in or buy anything.
