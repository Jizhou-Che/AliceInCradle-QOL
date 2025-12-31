# AliceInCradle-QOL

## 简介

[Alice In Cradle](https://aliceincradle.com) 是一款 R18 2D 动作游戏，目前正在由[ひなゆあ](https://x.com/hinayua_r18)和[橋野みずは](https://x.com/HashinoMizuha)合作开发。更多关于游戏的信息可以在[这里](https://nanamehacha.dev)找到。

这个项目为该游戏提供基于 BepInEx 的去除圣光、无限背包、自动转轮的独立插件，可以在[这里](https://www.bilibili.com/video/BV1mN411373X)找到这些插件的效果演示视频合集。这些插件可以独立或以整合包（仅限 Windows 版）的形式下载。

## 使用方法

这些插件需要随游戏配置 [BepInEx 5 x64](https://github.com/BepInEx/BepInEx) 来加载。如果你使用 Windows 且想要开袋即食，可以在 Releases 页面下载整合包，只需将压缩包内的全部内容解压至游戏文件夹，使 BepInEx 文件夹与游戏的可执行主程序位于同一文件夹内即可。如果你熟悉 BepInEx 的使用并且希望手动配置一切，可以下载独立的 dll 插件，只需注意你可能需要在 BepInEx\config\BepInEx.cfg 中将 `HideManagerGameObject` 选项设置为 `true` 来防止 BepInEx 在特定情况下无法加载的问题。如果你使用 macOS 或者 Linux，配置 BepInEx 会稍微麻烦一点，如果你不确定该怎么做请参考 BepInEx 的官方页面并按指示操作。

## About

[Alice In Cradle](https://aliceincradle.com) is an R18 2D action game currently under development by [ひなゆあ](https://x.com/hinayua_r18) and [橋野みずは](https://x.com/HashinoMizuha). More information about the game can be found [here](https://nanamehacha.dev).

This project provides individual BepInEx plugins for the game with QOL features including demosaic, infinite inventory and automatic reels. A collection of demonstration videos can be found [here](https://www.bilibili.com/video/BV1mN411373X). The plugins can be downloaded individually or as a ready-to-use package (Windows only).

## Installation

The plugins require [BepInEx 5 x64](https://github.com/BepInEx/BepInEx) to work with the game. If you are using Windows and just want things to work, there is a ready-to-use archive you can download from the Releases page, simply extract everything within the archive to your game folder so that the BepInEx folder is side by side with the main executable of the game. If you are familiar with BepInEx usage and prefer to set everything up manually, you can download the plugin dlls individually, just note that you may need to set the `HideManagerGameObject` option to `true` in BepInEx\config\BepInEx.cfg to prevent BepInEx from getting blocked in certain scenarios. If you use macOS or Linux, setting up BepInEx is a little bit more work, please refer to the official page of BepInEx and follow their instructions if you are unsure about the steps.