# Changelog

## [0.3.0] - 2026-09-22

### Added
- **Menu placement.** `YukiMenuRegistry` lets one TsiYuki tool install its menu inside a menu another one
  generates, whichever ran first: each tool registers the menu it made and asks for a move, and a pass here
  settles them all once every tool has had its turn, before Modular Avatar. Loops are detected and left
  alone. This package still references neither Modular Avatar nor the VRChat SDK — the tool that asked for
  the move supplies the code that performs it.

## [0.2.0] - 2026-09-21

### Added
- **Texture compositing**, moved here from the NonToon converter so other tools can use it:
  `Pixels` (reads any texture, readable or not, and keeps sRGB/linear straight), `TextureBaker`
  (layer compositing with lilToon's blend modes, tone correction, decal UVs, alpha modes and mask
  packing, with an LRU cache of source pixels), `MaskSource` and `BaseBakeSpec`.
  Conversion output is unchanged — every material and baked texture hashes identically to 0.1.0.

## [Unreleased]

### Added
- `YukiLanguage`: shared UI language setting (English / Chinese / Japanese) for all TsiYuki tools.
- `YukiLocalizer`: loads `Localization/<code>.txt` key-value files from each package.
- `YukiGUI`: common inspector header, sections and status labels.
