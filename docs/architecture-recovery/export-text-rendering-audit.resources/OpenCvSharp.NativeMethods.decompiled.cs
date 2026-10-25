using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using OpenCvSharp.Flann;
using OpenCvSharp.ML;
using OpenCvSharp.Tracking;
using OpenCvSharp.Util;

namespace OpenCvSharp;

[SuppressUnmanagedCodeSecurity]
public static class NativeMethods
{
	private static bool tried;

	public const string DllExtern = "OpenCvSharpExtern";

	public const string Version = "400";

	private static readonly string[] RuntimeDllNames;

	private static readonly string[] OpenCVDllNames;

	public const string DllFfmpegX86 = "opencv_ffmpeg400";

	public const string DllFfmpegX64 = "opencv_ffmpeg400_64";

	public static readonly CvErrorCallback ErrorHandlerThrowException;

	public static readonly CvErrorCallback ErrorHandlerIgnorance;

	public static CvErrorCallback ErrorHandlerDefault;

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_Rodrigues(IntPtr src, IntPtr dst, IntPtr jacobian);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_Rodrigues_VecToMat(IntPtr vector, IntPtr matrix, IntPtr jacobian);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_Rodrigues_MatToVec(IntPtr vector, IntPtr matrix, IntPtr jacobian);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_findHomography_InputArray(IntPtr srcPoints, IntPtr dstPoints, int method, double ransacReprojThreshold, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_findHomography_vector(Point2d[] srcPoints, int srcPointsLength, Point2d[] dstPoints, int dstPointsLength, int method, double ransacReprojThreshold, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_RQDecomp3x3_InputArray(IntPtr src, IntPtr mtxR, IntPtr mtxQ, IntPtr qx, IntPtr qy, IntPtr qz, out Vec3d outVal);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_RQDecomp3x3_Mat(IntPtr src, IntPtr mtxR, IntPtr mtxQ, IntPtr qx, IntPtr qy, IntPtr qz, out Vec3d outVal);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_decomposeProjectionMatrix_InputArray(IntPtr projMatrix, IntPtr cameraMatrix, IntPtr rotMatrix, IntPtr transVect, IntPtr rotMatrixX, IntPtr rotMatrixY, IntPtr rotMatrixZ, IntPtr eulerAngles);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_decomposeProjectionMatrix_Mat(IntPtr projMatrix, IntPtr cameraMatrix, IntPtr rotMatrix, IntPtr transVect, IntPtr rotMatrixX, IntPtr rotMatrixY, IntPtr rotMatrixZ, IntPtr eulerAngles);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_matMulDeriv(IntPtr a, IntPtr b, IntPtr dABdA, IntPtr dABdB);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_composeRT_InputArray(IntPtr rvec1, IntPtr tvec1, IntPtr rvec2, IntPtr tvec2, IntPtr rvec3, IntPtr tvec3, IntPtr dr3dr1, IntPtr dr3dt1, IntPtr dr3dr2, IntPtr dr3dt2, IntPtr dt3dr1, IntPtr dt3dt1, IntPtr dt3dr2, IntPtr dt3dt2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_composeRT_Mat(IntPtr rvec1, IntPtr tvec1, IntPtr rvec2, IntPtr tvec2, IntPtr rvec3, IntPtr tvec3, IntPtr dr3dr1, IntPtr dr3dt1, IntPtr dr3dr2, IntPtr dr3dt2, IntPtr dt3dr1, IntPtr dt3dt1, IntPtr dt3dr2, IntPtr dt3dt2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_projectPoints_InputArray(IntPtr objectPoints, IntPtr rvec, IntPtr tvec, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr imagePoints, IntPtr jacobian, double aspectRatio);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_projectPoints_Mat(IntPtr objectPoints, IntPtr rvec, IntPtr tvec, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr imagePoints, IntPtr jacobian, double aspectRatio);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_solvePnP_InputArray(IntPtr selfectPoints, IntPtr imagePoints, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr rvec, IntPtr tvec, int useExtrinsicGuess, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_solvePnP_vector(Point3f[] objectPoints, int objectPointsLength, Point2f[] imagePoints, int imagePointsLength, double[,] cameraMatrix, double[] distCoeffs, int distCoeffsLength, [Out] double[] rvec, [Out] double[] tvec, int useExtrinsicGuess, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_solvePnPRansac_InputArray(IntPtr objectPoints, IntPtr imagePoints, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr rvec, IntPtr tvec, int useExtrinsicGuess, int iterationsCount, float reprojectionError, double confidence, IntPtr inliers, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_solvePnPRansac_vector(Point3f[] objectPoints, int objectPointsLength, Point2f[] imagePoints, int imagePointsLength, double[,] cameraMatrix, double[] distCoeffs, int distCoeffsLength, [Out] double[] rvec, [Out] double[] tvec, int useExtrinsicGuess, int iterationsCount, float reprojectionError, double confidence, IntPtr inliers, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_initCameraMatrix2D_Mat(IntPtr[] objectPoints, int objectPointsLength, IntPtr[] imagePoints, int imagePointsLength, Size imageSize, double aspectRatio);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_initCameraMatrix2D_array(IntPtr[] objectPoints, int opSize1, int[] opSize2, IntPtr[] imagePoints, int ipSize1, int[] ipSize2, Size imageSize, double aspectRatio);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_findChessboardCorners_InputArray(IntPtr image, Size patternSize, IntPtr corners, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_findChessboardCorners_vector(IntPtr image, Size patternSize, IntPtr corners, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_checkChessboard(IntPtr img, Size size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_findChessboardCornersSB_OutputArray(IntPtr image, Size patternSize, IntPtr corners, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_findChessboardCornersSB_vector(IntPtr image, Size patternSize, IntPtr corners, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_find4QuadCornerSubpix_InputArray(IntPtr img, IntPtr corners, Size regionSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_find4QuadCornerSubpix_vector(IntPtr img, IntPtr corners, Size regionSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_drawChessboardCorners_InputArray(IntPtr image, Size patternSize, IntPtr corners, int patternWasFound);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_drawChessboardCorners_array(IntPtr image, Size patternSize, Point2f[] corners, int cornersLength, int patternWasFound);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_drawFrameAxes(IntPtr image, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr rvec, IntPtr tvec, float length, int thickness);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_findCirclesGrid_InputArray(IntPtr image, Size patternSize, IntPtr centers, int flags, IntPtr blobDetector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_findCirclesGrid_vector(IntPtr image, Size patternSize, IntPtr centers, int flags, IntPtr blobDetector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_calibrateCamera_InputArray(IntPtr[] objectPoints, int objectPointsSize, IntPtr[] imagePoints, int imagePointsSize, Size imageSize, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr rvecs, IntPtr tvecs, int flags, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_calibrateCamera_vector(IntPtr[] objectPoints, int opSize1, int[] opSize2, IntPtr[] imagePoints, int ipSize1, int[] ipSize2, Size imageSize, [In][Out] double[,] cameraMatrix, [In][Out] double[] distCoeffs, int distCoeffsSize, IntPtr rvecs, IntPtr tvecs, int flags, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_calibrationMatrixValues_InputArray(IntPtr cameraMatrix, Size imageSize, double apertureWidth, double apertureHeight, out double fovx, out double fovy, out double focalLength, out Point2d principalPoint, out double aspectRatio);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_calibrationMatrixValues_array(double[,] cameraMatrix, Size imageSize, double apertureWidth, double apertureHeight, out double fovx, out double fovy, out double focalLength, out Point2d principalPoint, out double aspectRatio);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_stereoCalibrate_InputArray(IntPtr[] objectPoints, int opSize, IntPtr[] imagePoints1, int ip1Size, IntPtr[] imagePoints2, int ip2Size, IntPtr cameraMatrix1, IntPtr distCoeffs1, IntPtr cameraMatrix2, IntPtr distCoeffs2, Size imageSize, IntPtr R, IntPtr T, IntPtr E, IntPtr F, int flags, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_stereoCalibrate_array(IntPtr[] objectPoints, int opSize1, int[] opSizes2, IntPtr[] imagePoints1, int ip1Size1, int[] ip1Sizes2, IntPtr[] imagePoints2, int ip2Size1, int[] ip2Sizes2, [In][Out] double[,] cameraMatrix1, [In][Out] double[] distCoeffs1, int dc1Size, [In][Out] double[,] cameraMatrix2, [In][Out] double[] distCoeffs2, int dc2Size, Size imageSize, IntPtr R, IntPtr T, IntPtr E, IntPtr F, int flags, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_stereoRectify_InputArray(IntPtr cameraMatrix1, IntPtr distCoeffs1, IntPtr cameraMatrix2, IntPtr distCoeffs2, Size imageSize, IntPtr R, IntPtr T, IntPtr R1, IntPtr R2, IntPtr P1, IntPtr P2, IntPtr Q, int flags, double alpha, Size newImageSize, out Rect validPixROI1, out Rect validPixROI2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_stereoRectify_array(double[,] cameraMatrix1, double[] distCoeffs1, int dc1Size, double[,] cameraMatrix2, double[] distCoeffs2, int dc2Size, Size imageSize, double[,] R, double[] T, double[,] R1, double[,] R2, double[,] P1, double[,] P2, double[,] Q, int flags, double alpha, Size newImageSize, out Rect validPixROI1, out Rect validPixROI2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_stereoRectifyUncalibrated_InputArray(IntPtr points1, IntPtr points2, IntPtr F, Size imgSize, IntPtr H1, IntPtr H2, double threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_stereoRectifyUncalibrated_array(Point2d[] points1, int points1Size, Point2d[] points2, int points2Size, [In] double[,] F, Size imgSize, [In][Out] double[,] H1, [In][Out] double[,] H2, double threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float calib3d_rectify3Collinear_InputArray(IntPtr cameraMatrix1, IntPtr distCoeffs1, IntPtr cameraMatrix2, IntPtr distCoeffs2, IntPtr cameraMatrix3, IntPtr distCoeffs3, IntPtr[] imgpt1, int imgpt1Size, IntPtr[] imgpt3, int imgpt3Size, Size imageSize, IntPtr R12, IntPtr T12, IntPtr R13, IntPtr T13, IntPtr R1, IntPtr R2, IntPtr R3, IntPtr P1, IntPtr P2, IntPtr P3, IntPtr Q, double alpha, Size newImgSize, out Rect roi1, out Rect roi2, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_getOptimalNewCameraMatrix_InputArray(IntPtr cameraMatrix, IntPtr distCoeffs, Size imageSize, double alpha, Size newImgSize, out Rect validPixROI, int centerPrincipalPoint);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_getOptimalNewCameraMatrix_array([In] double[,] cameraMatrix, [In] double[] distCoeffs, int distCoeffsSize, Size imageSize, double alpha, Size newImgSize, out Rect validPixROI, int centerPrincipalPoint);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsToHomogeneous_InputArray(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsToHomogeneous_array1([In] Vec2f[] src, [In][Out] Vec3f[] dst, int length);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsToHomogeneous_array2([In] Vec3f[] src, [In][Out] Vec4f[] dst, int length);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsFromHomogeneous_InputArray(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsFromHomogeneous_array1([In] Vec3f[] src, [In][Out] Vec2f[] dst, int length);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsFromHomogeneous_array2([In] Vec4f[] src, [In][Out] Vec3f[] dst, int length);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_convertPointsHomogeneous(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_findFundamentalMat_InputArray(IntPtr points1, IntPtr points2, int method, double param1, double param2, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_findFundamentalMat_array(Point2d[] points1, int points1Size, Point2d[] points2, int points2Size, int method, double param1, double param2, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_computeCorrespondEpilines_InputArray(IntPtr points, int whichImage, IntPtr F, IntPtr lines);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_computeCorrespondEpilines_array2d([In] Point2d[] points, int pointsSize, int whichImage, double[,] F, [In][Out] Point3f[] lines);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_computeCorrespondEpilines_array3d([In] Point3d[] points, int pointsSize, int whichImage, double[,] F, [In][Out] Point3f[] lines);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_triangulatePoints_InputArray(IntPtr projMatr1, IntPtr projMatr2, IntPtr projPoints1, IntPtr projPoints2, IntPtr points4D);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_triangulatePoints_array([In] double[,] projMatr1, [In] double[,] projMatr2, [In] Point2d[] projPoints1, int projPoints1Size, [In] Point2d[] projPoints2, int projPoints2Size, [In][Out] Vec4d[] points4D);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_correctMatches_InputArray(IntPtr F, IntPtr points1, IntPtr points2, IntPtr newPoints1, IntPtr newPoints2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_correctMatches_array(double[,] F, Point2d[] points1, int points1Size, Point2d[] points2, int points2Size, Point2d[] newPoints1, Point2d[] newPoints2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_filterSpeckles(IntPtr img, double newVal, int maxSpeckleSize, double maxDiff, IntPtr buf);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect calib3d_getValidDisparityROI(Rect roi1, Rect roi2, int minDisparity, int numberOfDisparities, int SADWindowSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_validateDisparity(IntPtr disparity, IntPtr cost, int minDisparity, int numberOfDisparities, int disp12MaxDisp);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_reprojectImageTo3D(IntPtr disparity, IntPtr _3dImage, IntPtr Q, int handleMissingValues, int ddepth);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_estimateAffine3D(IntPtr src, IntPtr dst, IntPtr outVal, IntPtr inliers, double ransacThreshold, double confidence);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_sampsonDistance_InputArray(IntPtr pt1, IntPtr pt2, IntPtr F);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_sampsonDistance_Point3d(Point3d pt1, Point3d pt2, [MarshalAs(UnmanagedType.LPArray)] double[,] F);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_estimateAffine2D(IntPtr from, IntPtr to, IntPtr inliers, int method, double ransacReprojThreshold, ulong maxIters, double confidence, ulong refineIters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_estimateAffinePartial2D(IntPtr from, IntPtr to, IntPtr inliers, int method, double ransacReprojThreshold, ulong maxIters, double confidence, ulong refineIters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_decomposeHomographyMat(IntPtr H, IntPtr K, IntPtr rotations, IntPtr translations, IntPtr normals);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_filterHomographyDecompByVisibleRefpoints(IntPtr rotations, IntPtr normals, IntPtr beforePoints, IntPtr afterPoints, IntPtr possibleSolutions, IntPtr pointsMask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_undistort(IntPtr src, IntPtr dst, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr newCameraMatrix);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_initUndistortRectifyMap(IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr R, IntPtr newCameraMatrix, Size size, int m1type, IntPtr map1, IntPtr map2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float calib3d_initWideAngleProjMap(IntPtr cameraMatrix, IntPtr distCoeffs, Size imageSize, int destImageWidth, int m1type, IntPtr map1, IntPtr map2, int projType, double alpha);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_getDefaultNewCameraMatrix(IntPtr cameraMatrix, Size imgsize, int centerPrincipalPoint);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_undistortPoints(IntPtr src, IntPtr dst, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr R, IntPtr P);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_undistortPointsIter(IntPtr src, IntPtr dst, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr R, IntPtr P, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_recoverPose_InputArray1(IntPtr E, IntPtr points1, IntPtr points2, IntPtr cameraMatrix, IntPtr R, IntPtr P, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_recoverPose_InputArray2(IntPtr E, IntPtr points1, IntPtr points2, IntPtr R, IntPtr P, double focal, IntPtr pp, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_recoverPose_InputArray3(IntPtr E, IntPtr points1, IntPtr points2, IntPtr cameraMatrix, IntPtr R, IntPtr P, double distanceTresh, IntPtr mask, IntPtr triangulatedPoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_findEssentialMat_InputArray1(IntPtr points1, IntPtr points2, IntPtr cameraMatrix, int method, double prob, double threshold, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_findEssentialMat_InputArray2(IntPtr points1, IntPtr points2, double focal, IntPtr pp, int method, double prob, double threshold, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_projectPoints1(IntPtr objectPoints, IntPtr imagePoints, IntPtr affine, IntPtr K, IntPtr D, double alpha, IntPtr jacobian);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_projectPoints2(IntPtr objectPoints, IntPtr imagePoints, IntPtr rvec, IntPtr tvec, IntPtr K, IntPtr D, double alpha, IntPtr jacobian);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_distortPoints(IntPtr undistorted, IntPtr distorted, IntPtr K, IntPtr D, double alpha);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_undistortPoints(IntPtr distorted, IntPtr undistorted, IntPtr K, IntPtr D, IntPtr R, IntPtr P);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_initUndistortRectifyMap(IntPtr K, IntPtr D, IntPtr R, IntPtr P, Size size, int m1type, IntPtr map1, IntPtr map2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_undistortImage(IntPtr distorted, IntPtr undistorted, IntPtr K, IntPtr D, IntPtr Knew, Size newSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_estimateNewCameraMatrixForUndistortRectify(IntPtr K, IntPtr D, Size image_size, IntPtr R, IntPtr P, double balance, Size newSize, double fov_scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_fisheye_calibrate(IntPtr objectPoints, IntPtr imagePoints, Size imageSize, IntPtr K, IntPtr D, IntPtr rvecs, IntPtr tvecs, int flags, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_fisheye_stereoRectify(IntPtr K1, IntPtr D1, IntPtr K2, IntPtr D2, Size imageSize, IntPtr R, IntPtr tvec, IntPtr R1, IntPtr R2, IntPtr P1, IntPtr P2, IntPtr Q, int flags, Size newImageSize, double balance, double fov_scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double calib3d_fisheye_stereoCalibrate(IntPtr objectPoints, IntPtr imagePoints1, IntPtr imagePoints2, IntPtr K1, IntPtr D1, IntPtr K2, IntPtr D2, Size imageSize, IntPtr R, IntPtr T, int flags, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_Ptr_StereoBM_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_Ptr_StereoBM_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_StereoBM_create(int numDisparities, int blockSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoBM_getPreFilterType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setPreFilterType(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoBM_getPreFilterSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setPreFilterSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoBM_getPreFilterCap(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setPreFilterCap(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoBM_getTextureThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setTextureThreshold(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoBM_getUniquenessRatio(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setUniquenessRatio(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoBM_getSmallerBlockSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setSmallerBlockSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect calib3d_StereoBM_getROI1(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setROI1(IntPtr obj, Rect value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect calib3d_StereoBM_getROI2(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoBM_setROI2(IntPtr obj, Rect value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_compute(IntPtr obj, IntPtr left, IntPtr right, IntPtr disparity);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoMatcher_getMinDisparity(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_setMinDisparity(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoMatcher_getNumDisparities(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_setNumDisparities(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoMatcher_getBlockSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_setBlockSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoMatcher_getSpeckleWindowSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_setSpeckleWindowSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoMatcher_getSpeckleRange(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_setSpeckleRange(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoMatcher_getDisp12MaxDiff(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoMatcher_setDisp12MaxDiff(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_Ptr_StereoSGBM_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_Ptr_StereoSGBM_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr calib3d_StereoSGBM_create(int minDisparity, int numDisparities, int blockSize, int P1, int P2, int disp12MaxDiff, int preFilterCap, int uniquenessRatio, int speckleWindowSize, int speckleRange, int mode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoSGBM_getPreFilterCap(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoSGBM_setPreFilterCap(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoSGBM_getUniquenessRatio(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoSGBM_setUniquenessRatio(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoSGBM_getP1(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoSGBM_setP1(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoSGBM_getP2(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoSGBM_setP2(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int calib3d_StereoSGBM_getMode(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void calib3d_StereoSGBM_setMode(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_setNumThreads(int nthreads);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getNumThreads();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getThreadNum();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_getBuildInformation([MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int maxLength);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int core_getBuildInformation_length();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_getVersionString([MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int maxLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getVersionMajor();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getVersionMinor();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getVersionRevision();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern long core_getTickCount();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_getTickFrequency();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern long core_getCPUTickCount();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_checkHardwareSupport(int feature);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_getHardwareFeatureName(int feature, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_getCPUFeaturesLine([MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getNumberOfCPUs();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_fastMalloc(IntPtr bufSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_fastFree(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_setUseOptimized(int onoff);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_useOptimized();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_glob([MarshalAs(UnmanagedType.LPStr)] string pattern, IntPtr result, int recursive);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_setBreakOnError(int flag);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr redirectError(CvErrorCallback errCallback, IntPtr userdata, ref IntPtr prevUserdata);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public unsafe static extern sbyte* core_format(IntPtr mtx, int fmt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern void core_char_delete(sbyte* buf);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_add(IntPtr src1, IntPtr src2, IntPtr dst, IntPtr mask, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_subtract_InputArray2(IntPtr src1, IntPtr src2, IntPtr dst, IntPtr mask, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_subtract_InputArrayScalar(IntPtr src1, Scalar src2, IntPtr dst, IntPtr mask, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_subtract_ScalarInputArray(Scalar src1, IntPtr src2, IntPtr dst, IntPtr mask, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_multiply(IntPtr src1, IntPtr src2, IntPtr dst, double scale, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_divide1(double scale, IntPtr src2, IntPtr dst, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_divide2(IntPtr src1, IntPtr src2, IntPtr dst, double scale, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_scaleAdd(IntPtr src1, double alpha, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_addWeighted(IntPtr src1, double alpha, IntPtr src2, double beta, double gamma, IntPtr dst, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_borderInterpolate(int p, int len, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_copyMakeBorder(IntPtr src, IntPtr dst, int top, int bottom, int left, int right, int borderType, Scalar value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_convertScaleAbs(IntPtr src, IntPtr dst, double alpha, double beta);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LUT(IntPtr src, IntPtr lut, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Scalar core_sum(IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_countNonZero(IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_findNonZero(IntPtr src, IntPtr idx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Scalar core_mean(IntPtr src, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_meanStdDev_OutputArray(IntPtr src, IntPtr mean, IntPtr stddev, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_meanStdDev_Scalar(IntPtr src, out Scalar mean, out Scalar stddev, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_norm1(IntPtr src1, int normType, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_norm2(IntPtr src1, IntPtr src2, int normType, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_batchDistance(IntPtr src1, IntPtr src2, IntPtr dist, int dtype, IntPtr nidx, int normType, int k, IntPtr mask, int update, int crosscheck);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_normalize(IntPtr src, IntPtr dst, double alpha, double beta, int normType, int dtype, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_minMaxLoc1(IntPtr src, out double minVal, out double maxVal);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_minMaxLoc2(IntPtr src, out double minVal, out double maxVal, out Point minLoc, out Point maxLoc, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_minMaxIdx1(IntPtr src, out double minVal, out double maxVal);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_minMaxIdx2(IntPtr src, out double minVal, out double maxVal, [MarshalAs(UnmanagedType.LPArray)] int[] minIdx, [MarshalAs(UnmanagedType.LPArray)] int[] maxIdx, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_reduce(IntPtr src, IntPtr dst, int dim, int rtype, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_merge([MarshalAs(UnmanagedType.LPArray)] IntPtr[] mv, uint count, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_split(IntPtr src, out IntPtr mv);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_mixChannels(IntPtr[] src, uint nsrcs, IntPtr[] dst, uint ndsts, int[] fromTo, uint npairs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_extractChannel(IntPtr src, IntPtr dst, int coi);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_insertChannel(IntPtr src, IntPtr dst, int coi);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_flip(IntPtr src, IntPtr dst, int flipCode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_repeat1(IntPtr src, int ny, int nx, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_repeat2(IntPtr src, int ny, int nx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_hconcat1([MarshalAs(UnmanagedType.LPArray)] IntPtr[] src, uint nsrc, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_hconcat2(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_vconcat1([MarshalAs(UnmanagedType.LPArray)] IntPtr[] src, uint nsrc, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_vconcat2(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_bitwise_and(IntPtr src1, IntPtr src2, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_bitwise_or(IntPtr src1, IntPtr src2, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_bitwise_xor(IntPtr src1, IntPtr src2, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_bitwise_not(IntPtr src, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_absdiff(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_inRange_InputArray(IntPtr src, IntPtr lowerb, IntPtr upperb, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_inRange_Scalar(IntPtr src, Scalar lowerb, Scalar upperb, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_compare(IntPtr src1, IntPtr src2, IntPtr dst, int cmpop);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_min1(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_max1(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_min_MatMat(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_min_MatDouble(IntPtr src1, double src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_max_MatMat(IntPtr src1, IntPtr src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_max_MatDouble(IntPtr src1, double src2, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_sqrt(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_pow_Mat(IntPtr src, double power, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_exp_Mat(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_log_Mat(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float core_cubeRoot(float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float core_fastAtan2(float y, float x);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_polarToCart(IntPtr magnitude, IntPtr angle, IntPtr x, IntPtr y, int angleInDegrees);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_cartToPolar(IntPtr x, IntPtr y, IntPtr magnitude, IntPtr angle, int angleInDegrees);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_phase(IntPtr x, IntPtr y, IntPtr angle, int angleInDegrees);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_magnitude_Mat(IntPtr x, IntPtr y, IntPtr magnitude);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_checkRange(IntPtr a, int quiet, out Point pos, double minVal, double maxVal);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_patchNaNs(IntPtr a, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_gemm(IntPtr src1, IntPtr src2, double alpha, IntPtr src3, double gamma, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_mulTransposed(IntPtr src, IntPtr dst, int aTa, IntPtr delta, double scale, int dtype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_transpose(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_transform(IntPtr src, IntPtr dst, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_perspectiveTransform(IntPtr src, IntPtr dst, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_perspectiveTransform_Mat(IntPtr src, IntPtr dst, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_perspectiveTransform_Point2f(IntPtr src, int srcLength, IntPtr dst, int dstLength, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_perspectiveTransform_Point2d(IntPtr src, int srcLength, IntPtr dst, int dstLength, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_perspectiveTransform_Point3f(IntPtr src, int srcLength, IntPtr dst, int dstLength, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_perspectiveTransform_Point3d(IntPtr src, int srcLength, IntPtr dst, int dstLength, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_completeSymm(IntPtr mtx, int lowerToUpper);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_setIdentity(IntPtr mtx, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_determinant(IntPtr mtx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Scalar core_trace(IntPtr mtx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_invert(IntPtr src, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_solve(IntPtr src1, IntPtr src2, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_solveLP(IntPtr func, IntPtr constr, IntPtr z);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_sort(IntPtr src, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_sortIdx(IntPtr src, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_solveCubic(IntPtr coeffs, IntPtr roots);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_solvePoly(IntPtr coeffs, IntPtr roots, int maxIters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_eigen(IntPtr src, IntPtr eigenvalues, IntPtr eigenvectors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_calcCovarMatrix_Mat([MarshalAs(UnmanagedType.LPArray)] IntPtr[] samples, int nsamples, IntPtr covar, IntPtr mean, int flags, int ctype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_calcCovarMatrix_InputArray(IntPtr samples, IntPtr covar, IntPtr mean, int flags, int ctype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCACompute(IntPtr data, IntPtr mean, IntPtr eigenvectors, int maxComponents);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCAComputeVar(IntPtr data, IntPtr mean, IntPtr eigenvectors, double retainedVariance);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCAProject(IntPtr data, IntPtr mean, IntPtr eigenvectors, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCABackProject(IntPtr data, IntPtr mean, IntPtr eigenvectors, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVDecomp(IntPtr src, IntPtr w, IntPtr u, IntPtr vt, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVBackSubst(IntPtr w, IntPtr u, IntPtr vt, IntPtr rhs, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_Mahalanobis(IntPtr v1, IntPtr v2, IntPtr icovar);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_dft(IntPtr src, IntPtr dst, int flags, int nonzeroRows);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_idft(IntPtr src, IntPtr dst, int flags, int nonzeroRows);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_dct(IntPtr src, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_idct(IntPtr src, IntPtr dst, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_mulSpectrums(IntPtr a, IntPtr b, IntPtr c, int flags, int conjB);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_getOptimalDFTSize(int vecsize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_kmeans(IntPtr data, int k, IntPtr bestLabels, TermCriteria criteria, int attempts, int flags, IntPtr centers);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_theRNG();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_randu_InputArray(IntPtr dst, IntPtr low, IntPtr high);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_randu_Scalar(IntPtr dst, Scalar low, Scalar high);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_randn_InputArray(IntPtr dst, IntPtr mean, IntPtr stddev);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_randn_Scalar(IntPtr dst, Scalar mean, Scalar stddev);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_randShuffle(IntPtr dst, double iterFactor, ref ulong rng);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_randShuffle(IntPtr dst, double iterFactor, IntPtr rng);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Algorithm_write(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Algorithm_read(IntPtr obj, IntPtr fn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Algorithm_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Algorithm_save(IntPtr obj, string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Algorithm_getDefaultName(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_new2(IntPtr data, IntPtr mean, int flags, int maxComponents);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_new3(IntPtr data, IntPtr mean, int flags, double retainedVariance);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCA_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCA_operatorThis(IntPtr obj, IntPtr data, IntPtr mean, int flags, int maxComponents);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCA_computeVar(IntPtr obj, IntPtr data, IntPtr mean, int flags, double retainedVariance);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_project1(IntPtr obj, IntPtr vec);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCA_project2(IntPtr obj, IntPtr vec, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_backProject1(IntPtr obj, IntPtr vec);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_PCA_backProject2(IntPtr obj, IntPtr vec, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_eigenvectors(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_eigenvalues(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_PCA_mean(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_RNG_fill(ref ulong state, IntPtr mat, int distType, IntPtr a, IntPtr b, int saturateRange);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_RNG_gaussian(ref ulong state, double sigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SVD_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SVD_new2(IntPtr src, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_operatorThis(IntPtr obj, IntPtr src, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_backSubst(IntPtr obj, IntPtr rhs, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_static_compute1(IntPtr src, IntPtr w, IntPtr u, IntPtr vt, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_static_compute2(IntPtr src, IntPtr w, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_static_backSubst(IntPtr w, IntPtr u, IntPtr vt, IntPtr rhs, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SVD_static_solveZ(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SVD_u(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SVD_w(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SVD_vt(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_new1(int numComponents);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_new2(IntPtr src, IntPtr labels, int numComponents);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LDA_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LDA_save_String(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LDA_load_String(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LDA_save_FileStorage(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LDA_load_FileStorage(IntPtr obj, IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_LDA_compute(IntPtr obj, IntPtr src, IntPtr labels);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_project(IntPtr obj, IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_reconstruct(IntPtr obj, IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_eigenvectors(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_eigenvalues(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_subspaceProject(IntPtr w, IntPtr mean, IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_LDA_subspaceReconstruct(IntPtr w, IntPtr mean, IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_new2(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_delete(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_operatorThis_byString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string nodeName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_operatorThis_byInt(IntPtr obj, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_type(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isNone(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isSeq(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isMap(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isInt(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isReal(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isString(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_isNamed(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_name(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_size(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNode_toInt(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float core_FileNode_toFloat(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_FileNode_toDouble(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_toString(IntPtr obj, StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_toMat(IntPtr obj, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_begin(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNode_end(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_readRaw(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string fmt, IntPtr vec, IntPtr len);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_writeComment(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string comment, int append);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_int(IntPtr node, out int value, int defaultValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_float(IntPtr node, out float value, float defaultValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_double(IntPtr node, out double value, double defaultValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_String(IntPtr node, [MarshalAs(UnmanagedType.LPStr)] StringBuilder value, int valueCapacity, [MarshalAs(UnmanagedType.LPStr)] string defaultValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_Mat(IntPtr node, IntPtr mat, IntPtr defaultMat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_SparseMat(IntPtr node, IntPtr mat, IntPtr defaultMat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_vectorOfKeyPoint(IntPtr node, IntPtr keypoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNode_read_vectorOfDMatch(IntPtr node, IntPtr matches);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Range core_FileNode_read_Range(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern KeyPoint core_FileNode_read_KeyPoint(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern DMatch core_FileNode_read_DMatch(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point core_FileNode_read_Point2i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point2f core_FileNode_read_Point2f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point2d core_FileNode_read_Point2d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point3i core_FileNode_read_Point3i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point3f core_FileNode_read_Point3f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point3d core_FileNode_read_Point3d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size core_FileNode_read_Size2i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size2f core_FileNode_read_Size2f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size2d core_FileNode_read_Size2d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect core_FileNode_read_Rect2i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect2f core_FileNode_read_Rect2f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect2d core_FileNode_read_Rect2d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Scalar core_FileNode_read_Scalar(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2i core_FileNode_read_Vec2i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec3i core_FileNode_read_Vec3i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4i core_FileNode_read_Vec4i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec6i core_FileNode_read_Vec6i(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2d core_FileNode_read_Vec2d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec3d core_FileNode_read_Vec3d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4d core_FileNode_read_Vec4d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec6d core_FileNode_read_Vec6d(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2f core_FileNode_read_Vec2f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec3f core_FileNode_read_Vec3f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4f core_FileNode_read_Vec4f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec6f core_FileNode_read_Vec6f(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2b core_FileNode_read_Vec2b(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec3b core_FileNode_read_Vec3b(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4b core_FileNode_read_Vec4b(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec6b core_FileNode_read_Vec6b(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2s core_FileNode_read_Vec2s(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec3s core_FileNode_read_Vec3s(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4s core_FileNode_read_Vec4s(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec6s core_FileNode_read_Vec6s(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2w core_FileNode_read_Vec2w(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec3w core_FileNode_read_Vec3w(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4w core_FileNode_read_Vec4w(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec6w core_FileNode_read_Vec6w(IntPtr node);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNodeIterator_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNodeIterator_new2(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileNodeIterator_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNodeIterator_operatorAsterisk(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNodeIterator_operatorIncrement(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNodeIterator_operatorPlusEqual(IntPtr obj, int ofs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNodeIterator_readRaw(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string fmt, IntPtr vec, IntPtr maxCount);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNodeIterator_readRaw(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string fmt, byte[] vec, IntPtr maxCount);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNodeIterator_operatorEqual(IntPtr it1, IntPtr it2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileNodeIterator_operatorMinus(IntPtr it1, IntPtr it2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileNodeIterator_operatorLessThan(IntPtr it1, IntPtr it2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileStorage_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileStorage_new2([MarshalAs(UnmanagedType.LPStr)] string source, int flags, [MarshalAs(UnmanagedType.LPStr)] string encoding);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileStorage_open(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string filename, int flags, [MarshalAs(UnmanagedType.LPStr)] string encoding);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileStorage_isOpened(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_release(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_releaseAndGetString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileStorage_getFirstTopLevelNode(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileStorage_root(IntPtr obj, int streamIdx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_FileStorage_indexer(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string nodeName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_writeRaw(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string fmt, IntPtr vec, IntPtr len);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_getDefaultObjectName([MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int bufLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern sbyte* core_FileStorage_elname(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_startWriteStruct(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string name, int flags, [MarshalAs(UnmanagedType.LPStr)] string typeName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_endWriteStruct(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_FileStorage_state(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_int(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_float(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_double(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_String(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_Mat(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_SparseMat(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_vectorOfKeyPoint(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_write_vectorOfDMatch(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_writeScalar_int(IntPtr fs, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_writeScalar_float(IntPtr fs, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_writeScalar_double(IntPtr fs, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_writeScalar_String(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_String(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_int(IntPtr fs, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_float(IntPtr fs, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_double(IntPtr fs, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Mat(IntPtr fs, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_SparseMat(IntPtr fs, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Range(IntPtr fs, Range val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_KeyPoint(IntPtr fs, KeyPoint val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_DMatch(IntPtr fs, DMatch val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_vectorOfKeyPoint(IntPtr fs, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_vectorOfDMatch(IntPtr fs, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Point2i(IntPtr fs, Point val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Point2f(IntPtr fs, Point2f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Point2d(IntPtr fs, Point2d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Point3i(IntPtr fs, Point3i val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Point3f(IntPtr fs, Point3f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Point3d(IntPtr fs, Point3d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Size2i(IntPtr fs, Size val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Size2f(IntPtr fs, Size2f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Size2d(IntPtr fs, Size2d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Rect2i(IntPtr fs, Rect val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Rect2f(IntPtr fs, Rect2f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Rect2d(IntPtr fs, Rect2d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Scalar(IntPtr fs, Scalar val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec2i(IntPtr fs, Vec2i val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec3i(IntPtr fs, Vec3i val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec4i(IntPtr fs, Vec4i val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec6i(IntPtr fs, Vec6i val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec2d(IntPtr fs, Vec2d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec3d(IntPtr fs, Vec3d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec4d(IntPtr fs, Vec4d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec6d(IntPtr fs, Vec6d val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec2f(IntPtr fs, Vec2f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec3f(IntPtr fs, Vec3f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec4f(IntPtr fs, Vec4f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec6f(IntPtr fs, Vec6f val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec2b(IntPtr fs, Vec2b val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec3b(IntPtr fs, Vec3b val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec4b(IntPtr fs, Vec4b val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec6b(IntPtr fs, Vec6b val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec2s(IntPtr fs, Vec2s val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec3s(IntPtr fs, Vec3s val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec4s(IntPtr fs, Vec4s val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec6s(IntPtr fs, Vec6s val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec2w(IntPtr fs, Vec2w val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec3w(IntPtr fs, Vec3w val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec4w(IntPtr fs, Vec4w val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_FileStorage_shift_Vec6w(IntPtr fs, Vec6w val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_new_byMat(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_new_byMatExpr(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_new_byScalar(Scalar val, out IntPtr handle);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_new_byDouble(IntPtr valPointer);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_new_byGpuMat(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_new_byVectorOfMat(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_delete(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_delete_withScalar(IntPtr ia, IntPtr handle);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_getMat(IntPtr ia, int idx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_getMat_(IntPtr ia, int idx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_getUMat(IntPtr ia, int idx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_getMatVector(IntPtr ia, IntPtr mv);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_getUMatVector(IntPtr ia, IntPtr umv);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_getGpuMatVector(IntPtr ia, IntPtr gpumv);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_getGpuMat(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_getFlags(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_getObj(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size core_InputArray_getSz(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_kind(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_dims(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_cols(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_rows(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size core_InputArray_size(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_sizend(IntPtr ia, int[] sz, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_sameSize(IntPtr self, IntPtr target);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_total(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_type(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_depth(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_channels(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isContinuous(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isSubmatrix(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_empty(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_copyTo1(IntPtr ia, IntPtr arr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_InputArray_copyTo2(IntPtr ia, IntPtr arr, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_offset(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_InputArray_step(IntPtr ia, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isMat(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isUMat(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isMatVector(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isUMatVector(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isMatx(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isVector(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_InputArray_isGpuMatVector(IntPtr ia);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_sizeof();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new2(int rows, int cols, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new3(int rows, int cols, int type, Scalar scalar);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new4(IntPtr mat, Range rowRange, Range colRange);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new5(IntPtr mat, Range rowRange);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new6(IntPtr mat, [MarshalAs(UnmanagedType.LPArray)] Range[] rowRange);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new7(IntPtr mat, Rect roi);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new8(int rows, int cols, int type, IntPtr data, IntPtr step);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new9(int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sizes, int type, IntPtr data, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] steps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new9(int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sizes, int type, IntPtr data, IntPtr steps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new10(int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sizes, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new11(int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sizes, int type, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new12(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new_FromIplImage(IntPtr img, int copyData);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_new_FromCvMat(IntPtr mat, int copyData);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_release(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_delete(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_adjustROI(IntPtr nativeObj, int dtop, int dbottom, int dleft, int dright);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_assignTo1(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_assignTo2(IntPtr self, IntPtr m, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_channels(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_checkVector1(IntPtr self, int elemChannels);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_checkVector2(IntPtr self, int elemChannels, int depth);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_checkVector3(IntPtr self, int elemChannels, int depth, int requireContinuous);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_clone(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_col_toMat(IntPtr self, int x);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_col_toMatExpr(IntPtr self, int x);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_cols(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_colRange_toMat(IntPtr self, int startCol, int endCol);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_colRange_toMatExpr(IntPtr self, int startCol, int endCol);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_dims(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_convertTo(IntPtr self, IntPtr m, int rtype, double alpha, double beta);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_copyTo(IntPtr self, IntPtr m, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_create1(IntPtr self, int rows, int cols, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_create2(IntPtr self, int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sizes, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_cross(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern byte* core_Mat_data(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_datastart(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_dataend(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_datalimit(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_depth(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_diag1(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_diag2(IntPtr self, int d);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_diag3(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_Mat_dot(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_elemSize(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_elemSize1(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_empty(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_eye(int rows, int cols, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_inv1(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_inv2(IntPtr self, int method);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_isContinuous(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_isSubmatrix(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_locateROI(IntPtr self, out Size wholeSize, out Point ofs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_mul1(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_mul2(IntPtr self, IntPtr m, double scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_ones1(int rows, int cols, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_ones2(int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sz, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_reshape1(IntPtr self, int cn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_reshape2(IntPtr self, int cn, int rows);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_reshape3(IntPtr self, int cn, int newndims, [MarshalAs(UnmanagedType.LPArray)] int[] newsz);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_rows(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_row_toMat(IntPtr self, int y);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_row_toMatExpr(IntPtr self, int y);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_rowRange_toMat(IntPtr self, int startRow, int endRow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_rowRange_toMatExpr(IntPtr self, int startRow, int endRow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_setTo_Scalar(IntPtr self, Scalar value, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_setTo_InputArray(IntPtr self, IntPtr value, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size core_Mat_size(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_sizeAt(IntPtr self, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_step11(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_step12(IntPtr self, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern long core_Mat_step(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_stepAt(IntPtr self, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_subMat1(IntPtr self, int rowStart, int rowEnd, int colStart, int colEnd);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_subMat2(IntPtr self, int nRanges, Range[] ranges);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_t(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_Mat_total(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_Mat_type(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_zeros1(int rows, int cols, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_zeros2(int ndims, [MarshalAs(UnmanagedType.LPArray)] int[] sz, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_ptr1d(IntPtr self, int i0);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_ptr2d(IntPtr self, int i0, int i1);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_ptr3d(IntPtr self, int i0, int i1, int i2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_ptrnd(IntPtr self, [MarshalAs(UnmanagedType.LPArray)] int[] idx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_assignment_FromMat(IntPtr self, IntPtr newMat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_assignment_FromMatExpr(IntPtr self, IntPtr newMatExpr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_assignment_FromScalar(IntPtr self, Scalar scalar);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_IplImage(IntPtr self, IntPtr outImage);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_IplImage_alignment(IntPtr self, out IntPtr outImage);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_CvMat(IntPtr self, IntPtr outMat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorUnaryMinus(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorAdd_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorAdd_MatScalar(IntPtr a, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorAdd_ScalarMat(Scalar s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorMinus_Mat(IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorSubtract_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorSubtract_MatScalar(IntPtr a, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorSubtract_ScalarMat(Scalar s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorMultiply_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorMultiply_MatDouble(IntPtr a, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorMultiply_DoubleMat(double s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorDivide_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorDivide_MatDouble(IntPtr a, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorDivide_DoubleMat(double s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorAnd_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorAnd_MatDouble(IntPtr a, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorAnd_DoubleMat(double s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorOr_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorOr_MatDouble(IntPtr a, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorOr_DoubleMat(double s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorXor_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorXor_MatDouble(IntPtr a, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorXor_DoubleMat(double s, IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorNot(IntPtr a);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorLT_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorLT_DoubleMat(double a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorLT_MatDouble(IntPtr a, double b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorLE_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorLE_DoubleMat(double a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorLE_MatDouble(IntPtr a, double b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorGT_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorGT_DoubleMat(double a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorGT_MatDouble(IntPtr a, double b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorGE_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorGE_DoubleMat(double a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorGE_MatDouble(IntPtr a, double b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorEQ_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorEQ_DoubleMat(double a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorEQ_MatDouble(IntPtr a, double b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorNE_MatMat(IntPtr a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorNE_DoubleMat(double a, IntPtr b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_Mat_operatorNE_MatDouble(IntPtr a, double b);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_abs_Mat(IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetB(IntPtr obj, int row, int col, byte* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetS(IntPtr obj, int row, int col, short* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetS(IntPtr obj, int row, int col, ushort* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetI(IntPtr obj, int row, int col, int* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetF(IntPtr obj, int row, int col, float* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetD(IntPtr obj, int row, int col, double* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetVec3b(IntPtr obj, int row, int col, Vec3b* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetVec3d(IntPtr obj, int row, int col, Vec3d* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetVec4f(IntPtr obj, int row, int col, Vec4f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetVec6f(IntPtr obj, int row, int col, Vec6f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetVec4i(IntPtr obj, int row, int col, Vec4i* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetPoint(IntPtr obj, int row, int col, Point* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetPoint2f(IntPtr obj, int row, int col, Point2f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetPoint2d(IntPtr obj, int row, int col, Point2d* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetPoint3i(IntPtr obj, int row, int col, Point3i* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetPoint3f(IntPtr obj, int row, int col, Point3f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetPoint3d(IntPtr obj, int row, int col, Point3d* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nSetRect(IntPtr obj, int row, int col, Rect* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetB(IntPtr obj, int row, int col, byte* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetS(IntPtr obj, int row, int col, short* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetS(IntPtr obj, int row, int col, ushort* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetI(IntPtr obj, int row, int col, int* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetF(IntPtr obj, int row, int col, float* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetD(IntPtr obj, int row, int col, double* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetVec3b(IntPtr obj, int row, int col, Vec3b* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetVec3d(IntPtr obj, int row, int col, Vec3d* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetVec4f(IntPtr obj, int row, int col, Vec4f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetVec6f(IntPtr obj, int row, int col, Vec6f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetVec4i(IntPtr obj, int row, int col, Vec4i* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetPoint(IntPtr obj, int row, int col, Point* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetPoint2f(IntPtr obj, int row, int col, Point2f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetPoint2d(IntPtr obj, int row, int col, Point2d* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetPoint3i(IntPtr obj, int row, int col, Point3i* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetPoint3f(IntPtr obj, int row, int col, Point3f* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetPoint3d(IntPtr obj, int row, int col, Point3d* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern int core_Mat_nGetRect(IntPtr obj, int row, int col, Rect* vals, int valsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Mat(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_char(IntPtr self, sbyte v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_uchar(IntPtr self, byte v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_short(IntPtr self, short v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_ushort(IntPtr self, ushort v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_int(IntPtr self, int v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_float(IntPtr self, float v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_double(IntPtr self, double v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec2b(IntPtr self, Vec2b v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec3b(IntPtr self, Vec3b v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec4b(IntPtr self, Vec4b v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec6b(IntPtr self, Vec6b v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec2s(IntPtr self, Vec2s v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec3s(IntPtr self, Vec3s v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec4s(IntPtr self, Vec4s v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec6s(IntPtr self, Vec6s v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec2w(IntPtr self, Vec2w v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec3w(IntPtr self, Vec3w v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec4w(IntPtr self, Vec4w v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec6w(IntPtr self, Vec6w v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec2i(IntPtr self, Vec2i v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec3i(IntPtr self, Vec3i v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec4i(IntPtr self, Vec4i v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec6i(IntPtr self, Vec6i v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec2f(IntPtr self, Vec2f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec3f(IntPtr self, Vec3f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec4f(IntPtr self, Vec4f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec6f(IntPtr self, Vec6f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec2d(IntPtr self, Vec2d v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec3d(IntPtr self, Vec3d v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Vec6d(IntPtr self, Vec6d v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Point(IntPtr self, Point v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Point2f(IntPtr self, Point2f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Point2d(IntPtr self, Point2d v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Point3i(IntPtr self, Point3i v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Point3f(IntPtr self, Point3f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Point3d(IntPtr self, Point3d v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Size(IntPtr self, Size v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Size2f(IntPtr self, Size2f v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_push_back_Rect(IntPtr self, Rect v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_reserve(IntPtr obj, IntPtr sz);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_resize1(IntPtr obj, IntPtr sz);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_resize2(IntPtr obj, IntPtr sz, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_pop_back(IntPtr obj, IntPtr nelems);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_uchar(IntPtr m, MatForeachFunctionByte proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec2b(IntPtr m, MatForeachFunctionVec2b proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec3b(IntPtr m, MatForeachFunctionVec3b proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec4b(IntPtr m, MatForeachFunctionVec4b proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec6b(IntPtr m, MatForeachFunctionVec6b proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_short(IntPtr m, MatForeachFunctionInt16 proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec2s(IntPtr m, MatForeachFunctionVec2s proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec3s(IntPtr m, MatForeachFunctionVec3s proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec4s(IntPtr m, MatForeachFunctionVec4s proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec6s(IntPtr m, MatForeachFunctionVec6s proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_int(IntPtr m, MatForeachFunctionInt32 proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec2i(IntPtr m, MatForeachFunctionVec2i proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec3i(IntPtr m, MatForeachFunctionVec3i proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec4i(IntPtr m, MatForeachFunctionVec4i proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec6i(IntPtr m, MatForeachFunctionVec6i proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_float(IntPtr m, MatForeachFunctionFloat proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec2f(IntPtr m, MatForeachFunctionVec2f proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec3f(IntPtr m, MatForeachFunctionVec3f proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec4f(IntPtr m, MatForeachFunctionVec4f proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec6f(IntPtr m, MatForeachFunctionVec6f proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_double(IntPtr m, MatForeachFunctionDouble proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec2d(IntPtr m, MatForeachFunctionVec2d proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec3d(IntPtr m, MatForeachFunctionVec3d proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec4d(IntPtr m, MatForeachFunctionVec4d proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_Mat_forEach_Vec6d(IntPtr m, MatForeachFunctionVec6d proc);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_new2(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_MatExpr_delete(IntPtr expr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_toMat(IntPtr expr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorUnaryMinus_MatExpr(IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorUnaryNot_MatExpr(IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorAdd_MatExprMat(IntPtr e, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorAdd_MatMatExpr(IntPtr m, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorAdd_MatExprScalar(IntPtr e, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorAdd_ScalarMatExpr(Scalar s, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorAdd_MatExprMatExpr(IntPtr e1, IntPtr e2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorSubtract_MatExprMat(IntPtr e, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorSubtract_MatMatExpr(IntPtr m, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorSubtract_MatExprScalar(IntPtr e, Scalar s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorSubtract_ScalarMatExpr(Scalar s, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorSubtract_MatExprMatExpr(IntPtr e1, IntPtr e2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorMultiply_MatExprMat(IntPtr e, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorMultiply_MatMatExpr(IntPtr m, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorMultiply_MatExprDouble(IntPtr e, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorMultiply_DoubleMatExpr(double s, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorMultiply_MatExprMatExpr(IntPtr e1, IntPtr e2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorDivide_MatExprMat(IntPtr e, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorDivide_MatMatExpr(IntPtr m, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorDivide_MatExprDouble(IntPtr e, double s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorDivide_DoubleMatExpr(double s, IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_operatorDivide_MatExprMatExpr(IntPtr e1, IntPtr e2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_row(IntPtr self, int y);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_col(IntPtr self, int x);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_diag1(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_diag2(IntPtr self, int d);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_submat(IntPtr self, int rowStart, int rowEnd, int colStart, int colEnd);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_cross(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double core_MatExpr_dot(IntPtr self, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_t(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_inv1(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_inv2(IntPtr self, int method);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_mul_toMatExpr(IntPtr self, IntPtr e, double scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_MatExpr_mul_toMat(IntPtr self, IntPtr m, double scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size core_MatExpr_size(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_MatExpr_type(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_abs_MatExpr(IntPtr e);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_OutputArray_new_byMat(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_OutputArray_new_byGpuMat(IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_OutputArray_new_byScalar(Scalar val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_OutputArray_new_byVectorOfMat(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_OutputArray_delete(IntPtr oa);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_OutputArray_getMat(IntPtr oa);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Scalar core_OutputArray_getScalar(IntPtr oa);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_OutputArray_getVectorOfMat(IntPtr oa, IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ulong core_SparseMat_sizeof();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_new2(int dims, int[] sizes, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_new3(IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_operatorAssign_SparseMat(IntPtr obj, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_operatorAssign_Mat(IntPtr obj, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_clone(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_copyTo_SparseMat(IntPtr obj, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_copyTo_Mat(IntPtr obj, IntPtr m);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_convertTo_SparseMat(IntPtr obj, IntPtr m, int rtype, double alpha);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_convertTo_Mat(IntPtr obj, IntPtr m, int rtype, double alpha, double beta);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_assignTo(IntPtr obj, IntPtr m, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_create(IntPtr obj, int dims, int[] sizes, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_clear(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_addref(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void core_SparseMat_release(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_elemSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_elemSize1(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_type(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_depth(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_channels(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_size1(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_size2(IntPtr obj, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int core_SparseMat_dims(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_nzcount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_hash_1d(IntPtr obj, int i0);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_hash_2d(IntPtr obj, int i0, int i1);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_hash_3d(IntPtr obj, int i0, int i1, int i2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_hash_nd(IntPtr obj, int[] idx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_1d(IntPtr obj, int i0, int createMissing, ref ulong hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_1d(IntPtr obj, int i0, int createMissing, IntPtr hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_2d(IntPtr obj, int i0, int i1, int createMissing, ref ulong hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_2d(IntPtr obj, int i0, int i1, int createMissing, IntPtr hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_3d(IntPtr obj, int i0, int i1, int i2, int createMissing, ref ulong hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_3d(IntPtr obj, int i0, int i1, int i2, int createMissing, IntPtr hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_nd(IntPtr obj, int[] idx, int createMissing, ref ulong hashval);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr core_SparseMat_ptr_nd(IntPtr obj, int[] idx, int createMissing, IntPtr hashval);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNetFromDarknet([MarshalAs(UnmanagedType.LPStr)] string cfgFile, [MarshalAs(UnmanagedType.LPStr)] string darknetModel);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNetFromCaffe([MarshalAs(UnmanagedType.LPStr)] string prototxt, [MarshalAs(UnmanagedType.LPStr)] string caffeModel);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNetFromTensorflow([MarshalAs(UnmanagedType.LPStr)] string model, [MarshalAs(UnmanagedType.LPStr)] string config);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNetFromTorch([MarshalAs(UnmanagedType.LPStr)] string model, int isBinary);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNet([MarshalAs(UnmanagedType.LPStr)] string model, [MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string framework);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readTorchBlob([MarshalAs(UnmanagedType.LPStr)] string filename, int isBinary);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNetFromModelOptimizer([MarshalAs(UnmanagedType.LPStr)] string xml, [MarshalAs(UnmanagedType.LPStr)] string bin);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readNetFromONNX([MarshalAs(UnmanagedType.LPStr)] string onnxFile);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_readTensorFromONNX([MarshalAs(UnmanagedType.LPStr)] string path);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_blobFromImage(IntPtr image, double scalefactor, Size size, Scalar mean, int swapRB, int crop);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_blobFromImages(IntPtr[] images, int imagesLength, double scalefactor, Size size, Scalar mean, int swapRB, int crop);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_shrinkCaffeModel([MarshalAs(UnmanagedType.LPStr)] string src, [MarshalAs(UnmanagedType.LPStr)] string dst);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_writeTextGraph([MarshalAs(UnmanagedType.LPStr)] string model, [MarshalAs(UnmanagedType.LPStr)] string output);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_NMSBoxes_Rect(IntPtr bboxes, IntPtr scores, float score_threshold, float nms_threshold, IntPtr indices, float eta, int top_k);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_NMSBoxes_Rect2d(IntPtr bboxes, IntPtr scores, float score_threshold, float nms_threshold, IntPtr indices, float eta, int top_k);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_NMSBoxes_RotatedRect(IntPtr bboxes, IntPtr scores, float score_threshold, float nms_threshold, IntPtr indices, float eta, int top_k);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_resetMyriadDevice();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr dnn_Net_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void dnn_Net_delete(IntPtr net);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int dnn_Net_empty(IntPtr net);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int dnn_Net_getLayerId(IntPtr net, [MarshalAs(UnmanagedType.LPStr)] string layer);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void dnn_Net_getLayerNames(IntPtr net, IntPtr outVec);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_connect1(IntPtr net, [MarshalAs(UnmanagedType.LPStr)] string outPin, [MarshalAs(UnmanagedType.LPStr)] string inpPin);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void dnn_Net_connect2(IntPtr net, int outLayerId, int outNum, int inpLayerId, int inpNum);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_setInputsNames(IntPtr net, string[] inputBlobNames, int inputBlobNamesLength);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr dnn_Net_forward1(IntPtr net, [MarshalAs(UnmanagedType.LPStr)] string outputName);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_forward2(IntPtr net, IntPtr[] outputBlobs, int outputBlobsLength, [MarshalAs(UnmanagedType.LPStr)] string outputName);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_forward3(IntPtr net, IntPtr[] outputBlobs, int outputBlobsLength, string[] outBlobNames, int outBlobNamesLength);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_setHalideScheduler(IntPtr net, [MarshalAs(UnmanagedType.LPStr)] string scheduler);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_setPreferableBackend(IntPtr net, int backendId);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_setPreferableTarget(IntPtr net, int targetId);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_setInput(IntPtr net, IntPtr blob, [MarshalAs(UnmanagedType.LPStr)] string name);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_getUnconnectedOutLayers(IntPtr net, IntPtr result);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_getUnconnectedOutLayersNames(IntPtr net, IntPtr result);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void dnn_Net_enableFusion(IntPtr net, int fusion);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern long dnn_Net_getPerfProfile(IntPtr net, IntPtr timings);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_BasicFaceRecognizer_getNumComponents(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_setNumComponents(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double face_BasicFaceRecognizer_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_setThreshold(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_getProjections(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_getLabels(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_getEigenValues(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_getEigenVectors(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_BasicFaceRecognizer_getMean(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_BasicFaceRecognizer_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_BasicFaceRecognizer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_EigenFaceRecognizer_create(int numComponents, double threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_EigenFaceRecognizer_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_EigenFaceRecognizer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Facemark_read(IntPtr obj, IntPtr fn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Facemark_write(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Facemark_loadModel(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string model);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_Facemark_fit(IntPtr obj, IntPtr image, IntPtr faces, IntPtr landmarks);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_FacemarkLBF_create(IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_FacemarkLBF_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_FacemarkLBF_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_FacemarkLBF_Params_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double face_FacemarkLBF_Params_shape_offset_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_shape_offset_set(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_cascade_face_get(IntPtr obj, IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_cascade_face_set(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_verbose_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_verbose_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_n_landmarks_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_n_landmarks_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_initShape_n_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_initShape_n_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_stages_n_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_stages_n_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_tree_n_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_tree_n_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_tree_depth_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_tree_depth_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double face_FacemarkLBF_Params_bagging_overlap_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_bagging_overlap_set(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_model_filename_get(IntPtr obj, IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_model_filename_set(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkLBF_Params_save_model_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_save_model_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern uint face_FacemarkLBF_Params_seed_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_seed_set(IntPtr obj, uint val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_feats_m_get(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_feats_m_set(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_radius_m_get(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_radius_m_set(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_pupils0_get(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_pupils0_set(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_pupils1_get(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_pupils1_set(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect face_FacemarkLBF_Params_detectROI_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_detectROI_set(IntPtr obj, Rect val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_read(IntPtr obj, IntPtr fn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkLBF_Params_write(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_FacemarkAAM_create(IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_FacemarkAAM_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_FacemarkAAM_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_FacemarkAAM_Params_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_model_filename_get(IntPtr obj, IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_model_filename_set(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_m_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_m_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_n_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_n_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_n_iter_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_n_iter_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_verbose_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_verbose_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_save_model_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_save_model_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_max_m_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_max_m_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_max_n_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_max_n_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FacemarkAAM_Params_texture_max_m_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_texture_max_m_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_scales_get(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_scales_set(IntPtr obj, IntPtr v);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_read(IntPtr obj, IntPtr fn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FacemarkAAM_Params_write(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_train(IntPtr obj, IntPtr[] src, int srcLength, int[] labels, int labelsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_update(IntPtr obj, IntPtr[] src, int srcLength, int[] labels, int labelsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_FaceRecognizer_predict1(IntPtr obj, IntPtr src);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_predict2(IntPtr obj, IntPtr src, out int label, out double confidence);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_write1(IntPtr obj, string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_read1(IntPtr obj, string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_write2(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_read2(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_setLabelInfo(IntPtr obj, int label, [MarshalAs(UnmanagedType.LPStr)] string strInfo);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_getLabelInfo(IntPtr obj, int label, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_getLabelsByString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string str, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double face_FaceRecognizer_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_FaceRecognizer_setThreshold(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_FaceRecognizer_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_FaceRecognizer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_FisherFaceRecognizer_create(int numComponents, double threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_FisherFaceRecognizer_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_FisherFaceRecognizer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_LBPHFaceRecognizer_create(int radius, int neighbors, int gridX, int gridY, double threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_LBPHFaceRecognizer_getGridX(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_setGridX(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_LBPHFaceRecognizer_getGridY(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_setGridY(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_LBPHFaceRecognizer_getRadius(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_setRadius(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int face_LBPHFaceRecognizer_getNeighbors(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_setNeighbors(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double face_LBPHFaceRecognizer_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_setThreshold(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_getHistograms(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_LBPHFaceRecognizer_getLabels(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr face_Ptr_LBPHFaceRecognizer_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void face_Ptr_LBPHFaceRecognizer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_drawKeypoints(IntPtr image, KeyPoint[] keypoints, int keypointsLength, IntPtr outImage, Scalar color, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_drawMatches1(IntPtr img1, KeyPoint[] keypoints1, int keypoints1Length, IntPtr img2, KeyPoint[] keypoints2, int keypoints2Length, DMatch[] matches1to2, int matches1to2Length, IntPtr outImg, Scalar matchColor, Scalar singlePointColor, byte[] matchesMask, int matchesMaskLength, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_drawMatches2(IntPtr img1, KeyPoint[] keypoints1, int keypoints1Length, IntPtr img2, KeyPoint[] keypoints2, int keypoints2Length, IntPtr[] matches1to2, int matches1to2Size1, int[] matches1to2Size2, IntPtr outImg, Scalar matchColor, Scalar singlePointColor, IntPtr[] matchesMask, int matchesMaskSize1, int[] matchesMaskSize2, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_evaluateFeatureDetector(IntPtr img1, IntPtr img2, IntPtr H1to2, IntPtr keypoints1, IntPtr keypoints2, out float repeatability, out int correspCount);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_computeRecallPrecisionCurve(IntPtr[] matches1to2, int matches1to2Size1, int[] matches1to2Size2, IntPtr[] correctMatches1to2Mask, int correctMatches1to2MaskSize1, int[] correctMatches1to2MaskSize2, IntPtr recallPrecisionCurve);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float features2d_getRecall(Point2f[] recallPrecisionCurve, int recallPrecisionCurveSize, float l_precision);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_getNearestPoint(Point2f[] recallPrecisionCurve, int recallPrecisionCurveSize, float l_precision);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AGAST(IntPtr image, IntPtr keypoints, int threshold, int nonmaxSuppression, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_AgastFeatureDetector_create(int threshold, int nonmaxSuppression, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_AgastFeatureDetector_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_AgastFeatureDetector_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AgastFeatureDetector_setThreshold(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AgastFeatureDetector_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AgastFeatureDetector_setNonmaxSuppression(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AgastFeatureDetector_getNonmaxSuppression(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AgastFeatureDetector_setType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AgastFeatureDetector_getType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_AKAZE_create(int descriptor_type, int descriptor_size, int descriptor_channels, float threshold, int nOctaves, int nOctaveLayers, int diffusivity);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_AKAZE_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_AKAZE_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setDescriptorType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AKAZE_getDescriptorType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setDescriptorSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AKAZE_getDescriptorSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setDescriptorChannels(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AKAZE_getDescriptorChannels(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setThreshold(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double features2d_AKAZE_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setNOctaves(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AKAZE_getNOctaves(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setNOctaveLayers(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AKAZE_getNOctaveLayers(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_AKAZE_setDiffusivity(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_AKAZE_getDiffusivity(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWTrainer_add(IntPtr obj, IntPtr descriptors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWTrainer_getDescriptors(IntPtr obj, IntPtr descriptors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_BOWTrainer_descriptorsCount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWTrainer_clear(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWKMeansTrainer_new(int clusterCount, TermCriteria termcrit, int attempts, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWKMeansTrainer_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWKMeansTrainer_cluster1(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWKMeansTrainer_cluster2(IntPtr obj, IntPtr descriptors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWImgDescriptorExtractor_new1_Ptr(IntPtr dextractor, IntPtr dmatcher);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWImgDescriptorExtractor_new2_Ptr(IntPtr dmatcher);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWImgDescriptorExtractor_new1_RawPtr(IntPtr dextractor, IntPtr dmatcher);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWImgDescriptorExtractor_new2_RawPtr(IntPtr dmatcher);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWImgDescriptorExtractor_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWImgDescriptorExtractor_setVocabulary(IntPtr obj, IntPtr vocabulary);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BOWImgDescriptorExtractor_getVocabulary(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWImgDescriptorExtractor_compute11(IntPtr obj, IntPtr image, IntPtr keypoints, IntPtr imgDescriptor, IntPtr pointIdxsOfClusters, IntPtr descriptors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWImgDescriptorExtractor_compute12(IntPtr obj, IntPtr keypointDescriptors, IntPtr imgDescriptor, IntPtr pointIdxsOfClusters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BOWImgDescriptorExtractor_compute2(IntPtr obj, IntPtr image, IntPtr keypoints, IntPtr imgDescriptor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_BOWImgDescriptorExtractor_descriptorSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_BOWImgDescriptorExtractor_descriptorType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BRISK_create1(int thresh, int octaves, float patternScale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BRISK_create2(float[] radiusList, int radiusListLength, int[] numberList, int numberListLength, float dMax, float dMin, int[] indexChange, int indexChangeLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_BRISK_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_BRISK_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorExtractor_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorExtractor_compute1(IntPtr obj, IntPtr image, IntPtr keypoint, IntPtr descriptors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorExtractor_compute2(IntPtr obj, IntPtr[] images, int imagesSize, IntPtr keypoints, IntPtr[] descriptors, int descriptorsSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_DescriptorExtractor_descriptorSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_DescriptorExtractor_descriptorType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_DescriptorExtractor_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_DescriptorExtractor_create([MarshalAs(UnmanagedType.LPStr)] string descriptorExtractorType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_DescriptorExtractor_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_DescriptorExtractor_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_add(IntPtr obj, IntPtr[] descriptors, int descriptorLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_getTrainDescriptors(IntPtr obj, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_clear(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_DescriptorMatcher_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_DescriptorMatcher_isMaskSupported(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_train(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_match1(IntPtr obj, IntPtr queryDescriptors, IntPtr trainDescriptors, IntPtr matches, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_knnMatch1(IntPtr obj, IntPtr queryDescriptors, IntPtr trainDescriptors, IntPtr matches, int k, IntPtr mask, int compactResult);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_radiusMatch1(IntPtr obj, IntPtr queryDescriptors, IntPtr trainDescriptors, IntPtr matches, float maxDistance, IntPtr mask, int compactResult);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_match2(IntPtr obj, IntPtr queryDescriptors, IntPtr matches, IntPtr[] masks, int masksSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_knnMatch2(IntPtr obj, IntPtr queryDescriptors, IntPtr matches, int k, IntPtr[] masks, int masksSize, int compactResult);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_DescriptorMatcher_radiusMatch2(IntPtr obj, IntPtr queryDescriptors, IntPtr matches, float maxDistance, IntPtr[] masks, int masksSize, int compactResult);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr features2d_DescriptorMatcher_create([MarshalAs(UnmanagedType.LPStr)] string descriptorMatcherType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_DescriptorMatcher_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_DescriptorMatcher_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_BFMatcher_new(int normType, int crossCheck);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_BFMatcher_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_BFMatcher_isMaskSupported(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_BFMatcher_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_BFMatcher_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_FlannBasedMatcher_new(IntPtr indexParams, IntPtr searchParams);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FlannBasedMatcher_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FlannBasedMatcher_add(IntPtr obj, IntPtr[] descriptors, int descriptorsSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FlannBasedMatcher_clear(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FlannBasedMatcher_train(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_FlannBasedMatcher_isMaskSupported(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_FlannBasedMatcher_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_FlannBasedMatcher_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FAST1(IntPtr image, IntPtr keypoints, int threshold, int nonmaxSupression);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FAST2(IntPtr image, IntPtr keypoints, int threshold, int nonmaxSupression, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_FastFeatureDetector_create(int threshold, int nonmaxSuppression);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_FastFeatureDetector_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_FastFeatureDetector_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FastFeatureDetector_setThreshold(IntPtr obj, int threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_FastFeatureDetector_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FastFeatureDetector_setNonmaxSuppression(IntPtr obj, int f);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_FastFeatureDetector_getNonmaxSuppression(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_FastFeatureDetector_setType(IntPtr obj, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_FastFeatureDetector_getType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_Feature2D_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_Feature2D_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Feature2D_detect_Mat1(IntPtr detector, IntPtr image, IntPtr keypoints, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Feature2D_detect_Mat2(IntPtr detector, IntPtr[] images, int imageLength, IntPtr keypoints, IntPtr[] mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Feature2D_detect_InputArray(IntPtr detector, IntPtr image, IntPtr keypoints, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Feature2D_compute1(IntPtr obj, IntPtr image, IntPtr keypoints, IntPtr descriptors);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Feature2D_compute2(IntPtr detector, IntPtr[] images, int imageLength, IntPtr keypoints, IntPtr[] descriptors, int descriptorsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Feature2D_detectAndCompute(IntPtr detector, IntPtr image, IntPtr mask, IntPtr keypoints, IntPtr descriptors, int useProvidedKeypoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_Feature2D_descriptorSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_Feature2D_descriptorType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_Feature2D_defaultNorm(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_Feature2D_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_GFTTDetector_create(int maxCorners, double qualityLevel, double minDistance, int blockSize, int useHarrisDetector, double k);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_GFTTDetector_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_GFTTDetector_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_setMaxFeatures(IntPtr obj, int maxFeatures);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_GFTTDetector_getMaxFeatures(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_setQualityLevel(IntPtr obj, double qlevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double features2d_GFTTDetector_getQualityLevel(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_setMinDistance(IntPtr obj, double minDistance);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double features2d_GFTTDetector_getMinDistance(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_setBlockSize(IntPtr obj, int blockSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_GFTTDetector_getBlockSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_setHarrisDetector(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_GFTTDetector_getHarrisDetector(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_GFTTDetector_setK(IntPtr obj, double k);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double features2d_GFTTDetector_getK(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_KAZE_create(bool extended, bool upright, float threshold, int nOctaves, int nOctaveLayers, int diffusivity);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_KAZE_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_KAZE_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KAZE_setDiffusivity(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_KAZE_getDiffusivity(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KAZE_setExtended(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_KAZE_getExtended(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KAZE_setNOctaveLayers(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_KAZE_getNOctaveLayers(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KAZE_setNOctaves(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_KAZE_getNOctaves(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KAZE_setThreshold(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double features2d_KAZE_getThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KAZE_setUpright(IntPtr obj, bool val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern bool features2d_KAZE_getUpright(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KeyPointsFilter_runByImageBorder(IntPtr keypoints, Size imageSize, int borderSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KeyPointsFilter_runByKeypointSize(IntPtr keypoints, float minSize, float maxSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KeyPointsFilter_runByPixelsMask(IntPtr keypoints, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KeyPointsFilter_removeDuplicated(IntPtr keypoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_KeyPointsFilter_retainBest(IntPtr keypoints, int npoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_MSER_create(int delta, int minArea, int maxArea, double maxVariation, double minDiversity, int maxEvolution, double areaThreshold, double minMargin, int edgeBlurSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_MSER_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_MSER_detect(IntPtr obj, IntPtr image, out IntPtr msers, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_MSER_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_MSER_detectRegions(IntPtr obj, IntPtr image, IntPtr msers, IntPtr bboxes);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_MSER_setDelta(IntPtr obj, int delta);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_MSER_getDelta(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_MSER_setMinArea(IntPtr obj, int minArea);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_MSER_getMinArea(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_MSER_setMaxArea(IntPtr obj, int maxArea);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_MSER_getMaxArea(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_MSER_setPass2Only(IntPtr obj, int f);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_MSER_getPass2Only(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_ORB_create(int nFeatures, float scaleFactor, int nlevels, int edgeThreshold, int firstLevel, int wtaK, int scoreType, int patchSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_ORB_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_ORB_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setMaxFeatures(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getMaxFeatures(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setScaleFactor(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double features2d_ORB_getScaleFactor(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setNLevels(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getNLevels(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setEdgeThreshold(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getEdgeThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setFirstLevel(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getFirstLevel(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setWTA_K(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getWTA_K(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setScoreType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getScoreType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setPatchSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getPatchSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_ORB_setFastThreshold(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int features2d_ORB_getFastThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_SimpleBlobDetector_create(ref SimpleBlobDetector.WParams parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr features2d_Ptr_SimpleBlobDetector_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void features2d_Ptr_SimpleBlobDetector_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getGaussianKernel(int ksize, double sigma, int ktype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_getDerivKernels(IntPtr kx, IntPtr ky, int dx, int dy, int ksize, int normalize, int ktype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getGaborKernel(Size ksize, double sigma, double theta, double lambd, double gamma, double psi, int ktype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getStructuringElement(int shape, Size ksize, Point anchor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_medianBlur(IntPtr src, IntPtr dst, int ksize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GaussianBlur(IntPtr src, IntPtr dst, Size ksize, double sigmaX, double sigmaY, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_bilateralFilter(IntPtr src, IntPtr dst, int d, double sigmaColor, double sigmaSpace, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_boxFilter(IntPtr src, IntPtr dst, int ddepth, Size ksize, Point anchor, int normalize, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_blur(IntPtr src, IntPtr dst, Size ksize, Point anchor, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_filter2D(IntPtr src, IntPtr dst, int ddepth, IntPtr kernel, Point anchor, double delta, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_sepFilter2D(IntPtr src, IntPtr dst, int ddepth, IntPtr kernelX, IntPtr kernelY, Point anchor, double delta, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Sobel(IntPtr src, IntPtr dst, int ddepth, int dx, int dy, int ksize, double scale, double delta, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Scharr(IntPtr src, IntPtr dst, int ddepth, int dx, int dy, double scale, double delta, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Laplacian(IntPtr src, IntPtr dst, int ddepth, int ksize, double scale, double delta, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Canny(IntPtr src, IntPtr edges, double threshold1, double threshold2, int apertureSize, int L2gradient);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_cornerEigenValsAndVecs(IntPtr src, IntPtr dst, int blockSize, int ksize, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_preCornerDetect(IntPtr src, IntPtr dst, int ksize, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_cornerSubPix(IntPtr image, IntPtr corners, Size winSize, Size zeroZone, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_goodFeaturesToTrack(IntPtr src, IntPtr corners, int maxCorners, double qualityLevel, double minDistance, IntPtr mask, int blockSize, int useHarrisDetector, double k);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_HoughLines(IntPtr src, IntPtr lines, double rho, double theta, int threshold, double srn, double stn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_HoughLinesP(IntPtr src, IntPtr lines, double rho, double theta, int threshold, double minLineLength, double maxLineG);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_HoughCircles(IntPtr src, IntPtr circles, int method, double dp, double minDist, double param1, double param2, int minRadius, int maxRadius);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_erode(IntPtr src, IntPtr dst, IntPtr kernel, Point anchor, int iterations, int borderType, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_dilate(IntPtr src, IntPtr dst, IntPtr kernel, Point anchor, int iterations, int borderType, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_morphologyEx(IntPtr src, IntPtr dst, int op, IntPtr kernel, Point anchor, int iterations, int borderType, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_resize(IntPtr src, IntPtr dst, Size dsize, double fx, double fy, int interpolation);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_warpAffine(IntPtr src, IntPtr dst, IntPtr m, Size dsize, int flags, int borderMode, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_warpPerspective_MisInputArray(IntPtr src, IntPtr dst, IntPtr m, Size dsize, int flags, int borderMode, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_warpPerspective_MisArray(IntPtr src, IntPtr dst, [MarshalAs(UnmanagedType.LPArray)] float[,] m, int mRow, int mCol, Size dsize, int flags, int borderMode, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_remap(IntPtr src, IntPtr dst, IntPtr map1, IntPtr map2, int interpolation, int borderMode, Scalar borderValue);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convertMaps(IntPtr map1, IntPtr map2, IntPtr dstmap1, IntPtr dstmap2, int dstmap1Type, int nninterpolation);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getRotationMatrix2D(Point2f center, double angle, double scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_invertAffineTransform(IntPtr m, IntPtr im);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getPerspectiveTransform1(Point2f[] src, Point2f[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getPerspectiveTransform2(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getAffineTransform1(Point2f[] src, Point2f[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_getAffineTransform2(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_getRectSubPix(IntPtr image, Size patchSize, Point2f center, IntPtr patch, int patchType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_logPolar(IntPtr src, IntPtr dst, Point2f center, double m, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_linearPolar(IntPtr src, IntPtr dst, Point2f center, double maxRadius, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_integral1(IntPtr src, IntPtr sum, int sdepth);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_integral2(IntPtr src, IntPtr sum, IntPtr sqsum, int sdepth);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_integral3(IntPtr src, IntPtr sum, IntPtr sqsum, IntPtr tilted, int sdepth);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_accumulate(IntPtr src, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_accumulateSquare(IntPtr src, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_accumulateProduct(IntPtr src1, IntPtr src2, IntPtr dst, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_accumulateWeighted(IntPtr src, IntPtr dst, double alpha, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_PSNR(IntPtr src1, IntPtr src2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point2d imgproc_phaseCorrelate(IntPtr src1, IntPtr src2, IntPtr window);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point2d imgproc_phaseCorrelateRes(IntPtr src1, IntPtr src2, IntPtr window, out double response);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_createHanningWindow(IntPtr dst, Size winSize, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_threshold(IntPtr src, IntPtr dst, double thresh, double maxval, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_adaptiveThreshold(IntPtr src, IntPtr dst, double maxValue, int adaptiveMethod, int thresholdType, int blockSize, double c);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_pyrDown(IntPtr src, IntPtr dst, Size dstsize, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_pyrUp(IntPtr src, IntPtr dst, Size dstsize, int borderType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_calcHist1(IntPtr[] images, int nimages, int[] channels, IntPtr mask, IntPtr hist, int dims, int[] histSize, IntPtr[] ranges, int uniform, int accumulate);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_calcBackProject(IntPtr[] images, int nimages, int[] channels, IntPtr hist, IntPtr backProject, IntPtr[] ranges, int uniform);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_compareHist1(IntPtr h1, IntPtr h2, int method);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_equalizeHist(IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float imgproc_EMD(IntPtr signature1, IntPtr signature2, int distType, IntPtr cost, out float lowerBound, IntPtr flow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_watershed(IntPtr image, IntPtr markers);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_pyrMeanShiftFiltering(IntPtr src, IntPtr dst, double sp, double sr, int maxLevel, TermCriteria termcrit);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_grabCut(IntPtr img, IntPtr mask, Rect rect, IntPtr bgdModel, IntPtr fgdModel, int iterCount, int mode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_distanceTransformWithLabels(IntPtr src, IntPtr dst, IntPtr labels, int distanceType, int maskSize, int labelType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_distanceTransform(IntPtr src, IntPtr dst, int distanceType, int maskSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_floodFill1(IntPtr image, Point seedPoint, Scalar newVal, out Rect rect, Scalar loDiff, Scalar upDiff, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_floodFill2(IntPtr image, IntPtr mask, Point seedPoint, Scalar newVal, out Rect rect, Scalar loDiff, Scalar upDiff, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_cvtColor(IntPtr src, IntPtr dst, int code, int dstCn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Moments.NativeStruct imgproc_moments(IntPtr arr, int binaryImage);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_matchTemplate(IntPtr image, IntPtr templ, IntPtr result, int method, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_connectedComponents(IntPtr image, IntPtr labels, int connectivity, int ltype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_connectedComponentsWithStats(IntPtr image, IntPtr labels, IntPtr stats, IntPtr centroids, int connectivity, int ltype);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_findContours1_vector(IntPtr image, out IntPtr contours, out IntPtr hierarchy, int mode, int method, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_findContours1_OutputArray(IntPtr image, out IntPtr contours, IntPtr hierarchy, int mode, int method, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_findContours2_vector(IntPtr image, out IntPtr contours, int mode, int method, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_findContours2_OutputArray(IntPtr image, out IntPtr contours, int mode, int method, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_drawContours_vector(IntPtr image, IntPtr[] contours, int contoursSize1, int[] contoursSize2, int contourIdx, Scalar color, int thickness, int lineType, Vec4i[] hierarchy, int hiearchyLength, int maxLevel, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_drawContours_vector(IntPtr image, IntPtr[] contours, int contoursSize1, int[] contoursSize2, int contourIdx, Scalar color, int thickness, int lineType, IntPtr hierarchy, int hiearchyLength, int maxLevel, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_drawContours_InputArray(IntPtr image, IntPtr[] contours, int contoursLength, int contourIdx, Scalar color, int thickness, int lineType, IntPtr hierarchy, int maxLevel, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_approxPolyDP_InputArray(IntPtr curve, IntPtr approxCurve, double epsilon, int closed);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_approxPolyDP_Point(Point[] curve, int curveLength, out IntPtr approxCurve, double epsilon, int closed);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_approxPolyDP_Point2f(Point2f[] curve, int curveLength, out IntPtr approxCurve, double epsilon, int closed);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_arcLength_InputArray(IntPtr curve, int closed);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_arcLength_Point(Point[] curve, int curveLength, int closed);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_arcLength_Point2f(Point2f[] curve, int curveLength, int closed);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect imgproc_boundingRect_InputArray(IntPtr curve);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect imgproc_boundingRect_Point(Point[] curve, int curveLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Rect imgproc_boundingRect_Point2f(Point2f[] curve, int curveLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_contourArea_InputArray(IntPtr contour, int oriented);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_contourArea_Point(Point[] contour, int contourLength, int oriented);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_contourArea_Point2f(Point2f[] contour, int contourLength, int oriented);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect imgproc_minAreaRect_InputArray(IntPtr points);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect imgproc_minAreaRect_Point(Point[] points, int pointsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect imgproc_minAreaRect_Point2f(Point2f[] points, int pointsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_minEnclosingCircle_InputArray(IntPtr points, out Point2f center, out float radius);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_minEnclosingCircle_Point(Point[] points, int pointsLength, out Point2f center, out float radius);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_minEnclosingCircle_Point2f(Point2f[] points, int pointsLength, out Point2f center, out float radius);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_matchShapes_InputArray(IntPtr contour1, IntPtr contour2, int method, double parameter);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_matchShapes_Point(Point[] contour1, int contour1Length, Point[] contour2, int contour2Length, int method, double parameter);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexHull_InputArray(IntPtr points, IntPtr hull, int clockwise, int returnPoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexHull_Point_ReturnsPoints(Point[] points, int pointsLength, out IntPtr hull, int clockwise);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexHull_Point2f_ReturnsPoints(Point2f[] points, int pointsLength, out IntPtr hull, int clockwise);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexHull_Point_ReturnsIndices(Point[] points, int pointsLength, out IntPtr hull, int clockwise);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexHull_Point2f_ReturnsIndices(Point2f[] points, int pointsLength, out IntPtr hull, int clockwise);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexityDefects_InputArray(IntPtr contour, IntPtr convexHull, IntPtr convexityDefects);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexityDefects_Point(Point[] contour, int contourLength, int[] convexHull, int convexHullLength, out IntPtr convexityDefects);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_convexityDefects_Point2f(Point2f[] contour, int contourLength, int[] convexHull, int convexHullLength, out IntPtr convexityDefects);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_isContourConvex_InputArray(IntPtr contour);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_isContourConvex_Point(Point[] contour, int contourLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_isContourConvex_Point2f(Point2f[] contour, int contourLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float imgproc_intersectConvexConvex_InputArray(IntPtr p1, IntPtr p2, IntPtr p12, int handleNested);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float imgproc_intersectConvexConvex_Point(Point[] p1, int p1Length, Point[] p2, int p2Length, out IntPtr p12, int handleNested);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float imgproc_intersectConvexConvex_Point2f(Point2f[] p1, int p1Length, Point2f[] p2, int p2Length, out IntPtr p12, int handleNested);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect imgproc_fitEllipse_InputArray(IntPtr points);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect imgproc_fitEllipse_Point(Point[] points, int pointsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect imgproc_fitEllipse_Point2f(Point2f[] points, int pointsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fitLine_InputArray(IntPtr points, IntPtr line, int distType, double param, double reps, double aeps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fitLine_Point(Point[] points, int pointsLength, [In][Out] float[] line, int distType, double param, double reps, double aeps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fitLine_Point2f(Point2f[] points, int pointsLength, [In][Out] float[] line, int distType, double param, double reps, double aeps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fitLine_Point3i(Point3i[] points, int pointsLength, [In][Out] float[] line, int distType, double param, double reps, double aeps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fitLine_Point3f(Point3f[] points, int pointsLength, [In][Out] float[] line, int distType, double param, double reps, double aeps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_pointPolygonTest_InputArray(IntPtr contour, Point2f pt, int measureDist);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_pointPolygonTest_Point(Point[] contour, int contourLength, Point2f pt, int measureDist);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_pointPolygonTest_Point2f(Point2f[] contour, int contourLength, Point2f pt, int measureDist);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_rotatedRectangleIntersection_OutputArray(RotatedRect rect1, RotatedRect rect2, IntPtr intersectingRegion);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_rotatedRectangleIntersection_vector(RotatedRect rect1, RotatedRect rect2, IntPtr intersectingRegion);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_applyColorMap(IntPtr src, IntPtr dst, int colormap);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_line(IntPtr img, Point pt1, Point pt2, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_arrowedLine(IntPtr img, Point pt1, Point pt2, Scalar color, int thickness, int lineType, int shift, double tipLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_rectangle_InputOutputArray(IntPtr img, Point pt1, Point pt2, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_rectangle_Mat(IntPtr img, Rect rect, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_circle(IntPtr img, Point center, int radius, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_ellipse1(IntPtr img, Point center, Size axes, double angle, double startAngle, double endAngle, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_ellipse2(IntPtr img, RotatedRect box, Scalar color, int thickness, int lineType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fillConvexPoly_Mat(IntPtr img, Point[] pts, int npts, Scalar color, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fillConvexPoly_InputOutputArray(IntPtr img, IntPtr points, Scalar color, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fillPoly_Mat(IntPtr img, IntPtr[] pts, int[] npts, int ncontours, Scalar color, int lineType, int shift, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_fillPoly_InputOutputArray(IntPtr img, IntPtr pts, Scalar color, int lineType, int shift, Point offset);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_polylines_Mat(IntPtr img, IntPtr[] pts, int[] npts, int ncontours, int isClosed, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_polylines_InputOutputArray(IntPtr img, IntPtr pts, int isClosed, Scalar color, int thickness, int lineType, int shift);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_clipLine1(Size imgSize, ref Point pt1, ref Point pt2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_clipLine2(Rect imgRect, ref Point pt1, ref Point pt2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_ellipse2Poly(Point center, Size axes, int angle, int arcStart, int arcEnd, int delta, IntPtr pts);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_putText(IntPtr img, [MarshalAs(UnmanagedType.LPStr)] string text, Point org, int fontFace, double fontScale, Scalar color, int thickness, int lineType, int bottomLeftOrigin);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern Size core_getTextSize([MarshalAs(UnmanagedType.LPStr)] string text, int fontFace, double fontScale, int thickness, out int baseLine);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_createCLAHE(double clipLimit, Size tileGridSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Ptr_CLAHE_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_Ptr_CLAHE_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_CLAHE_apply(IntPtr obj, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_CLAHE_setClipLimit(IntPtr obj, double clipLimit);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_CLAHE_getClipLimit(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_CLAHE_setTilesGridSize(IntPtr obj, Size tileGridSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size imgproc_CLAHE_getTilesGridSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_CLAHE_collectGarbage(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setTemplate1(IntPtr obj, IntPtr templ, Point templCenter);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setTemplate2(IntPtr obj, IntPtr edges, IntPtr dx, IntPtr dy, Point templCenter);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_detect1(IntPtr obj, IntPtr image, IntPtr positions, IntPtr votes);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_detect2(IntPtr obj, IntPtr edges, IntPtr dx, IntPtr dy, IntPtr positions, IntPtr votes);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setCannyLowThresh(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHough_getCannyLowThresh(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setCannyHighThresh(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHough_getCannyHighThresh(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setMinDist(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHough_getMinDist(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setDp(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHough_getDp(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHough_setMaxBufferSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHough_getMaxBufferSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_createGeneralizedHoughBallard();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_Ptr_GeneralizedHoughBallard_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Ptr_GeneralizedHoughBallard_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughBallard_setLevels(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHoughBallard_getLevels(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughBallard_setVotesThreshold(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHoughBallard_getVotesThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_createGeneralizedHoughGuil();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_Ptr_GeneralizedHoughGuil_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Ptr_GeneralizedHoughGuil_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setXi(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getXi(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setLevels(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHoughGuil_getLevels(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setAngleEpsilon(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getAngleEpsilon(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setMinAngle(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getMinAngle(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setMaxAngle(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getMaxAngle(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setAngleStep(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getAngleStep(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setAngleThresh(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHoughGuil_getAngleThresh(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setMinScale(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getMinScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setMaxScale(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getMaxScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setScaleStep(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double imgproc_GeneralizedHoughGuil_getScaleStep(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setScaleThresh(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHoughGuil_getScaleThresh(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_GeneralizedHoughGuil_setPosThresh(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_GeneralizedHoughGuil_getPosThresh(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_LineIterator_new(IntPtr img, Point pt1, Point pt2, int connectivity, int leftToRight);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_LineIterator_operatorEntity(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_operatorPP(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point imgproc_LineIterator_pos(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_LineIterator_ptr_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_ptr_set(IntPtr obj, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_LineIterator_ptr0_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_step_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_step_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_elemSize_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_elemSize_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_err_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_err_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_count_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_count_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_minusDelta_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_minusDelta_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_plusDelta_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_plusDelta_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_minusStep_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_minusStep_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineIterator_plusStep_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineIterator_plusStep_set(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineSegmentDetector_detect_OutputArray(IntPtr obj, IntPtr image, IntPtr lines, IntPtr width, IntPtr prec, IntPtr nfa);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineSegmentDetector_detect_vector(IntPtr obj, IntPtr image, IntPtr lines, IntPtr width, IntPtr prec, IntPtr nfa);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_LineSegmentDetector_drawSegments(IntPtr obj, IntPtr image, IntPtr lines);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_LineSegmentDetector_compareSegments(IntPtr obj, Size size, IntPtr lines1, IntPtr lines2, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_createLineSegmentDetector(int refine, double scale, double sigma_scale, double quant, double ang_th, double log_eps, double density_th, int n_bins);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Ptr_LineSegmentDetector_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_Ptr_LineSegmentDetector_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_Subdiv2D_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgproc_Subdiv2D_new2(Rect rect);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_initDelaunay(IntPtr obj, Rect rect);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_insert1(IntPtr obj, Point2f pt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_insert2(IntPtr obj, [MarshalAs(UnmanagedType.LPArray)] Point2f[] ptArray, int length);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_locate(IntPtr obj, Point2f pt, out int edge, out int vertex);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_findNearest(IntPtr obj, Point2f pt, out Point2f nearestPt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_getEdgeList(IntPtr obj, out IntPtr edgeList);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_getTriangleList(IntPtr obj, out IntPtr triangleList);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_getVoronoiFacetList(IntPtr obj, [MarshalAs(UnmanagedType.LPArray)] int[] idx, int idxCount, out IntPtr facetList, out IntPtr facetCenters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void imgproc_Subdiv2D_getVoronoiFacetList(IntPtr obj, IntPtr idx, int idxCount, out IntPtr facetList, out IntPtr facetCenters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Point2f imgproc_Subdiv2D_getVertex(IntPtr obj, int vertex, out int firstEdge);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_getEdge(IntPtr obj, int edge, int nextEdgeType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_nextEdge(IntPtr obj, int edge);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_rotateEdge(IntPtr obj, int edge, int rotate);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_symEdge(IntPtr obj, int edge);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_edgeOrg(IntPtr obj, int edge, out Point2f orgpt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int imgproc_Subdiv2D_edgeDst(IntPtr obj, int edge, out Point2f dstpt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setTrainMethod(IntPtr obj, int method, double param1, double param2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_ANN_MLP_getTrainMethod(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setActivationFunction(IntPtr obj, int type, double param1, double param2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setLayerSizes(IntPtr obj, IntPtr layerSizes);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_ANN_MLP_getLayerSizes(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern TermCriteria ml_ANN_MLP_getTermCriteria(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setTermCriteria(IntPtr obj, TermCriteria val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getBackpropWeightScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setBackpropWeightScale(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getBackpropMomentumScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setBackpropMomentumScale(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getRpropDW0(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setRpropDW0(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getRpropDWPlus(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setRpropDWPlus(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getRpropDWMinus(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setRpropDWMinus(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getRpropDWMin(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setRpropDWMin(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_ANN_MLP_getRpropDWMax(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_ANN_MLP_setRpropDWMax(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_ANN_MLP_getWeights(IntPtr obj, int layerIdx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_ANN_MLP_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_ANN_MLP_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_ANN_MLP_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_ANN_MLP_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_ANN_MLP_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_Boost_getBoostType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Boost_setBoostType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_Boost_getWeakCount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Boost_setWeakCount(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_Boost_getWeightTrimRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Boost_setWeightTrimRate(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Boost_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_Boost_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_Boost_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Boost_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Boost_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getMaxCategories(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setMaxCategories(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getMaxDepth(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setMaxDepth(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getMinSampleCount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setMinSampleCount(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getCVFolds(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setCVFolds(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getUseSurrogates(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setUseSurrogates(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getUse1SERule(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setUse1SERule(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_DTrees_getTruncatePrunedTree(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setTruncatePrunedTree(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ml_DTrees_getRegressionAccuracy(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setRegressionAccuracy(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_DTrees_getPriors(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_setPriors(IntPtr obj, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_getRoots(IntPtr obj, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_getNodes(IntPtr obj, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_getSplits(IntPtr obj, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_DTrees_getSubsets(IntPtr obj, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_DTrees_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_DTrees_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_DTrees_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_DTrees_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_DTrees_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_EM_getClustersNumber(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_EM_setClustersNumber(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_EM_getCovarianceMatrixType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_EM_setCovarianceMatrixType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern TermCriteria ml_EM_getTermCriteria(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_EM_setTermCriteria(IntPtr obj, TermCriteria val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_EM_getWeights(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_EM_getMeans(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_EM_getCovs(IntPtr obj, IntPtr covs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec2d ml_EM_predict2(IntPtr model, IntPtr sample, IntPtr probs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_EM_trainEM(IntPtr obj, IntPtr samples, IntPtr logLikelihoods, IntPtr labels, IntPtr probs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_EM_trainE(IntPtr model, IntPtr samples, IntPtr means0, IntPtr covs0, IntPtr weights0, IntPtr logLikelihoods, IntPtr labels, IntPtr probs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_EM_trainM(IntPtr model, IntPtr samples, IntPtr probs0, IntPtr logLikelihoods, IntPtr labels, IntPtr probs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_EM_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_EM_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_EM_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_EM_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_EM_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_KNearest_getDefaultK(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_KNearest_setDefaultK(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_KNearest_getIsClassifier(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_KNearest_setIsClassifier(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_KNearest_getEmax(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_KNearest_setEmax(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_KNearest_getAlgorithmType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_KNearest_setAlgorithmType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ml_KNearest_findNearest(IntPtr obj, IntPtr samples, int k, IntPtr results, IntPtr neighborResponses, IntPtr dist);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_KNearest_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_KNearest_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_KNearest_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_KNearest_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_KNearest_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_LogisticRegression_getLearningRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_LogisticRegression_setLearningRate(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_LogisticRegression_getIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_LogisticRegression_setIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_LogisticRegression_getRegularization(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_LogisticRegression_setRegularization(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_LogisticRegression_getTrainMethod(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_LogisticRegression_setTrainMethod(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_LogisticRegression_getMiniBatchSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_LogisticRegression_setMiniBatchSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern TermCriteria ml_LogisticRegression_getTermCriteria(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_LogisticRegression_setTermCriteria(IntPtr obj, TermCriteria val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ml_LogisticRegression_predict(IntPtr obj, IntPtr samples, IntPtr results, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_LogisticRegression_get_learnt_thetas(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_LogisticRegression_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_LogisticRegression_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_LogisticRegression_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_LogisticRegression_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_LogisticRegression_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ml_NormalBayesClassifier_predictProb(IntPtr obj, IntPtr inputs, IntPtr samples, IntPtr outputProbs, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_NormalBayesClassifier_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_NormalBayesClassifier_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_NormalBayesClassifier_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_NormalBayesClassifier_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_NormalBayesClassifier_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_RTrees_getCalculateVarImportance(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_RTrees_setCalculateVarImportance(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_RTrees_getActiveVarCount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_RTrees_setActiveVarCount(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern TermCriteria ml_RTrees_getTermCriteria(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_RTrees_setTermCriteria(IntPtr obj, TermCriteria val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_RTrees_getVarImportance(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_RTrees_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_RTrees_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_RTrees_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_RTrees_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_RTrees_loadFromString(string strModel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_StatModel_clear(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_StatModel_getVarCount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_StatModel_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_StatModel_isTrained(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_StatModel_isClassifier(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_StatModel_train1(IntPtr obj, IntPtr trainData, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_StatModel_train2(IntPtr obj, IntPtr samples, int layout, IntPtr responses);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ml_StatModel_calcError(IntPtr obj, IntPtr data, int test, IntPtr resp);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ml_StatModel_predict(IntPtr obj, IntPtr samples, IntPtr results, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_StatModel_save(IntPtr obj, string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_SVM_getType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setType(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getGamma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setGamma(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getCoef0(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setCoef0(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getDegree(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setDegree(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getC(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setC(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getP(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setP(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getNu(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setNu(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_SVM_getClassWeights(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setClassWeights(IntPtr obj, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern TermCriteria ml_SVM_getTermCriteria(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setTermCriteria(IntPtr obj, TermCriteria val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ml_SVM_getKernelType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_SVM_setKernel(IntPtr obj, int kernelType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_SVM_getSupportVectors(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ml_SVM_getDecisionFunction(IntPtr obj, int i, IntPtr alpha, IntPtr svidx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern ParamGrid ml_SVM_getDefaultGrid(int paramId);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_SVM_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ml_Ptr_SVM_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_Ptr_SVM_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_SVM_load(string filePath);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ml_SVM_loadFromString(string strModel);

	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	static NativeMethods()
	{
		tried = false;
		RuntimeDllNames = new string[0];
		OpenCVDllNames = new string[0];
		ErrorHandlerThrowException = (ErrorCode status, string funcName, string errMsg, string fileName, int line, IntPtr userdata) =>
		{
			try
			{
				int num = 0;
			}
			finally
			{
				throw new OpenCVException(status, funcName, errMsg, fileName, line);
			}
		};
		ErrorHandlerIgnorance = (ErrorCode status, string funcName, string errMsg, string fileName, int line, IntPtr userdata) => 0;
		LoadLibraries(WindowsLibraryLoader.Instance.AdditionalPaths);
		TryPInvoke();
	}

	public static void LoadLibraries(IEnumerable<string> additionalPaths = null)
	{
		if (!IsUnix())
		{
			string[] array = ((additionalPaths == null) ? new string[0] : EnumerableEx.ToArray(additionalPaths));
			List<string> list = new List<string>(array);
			list.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
			string[] runtimeDllNames = RuntimeDllNames;
			foreach (string dllName in runtimeDllNames)
			{
				WindowsLibraryLoader.Instance.LoadLibrary(dllName, list);
			}
			runtimeDllNames = OpenCVDllNames;
			foreach (string text in runtimeDllNames)
			{
				WindowsLibraryLoader.Instance.LoadLibrary(text + "400", array);
			}
			WindowsLibraryLoader.Instance.LoadLibrary("OpenCvSharpExtern", array);
			IntPtr prevUserdata = IntPtr.Zero;
			redirectError(ErrorHandlerThrowException, prevUserdata, ref prevUserdata);
		}
	}

	private static void SetDefaultHandler(IntPtr currentHandler)
	{
		try
		{
			ErrorHandlerDefault = (CvErrorCallback)Marshal.GetDelegateForFunctionPointer(currentHandler, typeof(CvErrorCallback));
		}
		catch (NotSupportedException ex)
		{
			ErrorHandlerDefault = null;
			try
			{
				Console.WriteLine(ex.Message);
			}
			catch
			{
			}
			try
			{
			}
			catch
			{
			}
		}
	}

	public static void TryPInvoke()
	{
		if (tried)
		{
			return;
		}
		tried = true;
		try
		{
			core_Mat_sizeof();
		}
		catch (DllNotFoundException ex)
		{
			OpenCvSharpException ex2 = PInvokeHelper.CreateException(ex);
			try
			{
				Console.WriteLine(ex2.Message);
			}
			catch
			{
			}
			try
			{
			}
			catch
			{
			}
			throw ex2;
		}
		catch (BadImageFormatException ex3)
		{
			OpenCvSharpException ex4 = PInvokeHelper.CreateException(ex3);
			try
			{
				Console.WriteLine(ex4.Message);
			}
			catch
			{
			}
			try
			{
			}
			catch
			{
			}
			throw ex4;
		}
		catch (Exception ex5)
		{
			Exception ex6 = ex5.InnerException ?? ex5;
			try
			{
				Console.WriteLine(ex6.Message);
			}
			catch
			{
			}
			try
			{
			}
			catch
			{
			}
			throw;
		}
	}

	public static bool IsWindows()
	{
		return !IsUnix();
	}

	public static bool IsUnix()
	{
		PlatformID platform = Environment.OSVersion.Platform;
		if (platform != PlatformID.Unix && platform != PlatformID.MacOSX)
		{
			return platform == (PlatformID)128;
		}
		return true;
	}

	public static bool IsMono()
	{
		return Type.GetType("Mono.Runtime") != null;
	}

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr aruco_getPredefinedDictionary(int name);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_detectMarkers(IntPtr image, IntPtr dictionary, IntPtr corners, IntPtr ids, IntPtr detectParameters, IntPtr outrejectedImgPoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_drawDetectedMarkers(IntPtr image, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] corners, int cornerSize1, int[] contoursSize2, [MarshalAs(UnmanagedType.LPArray)] int[] ids, int idxLength, Scalar borderColor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_drawDetectedMarkers(IntPtr image, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] corners, int cornerSize1, int[] contoursSize2, IntPtr ids, int idxLength, Scalar borderColor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_drawMarker(IntPtr dictionary, int id, int sidePixels, IntPtr mat, int borderBits);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_estimatePoseSingleMarkers([MarshalAs(UnmanagedType.LPArray)] IntPtr[] corners, int cornersLength1, int[] cornersLengths2, float markerLength, IntPtr cameraMatrix, IntPtr distCoeffs, IntPtr rvecs, IntPtr tvecs, IntPtr objPoints);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr aruco_DetectorParameters_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_Ptr_DetectorParameters_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr aruco_Ptr_DetectorParameters_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setDoCornerRefinement(IntPtr obj, bool value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setAdaptiveThreshConstant(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setCornerRefinementMinAccuracy(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setErrorCorrectionRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMaxErroneousBitsInBorderRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMaxMarkerPerimeterRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMinCornerDistanceRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMinMarkerDistanceRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMinMarkerPerimeterRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMinOtsuStdDev(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setPerspectiveRemoveIgnoredMarginPerCell(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setPolygonalApproxAccuracyRate(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setAdaptiveThreshWinSizeMax(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setAdaptiveThreshWinSizeMin(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setAdaptiveThreshWinSizeStep(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setCornerRefinementMethod(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setCornerRefinementWinSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setCornerRefinementMaxIterations(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMarkerBorderBits(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setMinDistanceToBorder(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_DetectorParameters_setPerspectiveRemovePixelPerCell(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern bool aruco_DetectorParameters_getDoCornerRefinement(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getAdaptiveThreshConstant(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getCornerRefinementMinAccuracy(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getErrorCorrectionRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getMaxErroneousBitsInBorderRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getMaxMarkerPerimeterRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getMinCornerDistanceRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getMinMarkerDistanceRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getMinMarkerPerimeterRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getMinOtsuStdDev(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getPerspectiveRemoveIgnoredMarginPerCell(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double aruco_DetectorParameters_getPolygonalApproxAccuracyRate(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getAdaptiveThreshWinSizeMax(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getAdaptiveThreshWinSizeMin(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getAdaptiveThreshWinSizeStep(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getCornerRefinementMaxIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getCornerRefinementMethod(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getCornerRefinementWinSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getMarkerBorderBits(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getMinDistanceToBorder(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_DetectorParameters_getPerspectiveRemovePixelPerCell(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_Ptr_Dictionary_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr aruco_Ptr_Dictionary_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_Dictionary_setMarkerSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void aruco_Dictionary_setMaxCorrectionBits(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr aruco_Dictionary_getBytesList(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_Dictionary_getMarkerSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int aruco_Dictionary_getMaxCorrectionBits(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr bgsegm_createBackgroundSubtractorMOG(int history, int nmixtures, double backgroundRatio, double noiseSigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_Ptr_BackgroundSubtractorMOG_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr bgsegm_Ptr_BackgroundSubtractorMOG_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorMOG_getHistory(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorMOG_setHistory(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorMOG_getNMixtures(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorMOG_setNMixtures(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorMOG_getBackgroundRatio(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorMOG_setBackgroundRatio(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorMOG_getNoiseSigma(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorMOG_setNoiseSigma(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr bgsegm_createBackgroundSubtractorGMG(int initializationFrames, double decisionThreshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_Ptr_BackgroundSubtractorGMG_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr bgsegm_Ptr_BackgroundSubtractorGMG_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorGMG_getMaxFeatures(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setMaxFeatures(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorGMG_getDefaultLearningRate(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setDefaultLearningRate(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorGMG_getNumFrames(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setNumFrames(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorGMG_getQuantizationLevels(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setQuantizationLevels(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorGMG_getBackgroundPrior(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setBackgroundPrior(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorGMG_getSmoothingRadius(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setSmoothingRadius(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorGMG_getDecisionThreshold(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setDecisionThreshold(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int bgsegm_BackgroundSubtractorGMG_getUpdateBackgroundModel(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setUpdateBackgroundModel(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorGMG_getMinVal(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setMinVal(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double bgsegm_BackgroundSubtractorGMG_getMaxVal(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void bgsegm_BackgroundSubtractorGMG_setMaxVal(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Index_new(IntPtr features, IntPtr @params, int distType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_knnSearch1(IntPtr obj, [In] float[] queries, int queriesLength, [Out] int[] indices, [Out] float[] dists, int knn, IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_knnSearch2(IntPtr obj, IntPtr queries, IntPtr indices, IntPtr dists, int knn, IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_knnSearch3(IntPtr obj, IntPtr queries, [Out] int[] indices, [Out] float[] dists, int knn, IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_radiusSearch1(IntPtr obj, [In] float[] queries, int queriesLength, [Out] int[] indices, int indicesLength, [Out] float[] dists, int dists_length, float radius, int maxResults, IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_radiusSearch2(IntPtr obj, IntPtr queries, IntPtr indices, IntPtr dists, float radius, int maxResults, IntPtr @params);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Index_radiusSearch3(IntPtr obj, IntPtr queries, [Out] int[] indices, int indicesLength, [Out] float[] dists, int distsLength, float radius, int maxResults, IntPtr @params);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_Index_save(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_IndexParams_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_IndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_IndexParams_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_IndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_IndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_IndexParams_getString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string defaultVal, [MarshalAs(UnmanagedType.LPStr)] StringBuilder result);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int flann_IndexParams_getInt(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, int defaultVal);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern double flann_IndexParams_getDouble(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, double defaultVal);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_IndexParams_setString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_IndexParams_setInt(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, int value);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_IndexParams_setDouble(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, double value);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_IndexParams_setFloat(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, float value);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void flann_IndexParams_setBool(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string key, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_IndexParams_setAlgorithm(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_LinearIndexParams_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_LinearIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_LinearIndexParams_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_LinearIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_LinearIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_KDTreeIndexParams_new(int trees);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_KDTreeIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_KDTreeIndexParams_new(int trees);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_KDTreeIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_KDTreeIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_KMeansIndexParams_new(int branching, int iterations, [MarshalAs(UnmanagedType.I4)] FlannCentersInit centersInit, float cbIndex);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_KMeansIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_KMeansIndexParams_new(int branching, int iterations, [MarshalAs(UnmanagedType.I4)] FlannCentersInit centersInit, float cbIndex);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_KMeansIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_KMeansIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_LshIndexParams_new(int tableNumber, int keySize, int multiProbeLevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_LshIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_LshIndexParams_new(int tableNumber, int keySize, int multiProbeLevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_LshIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_LshIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_CompositeIndexParams_new(int trees, int branching, int iterations, FlannCentersInit centersInit, float cbIndex);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_CompositeIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_CompositeIndexParams_new(int trees, int branching, int iterations, FlannCentersInit centersInit, float cbIndex);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_CompositeIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_CompositeIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_AutotunedIndexParams_new(float targetPrecision, float buildWeight, float memoryWeight, float sampleFraction);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_AutotunedIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_AutotunedIndexParams_new(float targetPrecision, float buildWeight, float memoryWeight, float sampleFraction);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_AutotunedIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_AutotunedIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr flann_SavedIndexParams_new([MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_SavedIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_SavedIndexParams_new([MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_SavedIndexParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_SavedIndexParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_SearchParams_new(int checks, float eps, int sorted);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_SearchParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_SearchParams_new(int checks, float eps, int sorted);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr flann_Ptr_SearchParams_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void flann_Ptr_SearchParams_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_namedWindow([MarshalAs(UnmanagedType.LPStr)] string winname, int flags);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_destroyWindow([MarshalAs(UnmanagedType.LPStr)] string winName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void highgui_destroyAllWindows();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_imshow([MarshalAs(UnmanagedType.LPStr)] string winname, IntPtr mat);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int highgui_startWindowThread();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int highgui_waitKey(int delay);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int highgui_waitKeyEx(int delay);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_resizeWindow([MarshalAs(UnmanagedType.LPStr)] string winName, int width, int height);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_moveWindow([MarshalAs(UnmanagedType.LPStr)] string winName, int x, int y);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_setWindowProperty([MarshalAs(UnmanagedType.LPStr)] string winName, int propId, double propValue);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_setWindowTitle([MarshalAs(UnmanagedType.LPStr)] string winname, [MarshalAs(UnmanagedType.LPStr)] string title);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern double highgui_getWindowProperty([MarshalAs(UnmanagedType.LPStr)] string winName, int propId);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_setMouseCallback(string winName, [MarshalAs(UnmanagedType.FunctionPtr)] CvMouseCallback onMouse, IntPtr userData);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int highgui_getMouseWheelDelta(int flags);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int highgui_createTrackbar([MarshalAs(UnmanagedType.LPStr)] string trackbarName, [MarshalAs(UnmanagedType.LPStr)] string winName, ref int value, int count, IntPtr onChange, IntPtr userData);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int highgui_createTrackbar([MarshalAs(UnmanagedType.LPStr)] string trackbarName, [MarshalAs(UnmanagedType.LPStr)] string winName, ref int value, int count, [MarshalAs(UnmanagedType.FunctionPtr)] CvTrackbarCallback2 onChange, IntPtr userData);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int highgui_getTrackbarPos([MarshalAs(UnmanagedType.LPStr)] string trackbarName, [MarshalAs(UnmanagedType.LPStr)] string winName);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_setTrackbarPos(string trackbarName, string winName, int pos);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_setTrackbarMax([MarshalAs(UnmanagedType.LPStr)] string trackbarname, [MarshalAs(UnmanagedType.LPStr)] string winname, int maxval);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void highgui_setTrackbarMin([MarshalAs(UnmanagedType.LPStr)] string trackbarname, [MarshalAs(UnmanagedType.LPStr)] string winname, int minval);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int highgui_createButton([MarshalAs(UnmanagedType.LPStr)] string barName, IntPtr onChange, IntPtr userdata, int type, int initialButtonState);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr imgcodecs_imread(string filename, int flags);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int imgcodecs_imreadmulti([MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr mats, int flags);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int imgcodecs_imwrite([MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr img, [In] int[] @params, int paramsLength);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int imgcodecs_imwrite_multi([MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr img, [In] int[] @params, int paramsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgcodecs_imdecode_Mat(IntPtr buf, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgcodecs_imdecode_vector(byte[] buf, IntPtr bufLength, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr imgcodecs_imdecode_InputArray(IntPtr buf, int flags);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int imgcodecs_imencode_vector([MarshalAs(UnmanagedType.LPStr)] string ext, IntPtr img, IntPtr buf, [In] int[] @params, int paramsLength);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int imgcodecs_haveImageReader([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int imgcodecs_haveImageWriter([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_ImgHashBase_compute(IntPtr obj, IntPtr inputArr, IntPtr outputArr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double img_hash_ImgHashBase_compare(IntPtr obj, IntPtr hashOne, IntPtr hashTwo);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_AverageHash_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_Ptr_AverageHash_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_Ptr_AverageHash_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_BlockMeanHash_create(int mode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_Ptr_BlockMeanHash_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_Ptr_BlockMeanHash_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_BlockMeanHash_setMode(IntPtr obj, int mode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_BlockMeanHash_getMean(IntPtr obj, IntPtr outVec);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_ColorMomentHash_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_Ptr_ColorMomentHash_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_Ptr_ColorMomentHash_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_MarrHildrethHash_create(float alpha, float scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_Ptr_MarrHildrethHash_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_Ptr_MarrHildrethHash_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_MarrHildrethHash_setKernelParam(IntPtr obj, float alpha, float scale);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float img_hash_MarrHildrethHash_getAlpha(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float img_hash_MarrHildrethHash_getScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_PHash_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_Ptr_PHash_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_Ptr_PHash_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_RadialVarianceHash_create(double sigma, int numOfAngleLine);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_Ptr_RadialVarianceHash_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr img_hash_Ptr_RadialVarianceHash_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_RadialVarianceHash_setNumOfAngleLine(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void img_hash_RadialVarianceHash_setSigma(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int img_hash_RadialVarianceHash_getNumOfAngleLine(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double img_hash_RadialVarianceHash_getSigma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void optflow_motempl_updateMotionHistory(IntPtr silhouette, IntPtr mhi, double timestamp, double duration);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void optflow_motempl_calcMotionGradient(IntPtr mhi, IntPtr mask, IntPtr orientation, double delta1, double delta2, int apertureSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double optflow_motempl_calcGlobalOrientation(IntPtr orientation, IntPtr mask, IntPtr mhi, double timestamp, double duration);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void optflow_motempl_segmentMotion(IntPtr mhi, IntPtr segmask, IntPtr boundingRects, double timestamp, double segThresh);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void optflow_calcOpticalFlowSF1(IntPtr from, IntPtr to, IntPtr flow, int layers, int averagingBlockSize, int maxFlow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void optflow_calcOpticalFlowSF2(IntPtr from, IntPtr to, IntPtr flow, int layers, int averagingBlockSize, int maxFlow, double sigmaDist, double sigmaColor, int postprocessWindow, double sigmaDistFix, double sigmaColorFix, double occThr, int upscaleAveragingRadius, double upscaleSigmaDist, double upscaleSigmaColor, double speedUpThr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr string_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, ExactSpelling = true)]
	public static extern IntPtr string_new2([MarshalAs(UnmanagedType.LPStr)] string str);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void string_delete(IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern sbyte* string_c_str(IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr string_size(IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_uchar_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_uchar_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_uchar_new3([In] byte[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_uchar_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_uchar_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_uchar_copy(IntPtr vector, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_uchar_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_char_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_char_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_char_new3([In] sbyte[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_char_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_char_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_char_copy(IntPtr vector, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_char_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_int32_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_int32_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_int32_new3([In] int[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_int32_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_int32_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_int32_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_float_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_float_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_float_new3([In] float[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_float_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_float_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_float_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_double_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_double_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_double_new3([In] double[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_double_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_double_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_double_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec2f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec2f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec2f_new3([In] Vec2f[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec2f_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec2f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Vec2f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec3f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec3f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec3f_new3([In] Vec3f[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec3f_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec3f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Vec3f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4f_new3([In] Vec4f[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4f_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Vec4f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4i_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4i_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4i_new3([In] Vec4i[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4i_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec4i_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Vec4i_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6f_new3([In] Vec6f[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6f_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Vec6f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6d_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6d_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6d_new3([In] Vec6d[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6d_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Vec6d_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Vec6d_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2i_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2i_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2i_new3([In] Point[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2i_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2i_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Point2i_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2f_new3([In] Point2f[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2f_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point2f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Point2f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point3f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point3f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point3f_new3([In] Point3f[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point3f_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Point3f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Point3f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect_new3([In] Rect[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Rect_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect2d_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect2d_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect2d_new3([In] Rect2d[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect2d_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Rect2d_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Rect2d_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_RotatedRect_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_RotatedRect_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_RotatedRect_new3([In] RotatedRect[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_RotatedRect_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_RotatedRect_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_RotatedRect_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_KeyPoint_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_KeyPoint_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_KeyPoint_new3([In] KeyPoint[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_KeyPoint_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_KeyPoint_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_KeyPoint_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DMatch_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DMatch_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DMatch_new3([In] DMatch[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DMatch_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DMatch_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_DMatch_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_int_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_int_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_int_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_int_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_int_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_int_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_int_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_float_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_float_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_float_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_float_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_float_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_float_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_float_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_double_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_double_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_double_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_double_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_double_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_double_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_double_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_KeyPoint_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_KeyPoint_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_KeyPoint_new3(IntPtr[] values, int size1, int[] size2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_KeyPoint_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_KeyPoint_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_KeyPoint_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_KeyPoint_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_KeyPoint_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_DMatch_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_DMatch_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_DMatch_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_DMatch_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_DMatch_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_DMatch_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_DMatch_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_Point_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_Point_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_Point_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point2f_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point2f_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point2f_getSize1(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_Point2f_getSize2(IntPtr vector, [In][Out] IntPtr[] size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_vector_Point2f_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_Point2f_copy(IntPtr vec, IntPtr[] dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_vector_Point2f_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_string_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_string_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_string_getSize(IntPtr vec);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_string_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern sbyte* vector_string_elemAt(IntPtr vector, int i);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_string_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Mat_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Mat_new2(uint size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Mat_new3(IntPtr[] data, uint dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Mat_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_Mat_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Mat_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Mat_assignToArray(IntPtr vector, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] arr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_Mat_addref(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Node_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Node_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Node_new3([In] DTrees.Node[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Node_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Node_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_DTrees_Node_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Split_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Split_new2(IntPtr size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Split_new3([In] DTrees.Split[] data, IntPtr dataLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Split_getSize(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr vector_DTrees_Split_getPointer(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void vector_DTrees_Split_delete(IntPtr vector);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr stitching_Stitcher_create(int mode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Ptr_Stitcher_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr stitching_Ptr_Stitcher_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double stitching_Stitcher_registrationResol(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_setRegistrationResol(IntPtr obj, double resolMpx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double stitching_Stitcher_seamEstimationResol(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_setSeamEstimationResol(IntPtr obj, double resolMpx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double stitching_Stitcher_compositingResol(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_setCompositingResol(IntPtr obj, double resolMpx);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double stitching_Stitcher_panoConfidenceThresh(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_setPanoConfidenceThresh(IntPtr obj, double confThresh);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_waveCorrection(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_setWaveCorrection(IntPtr obj, int flag);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_waveCorrectKind(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_setWaveCorrectKind(IntPtr obj, int kind);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_estimateTransform_InputArray1(IntPtr obj, IntPtr images);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_estimateTransform_InputArray2(IntPtr obj, IntPtr images, IntPtr[] rois, int roisSize1, int[] roisSize2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_estimateTransform_MatArray1(IntPtr obj, IntPtr[] images, int imagesSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_estimateTransform_MatArray2(IntPtr obj, IntPtr[] images, int imagesSize, IntPtr[] rois, int roisSize1, int[] roisSize2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_composePanorama1(IntPtr obj, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_composePanorama2_InputArray(IntPtr obj, IntPtr images, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_composePanorama2_MatArray(IntPtr obj, IntPtr[] images, int imagesSize, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_stitch1_InputArray(IntPtr obj, IntPtr images, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_stitch1_MatArray(IntPtr obj, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] images, int imagesSize, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_stitch2_InputArray(IntPtr obj, IntPtr images, IntPtr[] rois, int roisSize1, int[] roisSize2, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int stitching_Stitcher_stitch2_MatArray(IntPtr obj, IntPtr[] images, int imagesSize, IntPtr[] rois, int roisSize1, int[] roisSize2, IntPtr pano);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void stitching_Stitcher_component(IntPtr obj, out IntPtr pointer, out int length);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double stitching_Stitcher_workScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_BaseOCR_run1(IntPtr obj, IntPtr image, IntPtr outputText, IntPtr componentRects, IntPtr componentTexts, IntPtr componentConfidences, int componentLevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_BaseOCR_run2(IntPtr obj, IntPtr image, IntPtr mask, IntPtr outputText, IntPtr componentRects, IntPtr componentTexts, IntPtr componentConfidences, int componentLevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_OCRTesseract_run1(IntPtr obj, IntPtr image, IntPtr outputText, IntPtr componentRects, IntPtr componentTexts, IntPtr componentConfidences, int componentLevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_OCRTesseract_run2(IntPtr obj, IntPtr image, IntPtr mask, IntPtr outputText, IntPtr componentRects, IntPtr componentTexts, IntPtr componentConfidences, int componentLevel);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_OCRTesseract_run3(IntPtr obj, IntPtr image, int minConfidence, int componentLevel, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_OCRTesseract_run4(IntPtr obj, IntPtr image, IntPtr mask, int minConfidence, int componentLevel, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_OCRTesseract_setWhiteList(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string charWhitelist);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr text_OCRTesseract_create([MarshalAs(UnmanagedType.LPStr)] string datapath, [MarshalAs(UnmanagedType.LPStr)] string language, [MarshalAs(UnmanagedType.LPStr)] string charWhitelist, int oem, int psmode);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void text_Ptr_OCRTesseract_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr text_OCRTesseract_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr videoio_VideoCapture_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr videoio_VideoCapture_new2([MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr videoio_VideoCapture_new3(int device);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoCapture_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_open1(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_open2(IntPtr obj, int device);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_isOpened(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoCapture_release(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_grab(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_retrieve(IntPtr obj, IntPtr image, int flag);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoCapture_operatorRightShift_Mat(IntPtr obj, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoCapture_operatorRightShift_UMat(IntPtr obj, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_read(IntPtr obj, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoCapture_set(IntPtr obj, int propId, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double videoio_VideoCapture_get(IntPtr obj, int propId);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr videoio_VideoWriter_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr videoio_VideoWriter_new2([MarshalAs(UnmanagedType.LPStr)] string filename, int fourcc, double fps, Size frameSize, int isColor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoWriter_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoWriter_open(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string filename, int fourcc, double fps, Size frameSize, int isColor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoWriter_isOpened(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoWriter_release(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoWriter_OperatorLeftShift(IntPtr obj, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void videoio_VideoWriter_write(IntPtr obj, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoWriter_set(IntPtr obj, int propId, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double videoio_VideoWriter_get(IntPtr obj, int propId);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int videoio_VideoWriter_fourcc(byte c1, byte c2, byte c3, byte c4);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_BriefDescriptorExtractor_create(int bytes);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_BriefDescriptorExtractor_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_BriefDescriptorExtractor_read(IntPtr obj, IntPtr fn);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_BriefDescriptorExtractor_write(IntPtr obj, IntPtr fs);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xfeatures2d_BriefDescriptorExtractor_descriptorSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xfeatures2d_BriefDescriptorExtractor_descriptorType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_BriefDescriptorExtractor_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_FREAK_create(int orientationNormalized, int scaleNormalized, float patternScale, int nOctaves, int[] selectedPairs, int selectedPairsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_FREAK_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_FREAK_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_StarDetector_create(int maxSize, int responseThreshold, int lineThresholdProjected, int lineThresholdBinarized, int suppressNonmaxSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_StarDetector_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_StarDetector_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_LUCID_create(int lucid_kernel = 1, int blur_kernel = 2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_LUCID_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_LUCID_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_LATCH_create(int bytes, int rotationInvariance, int half_ssd_size, double sigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_LATCH_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_LATCH_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_SURF_create(double hessianThreshold, int nOctaves, int nOctaveLayers, int extended, int upright);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_SURF_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_SURF_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double xfeatures2d_SURF_getHessianThreshold(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xfeatures2d_SURF_getNOctaves(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xfeatures2d_SURF_getNOctaveLayers(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xfeatures2d_SURF_getExtended(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xfeatures2d_SURF_getUpright(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_SURF_setHessianThreshold(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_SURF_setNOctaves(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_SURF_setNOctaveLayers(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_SURF_setExtended(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_SURF_setUpright(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_SIFT_create(int nfeatures, int nOctaveLayers, double contrastThreshold, double edgeThreshold, double sigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xfeatures2d_Ptr_SIFT_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xfeatures2d_Ptr_SIFT_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_inpaint(IntPtr prt, IntPtr src, IntPtr dst, int algorithm);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_applyChannelGains(IntPtr src, IntPtr dst, float gainB, float gainG, float gainR);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_createGrayworldWB();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_Ptr_GrayworldWB_delete(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_Ptr_GrayworldWB_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_GrayworldWB_balanceWhite(IntPtr prt, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_GrayworldWB_SaturationThreshold_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_GrayworldWB_SaturationThreshold_set(IntPtr ptr, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_createLearningBasedWB(string trackerType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_Ptr_LearningBasedWB_delete(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_Ptr_LearningBasedWB_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_LearningBasedWB_balanceWhite(IntPtr prt, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_LearningBasedWB_extractSimpleFeatures(IntPtr prt, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_LearningBasedWB_HistBinNum_set(IntPtr prt, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_LearningBasedWB_RangeMaxVal_set(IntPtr prt, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_LearningBasedWB_SaturationThreshold_set(IntPtr prt, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xphoto_LearningBasedWB_HistBinNum_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int xphoto_LearningBasedWB_RangeMaxVal_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_LearningBasedWB_SaturationThreshold_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_createSimpleWB();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_Ptr_SimpleWB_delete(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr xphoto_Ptr_SimpleWB_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_SimpleWB_balanceWhite(IntPtr prt, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_InputMax_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_InputMax_set(IntPtr prt, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_InputMin_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_InputMin_set(IntPtr prt, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_OutputMax_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_OutputMax_set(IntPtr prt, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_OutputMin_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_OutputMin_set(IntPtr prt, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_P_get(IntPtr prt);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float xphoto_SimpleWB_P_set(IntPtr prt, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_dctDenoising(IntPtr src, IntPtr dst, double sigma, int psize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_bm3dDenoising1(IntPtr src, IntPtr dstStep1, IntPtr dstStep2, float h, int templateWindowSize, int searchWindowSize, int blockMatchingStep1, int blockMatchingStep2, int groupSize, int slidingStep, float beta, int normType, int step, int transformType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void xphoto_bm3dDenoising2(IntPtr src, IntPtr dst, float h, int templateWindowSize, int searchWindowSize, int blockMatchingStep1, int blockMatchingStep2, int groupSize, int slidingStep, float beta, int normType, int step, int transformType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_groupRectangles1(IntPtr rectList, int groupThreshold, double eps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_groupRectangles2(IntPtr rectList, IntPtr weights, int groupThreshold, double eps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_groupRectangles3(IntPtr rectList, int groupThreshold, double eps, IntPtr weights, IntPtr levelWeights);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_groupRectangles4(IntPtr rectList, IntPtr rejectLevels, IntPtr levelWeights, int groupThreshold, double eps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_groupRectangles_meanshift(IntPtr rectList, IntPtr foundWeights, IntPtr foundScales, double detectThreshold, Size winDetSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr objdetect_CascadeClassifier_new();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr objdetect_CascadeClassifier_newFromFile([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_CascadeClassifier_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_CascadeClassifier_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int objdetect_CascadeClassifier_load(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_CascadeClassifier_detectMultiScale1(IntPtr obj, IntPtr image, IntPtr objects, double scaleFactor, int minNeighbors, int flags, Size minSize, Size maxSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_CascadeClassifier_detectMultiScale2(IntPtr obj, IntPtr image, IntPtr objects, IntPtr rejectLevels, IntPtr levelWeights, double scaleFactor, int minNeighbors, int flags, Size minSize, Size maxSize, int outputRejectLevels);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_CascadeClassifier_isOldFormatCascade(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size objdetect_CascadeClassifier_getOriginalWindowSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_CascadeClassifier_getFeatureType(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_sizeof();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr objdetect_HOGDescriptor_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr objdetect_HOGDescriptor_new2(Size winSize, Size blockSize, Size blockStride, Size cellSize, int nbins, int derivAperture, double winSigma, [MarshalAs(UnmanagedType.I4)] HistogramNormType histogramNormType, double l2HysThreshold, int gammaCorrection, int nlevels);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr objdetect_HOGDescriptor_new3([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_delete(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr objdetect_HOGDescriptor_getDescriptorSize(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_checkDetectorSize(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double objdetect_HOGDescriptor_getWinSigma(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_setSVMDetector(IntPtr self, IntPtr svmdetector);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern bool objdetect_HOGDescriptor_load(IntPtr self, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string objname);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void objdetect_HOGDescriptor_save(IntPtr self, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string objname);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_compute(IntPtr self, IntPtr img, IntPtr descriptors, Size winStride, Size padding, [In] Point[] locations, int locationsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_detect1(IntPtr self, IntPtr img, IntPtr foundLocations, double hitThreshold, Size winStride, Size padding, [In] Point[] searchLocations, int searchLocationsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_detect2(IntPtr self, IntPtr img, IntPtr foundLocations, IntPtr weights, double hitThreshold, Size winStride, Size padding, [In] Point[] searchLocations, int searchLocationsLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_detectMultiScale1(IntPtr self, IntPtr img, IntPtr foundLocations, double hitThreshold, Size winStride, Size padding, double scale, int groupThreshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_detectMultiScale2(IntPtr self, IntPtr img, IntPtr foundLocations, IntPtr foundWeights, double hitThreshold, Size winStride, Size padding, double scale, int groupThreshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_computeGradient(IntPtr self, IntPtr img, IntPtr grad, IntPtr angleOfs, Size paddingTL, Size paddingBR);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_detectROI(IntPtr obj, IntPtr img, Point[] locations, int locationsLength, IntPtr foundLocations, IntPtr confidences, double hitThreshold, Size winStride, Size padding);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_detectMultiScaleROI(IntPtr obj, IntPtr img, IntPtr foundLocations, IntPtr roiScales, IntPtr roiLocations, IntPtr roiConfidences, double hitThreshold, int groupThreshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_groupRectangles(IntPtr obj, IntPtr rectList, IntPtr weights, int groupThreshold, double eps);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size objdetect_HOGDescriptor_winSize_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size objdetect_HOGDescriptor_blockSize_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size objdetect_HOGDescriptor_blockStride_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Size objdetect_HOGDescriptor_cellSize_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_nbins_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_derivAperture_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double objdetect_HOGDescriptor_winSigma_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_histogramNormType_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double objdetect_HOGDescriptor_L2HysThreshold_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_gammaCorrection_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_HOGDescriptor_nlevels_get(IntPtr self);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_winSize_set(IntPtr self, Size value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_blockSize_set(IntPtr self, Size value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_blockStride_set(IntPtr self, Size value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_cellSize_set(IntPtr self, Size value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_nbins_set(IntPtr self, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_derivAperture_set(IntPtr self, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_winSigma_set(IntPtr self, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_histogramNormType_set(IntPtr self, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_L2HysThreshold_set(IntPtr self, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_gammaCorrection_set(IntPtr self, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_HOGDescriptor_nlevels_set(IntPtr self, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr objdetect_LatentSvmDetector_new();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_LatentSvmDetector_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_LatentSvmDetector_clear(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_LatentSvmDetector_empty(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int objdetect_LatentSvmDetector_load(IntPtr obj, IntPtr[] fileNames, int fileNamesLength, IntPtr[] classNames, int classNamesLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_LatentSvmDetector_detect(IntPtr obj, IntPtr image, IntPtr objectDetections, float overlapThreshold, int numThreads);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void objdetect_LatentSvmDetector_getClassNames(IntPtr obj, IntPtr outValues);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr objdetect_LatentSvmDetector_getClassCount(IntPtr obj);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr objdetect_QRCodeDetector_new();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void objdetect_QRCodeDetector_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void objdetect_QRCodeDetector_setEpsX(IntPtr obj, double epsX);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void objdetect_QRCodeDetector_setEpsY(IntPtr obj, double epsY);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern int objdetect_QRCodeDetector_detect(IntPtr obj, IntPtr img, IntPtr points);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void objdetect_QRCodeDetector_decode(IntPtr obj, IntPtr img, IntPtr points, IntPtr straight_qrcode, IntPtr result);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void objdetect_QRCodeDetector_detectAndDecode(IntPtr obj, IntPtr img, IntPtr points, IntPtr straight_qrcode, IntPtr result);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_inpaint(IntPtr src, IntPtr inpaintMask, IntPtr dst, double inpaintRadius, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_fastNlMeansDenoising(IntPtr src, IntPtr dst, float h, int templateWindowSize, int searchWindowSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_fastNlMeansDenoisingColored(IntPtr src, IntPtr dst, float h, float hColor, int templateWindowSize, int searchWindowSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_fastNlMeansDenoisingMulti(IntPtr[] srcImgs, int srcImgsLength, IntPtr dst, int imgToDenoiseIndex, int temporalWindowSize, float h, int templateWindowSize, int searchWindowSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_fastNlMeansDenoisingColoredMulti(IntPtr[] srcImgs, int srcImgsLength, IntPtr dst, int imgToDenoiseIndex, int temporalWindowSize, float h, float hColor, int templateWindowSize, int searchWindowSize);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_denoise_TVL1(IntPtr[] observations, int observationsSize, IntPtr result, double lambda, int niters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_decolor(IntPtr src, IntPtr grayscale, IntPtr color_boost);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_seamlessClone(IntPtr src, IntPtr dst, IntPtr mask, Point p, IntPtr blend, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_colorChange(IntPtr src, IntPtr mask, IntPtr dst, float red_mul, float green_mul, float blue_mul);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_illuminationChange(IntPtr src, IntPtr mask, IntPtr dst, float alpha, float beta);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_textureFlattening(IntPtr src, IntPtr mask, IntPtr dst, float low_threshold, float high_threshold, int kernel_size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_edgePreservingFilter(IntPtr src, IntPtr dst, int flags, float sigma_s, float sigma_r);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_detailEnhance(IntPtr src, IntPtr dst, float sigma_s, float sigma_r);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_pencilSketch(IntPtr src, IntPtr dst1, IntPtr dst2, float sigma_s, float sigma_r, float shade_factor);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_stylization(IntPtr src, IntPtr dst, float sigma_s, float sigma_r);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr photo_createCalibrateDebevec(int samples, float lambda, int random);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr photo_createCalibrateRobertson(int maxIter, float threshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_Ptr_CalibrateDebevec_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_Ptr_CalibrateRobertson_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr photo_Ptr_CalibrateDebevec_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr photo_Ptr_CalibrateRobertson_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void photo_CalibrateCRF_process(IntPtr obj, IntPtr[] srcImgs, int srcImgsLength, IntPtr dst, [In][MarshalAs(UnmanagedType.LPArray)] float[] times);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern void core_putTextZh(IntPtr img, [MarshalAs(UnmanagedType.LPStr)] string text, Point org, int fontScale, Scalar color, [MarshalAs(UnmanagedType.LPStr)] string font, int italic, int underline);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeDistanceExtractor_computeDistance(IntPtr obj, IntPtr contour1, IntPtr contour2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_Ptr_ShapeContextDistanceExtractor_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr shape_Ptr_ShapeContextDistanceExtractor_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setAngularBins(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int shape_ShapeContextDistanceExtractor_getAngularBins(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setRadialBins(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int shape_ShapeContextDistanceExtractor_getRadialBins(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setInnerRadius(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeContextDistanceExtractor_getInnerRadius(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setOuterRadius(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeContextDistanceExtractor_getOuterRadius(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setRotationInvariant(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int shape_ShapeContextDistanceExtractor_getRotationInvariant(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setShapeContextWeight(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeContextDistanceExtractor_getShapeContextWeight(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setImageAppearanceWeight(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeContextDistanceExtractor_getImageAppearanceWeight(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setBendingEnergyWeight(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeContextDistanceExtractor_getBendingEnergyWeight(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setImages(IntPtr obj, IntPtr image1, IntPtr image2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_getImages(IntPtr obj, IntPtr image1, IntPtr image2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int shape_ShapeContextDistanceExtractor_getIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_ShapeContextDistanceExtractor_setStdDev(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_ShapeContextDistanceExtractor_getStdDev(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr shape_createShapeContextDistanceExtractor(int nAngularBins, int nRadialBins, float innerRadius, float outerRadius, int iterations);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_Ptr_HausdorffDistanceExtractor_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr shape_Ptr_HausdorffDistanceExtractor_get(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_HausdorffDistanceExtractor_setDistanceFlag(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int shape_HausdorffDistanceExtractor_getDistanceFlag(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void shape_HausdorffDistanceExtractor_setRankProportion(IntPtr obj, float val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float shape_HausdorffDistanceExtractor_getRankProportion(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr shape_createHausdorffDistanceExtractor(int distanceFlag, float rankProp);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DenseOpticalFlowExt_calc(IntPtr obj, IntPtr frame0, IntPtr frame1, IntPtr flow1, IntPtr flow2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DenseOpticalFlowExt_collectGarbage(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createOptFlow_Farneback();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createOptFlow_Farneback_CUDA();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_Ptr_FarnebackOpticalFlow_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_Ptr_FarnebackOpticalFlow_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_FarnebackOpticalFlow_getPyrScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setPyrScale(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_FarnebackOpticalFlow_getLevelsNumber(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setLevelsNumber(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_FarnebackOpticalFlow_getWindowSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setWindowSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_FarnebackOpticalFlow_getIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_FarnebackOpticalFlow_getPolyN(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setPolyN(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_FarnebackOpticalFlow_getPolySigma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setPolySigma(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_FarnebackOpticalFlow_getFlags(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FarnebackOpticalFlow_setFlags(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createOptFlow_DualTVL1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createOptFlow_DualTVL1_CUDA();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_Ptr_DualTVL1OpticalFlow_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_Ptr_DualTVL1OpticalFlow_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_DualTVL1OpticalFlow_getTau(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setTau(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_DualTVL1OpticalFlow_getLambda(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setLambda(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_DualTVL1OpticalFlow_getTheta(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setTheta(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_DualTVL1OpticalFlow_getScalesNumber(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setScalesNumber(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_DualTVL1OpticalFlow_getWarpingsNumber(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setWarpingsNumber(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_DualTVL1OpticalFlow_getEpsilon(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setEpsilon(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_DualTVL1OpticalFlow_getIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_DualTVL1OpticalFlow_getUseInitialFlow(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_DualTVL1OpticalFlow_setUseInitialFlow(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createOptFlow_Brox_CUDA();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_Ptr_BroxOpticalFlow_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_Ptr_BroxOpticalFlow_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_BroxOpticalFlow_getAlpha(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_BroxOpticalFlow_setAlpha(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_BroxOpticalFlow_getGamma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_BroxOpticalFlow_setGamma(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_BroxOpticalFlow_getScaleFactor(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_BroxOpticalFlow_setScaleFactor(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_BroxOpticalFlow_getInnerIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_BroxOpticalFlow_setInnerIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_BroxOpticalFlow_getOuterIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_BroxOpticalFlow_setOuterIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_BroxOpticalFlow_getSolverIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_BroxOpticalFlow_setSolverIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createOptFlow_PyrLK_CUDA();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_Ptr_PyrLKOpticalFlow_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_Ptr_PyrLKOpticalFlow_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_PyrLKOpticalFlow_getWindowSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_PyrLKOpticalFlow_setWindowSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_PyrLKOpticalFlow_getMaxLevel(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_PyrLKOpticalFlow_setMaxLevel(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_PyrLKOpticalFlow_getIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_PyrLKOpticalFlow_setIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FrameSource_nextFrame(IntPtr obj, IntPtr frame);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_FrameSource_reset(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createFrameSource_Empty();

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr superres_createFrameSource_Video([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr superres_createFrameSource_Video_CUDA([MarshalAs(UnmanagedType.LPStr)] string fileName);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createFrameSource_Camera(int deviceId);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_Ptr_FrameSource_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_Ptr_FrameSource_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setInput(IntPtr obj, IntPtr frameSource);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_nextFrame(IntPtr obj, IntPtr frame);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_reset(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_collectGarbage(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createSuperResolution_BTVL1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_createSuperResolution_BTVL1_CUDA();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_Ptr_SuperResolution_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_Ptr_SuperResolution_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_SuperResolution_getScale(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setScale(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_SuperResolution_getIterations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setIterations(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_SuperResolution_getTau(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setTau(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_SuperResolution_getLambda(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setLambda(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_SuperResolution_getAlpha(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setAlpha(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_SuperResolution_getKernelSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setKernelSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_SuperResolution_getBlurKernelSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setBlurKernelSize(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double superres_SuperResolution_getBlurSigma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setBlurSigma(IntPtr obj, double val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int superres_SuperResolution_getTemporalAreaRadius(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setTemporalAreaRadius(IntPtr obj, int val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr superres_SuperResolution_getOpticalFlow(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void superres_SuperResolution_setOpticalFlow(IntPtr obj, IntPtr val);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool tracking_Tracker_init(IntPtr obj, IntPtr image, Rect2d boundingBox);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool tracking_Tracker_update(IntPtr obj, IntPtr image, ref Rect2d boundingBox);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_Tracker_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_Tracker_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerKCF_create1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerKCF_create2(TrackerKCF.Params parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerKCF_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerKCF_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerMIL_create1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern IntPtr tracking_TrackerMIL_create2(TrackerMIL.Params* parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerMIL_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerMIL_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerBoosting_create1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern IntPtr tracking_TrackerBoosting_create2(TrackerBoosting.Params* parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerBoosting_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerBoosting_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerMedianFlow_create1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern IntPtr tracking_TrackerMedianFlow_create2(TrackerMedianFlow.Params* parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerMedianFlow_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerMedianFlow_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerTLD_create1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern IntPtr tracking_TrackerTLD_create2(TrackerTLD.Params* parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerTLD_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerTLD_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerGOTURN_create1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public unsafe static extern IntPtr tracking_TrackerGOTURN_create2(TrackerGOTURN.Params* parameters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerGOTURN_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerGOTURN_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_TrackerMOSSE_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_TrackerMOSSE_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_TrackerMOSSE_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_MultiTracker_create();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_Ptr_MultiTracker_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr tracking_Ptr_MultiTracker_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int tracking_MultiTracker_add1(IntPtr obj, IntPtr newTracker, IntPtr image, Rect2d boundingBox);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int tracking_MultiTracker_add2(IntPtr obj, IntPtr[] newTrackers, int newTrackersLength, IntPtr image, Rect2d[] boundingBox, int boundingBoxLength);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int tracking_MultiTracker_update1(IntPtr obj, IntPtr image);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int tracking_MultiTracker_update2(IntPtr obj, IntPtr image, IntPtr boundingBox);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void tracking_MultiTracker_getObjects(IntPtr obj, IntPtr boundingBox);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern RotatedRect video_CamShift(IntPtr probImage, ref Rect window, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_meanShift(IntPtr probImage, ref Rect window, TermCriteria criteria);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_new1();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_new2(int dynamParams, int measureParams, int controlParams, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_KalmanFilter_init(IntPtr obj, int dynamParams, int measureParams, int controlParams, int type);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_KalmanFilter_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_predict(IntPtr obj, IntPtr control);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_correct(IntPtr obj, IntPtr measurement);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_statePre(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_statePost(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_transitionMatrix(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_controlMatrix(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_measurementMatrix(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_processNoiseCov(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_measurementNoiseCov(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_errorCovPre(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_gain(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_KalmanFilter_errorCovPost(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_buildOpticalFlowPyramid1(IntPtr img, IntPtr pyramid, Size winSize, int maxLevel, int withDerivatives, int pyrBorder, int derivBorder, int tryReuseInputImage);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_buildOpticalFlowPyramid2(IntPtr img, IntPtr pyramidVec, Size winSize, int maxLevel, int withDerivatives, int pyrBorder, int derivBorder, int tryReuseInputImage);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_calcOpticalFlowPyrLK_InputArray(IntPtr prevImg, IntPtr nextImg, IntPtr prevPts, IntPtr nextPts, IntPtr status, IntPtr err, Size winSize, int maxLevel, TermCriteria criteria, int flags, double minEigThreshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_calcOpticalFlowPyrLK_vector(IntPtr prevImg, IntPtr nextImg, Point2f[] prevPts, int prevPtsSize, IntPtr nextPts, IntPtr status, IntPtr err, Size winSize, int maxLevel, TermCriteria criteria, int flags, double minEigThreshold);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_calcOpticalFlowFarneback(IntPtr prev, IntPtr next, IntPtr flow, double pyrScale, int levels, int winSize, int iterations, int polyN, double polySigma, int flags);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_DenseOpticalFlow_calc(IntPtr obj, IntPtr i0, IntPtr i1, IntPtr flow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_DenseOpticalFlow_collectGarbage(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_Ptr_DenseOpticalFlow_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_Ptr_DenseOpticalFlow_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractor_getBackgroundImage(IntPtr self, IntPtr backgroundImage);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractor_apply(IntPtr self, IntPtr image, IntPtr fgmask, double learningRate);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_Ptr_BackgroundSubtractor_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_Ptr_BackgroundSubtractor_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_createBackgroundSubtractorMOG2(int history, double varThreshold, int detectShadows);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_Ptr_BackgroundSubtractorMOG2_delete(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_Ptr_BackgroundSubtractorMOG2_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorMOG2_getHistory(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setHistory(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorMOG2_getNMixtures(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setNMixtures(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getBackgroundRatio(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setBackgroundRatio(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getVarThreshold(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setVarThreshold(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getVarThresholdGen(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setVarThresholdGen(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getVarInit(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setVarInit(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getVarMin(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setVarMin(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getVarMax(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setVarMax(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getComplexityReductionThreshold(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setComplexityReductionThreshold(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorMOG2_getDetectShadows(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setDetectShadows(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorMOG2_getShadowValue(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setShadowValue(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorMOG2_getShadowThreshold(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorMOG2_setShadowThreshold(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_createBackgroundSubtractorKNN(int history, double dist2Threshold, int detectShadows);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_Ptr_BackgroundSubtractorKNN_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr video_Ptr_BackgroundSubtractorKNN_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorKNN_getHistory(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setHistory(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorKNN_getNSamples(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setNSamples(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorKNN_getDist2Threshold(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setDist2Threshold(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorKNN_getkNNSamples(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setkNNSamples(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorKNN_getDetectShadows(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setDetectShadows(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int video_BackgroundSubtractorKNN_getShadowValue(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setShadowValue(IntPtr ptr, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double video_BackgroundSubtractorKNN_getShadowThreshold(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void video_BackgroundSubtractorKNN_setShadowThreshold(IntPtr ptr, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_niBlackThreshold(IntPtr src, IntPtr dst, double maxValue, int type, int blockSize, double delta);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_thinning(IntPtr src, IntPtr dst, int thinningType);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_anisotropicDiffusion(IntPtr src, IntPtr dst, float alpha, float K, int niters);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_weightedMedianFilter(IntPtr joint, IntPtr src, IntPtr dst, int r, double sigma, int weightType, IntPtr mask);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_covarianceEstimation(IntPtr src, IntPtr dst, int windowRows, int windowCols);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_getBoundingBoxes(IntPtr obj, IntPtr edge_map, IntPtr orientation_map, IntPtr boxes);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getAlpha(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setAlpha(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getBeta(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setBeta(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getEta(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setEta(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getMinScore(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setMinScore(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ximgproc_EdgeBoxes_getMaxBoxes(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setMaxBoxes(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getEdgeMinMag(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setEdgeMinMag(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getEdgeMergeThr(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setEdgeMergeThr(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getClusterMinMag(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setClusterMinMag(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getMaxAspectRatio(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setMaxAspectRatio(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getMinBoxArea(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setMinBoxArea(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getGamma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setGamma(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_EdgeBoxes_getKappa(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_EdgeBoxes_setKappa(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_createEdgeBoxes(float alpha, float beta, float eta, float minScore, int maxBoxes, float edgeMinMag, float edgeMergeThr, float clusterMinMag, float maxAspectRatio, float minBoxArea, float gamma, float kappa);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_Ptr_EdgeBoxes_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_Ptr_EdgeBoxes_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_FastHoughTransform(IntPtr src, IntPtr dst, int dstMatDepth, int angleRange, int op, int makeSkew);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern Vec4i ximgproc_HoughPoint2Line(Point houghPoint, IntPtr srcImgInfo, int angleRange, int makeSkew, int rules);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_FastLineDetector_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_Ptr_FastLineDetector_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_FastLineDetector_detect_OutputArray(IntPtr obj, IntPtr image, IntPtr lines);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_FastLineDetector_detect_vector(IntPtr obj, IntPtr image, IntPtr lines);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_FastLineDetector_drawSegments_InputArray(IntPtr obj, IntPtr image, IntPtr lines, int draw_arrow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_FastLineDetector_drawSegments_vector(IntPtr obj, IntPtr image, IntPtr lines, int draw_arrow);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_createFastLineDetector(int length_threshold, float distance_threshold, double canny_th1, double canny_th2, int canny_aperture_size, int do_merge);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createGraphSegmentation(double sigma, float k, int min_size);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_GraphSegmentation_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_GraphSegmentation_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_GraphSegmentation_processImage(IntPtr obj, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_GraphSegmentation_setSigma(IntPtr obj, double value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern double ximgproc_segmentation_GraphSegmentation_getSigma(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_GraphSegmentation_setK(IntPtr obj, float value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_segmentation_GraphSegmentation_getK(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_GraphSegmentation_setMinSize(IntPtr obj, int value);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern int ximgproc_segmentation_GraphSegmentation_getMinSize(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentationStrategy_setImage(IntPtr obj, IntPtr img, IntPtr regions, IntPtr sizes, int image_id);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern float ximgproc_segmentation_SelectiveSearchSegmentationStrategy_get(IntPtr obj, int r1, int r2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentationStrategy_merge(IntPtr obj, int r1, int r2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyColor();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategySize();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyTexture();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyFill();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyColor_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategySize_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyTexture_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyFill_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyColor_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategySize_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyTexture_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyFill_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentationStrategyMultiple_addStrategy(IntPtr obj, IntPtr g, float weight);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentationStrategyMultiple_clearStrategies(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyMultiple0();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyMultiple1(IntPtr s1);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyMultiple2(IntPtr s1, IntPtr s2);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyMultiple3(IntPtr s1, IntPtr s2, IntPtr s3);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentationStrategyMultiple4(IntPtr s1, IntPtr s2, IntPtr s3, IntPtr s4);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyMultiple_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_SelectiveSearchSegmentationStrategyMultiple_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_setBaseImage(IntPtr obj, IntPtr img);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_switchToSingleStrategy(IntPtr obj, int k, float sigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_switchToSelectiveSearchFast(IntPtr obj, int base_k, int inc_k, float sigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_switchToSelectiveSearchQuality(IntPtr obj, int base_k, int inc_k, float sigma);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_addImage(IntPtr obj, IntPtr img);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_clearImages(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_addGraphSegmentation(IntPtr obj, IntPtr g);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_clearGraphSegmentations(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_addStrategy(IntPtr obj, IntPtr s);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_clearStrategies(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_SelectiveSearchSegmentation_process(IntPtr obj, IntPtr rects);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_createSelectiveSearchSegmentation();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_segmentation_Ptr_SelectiveSearchSegmentation_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_segmentation_Ptr_SelectiveSearchSegmentation_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_createRFFeatureGetter();

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_Ptr_RFFeatureGetter_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_Ptr_RFFeatureGetter_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_RFFeatureGetter_getFeatures(IntPtr obj, IntPtr src, IntPtr features, int gnrmRad, int gsmthRad, int shrink, int outNum, int gradNum);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_createStructuredEdgeDetection([MarshalAs(UnmanagedType.LPStr)] string model, IntPtr howToGetFeatures);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_Ptr_StructuredEdgeDetection_delete(IntPtr obj);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern IntPtr ximgproc_Ptr_StructuredEdgeDetection_get(IntPtr ptr);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_StructuredEdgeDetection_detectEdges(IntPtr obj, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_StructuredEdgeDetection_computeOrientation(IntPtr obj, IntPtr src, IntPtr dst);

	[DllImport("OpenCvSharpExtern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	public static extern void ximgproc_StructuredEdgeDetection_edgesNms(IntPtr obj, IntPtr edge_image, IntPtr orientation_image, IntPtr dst, int r, int s, float m, int isParallel);
}
