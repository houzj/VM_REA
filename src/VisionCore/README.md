# VisionCore

Independent net10.0 baseline with no third-party packages or vendor DLL references. GrayImage copies row-major 8-bit pixels. Threshold uses inclusive >= and emits 0/255. ConnectedComponents8 treats nonzero as foreground, includes diagonal neighbors, returns pixel area, inclusive bounds and arithmetic centroid in scan discovery order. FitCircle uses normalized algebraic least squares, rejects singular/ill-conditioned point sets, and reports radial RMS residual.

This is newly implemented mathematics, not recovered VisionMaster code. VisionMaster numeric equivalence, file formats, ROI conventions, calipers, preprocessing, robust outlier rejection, camera integration and SDK binary compatibility are unverified/unimplemented. Circle fitting receives points; it does not detect circle edges in an image. Almost-collinear sets can be rejected by the fixed numeric condition threshold. Algorithms allocate managed buffers and are not optimized for industrial throughput.

Run checks: dotnet run --project tests/VisionCore.Checks/VisionCore.Checks.csproj

## Independent radial circle detection

`RadialCircleDetector.Detect(image, options)` searches an annulus around an explicit initial center and radius. `RadialCircleOptions` takes center X/Y, initial radius, search half width, angular sample count, gradient threshold, polarity, radial step, gradient half span, and minimum edge count. All lengths are pixels and angles are uniformly spaced over a full circle. The gradient is the bilinear intensity at radius + half span minus the intensity at radius - half span (not divided by distance). DarkToLight means increasing intensity outward; LightToDark means decreasing intensity outward. Both uses magnitude. The strongest qualifying edge on each ray is selected; equal strengths choose the smaller radius. No subpixel peak interpolation or vendor filter equivalence is claimed.

The result contains `Status`, nullable `Circle`, read-only `EdgePoints`, and `OutOfBoundsSamples` (number of rays with no valid gradient pair anywhere in the search interval). Positions outside the image are skipped, never padded or clamped. A partially visible circle can succeed if enough observed points permit a fit. Flat images, wrong polarity, insufficient edges and degenerate edge geometry do not produce a successful circle. Parameters reject nonfinite values, zero/negative scales, a nonpositive inner sampling radius, invalid polarity, thresholds outside (0,255], and excessive sampling work (at most 5 million gradient evaluations).

The final fit uses the existing algebraic `Algorithms.FitCircle`; this baseline does not reject outliers, enforce angular coverage, or guarantee equivalence with VisionMaster. Success establishes a fit to the observed selected edges, not correspondence to a vendor algorithm or the intended physical object. `GradientHalfSpan` is an independent sampling distance and must not be identified with vendor `EdgeWidth`.
