<h1 align="center">Unity Screen Navigator</h1>

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

[日本語ドキュメント(Japanese Documents Available)](README_JA.md)

Transition pages and modals, play transition animation, stack their history and manage their lifecycle in Unity.

<p align="center">
  <img width=700 src="https://user-images.githubusercontent.com/47441314/137313323-b2f24a0c-1ee3-4df0-a175-05fba32d9af3.gif" alt="Demo">
</p>

## Table of Contents

<!-- START doctoc -->
<!-- param::isNotitle::true:: -->
<!-- END doctoc -->

## Overview

#### Features
* You can build pages, modals, tabs and their transitions easily and flexibly.
* Lifecycle of screens from load to destroy and associated memory management.
* Separated workflow with animators for complex screen transition animations.
* Simple library with no extra functions like state machine.

#### Demo
You can play the demo scene with the following steps.

1. Clone this repository.
2. Open and play the following scene.
    * https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/Demo/Scenes/Demo.unity

Please note that some of the images used in this demo are from the following free contents.  
For more information, including copyright, please refer to the following website.

* [JewelSaviorFREE](https://docs.google.com/a/brilliantservice.co.jp/forms/d/1N4xCA6lY_5d_pNC0TSLJMg-GqJyY2jDhmOvegN1QRjU/viewform)

## Setup

#### Requirement
Unity 2019.4 or higher.

#### Install
1. Open the Package Manager from Window > Package Manager
2. "+" button > Add package from git URL
3. Enter the following to install
   * https://github.com/Haruma-K/UnityScreenNavigator.git?path=/Assets/UnityScreenNavigator

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/118421190-97842b00-b6fb-11eb-9f94-4dc94e82367a.png" alt="Package Manager">
</p>

Or, open Packages/manifest.json and add the following to the dependencies block.

```json
{
    "dependencies": {
        "com.harumak.unityscreennavigator": "https://github.com/Haruma-K/UnityScreenNavigator.git?path=/Assets/UnityScreenNavigator"
    }
}
```

If you want to set the target version, specify it like follow.

* https://github.com/Haruma-K/UnityScreenNavigator.git?path=/Assets/UnityScreenNavigator#1.0.0

## Usage

## License
This software is released under the MIT License.  
You are free to use it within the scope of the license.  
However, the following copyright and license notices are required for use.

* https://github.com/Haruma-K/UnityScreenNavigator/blob/master/LICENSE.md
