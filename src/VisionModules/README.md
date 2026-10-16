# Independent built-in modules
Register with `BuiltinModules.RegisterAll(registry)`. All contracts are version 1.0.0. Factories create a fresh instance for every call. Image memory is immutable through GrayImage.

- vision.image-file: required text Path resolved relative to context BaseDirectory. BI_RGB BMP 8/24/32-bit. Image: GrayImage output.
- vision.threshold: Image input/output. Required integer Threshold 0..255. Foreground includes equality.
- vision.components: Image input, Components and integer Count outputs. Nonzero foreground, 8-connectivity.
- vision.circle: Image input; required CenterX, CenterY, InitialRadius, SearchHalfWidth, SampleCount, GradientThreshold, Polarity, RadialStep, GradientHalfSpan, MinimumEdges. Coordinates and lengths are pixels. Polarity is DarkToLight/LightToDark/Both, case sensitive. Radius must exceed SearchHalfWidth + GradientHalfSpan; positive widths, step, span and gradient; minimum edges cannot exceed sample count; sampling budget is capped by VisionCore. Annular radial strongest-edge search with bilinear sampling and algebraic fitting. Found is always emitted for valid execution; Circle and Radius appear only on success. Insufficient/degenerate edges are NoMatch, invalid options are Failed.
- logic.number-compare: Number input (finite double), Passed boolean output. Required Operator (==, !=, >, >=, <, <=), Reference (finite double). Equality is exact.

Cancellation is checked before and after synchronous bounded algorithms; there is currently no mid-algorithm cancellation. No vendor DLL or vendor-equivalence claim.
