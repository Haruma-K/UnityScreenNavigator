<h1 align="center">Unity Screen Navigator</h1>

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

[English Documents Available(英語ドキュメント)](README.md)

UnityのuGUIで画面遷移、画面遷移アニメーション、遷移履歴のスタック、画面のライフサイクルマネジメントを行うためのライブラリです。

<p align="center">
  <img width="80%" src="https://user-images.githubusercontent.com/47441314/137313323-b2f24a0c-1ee3-4df0-a175-05fba32d9af3.gif" alt="Demo">
</p>

## 目次

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<!-- param::title::詳細:: -->
<details>
<summary>詳細</summary>

- [概要](#%E6%A6%82%E8%A6%81)
    - [特徴](#%E7%89%B9%E5%BE%B4)
    - [デモ](#%E3%83%87%E3%83%A2)
- [セットアップ](#%E3%82%BB%E3%83%83%E3%83%88%E3%82%A2%E3%83%83%E3%83%97)
    - [要件](#%E8%A6%81%E4%BB%B6)
    - [インストール](#%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB)
- [基本的な画面遷移](#%E5%9F%BA%E6%9C%AC%E7%9A%84%E3%81%AA%E7%94%BB%E9%9D%A2%E9%81%B7%E7%A7%BB)
    - [画面と遷移の概念](#%E7%94%BB%E9%9D%A2%E3%81%A8%E9%81%B7%E7%A7%BB%E3%81%AE%E6%A6%82%E5%BF%B5)
    - [ページを作成して遷移させる](#%E3%83%9A%E3%83%BC%E3%82%B8%E3%82%92%E4%BD%9C%E6%88%90%E3%81%97%E3%81%A6%E9%81%B7%E7%A7%BB%E3%81%95%E3%81%9B%E3%82%8B)
    - [モーダルを作成して遷移させる](#%E3%83%A2%E3%83%BC%E3%83%80%E3%83%AB%E3%82%92%E4%BD%9C%E6%88%90%E3%81%97%E3%81%A6%E9%81%B7%E7%A7%BB%E3%81%95%E3%81%9B%E3%82%8B)
    - [シートを作成して遷移させる](#%E3%82%B7%E3%83%BC%E3%83%88%E3%82%92%E4%BD%9C%E6%88%90%E3%81%97%E3%81%A6%E9%81%B7%E7%A7%BB%E3%81%95%E3%81%9B%E3%82%8B)
    - [遷移処理を待機する方法](#%E9%81%B7%E7%A7%BB%E5%87%A6%E7%90%86%E3%82%92%E5%BE%85%E6%A9%9F%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95)
    - [コンテナを取得する方法](#%E3%82%B3%E3%83%B3%E3%83%86%E3%83%8A%E3%82%92%E5%8F%96%E5%BE%97%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95)
- [画面遷移アニメーション](#%E7%94%BB%E9%9D%A2%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3)
    - [共通の遷移アニメーションを設定する](#%E5%85%B1%E9%80%9A%E3%81%AE%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E8%A8%AD%E5%AE%9A%E3%81%99%E3%82%8B)
    - [画面ごとに遷移アニメーションを設定する](#%E7%94%BB%E9%9D%A2%E3%81%94%E3%81%A8%E3%81%AB%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E8%A8%AD%E5%AE%9A%E3%81%99%E3%82%8B)
    - [相手画面に応じて遷移アニメーションを変更する](#%E7%9B%B8%E6%89%8B%E7%94%BB%E9%9D%A2%E3%81%AB%E5%BF%9C%E3%81%98%E3%81%A6%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E5%A4%89%E6%9B%B4%E3%81%99%E3%82%8B)
    - [画面遷移アニメーションと描画順](#%E7%94%BB%E9%9D%A2%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%81%A8%E6%8F%8F%E7%94%BB%E9%A0%86)
    - [シンプルな遷移アニメーションを簡単に作る](#%E3%82%B7%E3%83%B3%E3%83%97%E3%83%AB%E3%81%AA%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E7%B0%A1%E5%8D%98%E3%81%AB%E4%BD%9C%E3%82%8B)
    - [相手画面とのインタラクティブなアニメーションの実装](#%E7%9B%B8%E6%89%8B%E7%94%BB%E9%9D%A2%E3%81%A8%E3%81%AE%E3%82%A4%E3%83%B3%E3%82%BF%E3%83%A9%E3%82%AF%E3%83%86%E3%82%A3%E3%83%96%E3%81%AA%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%81%AE%E5%AE%9F%E8%A3%85)
    - [Timelineで画面遷移アニメーションをつける](#timeline%E3%81%A7%E7%94%BB%E9%9D%A2%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E3%81%A4%E3%81%91%E3%82%8B)
- [ライフサイクルイベント](#%E3%83%A9%E3%82%A4%E3%83%95%E3%82%B5%E3%82%A4%E3%82%AF%E3%83%AB%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88)
    - [ページのライフサイクルイベント](#%E3%83%9A%E3%83%BC%E3%82%B8%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%95%E3%82%B5%E3%82%A4%E3%82%AF%E3%83%AB%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88)
    - [モーダルのライフサイクルイベント](#%E3%83%A2%E3%83%BC%E3%83%80%E3%83%AB%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%95%E3%82%B5%E3%82%A4%E3%82%AF%E3%83%AB%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88)
    - [シートのライフサイクルイベント](#%E3%82%B7%E3%83%BC%E3%83%88%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%95%E3%82%B5%E3%82%A4%E3%82%AF%E3%83%AB%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88)
    - [コルーチンの代わりに非同期メソッドを使う](#%E3%82%B3%E3%83%AB%E3%83%BC%E3%83%81%E3%83%B3%E3%81%AE%E4%BB%A3%E3%82%8F%E3%82%8A%E3%81%AB%E9%9D%9E%E5%90%8C%E6%9C%9F%E3%83%A1%E3%82%BD%E3%83%83%E3%83%89%E3%82%92%E4%BD%BF%E3%81%86)
- [画面リソースのロード](#%E7%94%BB%E9%9D%A2%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%AE%E3%83%AD%E3%83%BC%E3%83%89)
    - [画面リソースの読み込み方法を変更する](#%E7%94%BB%E9%9D%A2%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%AE%E8%AA%AD%E3%81%BF%E8%BE%BC%E3%81%BF%E6%96%B9%E6%B3%95%E3%82%92%E5%A4%89%E6%9B%B4%E3%81%99%E3%82%8B)
    - [Addressableアセットシステムを使って読み込む](#addressable%E3%82%A2%E3%82%BB%E3%83%83%E3%83%88%E3%82%B7%E3%82%B9%E3%83%86%E3%83%A0%E3%82%92%E4%BD%BF%E3%81%A3%E3%81%A6%E8%AA%AD%E3%81%BF%E8%BE%BC%E3%82%80)
    - [同期的にロードする](#%E5%90%8C%E6%9C%9F%E7%9A%84%E3%81%AB%E3%83%AD%E3%83%BC%E3%83%89%E3%81%99%E3%82%8B)
    - [プリロードする](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%89%E3%81%99%E3%82%8B)
- [その他の機能](#%E3%81%9D%E3%81%AE%E4%BB%96%E3%81%AE%E6%A9%9F%E8%83%BD)
    - [ページを履歴にスタッキングしない](#%E3%83%9A%E3%83%BC%E3%82%B8%E3%82%92%E5%B1%A5%E6%AD%B4%E3%81%AB%E3%82%B9%E3%82%BF%E3%83%83%E3%82%AD%E3%83%B3%E3%82%B0%E3%81%97%E3%81%AA%E3%81%84)
    - [モーダルの背景を変更する](#%E3%83%A2%E3%83%BC%E3%83%80%E3%83%AB%E3%81%AE%E8%83%8C%E6%99%AF%E3%82%92%E5%A4%89%E6%9B%B4%E3%81%99%E3%82%8B)
    - [モーダルの背景がクリックされたらモーダルを閉じる](#%E3%83%A2%E3%83%BC%E3%83%80%E3%83%AB%E3%81%AE%E8%83%8C%E6%99%AF%E3%81%8C%E3%82%AF%E3%83%AA%E3%83%83%E3%82%AF%E3%81%95%E3%82%8C%E3%81%9F%E3%82%89%E3%83%A2%E3%83%BC%E3%83%80%E3%83%AB%E3%82%92%E9%96%89%E3%81%98%E3%82%8B)
    - [遷移中のインタラクションを有効にする](#%E9%81%B7%E7%A7%BB%E4%B8%AD%E3%81%AE%E3%82%A4%E3%83%B3%E3%82%BF%E3%83%A9%E3%82%AF%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E6%9C%89%E5%8A%B9%E3%81%AB%E3%81%99%E3%82%8B)
    - [Containerのマスクを外す](#container%E3%81%AE%E3%83%9E%E3%82%B9%E3%82%AF%E3%82%92%E5%A4%96%E3%81%99)
    - [再生中の遷移アニメーションの情報を取得する](#%E5%86%8D%E7%94%9F%E4%B8%AD%E3%81%AE%E9%81%B7%E7%A7%BB%E3%82%A2%E3%83%8B%E3%83%A1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%81%AE%E6%83%85%E5%A0%B1%E3%82%92%E5%8F%96%E5%BE%97%E3%81%99%E3%82%8B)
    - [画面ロード時に読み込み済みの Prefab インスタンスを使用する](#%E7%94%BB%E9%9D%A2%E3%83%AD%E3%83%BC%E3%83%89%E6%99%82%E3%81%AB%E8%AA%AD%E3%81%BF%E8%BE%BC%E3%81%BF%E6%B8%88%E3%81%BF%E3%81%AE-prefab-%E3%82%A4%E3%83%B3%E3%82%B9%E3%82%BF%E3%83%B3%E3%82%B9%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%99%E3%82%8B)
- [FAQ](#faq)
    - [各画面をPrefabではなくシーンで作りたい](#%E5%90%84%E7%94%BB%E9%9D%A2%E3%82%92prefab%E3%81%A7%E3%81%AF%E3%81%AA%E3%81%8F%E3%82%B7%E3%83%BC%E3%83%B3%E3%81%A7%E4%BD%9C%E3%82%8A%E3%81%9F%E3%81%84)
    - [ビューとロジックを分離する方法を知りたい](#%E3%83%93%E3%83%A5%E3%83%BC%E3%81%A8%E3%83%AD%E3%82%B8%E3%83%83%E3%82%AF%E3%82%92%E5%88%86%E9%9B%A2%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95%E3%82%92%E7%9F%A5%E3%82%8A%E3%81%9F%E3%81%84)
    - [各画面にデータを受け渡す方法を知りたい](#%E5%90%84%E7%94%BB%E9%9D%A2%E3%81%AB%E3%83%87%E3%83%BC%E3%82%BF%E3%82%92%E5%8F%97%E3%81%91%E6%B8%A1%E3%81%99%E6%96%B9%E6%B3%95%E3%82%92%E7%9F%A5%E3%82%8A%E3%81%9F%E3%81%84)
    - [Popしたページやモーダルを再利用したい](#pop%E3%81%97%E3%81%9F%E3%83%9A%E3%83%BC%E3%82%B8%E3%82%84%E3%83%A2%E3%83%BC%E3%83%80%E3%83%AB%E3%82%92%E5%86%8D%E5%88%A9%E7%94%A8%E3%81%97%E3%81%9F%E3%81%84)
- [ライセンス](#%E3%83%A9%E3%82%A4%E3%82%BB%E3%83%B3%E3%82%B9)

</details>
<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## 概要

#### 特徴
* ページやモーダル、タブとその画面遷移を簡単かつ柔軟に構築
* 画面のロードから破棄までライフサイクルとメモリを管理
* 複雑な画面遷移アニメーションをアニメーターと分業して実装可能なワークフロー
* 余計な機能（ex. GUIライブラリやステートマシンなど）を含まない、よく分離された単機能ライブラリ
* その他、履歴のスタッキングや遷移中のクリック防止などの機能を標準実装

#### デモ
デモシーンは以下の手順で再生できます。

1. リポジトリをクローンする
2. 以下のシーンを開いて再生する
    * https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/Demo/Scenes/Demo.unity


なお、本デモで使用している画像の一部は以下のフリーコンテンツを利用させていただいております。  
著作権情報などの詳細は以下のウェブサイトを参照してください。

* [ジュエルセイバーFREE](http://www.jewel-s.jp/)

## セットアップ

#### 要件
* Unity 2019.4 以上
* uGUI (UIElementsには非対応)

#### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力してインストール
   * https://github.com/Haruma-K/UnityScreenNavigator.git?path=/Assets/UnityScreenNavigator

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/118421190-97842b00-b6fb-11eb-9f94-4dc94e82367a.png">
</p>

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記します。

```json
{
    "dependencies": {
        "com.harumak.unityscreennavigator": "https://github.com/Haruma-K/UnityScreenNavigator.git?path=/Assets/UnityScreenNavigator"
    }
}
```

バージョンを指定したい場合には以下のように記述します。

* https://github.com/Haruma-K/UnityScreenNavigator.git?path=/Assets/UnityScreenNavigator#1.0.0

## 基本的な画面遷移

#### 画面と遷移の概念
Unity Screen Navigatorは、画面を「ページ」「モーダル」「シート」の3つに分類します。

「ページ」は順番に遷移をしていく画面です。  
例えば、ページAからページBへ遷移すると、ページAは履歴にスタッキングされます。  
ページBから戻るとページAが状態を保ったまま再表示されます。

<p align="center">
  <img width="50%" src="https://user-images.githubusercontent.com/47441314/136680850-2aca1977-02c2-4730-a0d8-603934f71c80.gif" alt="Demo">
</p>

「モーダル」は画面上に積み重なっていく画面です。  
モーダルの表示中は最前面にあるモーダル以外のインタラクションはブロックされます。

<p align="center">
  <img width="50%" src="https://user-images.githubusercontent.com/47441314/136698982-21ff5172-e38d-4d80-a976-a7ecc511c048.gif" alt="Demo">
</p>

「シート」はタブのようなGUIに使われます。  
履歴を持たず、一つのアクティブな画面だけが表示されます。    


<p align="center">
  <img width="50%" src="https://user-images.githubusercontent.com/47441314/136700074-2a4fa134-dc5d-4b72-90d8-f6b12c91fc0f.gif" alt="Demo">

さらに、これらの画面は入れ子にすることが可能です。  
また、各画面の領域は自由に指定できます（必ずしも全画面に表示する必要はありません）。

</p>
<p align="center">
  <img width="50%" src="https://user-images.githubusercontent.com/47441314/137634860-ae202ce7-5d2d-48b1-a938-358381d16780.gif" alt="Demo">
</p>

#### ページを作成して遷移させる
ページ遷移を作成するには、まずCanvas配下の任意のGameObjectに`Page Container`コンポーネントをアタッチします。  
ページはこのGameObjectのRectTransformにフィットするように表示されるのでサイズを調整します。

次にページのViewとなるGameObjectのルートに`Page`コンポーネントをアタッチします。  
このGameObjectは任意の名前をつけてPrefab化して、Resourcesフォルダ配下に配置しておきます。

このResourcesフォルダ以下のパスを`PageContainer.Push()`に与えることでページが表示されます。  
以下は`Assets/Resources/ExamplePage.prefab`に配置したページを読み込む例です。

```cs
PageContainer pageContainer;

// 「ExamplePage」に遷移する
var handle = pageContainer.Push("ExamplePage", true);

// 遷移の終了を待機する
yield return handle;
//await handle.Task; // awaitでも待機できます
//handle.OnTerminate += () => { }; // コールバックも使えます
```

また、アクティブなページを破棄して前のページを表示するには、`PageContainer.Pop()`を使います。

```cs
PageContainer pageContainer;

// アクティブなページから戻る
var handle = pageContainer.Pop(true);

// 遷移の終了を待機する
yield return handle;
```

戻る際にあるページをスキップしたい場合には、オプション引数を使うことで [履歴へのスタッキングを無効に](#ページを履歴にスタッキングしない) できます。

#### モーダルを作成して遷移させる
モーダル遷移を作成するには、Canvas配下の任意のGameObjectに`Modal Container`コンポーネントをアタッチします。  
一般的なモーダルは、その背景が画面全体を覆うことでクリックをブロッキングするデザインになります。  
したがってこのGameObjectのRectTransformのサイズは基本的には画面サイズと一致するように設定します。

次にモーダルのViewとなるGameObjectのルートに`Modal`コンポーネントをアタッチします。  
このルートGameObjectは`Modal Container`のサイズにフィットするように調整されます。  
したがって余白を持ったモーダルを作成する場合には、小さめのサイズを持った子GameObjectを作り、その中にコンテンツを作成します。

<p align="center">
  <img width="70%" src="https://user-images.githubusercontent.com/47441314/136698661-e4e247b6-7938-4fb5-8f6f-f2897f42eebe.png" alt="Demo">
</p>

このGameObjectは任意の名前をつけてPrefab化して、Resourcesフォルダ配下に配置しておきます。

このResourcesフォルダ以下のパスを`ModalContainer.Push()`に与えることでモーダルが表示されます。  
以下は`Assets/Resources/ExampleModal.prefab`に配置したモーダルを読み込む例です。

```cs
ModalContainer modalContainer;

// 「ExampleModal」に遷移する
var handle = modalContainer.Push("ExampleModal", true);

// 遷移の終了を待機する
yield return handle;
//await handle.Task; // awaitでも待機できます
//handle.OnTerminate += () => { }; // コールバックも使えます
```

また、アクティブなモーダルを破棄して前のモーダルを表示するには、`ModalContainer.Pop()`を使います。

```cs
ModalContainer modalContainer;

// アクティブなモーダルから戻る
var handle = modalContainer.Pop(true);

// 遷移の終了を待機する
yield return handle;
```

なお、 [モーダルの背景は自由に変更する](#モーダルの背景を変更する) ことができます。

#### シートを作成して遷移させる
シート遷移を作成するには、Canvas配下の任意のGameObjectに`Sheet Container`コンポーネントをアタッチします。  
シートはこのGameObjectのRectTransformにフィットするように表示されるのでサイズを調整します。

次にシートのViewとなるGameObjectのルートに`Sheet`コンポーネントをアタッチします。  
このGameObjectは任意の名前をつけてPrefab化して、Resourcesフォルダ配下に配置しておきます。

このResourcesフォルダ以下のパスを`SheetContainer.Register()`に与えることでシートが生成されます。  
生成後に`SheetContainer.Show()`を呼ぶことでアクティブなシートを切り替えられます。  
この時、すでにアクティブなシートが存在した場合にはそのシートは非アクティブになります。

以下は`Assets/Resources/ExampleSheet.prefab`に配置したシートを表示する例です。

```cs
SheetContainer sheetContainer;

// 「ExampleSheet」をインスタンス化する
var registerHandle = sheetContainer.Register("ExampleSheet");
yield return registerHandle;

// 「ExampleSheet」を表示する
var showHandle = sheetContainer.Show("ExampleSheet", false);
yield return showHandle;
```

なお、`Register()`メソッドにより同じリソースキーのシートを複数インスタンス化する場合には、  
リソースキーによるインスタンスの同一性を担保することができません。  
このようなケースでは以下のように、リソースキーの代わりにシートIDを使います。

```cs
SheetContainer sheetContainer;

// 「ExampleSheet」をインスタンス化してシートIDを得る
var sheetId = 0;
var registerHandle = sheetContainer.Register("ExampleSheet", x =>
{
    sheetId = x.sheetId;
});
yield return registerHandle;

// シートIDを使ってシートを表示する
var showHandle = sheetContainer.Show(sheetId, false);
yield return showHandle;
```

また、アクティブなシートを切り替えるのではなく非表示にするには、`SheetContainer.Hide()`を使います。

```cs
SheetContainer sheetContainer;

// アクティブなシートを非表示にする
var handle = sheetContainer.Hide(true);

// 遷移の終了を待機する
yield return handle;
```

#### 遷移処理を待機する方法
画面遷移のための各メソッドは、戻り値として`AsyncProcessHandle`を返します。  
このオブジェクトを使用すると、遷移処理が終了するまで待機することができます。

また待機方法としては、コルーチン、非同期メソッド、コールバックに対応しています。  
コルーチン内で待機するには以下のように`yield return`を使用します。

```cs
yield return pageContainer.Push("ExamplePage", true);
```

非同期メソッド内で待機するには以下のように`AsyncProcessHandle.Task`に対してawaitを使います。

```cs
await pageContainer.Push("ExamplePage", true).Task;
```

コールバックを使う場合には`AsyncProcessHandle.OnTerminate`を使います。

```cs
pageContainer.Push("ExamplePage", true).OnTerminate += () => { };
```

#### コンテナを取得する方法
各コンテナ（`PageContainer`/`ModalContainer`/`SheetContainer`）には、インスタンスを取得するための静的メソッドが用意されています。

以下のように`Container.Of()`を使うと、与えたTransformやRectTransformのから最も近い親にアタッチされているコンテナを取得できます。

```cs
var pageContainer = PageContainer.Of(transform);
var modalContainer = ModalContainer.Of(transform);
var sheetContainer = SheetContainer.Of(transform);
```

また、コンテナのインスペクタから`Name`プロパティを設定すると、その名前を用いてコンテナを取得することもできます。  
この場合、`Container.Find()`メソッドを使用します。

```cs
var pageContainer = PageContainer.Find("SomePageContainer");
var modalContainer = ModalContainer.Find("SomeModalContainer");
var sheetContainer = SheetContainer.Find("SomeSheetContainer");
```

## 画面遷移アニメーション

#### 共通の遷移アニメーションを設定する
デフォルトでは、画面の種類ごとに標準的な遷移アニメーションが設定されています。  

独自の遷移アニメーションを作成する場合には、`TransitionAnimationObject`を継承したクラスを作成します。  
このクラスはアニメーションの挙動を定義するためのプロパティやメソッドを持ちます。

```cs
// アニメーションの時間（秒）
public abstract float Duration { get; }

// 初期化する
public abstract void Setup();

// ある時間における状態を決める
public abstract void SetTime(float time);
```

実際の実装方法は [SimpleTransitionAnimationObject](https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/UnityScreenNavigator/Runtime/Core/Shared/SimpleTransitionAnimationObject.cs) を参考にしてください。

次にこのScriptableObjectをインスタンス化し、`UnityScreenNavigatorSettings`に設定します。  
`UnityScreenNavigatorSettings`は`Assets > Create > Screen Navigator Settings`から作成できます。

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/137321487-e2267184-6eba-46a7-9f4e-468176822408.png">
</p>

#### 画面ごとに遷移アニメーションを設定する
各画面ごとに個別のアニメーションを設定することもできます。

Page、Modal、SheetコンポーネントにはそれぞれAnimation Containerというプロパティが用意されています。  
ここでこの画面の遷移に使用するアニメーションを設定できます。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137632127-2e224b47-3ef1-4fdd-a64a-986b38d5ea6a.png">
</p>

`Asset Type`を`Scriptable Object`に設定した上で、前節で説明した`TransitionAnimationObject`を`Animation Object`にアサインすると、この画面の遷移アニメーションを変更できます。

また、ScriptableObjectではなくMonoBehaviourを使うこともできます。  
この場合、まず`TransitionAnimationBehaviour`を継承したクラスを作成します。  
実際の実装方法は [SimpleTransitionAnimationBehaviour](https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/UnityScreenNavigator/Runtime/Core/Shared/SimpleTransitionAnimationBehaviour.cs) を参考にしてください。

クラスを作ったら、このコンポーネントをアタッチした上で、`Asset Type`を`Mono Behaviour`にして参照をアサインします。

#### 相手画面に応じて遷移アニメーションを変更する
例えば画面Aが入ってきて画面Bが出ていくとき、画面Bを画面Aの「相手画面」と呼びます。

下図のプロパティに相手画面の名前を入力すると、相手画面と名前が一致したときのみその遷移アニメーションが適用されます。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137632918-9d777817-d2dc-43c9-bd7e-c6a1713a5f26.png">
</p>

デフォルトでは、Prefab名が画面名として使われます。  
明示的に命名したい場合、`Use Prefab Name As Identifer`のチェックを外した上で`Identifier`プロパティに名前を入力します。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137632986-f5727a42-4c27-48aa-930d-e7b0673b978f.png">
</p>

さらに、`Partner Page Identifier Regex`には正規表現も使用できます。  
なお複数のアニメーションを指定した場合、上から順に評価されます。

#### 画面遷移アニメーションと描画順
相手画面が存在するページやシートの遷移アニメーションでは描画順が重要になることがあります。  
例えば画面が相手画面に覆い被さるようなアニメーションです。

描画順を制御したい場合には、`Rendering Order`プロパティを使用します。

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/137633021-4e864c77-baa0-4d42-a8e7-b0183f7302f5.png">
</p>

画面遷移時にはこの値が小さいものから順に描画されます。

なおモーダルは常に新しいものが手前に表示されるため、`Rendering Order`プロパティを持ちません。

#### シンプルな遷移アニメーションを簡単に作る
シンプルな遷移アニメーションの実装として`SimpleTransitionAnimationObject`を使うことができます。

これを使うには、`Assets > Create > Screen Navigator > Simple Transition Animation`を選択します。  
すると以下のようなScriptableObjectが生成されるので、Inspectorからアニメーションを設定します。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137326944-112e0254-cd27-4d49-a32b-9c436b9537e4.png">
</p>

これのMonoBehaviour実装版である`SimpleTransitionAnimationBehaviour`も用意しています。  
こちらは直接GameObjectにアタッチして使用します。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137326555-90cdce8d-98da-4a00-99cc-5a65c1086760.png">
</p>

各プロパティの説明は以下の通りです。

|名前|説明|
|-|-|
|Delay|アニメーション開始前の遅延時間（秒）|
|Duration|アニメーション時間（秒）|
|Ease Type|イージング関数の種類|
|Before Alignment|遷移前の、コンテナからの相対位置|
|Before Scale|遷移前のスケール値|
|Before Alpha|遷移前の透明度|
|After Alignment|遷移後の、コンテナからの相対位置|
|After Scale|遷移後のスケール値|
|After Alpha|遷移後の透明度|

#### 相手画面とのインタラクティブなアニメーションの実装
相手画面の状態を参照したアニメーションを作成することもできます。  
以下の例では、前のモーダルの画像を拡大しつつシームレスに次のダイアログに遷移しています。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137315378-974395a8-f910-41a9-8e07-2964efded848.gif">
</p>

これを実装するには、まず`TransitionAnimationObject`や`TransitionAnimationBehaviour`を継承したクラスを作成します。  
そして`PartnerRectTransform`プロパティを参照することで相手画面を取得します。  
相手画面が存在しない場合には`PartnerRectTransform`はnullになります。

具体的な実装方法は、デモに含まれる [CharacterImageModalTransitionAnimation](https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/Demo/Scripts/CharacterImageModalTransitionAnimation.cs) を参考にしてください。

#### Timelineで画面遷移アニメーションをつける
Timelineを使って画面遷移アニメーションを作成することもできます。  
複雑な遷移アニメーションにはTimelineを使用することをおすすめします。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137634258-135b454e-04b5-49e8-a87a-bfb6ede03f49.gif">
</p>

これを実装するためにはまず適当なGameObjectに`Timeline Transition Animation Behaviour`をアタッチします。  
プロパティに`Playable Director`と`Timeline Asset`をアサインしておきます。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137633599-dd8b204e-e6ec-46bf-b93c-ee54b4ac3d59.png">
</p>

`Playable Director`の`Play On Awake`プロパティのチェックは外しておいてください。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137633492-4d837177-a381-486f-8942-df26e522da91.png">
</p>

最後にこの`Timeline Transition Animation Behaviour`を`Animation Container`にアサインします。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137633821-1fa1a8d6-ca41-49ca-aacf-dcf7f744c0b1.png">
</p>

なお、Timelineを使ってuGUIのアニメーションを作成するには [UnityUIPlayables](https://github.com/Haruma-K/UnityUIPlayables) がおすすめです。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/113313016-cf9afe80-9345-11eb-9aa9-422c53b5a3f8.gif">
</p>

## ライフサイクルイベント
画面遷移中には、画面のライフサイクルに紐づいたイベントが実行されます。  
これをフックすることで画面の初期化時や遷移前後の処理を作成することができます。

#### ページのライフサイクルイベント
`Page`クラスを継承したクラスで以下のようにメソッドをオーバーライドすることで、  
そのページのライフサイクルに紐づく処理を記述することができます。

```cs
using System.Collections;
using UnityScreenNavigator.Runtime.Core.Page;

public class SomePage : Page
{
    // このページがロードされた直後に呼ばれる
    public override IEnumerator Initialize() { yield break; }
    // このページがリリースされる直前に呼ばれる
    public override IEnumerator Cleanup() { yield break; }
    // Push遷移によりこのページが表示される直前に呼ばれる
    public override IEnumerator WillPushEnter() { yield break; }
    // Push遷移によりこのページが表示された直後に呼ばれる
    public override void DidPushEnter() { }
    // Push遷移によりこのページが非表示になる直前に呼ばれる
    public override IEnumerator WillPushExit() { yield break; }
    // Push遷移によりこのページが非表示になった直後に呼ばれる
    public override void DidPushExit() { }
    // Pop遷移によりこのページが表示される直前に呼ばれる
    public override IEnumerator WillPopEnter() { yield break; }
    // Pop遷移によりこのページが表示された直後に呼ばれる
    public override void DidPopEnter() { }
    // Pop遷移によりこのページが非表示になる直前に呼ばれる
    public override IEnumerator WillPopExit() { yield break; }
    // Pop遷移によりこのページが非表示になった直後に呼ばれる
    public override void DidPopExit() { }
}
```

また、以下のように `Page.AddLifecycleEvent()` により外部からライフサイクルイベントを登録することもできます。

```cs
// IPageLifecycleEventは上記のライフサイクルイベントが定義されているインターフェース
// 第二引数で実行優先度を指定できる
//  0未満: Pageのライフサイクルイベントよりも前に実行
//  1以上: Pageのライフサイクルイベントよりも後に実行
IPageLifecycleEvent lifecycleEventImpl;
Page page;
page.AddLifecycleEvent(lifecycleEventImpl, -1);

// 以下のように一部のライフサイクルイベントだけを登録することもできる
IEnumerator OnWillPushEnter()
{
    // 何かしらの処理
    yield break;
}
page.AddLifecycleEvent(onWillPushEnter: OnWillPushEnter);
```

さらに、`IPageContainerCallbackReceiver`を実装したクラスを`PageContainer.AddCallbackReceiver()`に渡すことで、コンテナから遷移イベントをフックできます。

```cs
public interface IPageContainerCallbackReceiver
{
    // ページがPushされる直前に呼ばれる
    void BeforePush(Page enterPage, Page exitPage);
    // ページがPushされた直後に呼ばれる
    void AfterPush(Page enterPage, Page exitPage);
    // ページがPopされる直前に呼ばれる
    void BeforePop(Page enterPage, Page exitPage);
    // ページがPopされた直後に呼ばれる
    void AfterPop(Page enterPage, Page exitPage);
}
```

なお`IPageContainerCallbackReceiver`を`MonoBehaviour`に実装してページのGameObjectにアタッチしておけば、  
`PageContainer.AddCallbackReceiver()`を呼ばなくても初期化時に`PageContainer`に登録されます。

#### モーダルのライフサイクルイベント
`Modal`クラスを継承したクラスで以下のようにメソッドをオーバーライドすることで、  
そのモーダルのライフサイクルに紐づく処理を記述することができます。

```cs
using System.Collections;
using UnityScreenNavigator.Runtime.Core.Modal;

public class SomeModal : Modal
{
    // このモーダルがロードされた直後に呼ばれる
    public override IEnumerator Initialize() { yield break; }
    // このモーダルがリリースされる直前に呼ばれる
    public override IEnumerator Cleanup() { yield break; }
    // Push遷移によりこのモーダルが表示される直前に呼ばれる
    public override IEnumerator WillPushEnter() { yield break; }
    // Push遷移によりこのモーダルが表示された直後に呼ばれる
    public override void DidPushEnter() { }
    // Push遷移によりこのモーダルが非表示になる直前に呼ばれる
    public override IEnumerator WillPushExit() { yield break; }
    // Push遷移によりこのモーダルが非表示になった直後に呼ばれる
    public override void DidPushExit() { }
    // Pop遷移によりこのモーダルが表示される直前に呼ばれる
    public override IEnumerator WillPopEnter() { yield break; }
    // Pop遷移によりこのモーダルが表示された直後に呼ばれる
    public override void DidPopEnter() { }
    // Pop遷移によりこのモーダルが非表示になる直前に呼ばれる
    public override IEnumerator WillPopExit() { yield break; }
    // Pop遷移によりこのモーダルが非表示になった直後に呼ばれる
    public override void DidPopExit() { }
}
```

また、以下のように `Modal.AddLifecycleEvent()` により外部からライフサイクルイベントを登録することもできます。

```cs
// IModalLifecycleEventは上記のライフサイクルイベントが定義されているインターフェース
// 第二引数で実行優先度を指定できる
//  0未満: Modalのライフサイクルイベントよりも前に実行
//  1以上: Modalのライフサイクルイベントよりも後に実行
IModalLifecycleEvent lifecycleEventImpl;
Modal modal;
modal.AddLifecycleEvent(lifecycleEventImpl, -1);

// 以下のように一部のライフサイクルイベントだけを登録することもできる
IEnumerator OnWillPushEnter()
{
    // 何かしらの処理
    yield break;
}
modal.AddLifecycleEvent(onWillPushEnter: OnWillPushEnter);
```

さらに、`IModalContainerCallbackReceiver`を実装したクラスを`ModalContainer.AddCallbackReceiver()`に渡すことで、コンテナから遷移イベントをフックできます。

```cs
public interface IModalContainerCallbackReceiver
{
    // モーダルがPushされる直前に呼ばれる
    void BeforePush(Modal enterModal, Modal exitModal);
    // モーダルがPushされた直後に呼ばれる
    void AfterPush(Modal enterModal, Modal exitModal);
    // モーダルがPopされる直前に呼ばれる
    void BeforePop(Modal enterModal, Modal exitModal);
    // モーダルがPopされた直後に呼ばれる
    void AfterPop(Modal enterModal, Modal exitModal);
}
```

なお`IModalContainerCallbackReceiver`を`MonoBehaviour`に実装してモーダルのGameObjectにアタッチしておけば、  
`ModalContainer.AddCallbackReceiver()`を呼ばなくても初期化時に`ModalContainer`に登録されます。

#### シートのライフサイクルイベント
`Sheet`クラスを継承したクラスで以下のようにメソッドをオーバーライドすることで、  
そのシートのライフサイクルに紐づく処理を記述することができます。

```cs
using System.Collections;
using UnityScreenNavigator.Runtime.Core.Sheet;

public class SomeSheet : Sheet
{
    // このシートがロードされた直後に呼ばれる
    public override IEnumerator Initialize() { yield break; }
    // このシートがリリースされる直前に呼ばれる
    public override IEnumerator Cleanup() { yield break; }
    // このシートが表示される直前に呼ばれる
    public override IEnumerator WillEnter() { yield break; }
    // このシートが表示された直後に呼ばれる
    public override void DidEnter() { }
    // このシートが非表示になる直前に呼ばれる
    public override IEnumerator WillExit() { yield break; }
    // このシートが非表示になった直後に呼ばれる
    public override void DidExit() { }
}
```

また、以下のように `Sheet.AddLifecycleEvent()` により外部からライフサイクルイベントを登録することもできます。

```cs
// IModalLifecycleEventは上記のライフサイクルイベントが定義されているインターフェース
// 第二引数で実行優先度を指定できる
//  0未満: Modalのライフサイクルイベントよりも前に実行
//  1以上: Modalのライフサイクルイベントよりも後に実行
ISheetLifecycleEvent lifecycleEventImpl;
Sheet sheet;
sheet.AddLifecycleEvent(lifecycleEventImpl, -1);

// 以下のように一部のライフサイクルイベントだけを登録することもできる
IEnumerator OnWillEnter()
{
    // 何かしらの処理
    yield break;
}
modal.AddLifecycleEvent(onWillEnter: OnWillEnter);
```

さらに、`ISheetContainerCallbackReceiver`を実装したクラスを`SheetContainer.AddCallbackReceiver()`に渡すことで、コンテナから遷移イベントをフックできます。

```cs
public interface ISheetContainerCallbackReceiver
{
    // シートがShowされる直前に呼ばれる
    void BeforeShow(Sheet enterSheet, Sheet exitSheet);
    // シートがShowされた直後に呼ばれる
    void AfterShow(Sheet enterSheet, Sheet exitSheet);
    // シートがHideされる直前に呼ばれる
    void BeforeHide(Sheet exitSheet);
    // シートがHideされた直後に呼ばれる
    void AfterHide(Sheet exitSheet);
}
```

なお`ISheetContainerCallbackReceiver`を`MonoBehaviour`に実装してシートのGameObjectにアタッチしておけば、  
`SheetContainer.AddCallbackReceiver()`を呼ばなくても初期化時に`SheetContainer`に登録されます。

#### コルーチンの代わりに非同期メソッドを使う
以下のように、コルーチンの代わりに非同期メソッドを使用してライフサイクルイベントを定義することもできます。

```cs
using System.Threading.Tasks;
using UnityScreenNavigator.Runtime.Core.Page;

public class SomePage : Page
{
    // 非同期メソッドを使ってライフサイクルイベントを定義する
    public override async Task Initialize()
    {
        await Task.Delay(100);
    }
}
```

非同期メソッドを使うには、以下の手順で`Scripting Define Symbols`を追加します。

* Player Settings > Other Settingsを開く
* Scripting Define Symbolsに`USN_USE_ASYNC_METHODS`を追加

`Scripting Define Symbols`は全てのプラットフォームに対して設定する必要がある点に注意してください。

## 画面リソースのロード

#### 画面リソースの読み込み方法を変更する
上述の通り、デフォルトでは各画面のリソースはResourcesフォルダにPrefabとして配置して読み込みます。

リソースの読み込み方法を変更するには、まず`AssetLoaderObject`を継承したScriptable Objectを作成します。  
`AssetLoaderObject`は`IAssetLoader`の実装であり、以下のメソッドを持ちます。

```cs
// keyに対応するリソースを読み込む
public abstract AssetLoadHandle<T> Load<T>(string key) where T : Object;

// keyに対応するリソースを非同期で読み込む
public abstract AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object;

// handleが示すリソースを解放する
public abstract void Release(AssetLoadHandle handle);
```

具体的な実装方法は [ResourcesAssetLoader](https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/UnityScreenNavigator/Runtime/Foundation/AssetLoader/ResourcesAssetLoader.cs) を参考にしてください。  
このクラスを作成したら、Scriptable Objectをインスタンス化し、`UnityScreenNavigatorSettings`の`AssetLoader`プロパティにアサインします。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137721037-18c84aad-28d3-4dd8-9a4a-4450a80badd2.png">
</p>

`UnityScreenNavigatorSettings`は`Assets > Create > Screen Navigator Settings`から作成できます。

また、各 `Container` の `AssetLoader` プロパティに値を設定することで、Container ごとに使用する `IAssetLoader` を設定できます。

#### Addressableアセットシステムを使って読み込む
Addressableアセットシステム用の`IAssetLoader`の実装は標準で用意しています。  
アドレスを使って各画面を読み込みたい場合には以下の手順で設定します。

1. Assets > Create > Resource Loader > Addressable Asset Loader を選択
2. 1.で作成したScriptable Objectを`UnityScreenNavigatorSettings`の`AssetLoader`プロパティにアサイン

#### 同期的にロードする
画面を同期的にロードするには、各コンテナの遷移メソッドの`loadAsync`引数にfalseを渡します。  
例えば`PageContainer.Push()`の場合は以下のように記述します。

```cs
PageContainer container;

// 同期ロード
var handle = container.Push("FooPage", true, loadAsync: false);

// 遷移アニメーションの終了を待つ
yield return handle;
```

さらに`onLoad`コールバックを併用すると、遷移メソッドのコールと同じフレームで初期化処理を行えます。

```cs
PageContainer container;

// 同期ロード & ロード後のコールバック
var handle = container.Push("FooPage", true, loadAsync: false, onLoad: page =>
{
    // ページの初期化処理（Pushと同フレームに呼ばれる）
    page.Setup();
});

// 遷移アニメーションの終了を待つ
yield return handle;
```

なお、`AddressableAssetLoader`を使う場合、同期ロードを行うにはAddressables 1.17.4以上が必要です。  
さらに [Addressableの仕様](https://docs.unity3d.com/Packages/com.unity.addressables@1.17/manual/SynchronousAddressables.html) によりパフォーマンス上の注意点が存在するのでご注意ください。

#### プリロードする
ページやモーダルのリソースは、画面遷移がリクエストされて初めてロードされます。  
容量の大きいページやモーダルをロードする際には、ロードに時間がかかりスムーズな遷移を妨げるかもしれません。

そのような場合にはプリロードして事前にリソースをロードしておく手法が有用です。  
以下は`PageContainer`でプリロードを行う例です。

```cs
const string pageName = "FooPage";
PageContainer container;

// FooPageをプリロードする
var preloadHandle = container.Preload(pageName);

// プリロードの完了を待つ
yield return preloadHandle;

// FooPageがプリロードされているのでスムーズに遷移できる
container.Push(pageName, true);

// プリロードしたFooPageを破棄する
container.ReleasePreloaded(pageName);
```

具体的な使用例としては [DemoのHomePage](https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/Demo/Scripts/HomePage.cs) を参考にしてください。  
`Home`ページの初期化時に`Shop`ページも同時に読み込み、破棄も同時に行っています。

## その他の機能

#### ページを履歴にスタッキングしない
ロード画面や演出用のページのように、履歴にスタッキングせずに戻る遷移の際にはスキップしたいページがあります。

このようなケースでは、`PageContainer.Push()`メソッドのオプション引数`stack`をfalseに指定すると、そのページは履歴に積まれなくなります。  
次のページに遷移する際にはこのページのインスタンスは破棄され、したがって戻る際にはスキップされます。

```cs
PageContainer container;

// FooPageを履歴に積まずに遷移する
yield return container.Push("FooPage", true, stack: false);

// BarPageに遷移し、FooPageは破棄される
yield return container.Push("BarPage", true);

// PopするとFooPageには戻らず、さらにその前のページに戻る
yield return container.Pop(true);
```

具体的な使用例としては [DemoのTopPage](https://github.com/Haruma-K/UnityScreenNavigator/blob/master/Assets/Demo/Scripts/TopPage.cs) を参考にしてください。  
スタッキングしないロード用のページに遷移しています。

#### モーダルの背景を変更する
デフォルトでは、モーダルの背景として黒い半透明の画面が設定されています。  
これは設定により変更することができます。

変更するにはまずモーダル背景のViewに`Modal Backdrop`コンポーネントをアタッチし、これをPrefab化します。

次にこのPrefabをモーダルの背景としてアサインします。  
アプリケーション全体のモーダルの背景を変更するには、`UnityScreenNavigatorSettings`の`Modal Backdrop Prefab`にアサインします。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137837643-f5c7cc13-a379-4c0f-9c34-1e9c19869136.png">
</p>

`UnityScreenNavigatorSettings`は`Assets > Create > Screen Navigator Settings`から作成できます。

また、`Modal Container`ごとに背景を設定するには、`Modal Container`の`Override Backdrop Prefab`にPrefabをアサインします。

#### モーダルの背景がクリックされたらモーダルを閉じる
デフォルトではモーダル背景はクリックできません。  
モーダルの背景がクリックされたときに最前面のモーダルを閉じるには、まず上記の手順でモーダルの背景を変更します。  
その上で **Modal Backdrop** の **Close Modal When Clicked** プロパティにチェックを入れます。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/182382933-14af7d19-50e4-4cbc-ac89-bd9702df26af.png">
</p>

#### 遷移中のインタラクションを有効にする
遷移開始から終了までは、全てのコンテナにおいて画面のクリックなどのインタラクションが無効になります。  
この設定は`UnityScreenNavigatorSettings`の`Enable Interaction In Transition`と`Control Interactions Of All Containers`で変更できます。  
デフォルトでは、`Enable Interaction In Transition`はfalseで、`Control Interactions Of All Containers`はtrueになっています。

`UnityScreenNavigatorSettings`の`Enable Interaction In Transition`をtrueに設定すると、遷移中でもインタラクションが有効になります。  
また`Enable Interaction In Transition`をfalseにしたまま`Control Interactions Of All Containers`をfalseにすると、遷移処理を行なっているコンテナのみインタラクションを無効にします。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/200176139-4de8c94e-60f4-4db9-8abf-7b6d50bea09e.png">
</p>

`UnityScreenNavigatorSettings`は`Assets > Create > Screen Navigator Settings`から作成できます。

ただし、あるコンテナについて遷移中に他の画面への遷移はできないので、  
インタラクションを有効にする場合には遷移のタイミングを自身で適切に制御する必要があります。

#### Containerのマスクを外す
デフォルトでは、コンテナの配下の画面のうち、コンテナの外に出た部分はマスクされます。  
コンテナ外の画面も表示したい場合には、コンテナのGameObjectにアタッチされている`Rect Mask 2D`コンポーネントのチェックボックスを外してください。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/137837996-42eacaae-8852-40f4-acfd-41f8bf9110a3.png">
</p>

#### 再生中の遷移アニメーションの情報を取得する
再生中の遷移アニメーションの情報は `Page`、`Modal`、`Sheet` クラスの以下のプロパティから取得できます。

| プロパティ名 | 説明 |
|-|-|
| IsTransitioning | 遷移中かどうか。 |
| TransitionAnimationType | 遷移アニメーションの種類。遷移中じゃない場合にはnullを返す。 |
| TransitionAnimationProgress | 遷移アニメーションの進捗。 |
| TransitionAnimationProgressChanged | 遷移アニメーションの進捗が変わった時のイベント。 |

#### 画面ロード時に読み込み済みの Prefab インスタンスを使用する
`PreloadedAssetLoaderObject` を使用すると、画面読み込み時に Resources や Addressables を経由せず、読み込み済みの Prefab インスタンスを直接できます。  
Assets > Create > Resource Loader > Preloaded Asset Loader から作成した Scriptable Object に以下のようにキーと Prefab を入力することで使用できます。

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/174422762-5942a833-3a89-43bb-ab5f-a59eecaff1f6.png">
</p>

また、ランタイム用の実装として `PreloadedAssetLoader` も用意しています。

## FAQ

#### 各画面をPrefabではなくシーンで作りたい
`AssetLoader` を実装することで、シーンに配置された画面を読み込むことができます。  
リクエストされたページに対応したシーンファイルを読み込み、その中から適切な GameObject を返すような `AssetLoader` を実装します。  
詳細は [画面リソースの読み込み方法を変更する](#%E7%94%BB%E9%9D%A2%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%AE%E8%AA%AD%E3%81%BF%E8%BE%BC%E3%81%BF%E6%96%B9%E6%B3%95%E3%82%92%E5%A4%89%E6%9B%B4%E3%81%99%E3%82%8B) を参照してください。

#### ビューとロジックを分離する方法を知りたい
以下のブログ記事に考え方と実装方法を掲載しています。

https://light11.hatenadiary.com/entry/2022/01/11/193925

#### 各画面にデータを受け渡す方法を知りたい
まず、例として、デモシーンでは以下のように、ロードが完了したタイミングで画面にデータを受け渡しています。

https://github.com/Haruma-K/UnityScreenNavigator/blob/8a115b1b25ac1d9fcf4b1ab6d5f2c1cd1d915ee5/Assets/Demo/Scripts/CharacterModal.cs#L91

ただし、これ以外にもさまざまな受け渡し方法が考えられます。  
例えばDIコンテナを使ってデータを各画面にセットしたいケースも考えられます。  
したがって本ライブラリとしては、特定の方法を実装して強制することは行わない方針です。

#### Popしたページやモーダルを再利用したい
Popしたページやモーダルは即座に破棄され、再利用することはできません。

再利用をしたいという要望の本質は、以下の二つに分類できます。

1. 毎回画面のリソースをロードしたくない
2. 画面の状態を保持したい

このうち、ロード時間の問題は [プリロード](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%89%E3%81%99%E3%82%8B) で解決できます。  
状態の保持については、保守性の観点からも、状態とビューを切り離して再構築できる実装を行うことをお勧めします。

また、一般的にユーザビリティとして状態を保持するべきなのは「タブ」による遷移です。  
本ライブラリにおいてもタブを実現するための「シート」による遷移では状態が常に保持されます。  
詳しくは [シートを作成して遷移させる](#%E3%82%B7%E3%83%BC%E3%83%88%E3%82%92%E4%BD%9C%E6%88%90%E3%81%97%E3%81%A6%E9%81%B7%E7%A7%BB%E3%81%95%E3%81%9B%E3%82%8B) を参照してください。

なお、仮に再利用できるようにした場合、ライフサイクルをユーザ自身が管理する必要が出てきます。  
つまり不要になった際に Cleanup メソッドを呼び、インスタンスを破棄してメモリを綺麗にする必要があります。

## ライセンス
本ソフトウェアはMITライセンスで公開しています。  
ライセンスの範囲内で自由に使っていただけますが、著作権表示とライセンス表示が必須となります。

* https://github.com/Haruma-K/UnityScreenNavigator/blob/master/LICENSE.md
