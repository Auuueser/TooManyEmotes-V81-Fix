# Notice

This project is a modified fork/source copy of TooManyEmotes by cmooref17.

Original source:
https://github.com/cmooref17/Lethal-Company-TooManyEmotes

Original license:
MIT License

Original copyright:
Copyright (c) 2023 cmooref17

## Modifications

- Added V81 compatibility fixes for audio listener and camera switching behavior.
- Hardened `ThirdPersonEmoteController.CallChangeAudioListenerToObject` against missing or delayed `activeAudioListener` initialization.
- Added fallback handling when the original `ChangeAudioListenerToObject` method is unavailable or fails.
- Improved reset/stop emote audio listener restoration checks.

These changes are intended to keep TooManyEmotes functional on newer Lethal Company versions while preserving the original MIT license terms.
