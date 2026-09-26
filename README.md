# TsiYuki Core

Shared editor code every TsiYuki VRChat tool uses: the shared UI language and localization, the common
inspector look, undo-safe edits and NDMF error reporting.

## What goes in Core

A user installs one or two TsiYuki tools, not all of them, so Core stays small. Code goes in Core only when it
is either:

- needed by nearly every TsiYuki tool, or
- an agreement between tools that only works if they all share one copy (the UI language setting, for one).

Code that belongs to one kind of tool goes in the matching TsiYuki Core package instead, and VCC installs it
only with the tools that depend on it. A new one is made when a second tool in that field needs the code; until
then it stays in the tool that has it.

| Package | For tools that | Contents |
| --- | --- | --- |
| `moe.tsiyuki.core` | all | language, localization, GUI, undo, NDMF errors |
| `moe.tsiyuki.core.animation` | generate FX layers | layers, states, entry transitions |
| `moe.tsiyuki.core.texture` | bake textures and materials | texture reading and lilToon-style compositing |
| `moe.tsiyuki.core.menus` | generate Modular Avatar menus | menu items, placing a menu inside another |

None of them reference Modular Avatar or the VRChat SDK except `moe.tsiyuki.core.menus`, which exists to build
Modular Avatar menus.

> Work in progress. Not released yet.

## Install

Add the TsiYuki VPM listing to VCC / ALCOM: <https://tsiyukino.github.io/vpm-repos/>, then add **TsiYuki Core** to your project.

## License

MIT

---

## 中文

TsiYuki 各个 VRChat 工具都要用的编辑器代码（界面语言与多语言、界面样式、可撤销的编辑、NDMF 报错）。一般不需要单独安装，安装其他插件时 VCC 会自动带上。

用户通常只装一两个 TsiYuki 工具，所以 Core 只收两类东西：几乎所有工具都需要的代码，以及必须由所有工具共用一份才能成立的约定（比如界面语言设置）。只属于某一类工具的代码放在对应的 TsiYuki Core 子包里（`core.animation`、`core.texture`、`core.menus`），VCC 只会随依赖它的工具一起安装。当同一领域出现第二个需要它的工具时才新建子包，在那之前代码留在用到它的工具里。

> 开发中，尚未发布。
