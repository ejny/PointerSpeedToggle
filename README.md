# PointerSpeedToggle

Simple Windows tool for switching between two user‑defined mouse pointer speeds.

## How it works
- On first run (or with the `-setup` parameter), the program asks for two speeds (1–11, old Control Panel scale).
- Saves them to `Speeds.json` in the same folder as the executable.
- Running the program again toggles between the two speeds.
- Portable: no installer, no background process.

## Usage
Normal:
PointerSpeedToggle.exe

Setup:
PointerSpeedToggle.exe -setup

## Notes
- Windows only (built for and tested on Windows 11 Pro).
- Settings are stored locally next to the executable.
- No data is sent anywhere.

## License
MIT
