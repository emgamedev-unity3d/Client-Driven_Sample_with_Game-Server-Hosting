![Banner](Basic/ClientDriven/Resources/clientdriven_banner.png)
<br><br>
# Client-Driven Bitesize Sample w/ Game Server Hosting & Matchmaker

[![UnityVersion](https://img.shields.io/badge/Unity%20Version:-2021.3%20LTS-57b9d3.svg?logo=unity&color=2196F3)](https://unity.com/releases/editor/qa/lts-releases#:~:text=February%2014%2C%202023-,LTS%20Release,2021.3.24f1,-Released%3A%20February)
[![NetcodeVersion](https://img.shields.io/badge/Netcode%20Version:-1.3.1-57b9d3.svg?logo=unity&color=2196F3)](https://docs-multiplayer.unity3d.com/netcode/current/about)
<br><br>

This repository is a fork of Unity's [bitesize samples repository](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.bitesize), which are built with our netcode solution, [Netcode for GameObjects](https://github.com/Unity-Technologies/com.unity.netcode.gameobjects). This copy is a fun project to help us answer the question of what challenges does a developer face, when they'd like to implementing Game Server Hosting(Multiplay) and Matchmaker, to their multiplayer game made with Netcode for GameObjects.

The code you'll see in this sample is based on Unity's own [Matchplay Sample](https://docs.unity.com/matchmaker/en/manual/matchmaker-and-multiplay-sample), which demonstrates how to create a Matchmake button, a basic networked client-server game with a matchmaking feature from end-to-end using the Unity Engine and Cloud Services SDK.

**PLEASE NOTE:** This is a basic sample, and does not cover advanced features of Matchmaking and Server Hosting.

<br>

## Client Driven Sample Overview
See the [Client Driven Sample](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.bitesize/tree/main/Basic/ClientDriven) to learn about client driven movements, networked physics, spawning vs in-scene placed `NetworkObjects`, and `NetworkObject` parenting.
<br><br>

---
## Readme Contents and Quick Links
<details open> <summary> Click to expand/collapse contents </summary>

- ### [Getting the Project](#getting-the-project-1)
- ### [Requirements](#requirements-1)
- ### [Troubleshooting](#troubleshooting-1)
  - [Bugs](#bugs)
  - [Documentation](#documentation)
- ### [Contributing to the Bitesize Samples](#contributing-1)
- ### [Community](#community-1)
- ### [Feedback Form for Bitesize Samples](#feedback)
- ### [Other Samples](#other-samples-1)
  - [Boss Room](#boss-room-sample)

</details>

---
<br>

## Getting the project
### Direct download

 - This repository is public! Feel free to clone it, or download the contents as a `.zip` file.

 - __For Zipped File download:__ select `Code` and select the 'Download Zip' option.  Please note that this will download the branch you're currently viewing on Github
<br><br>

## Requirements

This project is compatible with the Unity Long Term Support (LTS) editor version, [2021.3.XX LTS](https://unity.com/releases/2021-lts). Please include standalone support for Windows/Mac in your installation.

**PLEASE NOTE:** <br>
- When you first download the project, it will not automatically work using the services. Every developer must create a [Unity dashboard](dashboard.unity3d.com) project, and run through the necessary setup guides for [Game Server Hosting](https://docs.unity.com/game-server-hosting/en/manual/guides/get-started), and then [Matchmaker](https://docs.unity.com/matchmaker/en/manual/matchmaker-quick-start) as well.
- You will also need Netcode for Game Objects to use these samples. Complete the [Hello World](https://docs-multiplayer.unity3d.com/netcode/current/tutorials/helloworld) tutorial to prepare your environment.
<br><br>

## Troubleshooting
### Bugs
- Report bugs in the Bitesize samples using Github [issues](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.bitesize/issues)
- Report NGO bugs using NGO Github [issues](https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues)
- Report Unity bugs using the [Unity bug submission process](https://unity3d.com/unity/qa/bug-reporting)
  
### Multiplayer Development Documentation
For a deep dive into Netcode for GameObjects and the Bitesize Samples, visit our [documentation site](https://docs-multiplayer.unity3d.com/).
<br><br>

## Community
For help, questions, networking advice, or discussions about Netcode for GameObjects and its samples, please join our [Discord Community](https://discord.gg/FM8SE9E) or create a post in the [Unity Multiplayer Forum](https://forum.unity.com/forums/netcode-for-gameobjects.661/).
<br><br>

## Feedback
If you'd like to leave feedback for this sample, or the other bitesize samples, please consider leaving us feedback [here](https://unitytech.typeform.com/bitesize)--it will only take a couple of minutes. Thanks!
<br><br>

## Other samples

### MatchPlay Sample
[MatchPlay](https://docs.unity.com/matchmaker/en/manual/matchmaker-and-multiplay-sample) demonstrates how to create a Matchmake button: a basic networked client-server game with a matchmaking feature from end-to-end using the Unity Engine and Cloud Services SDK.

### Galactic Kittens
[Galactic Kittens](https://github.com/UnityTechnologies/GalacticKittens) is a small 2D co-op space adventure sample game where players must blast off to space and defeat all of the possible enemies in a small time range.

### Boss Room Sample
[Boss Room](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/) is a fully functional co-op multiplayer RPG made with Unity Netcode. It is an educational sample designed to showcase typical netcode patterns that are frequently featured in similar multiplayer games.
<br><br>

[![Documentation](https://img.shields.io/badge/Unity-bitesize--docs-57b9d3.svg?logo=unity&color=2196F3)](https://docs-multiplayer.unity3d.com/netcode/current/learn/bitesize/bitesize-introduction)
[![Forums](https://img.shields.io/badge/Unity-multiplayer--forum-57b9d3.svg?logo=unity&color=2196F3)](https://forum.unity.com/forums/multiplayer.26/)
[![Discord](https://img.shields.io/discord/449263083769036810.svg?label=discord&logo=discord&color=5865F2)](https://discord.gg/FM8SE9E)
