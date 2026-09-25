# Changelog

## [0.4.0] - 2026-09-26

### Added
- `UndoEdit`: records a component before a tool edits it, then marks it dirty and registers the prefab
  override, so the edit can be undone, is saved, and shows as an override on a prefab instance. Moved here
  from Yuki Wardrobe and Yuki Material, which each had their own copy.
- `AnimatorGraph`: the layers, write-defaults-off states and any-state entry transitions that generated FX
  controllers are made of, each handed to the caller to persist. Moved here from the same two tools.
  States are now built directly instead of through `AnimatorStateMachine.AddState(name, position)`, which
  also wrote to the machine's asset file and registered an undo step behind the caller's back; names are
  still made unique the same way.
- `YukiNdmfReport`: reports a build error to NDMF in a package's own words, from the tables its
  `YukiLocalizer` already loads. Replaces the NDMF localizer and `Report` helper each tool kept, along with
  the copy of the localization file parser that came with them. NDMF's error window now picks up edits to
  the localization files after TsiYuki → Reload Localization, instead of keeping what it read first.
- EditMode tests for all three (`TsiYuki.Core.Editor.Tests`).

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
