# TooManyEmotes V81 Fix

Unofficial compatibility fix for TooManyEmotes on Lethal Company V81.

This repository contains a repaired source copy of [TooManyEmotes](https://github.com/cmooref17/Lethal-Company-TooManyEmotes) by cmooref17. The goal is to keep the mod usable on newer Lethal Company builds where camera and audio listener initialization behavior can differ from the original supported versions.

## What Was Fixed

- Fixed `ChangeAudioListenerToObject` compatibility with V81, where the original game method is public instead of only non-public.
- Added null-safety around local player camera and audio listener initialization.
- Added fallback recovery when `PlayerControllerB.activeAudioListener` is missing or initialized later than TooManyEmotes expects.
- Added fallback behavior if the original `ChangeAudioListenerToObject` call fails because of game updates or another mod.
- Fixed audio listener restoration checks when stopping a custom emote.
- Hardened camera reset handling so missing `StartOfRound.activeCamera` falls back to the gameplay camera instead of breaking emote flow.
- Added runtime MoreCompany cosmetic compatibility so `ApplyCosmetic` return-type/signature changes do not interrupt emote playback or preview rendering.
- Added extra MoreCompany fallback handling for early local-player initialization and alternate cosmetic method signatures.

## Repository Layout

```text
src/
  TooManyEmotes.sln
  TooManyEmotes/
  TooManyEmotesScrap/
LICENSE
NOTICE.md
```

The main compatibility changes are in:

```text
src/TooManyEmotes/Patches/ThirdPersonEmoteController.cs
```

## Thunderstore

This repository is intended to support an unofficial Thunderstore compatibility package. If you publish a package from this work, clearly identify it as an unofficial V81 compatibility fix rather than an official TooManyEmotes release.

If the original TooManyEmotes mod is updated to resolve these issues, this compatibility fix may be deprecated depending on the situation.

The current Thunderstore package version is `0.0.4`. The package version uses the `0.0.x` line because this is an independent compatibility fix package. The original plugin source still reports its upstream `2.3.12` plugin version internally.

Packaging note: the repaired DLL must be distributed together with the original TooManyEmotes `Assets/` folder. The DLL loads AssetBundles from `Assets/` relative to `TooManyEmotes.dll`, so omitting those files will cause missing AssetBundle errors for emotes, props, audio, and the radial menu.

## Credits

Original TooManyEmotes project by cmooref17:

https://github.com/cmooref17/Lethal-Company-TooManyEmotes

## License

This project preserves the original MIT License.

Original copyright:

```text
Copyright (c) 2023 cmooref17
```

See [LICENSE](LICENSE) and [NOTICE.md](NOTICE.md) for details.
