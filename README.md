# WPF Dimension Sample (.NET Framework)

テキストボックスの値と図形の寸法を連携するWPFサンプルです。
外部パッケージは使用していません。

## 対象環境

- .NET Framework 4.8
- WPF
- C# 7.3
- Visual Studio 2019以降

## 実行

Visual Studioで `WpfDimensionSample.csproj` を開いて実行します。

コマンドラインからビルドする場合:

```powershell
msbuild .\WpfDimensionSample.csproj /t:Build /p:Configuration=Debug
```

## 含まれる機能

- コンボボックスによる図形パターン切り替え
- パターンに応じた入力欄の動的生成
- 入力値に連動する図形、寸法線、矢印、ラベルの描画
- 数値以外、0以下の入力に対する検証エラー表示
- 実寸値と画面上の描画倍率の分離

## パターン追加

`ViewModels/MainViewModel.cs` に `ShapePattern` を追加します。
入力項目は `DimensionParameter`、描画内容は `BuildDrawing` で定義します。
