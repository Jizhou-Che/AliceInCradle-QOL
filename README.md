# AliceInCradle-QOL

## 简介

[Alice In Cradle](https://aliceincradle.com) 是一款 R18 2D 动作游戏，目前正在由[ひなゆあ](https://x.com/hinayua_r18)和[橋野みずは](https://x.com/HashinoMizuha)合作开发。更多关于游戏的信息可以在[这里](https://nanamehacha.dev)找到。

这个项目为该游戏提供基于 BepInEx 的去除圣光、无限背包、自动转轮的独立插件，可以在[这里](https://www.bilibili.com/video/BV1mN411373X)找到这些插件的效果演示视频合集。这些插件可以独立或以整合包（仅限 Windows 版）的形式下载。此外，一些用于调试的插件也支持独立下载以供选用。

## 使用方法

这些插件需要随游戏配置 [BepInEx 5 x64](https://github.com/BepInEx/BepInEx) 来加载。如果你使用 Windows 且想要开袋即食，可以在右侧的 Releases 页面下载整合包，只需将压缩包内的**全部内容**解压至游戏文件夹，使 BepInEx 文件夹等几个项目与游戏的可执行主程序位于同一文件夹内即可。如果你熟悉 BepInEx 的使用并且希望手动配置一切，可以下载独立的 dll 插件。如果你使用 macOS 或者 Linux，配置 BepInEx 会稍微麻烦一点，如果你不确定该怎么做请参考 BepInEx 的官方页面并按指示操作。

## 注意事项

请不要将游戏安装在带有中文或特殊字符的路径，以及过长或嵌套了过多层文件夹的路径中，否则 BepInEx 很有可能失效。

## 补充说明

### 自动转轮 AutomaticReels

使用方法为在执行屏幕下方的最多八个**效果转轮**的选择时按住<kbd>右Shift</kbd>键，可以使效果转轮的选择最优化。默认为按住指定按键触发，也可以通过配置文件将 `TriggerMode` 设为 `Always` 使其始终触发，或设为 `Never` 使其始终不触发；`HoldKey` 模式下的触发按键可以通过 `TriggerKey` 修改。

### 无限背包 InfiniteInventory

自游戏 0.30d 版本起，由于仓库物品管理代码的逻辑变动，在仓库和背包之间整组移动大量物品可能会导致临时卡顿。

### 活点地图 MapTraveller

建议仅用于调试。这个插件会在存档中添加冗余信息，在意原版存档纯洁性的话请避免在使用插件功能后覆盖原版存档。

加载插件会在地图绘制时视所有地图边框为已解锁。在此基础上，在触发地图重绘帧（比如打开地图）时按住<kbd>右Shift</kbd>键会遍历当前区域并解锁全部地图（触发成功会卡几秒钟，这时候就可以松手了，否则随后的地图重绘帧会一直卡）。在按住<kbd>右Shift</kbd>键时可以额外按住以下按键来绘制对应的地图标记：

- <kbd>\,</kbd>键：战斗点。
- <kbd>\.</kbd>键：宝箱。
- <kbd>\/</kbd>键：长椅。
- <kbd>\[</kbd>键：咖啡师。
- <kbd>\]</kbd>键：木偶店主。
- <kbd>\\</kbd>键：提尔德。
- <kbd>\;</kbd>键：采集点。
- <kbd>\'</kbd>键：钓鱼点（存在无法绘制特殊位置的钓鱼点的情况）。

由于地图完全加载需要游戏帧，首次遍历地图完成后可能需要等待几秒才能绘制标记。此外，绘制的标记可能会互相遮挡，建议利用存读档分别绘制标记以防遗漏。

## About

[Alice In Cradle](https://aliceincradle.com) is an R18 2D action game currently under development by [ひなゆあ](https://x.com/hinayua_r18) and [橋野みずは](https://x.com/HashinoMizuha). More information about the game can be found [here](https://nanamehacha.dev).

This project provides individual BepInEx plugins for the game with QOL features including demosaic, infinite inventory and automatic reels. A collection of demonstration videos can be found [here](https://www.bilibili.com/video/BV1mN411373X). The plugins can be downloaded individually or as a ready-to-use package (Windows only). In addition, some debugging-purpose plugins are provided as individual downloads for optional use.

## Usage

The plugins require [BepInEx 5 x64](https://github.com/BepInEx/BepInEx) to work with the game. If you are using Windows and just want things to work, there is a ready-to-use archive you can download from the Releases page on the right, simply extract **everything** within the archive to your game folder so that the several entries including the BepInEx folder are in the same directory as the main executable of the game. If you are familiar with BepInEx usage and prefer to set everything up manually, you can download the plugin dlls individually. If you use macOS or Linux, setting up BepInEx is a little bit more work, please refer to the official page of BepInEx and follow their instructions if you are unsure about the steps.

## Note

Please do not put the game files inside paths with special characters, or paths that are too long or with too many nested folders, in which cases BepInEx is likely to fail.
