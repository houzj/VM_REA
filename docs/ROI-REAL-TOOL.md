# ROI source, controls and real execution

This delivery is source-guided reconstruction with differential fixtures; it is not complete native parity.

## Source and implementation

`roi-source-inventory.json`, `roi-style-assets.json` and `roi-runtime-evidence.json` record original templates, assets and source hashes. Original evidence is local under analysis/decompiled and analysis/full-audit. `CPreproMaskTool.SetImgDataBySingleAnnularSector`, `SetImgDataBySingleCircle`, `SetImgDataBySinglePolygon`, `SetImgDataBySingleRect` and `CMvdAnnularSectorF.GetBoundingRect` establish raster semantics. VisionFramework/RoiRasterizer.cs implements those semantics using safe managed buffers, including single precision, truncation, scanline endpoint rules and the tangent-row pointer behavior. Continuous geometric membership is not substituted for algorithm rasterization.

VisionUi includes shape-specific templates for rectangle variants, polygon vertices, ring, sector/720-degree sector, point, line, line/circle/sector calipers, matrix circles, coordinate and numeric Box3D data. The source Box3D selector has no 2D drawing branch, so no invented projection is provided. Line-family creation uses click/move/click; radial creation uses press/drag/release. Numeric fields, clockwise/large-arc choices, handles, keyboard movement, copy/delete and commit events update ROI documents. Pixel editors preserve normalized storage and rescale with image dimensions; Fixture requires a transform and is explicitly rejected.

The shared editor keeps the source 380px parameter pane, selector icon resources/order and numeric widths. ImageBinary full debug composition includes parameter/result tabs, image selector and zoom toolbar, actual input subscription picker, reset/single/continuous/stop, result data and bounded history. Source BottomButton template and footer placement are retained. Group ROI opens the dedicated 1000x600 confirmation frame and commits to real related tool targets.

## Actual algorithm and integration

Independent ImageBinary executes MONO8 hard interval threshold with inclusive bounds, ROI union, polygon exclusions, shape/input-mask selection, outside zero/source and optional OutMask. Unsupported numerical modes are disabled and fail explicitly. This is an actual consumer, not a test-only adapter. The existing ImageSource implementation and other worktree were not changed.

A separate original-SDK test probe supplies 120 sector/ring cases at 16/64 sizes, fractional/integer coordinates, different inner radii and angular ranges. All 120 comparisons have zero differing pixels. This resolves the earlier 11-pixel sector fixture discrepancy; it does not establish every possible native input. Product assemblies do not reference original SDK binaries. Raw native probe outputs are local and excluded from Git.

Actual main-shell tests bind a file-image node to the binary tool, open its real editor factory, select an input subscription, edit a 400x400 ROI and execute 160000 pixels. Group tests verify that ROI proxy editing changes algorithm output and undo restores it. Test code captures screenshots; no capture/export helper is present in product assemblies.

## Explicit remaining gaps

Native display-object eye/color/opacity/line-width popup is not fully reconstructed (current display page only toggles ROI visibility). Fixture/inheritance and inherited blind-polygon paths remain incomplete. Other threshold numerical modes and native C++ engine internals are not recovered. Some auxiliary ROI markers, full original multi-selection behavior and complete slider/combobox/scrollbar templates still need source/runtime acceptance. PolygonCaliper parameter mapping has less evidence than the other listed templates. Original VM side-by-side visual acceptance remains separate from automated checks.

Rendering throttle (100ms) and history bound (256 rows) are independent implementation policies, not claimed native constants. Passing checks proves the exercised paths only. See local HTML report for images, actual check logs and remaining gates.
