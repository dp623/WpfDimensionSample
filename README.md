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

## 真円付き四角形

`四角形 + 真円`パターンを追加しています。

入力項目:

- 四角形 幅
- 四角形 高さ
- 真円 半径
- 真円 中心X
- 真円 中心Y

真円は中心座標と半径で描画します。
中心X、中心Yは四角形の左上を原点とした位置です。

入力欄はこのパターンだけ3行表示にしています。

```text
四角形      [   ] W x [   ] H
真円　半径  [   ]
真円　位置  [✓] X[   ] x Y[   ]
```

パターンごとの入力レイアウトは `InputRow` と `InputSlot` で定義できます。

`InputRow`の追加引数で、行ラベルの横にチェックボックスを表示できます。
チェックが外れると、その行に含まれるすべてのテキストボックスは非活性になります。

```csharp
new InputRow(
    "真円　位置",
    slots,
    isVisible: true,
    isCheckBoxVisible: true,
    isChecked: true)
```

`真円入り矩形 + 矩形 + 台形`パターンも追加しています。
上から順に、真円を内包した矩形、通常の矩形、台形を縦に配置します。
真円の半径と位置、各図形の寸法、図形間隔を入力項目で変更できます。

このパターンは、次のように部品を積み上げるだけで定義できます。

```csharp
return new VerticalCompositePatternBuilder("真円入り矩形 + 矩形 + 台形", 25)
    .AddRectangleWithCircle("R1", "真円入り矩形", 180, 130, 30, 90, 65)
    .AddRectangle("R2", "矩形", 150, 80)
    .AddTrapezoid("T1", "台形", 110, 170, 90)
    .Build();
```

`AddRectangle()`、`AddTrapezoid()`、`AddRectangleWithCircle()`は、それぞれの部品に必要な複数入力を1行表示する入力レイアウトも自動生成します。

`AddRectangleWithCircle()`では、次の派生項目も表示します。

- 円と矩形の左辺との距離
- 円と矩形の上辺との距離

これらは表示専用の非活性入力欄です。
半径、中心X、中心Yが変更されると自動で再計算され、寸法線とラベルにも反映されます。

## 入力項目・寸法線・ラベルの表示制御

`DimensionDisplay`で、入力項目、寸法線、ラベルを個別に表示または非表示にできます。

四角形の幅を固定値として非表示にし、高さだけを変更可能にする例:

```csharp
return new VerticalCompositePatternBuilder("四角形（高さのみ変更）", 0)
    .AddRectangle(
        "R1",
        "四角形",
        160,
        100,
        DimensionDisplay.Hidden(),
        DimensionDisplay.Visible())
    .Build();
```

個別指定も可能です。

```csharp
new DimensionDisplay(
    isInputVisible: false,
    isLineVisible: true,
    isLabelVisible: false)
```

## 入力行の並び順

左側の入力行は `SortOrder` の昇順で表示されます。
同じ `SortOrder` の場合は、定義した順序を維持します。

1項目1行の標準レイアウトでは、`DimensionParameter` の `sortOrder` を指定します。

```csharp
new DimensionParameter(
    key: "Width",
    label: "幅",
    initialValue: 180,
    sortOrder: 20)
```

複数入力を1行にまとめる場合は、`InputRow` の `sortOrder` を指定します。

```csharp
new InputRow(
    "真円　位置",
    slots,
    isVisible: true,
    isCheckBoxVisible: true,
    isChecked: true,
    sortOrder: 30)
```

`VerticalCompositePatternBuilder`で生成される入力行には、部品の追加順に応じて自動で `SortOrder` が設定されます。
