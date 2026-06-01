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

## 構成

- `ViewModels/MainViewModel.cs`
  - 選択されたパターンの管理と再描画のみを担当します。
- `Patterns/ShapePatternCatalog.cs`
  - コンボボックスへ表示するパターン一覧を定義します。
- `Patterns/VerticalCompositePatternBuilder.cs`
  - 四角形や台形を縦並びにする複合パターンを生成します。
- `Controls/DimensionDrawingView.cs`
  - 図形、寸法線、矢印、ラベルを描画します。

## 複合パターン追加

縦並びの複合パターンは、`ShapePatternCatalog.cs`へ次のように追加できます。

```csharp
return new VerticalCompositePatternBuilder("四角形 x4 + 台形 x2", 25)
    .AddRectangle("R1", "四角形1", 80, 70)
    .AddRectangle("R2", "四角形2", 90, 85)
    .AddRectangle("R3", "四角形3", 100, 95)
    .AddRectangle("R4", "四角形4", 110, 105)
    .AddTrapezoid("T1", "台形1", 70, 120, 100)
    .AddTrapezoid("T2", "台形2", 85, 135, 115)
    .Build();
```

`VerticalCompositePatternBuilder`は入力項目、点列、寸法線を自動生成します。
段付き形状のような特殊な輪郭は、`ShapePatternCatalog.cs`で個別に定義します。
