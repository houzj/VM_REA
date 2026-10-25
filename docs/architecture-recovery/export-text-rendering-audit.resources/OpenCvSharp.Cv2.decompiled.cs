using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenCvSharp.Util;

namespace OpenCvSharp;

public static class Cv2
{
	public static class FishEye
	{
		public static void ProjectPoints(InputArray objectPoints, OutputArray imagePoints, InputArray rvec, InputArray tvec, InputArray k, InputArray d, double alpha = 0.0, OutputArray jacobian = null)
		{
			if (objectPoints == null)
			{
				throw new ArgumentNullException("objectPoints");
			}
			if (rvec == null)
			{
				throw new ArgumentNullException("rvec");
			}
			if (tvec == null)
			{
				throw new ArgumentNullException("tvec");
			}
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			objectPoints.ThrowIfDisposed();
			rvec.ThrowIfDisposed();
			tvec.ThrowIfDisposed();
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			jacobian?.ThrowIfNotReady();
			NativeMethods.calib3d_fisheye_projectPoints2(objectPoints.CvPtr, imagePoints.CvPtr, rvec.CvPtr, tvec.CvPtr, k.CvPtr, d.CvPtr, alpha, ToPtr(jacobian));
			GC.KeepAlive(objectPoints);
			GC.KeepAlive(rvec);
			GC.KeepAlive(tvec);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
			GC.KeepAlive(imagePoints);
			GC.KeepAlive(jacobian);
		}

		public static void DistortPoints(InputArray undistorted, OutputArray distorted, InputArray k, InputArray d, double alpha = 0.0)
		{
			if (undistorted == null)
			{
				throw new ArgumentNullException("undistorted");
			}
			if (distorted == null)
			{
				throw new ArgumentNullException("distorted");
			}
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			undistorted.ThrowIfDisposed();
			distorted.ThrowIfNotReady();
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			NativeMethods.calib3d_fisheye_distortPoints(undistorted.CvPtr, distorted.CvPtr, k.CvPtr, d.CvPtr, alpha);
			GC.KeepAlive(undistorted);
			GC.KeepAlive(distorted);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
		}

		public static void UndistortPoints(InputArray distorted, OutputArray undistorted, InputArray k, InputArray d, InputArray r = null, InputArray p = null)
		{
			if (distorted == null)
			{
				throw new ArgumentNullException("distorted");
			}
			if (undistorted == null)
			{
				throw new ArgumentNullException("undistorted");
			}
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			distorted.ThrowIfDisposed();
			undistorted.ThrowIfNotReady();
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			r?.ThrowIfDisposed();
			p?.ThrowIfDisposed();
			NativeMethods.calib3d_fisheye_undistortPoints(distorted.CvPtr, undistorted.CvPtr, k.CvPtr, d.CvPtr, ToPtr(r), ToPtr(p));
			GC.KeepAlive(distorted);
			GC.KeepAlive(undistorted);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
			GC.KeepAlive(r);
			GC.KeepAlive(p);
		}

		public static void InitUndistortRectifyMap(InputArray k, InputArray d, InputArray r, InputArray p, Size size, int m1type, OutputArray map1, OutputArray map2)
		{
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (map1 == null)
			{
				throw new ArgumentNullException("map1");
			}
			if (map2 == null)
			{
				throw new ArgumentNullException("map2");
			}
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			r.ThrowIfDisposed();
			p.ThrowIfDisposed();
			map1.ThrowIfNotReady();
			map2.ThrowIfNotReady();
			NativeMethods.calib3d_fisheye_initUndistortRectifyMap(k.CvPtr, d.CvPtr, r.CvPtr, p.CvPtr, size, m1type, map1.CvPtr, map2.CvPtr);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
			GC.KeepAlive(r);
			GC.KeepAlive(p);
			GC.KeepAlive(map1);
			GC.KeepAlive(map2);
		}

		public static void UndistortImage(InputArray distorted, OutputArray undistorted, InputArray k, InputArray d, InputArray knew = null, Size newSize = default(Size))
		{
			if (distorted == null)
			{
				throw new ArgumentNullException("distorted");
			}
			if (undistorted == null)
			{
				throw new ArgumentNullException("undistorted");
			}
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			distorted.ThrowIfDisposed();
			undistorted.ThrowIfNotReady();
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			knew?.ThrowIfDisposed();
			NativeMethods.calib3d_fisheye_undistortImage(distorted.CvPtr, undistorted.CvPtr, k.CvPtr, d.CvPtr, ToPtr(knew), newSize);
			GC.KeepAlive(distorted);
			GC.KeepAlive(undistorted);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
			GC.KeepAlive(knew);
		}

		public static void EstimateNewCameraMatrixForUndistortRectify(InputArray k, InputArray d, Size imageSize, InputArray r, OutputArray p, double balance = 0.0, Size newSize = default(Size), double fovScale = 1.0)
		{
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			r.ThrowIfDisposed();
			p.ThrowIfNotReady();
			NativeMethods.calib3d_fisheye_estimateNewCameraMatrixForUndistortRectify(k.CvPtr, d.CvPtr, imageSize, r.CvPtr, p.CvPtr, balance, newSize, fovScale);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
			GC.KeepAlive(r);
			GC.KeepAlive(p);
		}

		public static double Calibrate(IEnumerable<Mat> objectPoints, IEnumerable<Mat> imagePoints, Size imageSize, InputOutputArray k, InputOutputArray d, out IEnumerable<Mat> rvecs, out IEnumerable<Mat> tvecs, FishEyeCalibrationFlags flags = FishEyeCalibrationFlags.None, TermCriteria? criteria = null)
		{
			if (objectPoints == null)
			{
				throw new ArgumentNullException("objectPoints");
			}
			if (imagePoints == null)
			{
				throw new ArgumentNullException("imagePoints");
			}
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			k.ThrowIfDisposed();
			d.ThrowIfDisposed();
			TermCriteria valueOrDefault = criteria.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 100, double.Epsilon));
			double result;
			using (VectorOfMat vectorOfMat = new VectorOfMat(objectPoints))
			{
				using VectorOfMat vectorOfMat2 = new VectorOfMat(imagePoints);
				using VectorOfMat vectorOfMat3 = new VectorOfMat();
				using VectorOfMat vectorOfMat4 = new VectorOfMat();
				result = NativeMethods.calib3d_fisheye_calibrate(vectorOfMat.CvPtr, vectorOfMat2.CvPtr, imageSize, k.CvPtr, d.CvPtr, vectorOfMat3.CvPtr, vectorOfMat4.CvPtr, (int)flags, valueOrDefault);
				rvecs = vectorOfMat3.ToArray();
				tvecs = vectorOfMat4.ToArray();
			}
			GC.KeepAlive(objectPoints);
			GC.KeepAlive(imagePoints);
			GC.KeepAlive(k);
			GC.KeepAlive(d);
			return result;
		}

		public static void StereoRectify(InputArray k1, InputArray d1, InputArray k2, InputArray d2, Size imageSize, InputArray r, InputArray tvec, OutputArray r1, OutputArray r2, OutputArray p1, OutputArray p2, OutputArray q, FishEyeCalibrationFlags flags, Size newImageSize = default(Size), double balance = 0.0, double fovScale = 1.0)
		{
			if (k1 == null)
			{
				throw new ArgumentNullException("k1");
			}
			if (d1 == null)
			{
				throw new ArgumentNullException("d1");
			}
			if (k2 == null)
			{
				throw new ArgumentNullException("k2");
			}
			if (d2 == null)
			{
				throw new ArgumentNullException("d2");
			}
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			if (tvec == null)
			{
				throw new ArgumentNullException("tvec");
			}
			if (r1 == null)
			{
				throw new ArgumentNullException("r1");
			}
			if (r2 == null)
			{
				throw new ArgumentNullException("r2");
			}
			if (p1 == null)
			{
				throw new ArgumentNullException("p1");
			}
			if (p2 == null)
			{
				throw new ArgumentNullException("p2");
			}
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			k1.ThrowIfDisposed();
			d1.ThrowIfDisposed();
			k2.ThrowIfDisposed();
			d2.ThrowIfDisposed();
			r.ThrowIfDisposed();
			tvec.ThrowIfDisposed();
			r1.ThrowIfNotReady();
			r2.ThrowIfNotReady();
			p1.ThrowIfNotReady();
			p2.ThrowIfNotReady();
			q.ThrowIfNotReady();
			NativeMethods.calib3d_fisheye_stereoRectify(k1.CvPtr, d1.CvPtr, k2.CvPtr, d2.CvPtr, imageSize, r.CvPtr, tvec.CvPtr, r1.CvPtr, r2.CvPtr, p1.CvPtr, p2.CvPtr, q.CvPtr, (int)flags, newImageSize, balance, fovScale);
			GC.KeepAlive(k1);
			GC.KeepAlive(d1);
			GC.KeepAlive(k2);
			GC.KeepAlive(d2);
			GC.KeepAlive(r);
			GC.KeepAlive(tvec);
			GC.KeepAlive(r1);
			GC.KeepAlive(r2);
			GC.KeepAlive(p1);
			GC.KeepAlive(p2);
			GC.KeepAlive(q);
		}

		public static double StereoCalibrate(IEnumerable<Mat> objectPoints, IEnumerable<Mat> imagePoints1, IEnumerable<Mat> imagePoints2, InputOutputArray k1, InputOutputArray d1, InputOutputArray k2, InputOutputArray d2, Size imageSize, OutputArray r, OutputArray t, FishEyeCalibrationFlags flags = FishEyeCalibrationFlags.FixIntrinsic, TermCriteria? criteria = null)
		{
			if (objectPoints == null)
			{
				throw new ArgumentNullException("objectPoints");
			}
			if (imagePoints1 == null)
			{
				throw new ArgumentNullException("imagePoints1");
			}
			if (imagePoints2 == null)
			{
				throw new ArgumentNullException("imagePoints2");
			}
			if (k1 == null)
			{
				throw new ArgumentNullException("k1");
			}
			if (d1 == null)
			{
				throw new ArgumentNullException("d1");
			}
			if (k2 == null)
			{
				throw new ArgumentNullException("k2");
			}
			if (d2 == null)
			{
				throw new ArgumentNullException("d2");
			}
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			k1.ThrowIfNotReady();
			d1.ThrowIfNotReady();
			k2.ThrowIfNotReady();
			d2.ThrowIfNotReady();
			r.ThrowIfNotReady();
			t.ThrowIfNotReady();
			TermCriteria valueOrDefault = criteria.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 100, double.Epsilon));
			double result;
			using (VectorOfMat vectorOfMat = new VectorOfMat(objectPoints))
			{
				using VectorOfMat vectorOfMat2 = new VectorOfMat(imagePoints1);
				using VectorOfMat vectorOfMat3 = new VectorOfMat(imagePoints2);
				result = NativeMethods.calib3d_fisheye_stereoCalibrate(vectorOfMat.CvPtr, vectorOfMat2.CvPtr, vectorOfMat3.CvPtr, k1.CvPtr, d1.CvPtr, k2.CvPtr, d2.CvPtr, imageSize, r.CvPtr, t.CvPtr, (int)flags, valueOrDefault);
			}
			GC.KeepAlive(objectPoints);
			GC.KeepAlive(imagePoints1);
			GC.KeepAlive(imagePoints2);
			GC.KeepAlive(k1);
			GC.KeepAlive(d1);
			GC.KeepAlive(k2);
			GC.KeepAlive(d2);
			GC.KeepAlive(r);
			GC.KeepAlive(t);
			return result;
		}
	}

	public delegate bool PartitionPredicate<in T>(T t1, T t2);

	public const double PI = Math.PI;

	public const double LOG2 = 0.6931471805599453;

	public const int FILLED = -1;

	public static void Initialize()
	{
		NativeMethods.TryPInvoke();
	}

	internal static IntPtr ToPtr(ICvPtrHolder obj)
	{
		return obj?.CvPtr ?? IntPtr.Zero;
	}

	public static void Rodrigues(InputArray src, OutputArray dst, OutputArray jacobian = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.calib3d_Rodrigues(src.CvPtr, dst.CvPtr, ToPtr(jacobian));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(jacobian);
		dst.Fix();
		jacobian?.Fix();
	}

	public static void Rodrigues(double[] vector, out double[,] matrix, out double[,] jacobian)
	{
		if (vector == null)
		{
			throw new ArgumentNullException("vector");
		}
		if (vector.Length != 3)
		{
			throw new ArgumentException("vector.Length != 3");
		}
		using Mat mat = new Mat(3, 1, MatType.CV_64FC1, vector, 0L);
		using MatOfDouble matOfDouble = new MatOfDouble();
		using MatOfDouble matOfDouble2 = new MatOfDouble();
		NativeMethods.calib3d_Rodrigues_VecToMat(mat.CvPtr, matOfDouble.CvPtr, matOfDouble2.CvPtr);
		matrix = matOfDouble.ToRectangularArray();
		jacobian = matOfDouble2.ToRectangularArray();
	}

	public static void Rodrigues(double[] vector, out double[,] matrix)
	{
		Rodrigues(vector, out matrix, out var _);
	}

	public static void Rodrigues(double[,] matrix, out double[] vector, out double[,] jacobian)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3)
		{
			throw new ArgumentException("matrix must be double[3,3]");
		}
		using Mat mat = new Mat(3, 3, MatType.CV_64FC1, matrix, 0L);
		using MatOfDouble matOfDouble = new MatOfDouble();
		using MatOfDouble matOfDouble2 = new MatOfDouble();
		NativeMethods.calib3d_Rodrigues_MatToVec(mat.CvPtr, matOfDouble.CvPtr, matOfDouble2.CvPtr);
		vector = matOfDouble.ToArray();
		jacobian = matOfDouble2.ToRectangularArray();
	}

	public static void Rodrigues(double[,] matrix, out double[] vector)
	{
		Rodrigues(matrix, out vector, out var _);
	}

	public static Mat FindHomography(InputArray srcPoints, InputArray dstPoints, HomographyMethods method = HomographyMethods.None, double ransacReprojThreshold = 3.0, OutputArray mask = null)
	{
		if (srcPoints == null)
		{
			throw new ArgumentNullException("srcPoints");
		}
		if (dstPoints == null)
		{
			throw new ArgumentNullException("dstPoints");
		}
		srcPoints.ThrowIfDisposed();
		dstPoints.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.calib3d_findHomography_InputArray(srcPoints.CvPtr, dstPoints.CvPtr, (int)method, ransacReprojThreshold, ToPtr(mask));
		GC.KeepAlive(srcPoints);
		GC.KeepAlive(dstPoints);
		GC.KeepAlive(mask);
		mask?.Fix();
		return new Mat(ptr);
	}

	public static Mat FindHomography(IEnumerable<Point2d> srcPoints, IEnumerable<Point2d> dstPoints, HomographyMethods method = HomographyMethods.None, double ransacReprojThreshold = 3.0, OutputArray mask = null)
	{
		if (srcPoints == null)
		{
			throw new ArgumentNullException("srcPoints");
		}
		if (dstPoints == null)
		{
			throw new ArgumentNullException("dstPoints");
		}
		Point2d[] array = EnumerableEx.ToArray(srcPoints);
		Point2d[] array2 = EnumerableEx.ToArray(dstPoints);
		IntPtr ptr = NativeMethods.calib3d_findHomography_vector(array, array.Length, array2, array2.Length, (int)method, ransacReprojThreshold, ToPtr(mask));
		GC.KeepAlive(mask);
		mask?.Fix();
		return new Mat(ptr);
	}

	public static Vec3d RQDecomp3x3(InputArray src, OutputArray mtxR, OutputArray mtxQ, OutputArray qx = null, OutputArray qy = null, OutputArray qz = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (mtxR == null)
		{
			throw new ArgumentNullException("mtxR");
		}
		if (mtxQ == null)
		{
			throw new ArgumentNullException("mtxQ");
		}
		src.ThrowIfDisposed();
		mtxR.ThrowIfNotReady();
		mtxQ.ThrowIfNotReady();
		NativeMethods.calib3d_RQDecomp3x3_InputArray(src.CvPtr, mtxR.CvPtr, mtxQ.CvPtr, ToPtr(qx), ToPtr(qy), ToPtr(qz), out var outVal);
		GC.KeepAlive(src);
		GC.KeepAlive(mtxR);
		GC.KeepAlive(mtxQ);
		qx?.Fix();
		qy?.Fix();
		qz?.Fix();
		return outVal;
	}

	public static Vec3d RQDecomp3x3(double[,] src, out double[,] mtxR, out double[,] mtxQ)
	{
		double[,] qx;
		double[,] qy;
		double[,] qz;
		return RQDecomp3x3(src, out mtxR, out mtxQ, out qx, out qy, out qz);
	}

	public static Vec3d RQDecomp3x3(double[,] src, out double[,] mtxR, out double[,] mtxQ, out double[,] qx, out double[,] qy, out double[,] qz)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (src.GetLength(0) != 3 || src.GetLength(1) != 3)
		{
			throw new ArgumentException("src must be double[3,3]");
		}
		using Mat mat = new Mat(3, 3, MatType.CV_64FC1, src, 0L);
		using MatOfDouble matOfDouble = new MatOfDouble();
		using MatOfDouble matOfDouble2 = new MatOfDouble();
		using MatOfDouble matOfDouble3 = new MatOfDouble();
		using MatOfDouble matOfDouble4 = new MatOfDouble();
		using MatOfDouble matOfDouble5 = new MatOfDouble();
		NativeMethods.calib3d_RQDecomp3x3_Mat(mat.CvPtr, matOfDouble.CvPtr, matOfDouble2.CvPtr, matOfDouble3.CvPtr, matOfDouble4.CvPtr, matOfDouble5.CvPtr, out var outVal);
		mtxR = matOfDouble.ToRectangularArray();
		mtxQ = matOfDouble2.ToRectangularArray();
		qx = matOfDouble3.ToRectangularArray();
		qy = matOfDouble4.ToRectangularArray();
		qz = matOfDouble5.ToRectangularArray();
		return outVal;
	}

	public static void DecomposeProjectionMatrix(InputArray projMatrix, OutputArray cameraMatrix, OutputArray rotMatrix, OutputArray transVect, OutputArray rotMatrixX = null, OutputArray rotMatrixY = null, OutputArray rotMatrixZ = null, OutputArray eulerAngles = null)
	{
		if (projMatrix == null)
		{
			throw new ArgumentNullException("projMatrix");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (rotMatrix == null)
		{
			throw new ArgumentNullException("rotMatrix");
		}
		projMatrix.ThrowIfDisposed();
		cameraMatrix.ThrowIfNotReady();
		rotMatrix.ThrowIfNotReady();
		transVect.ThrowIfNotReady();
		NativeMethods.calib3d_decomposeProjectionMatrix_InputArray(projMatrix.CvPtr, cameraMatrix.CvPtr, rotMatrix.CvPtr, transVect.CvPtr, ToPtr(rotMatrixX), ToPtr(rotMatrixY), ToPtr(rotMatrixZ), ToPtr(eulerAngles));
		GC.KeepAlive(projMatrix);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(rotMatrix);
		GC.KeepAlive(transVect);
		GC.KeepAlive(rotMatrixX);
		GC.KeepAlive(rotMatrixY);
		GC.KeepAlive(rotMatrixZ);
		GC.KeepAlive(eulerAngles);
		cameraMatrix.Fix();
		rotMatrix.Fix();
		transVect.Fix();
		rotMatrixX?.Fix();
		rotMatrixY?.Fix();
		rotMatrixZ?.Fix();
		eulerAngles?.Fix();
	}

	public static void DecomposeProjectionMatrix(double[,] projMatrix, out double[,] cameraMatrix, out double[,] rotMatrix, out double[] transVect, out double[,] rotMatrixX, out double[,] rotMatrixY, out double[,] rotMatrixZ, out double[] eulerAngles)
	{
		if (projMatrix == null)
		{
			throw new ArgumentNullException("projMatrix");
		}
		int length = projMatrix.GetLength(0);
		int length2 = projMatrix.GetLength(1);
		if ((length != 3 || length2 != 4) && (length != 4 || length2 != 3))
		{
			throw new ArgumentException("projMatrix must be double[3,4] or double[4,3]");
		}
		using Mat mat = new Mat(3, 4, MatType.CV_64FC1, projMatrix, 0L);
		using MatOfDouble matOfDouble = new MatOfDouble();
		using MatOfDouble matOfDouble2 = new MatOfDouble();
		using MatOfDouble matOfDouble3 = new MatOfDouble();
		using MatOfDouble matOfDouble4 = new MatOfDouble();
		using MatOfDouble matOfDouble5 = new MatOfDouble();
		using MatOfDouble matOfDouble6 = new MatOfDouble();
		using MatOfDouble matOfDouble7 = new MatOfDouble();
		NativeMethods.calib3d_decomposeProjectionMatrix_Mat(mat.CvPtr, matOfDouble.CvPtr, matOfDouble2.CvPtr, matOfDouble3.CvPtr, matOfDouble4.CvPtr, matOfDouble5.CvPtr, matOfDouble6.CvPtr, matOfDouble7.CvPtr);
		cameraMatrix = matOfDouble.ToRectangularArray();
		rotMatrix = matOfDouble2.ToRectangularArray();
		transVect = matOfDouble3.ToArray();
		rotMatrixX = matOfDouble4.ToRectangularArray();
		rotMatrixY = matOfDouble5.ToRectangularArray();
		rotMatrixZ = matOfDouble6.ToRectangularArray();
		eulerAngles = matOfDouble7.ToArray();
	}

	public static void DecomposeProjectionMatrix(double[,] projMatrix, out double[,] cameraMatrix, out double[,] rotMatrix, out double[] transVect)
	{
		DecomposeProjectionMatrix(projMatrix, out cameraMatrix, out rotMatrix, out transVect, out var _, out var _, out var _, out var _);
	}

	public static void MatMulDeriv(InputArray a, InputArray b, OutputArray dABdA, OutputArray dABdB)
	{
		if (a == null)
		{
			throw new ArgumentNullException("a");
		}
		if (b == null)
		{
			throw new ArgumentNullException("b");
		}
		if (dABdA == null)
		{
			throw new ArgumentNullException("dABdA");
		}
		if (dABdB == null)
		{
			throw new ArgumentNullException("dABdB");
		}
		a.ThrowIfDisposed();
		b.ThrowIfDisposed();
		dABdA.ThrowIfNotReady();
		dABdB.ThrowIfNotReady();
		NativeMethods.calib3d_matMulDeriv(a.CvPtr, b.CvPtr, dABdA.CvPtr, dABdB.CvPtr);
		GC.KeepAlive(a);
		GC.KeepAlive(b);
		dABdA.Fix();
		dABdB.Fix();
	}

	public static void ComposeRT(InputArray rvec1, InputArray tvec1, InputArray rvec2, InputArray tvec2, OutputArray rvec3, OutputArray tvec3, OutputArray dr3dr1 = null, OutputArray dr3dt1 = null, OutputArray dr3dr2 = null, OutputArray dr3dt2 = null, OutputArray dt3dr1 = null, OutputArray dt3dt1 = null, OutputArray dt3dr2 = null, OutputArray dt3dt2 = null)
	{
		if (rvec1 == null)
		{
			throw new ArgumentNullException("rvec1");
		}
		if (tvec1 == null)
		{
			throw new ArgumentNullException("tvec1");
		}
		if (rvec2 == null)
		{
			throw new ArgumentNullException("rvec2");
		}
		if (tvec2 == null)
		{
			throw new ArgumentNullException("tvec2");
		}
		rvec1.ThrowIfDisposed();
		tvec1.ThrowIfDisposed();
		rvec2.ThrowIfDisposed();
		tvec2.ThrowIfDisposed();
		rvec3.ThrowIfNotReady();
		tvec3.ThrowIfNotReady();
		NativeMethods.calib3d_composeRT_InputArray(rvec1.CvPtr, tvec1.CvPtr, rvec2.CvPtr, tvec2.CvPtr, rvec3.CvPtr, tvec3.CvPtr, ToPtr(dr3dr1), ToPtr(dr3dt1), ToPtr(dr3dr2), ToPtr(dr3dt2), ToPtr(dt3dr1), ToPtr(dt3dt1), ToPtr(dt3dr2), ToPtr(dt3dt2));
		GC.KeepAlive(rvec1);
		GC.KeepAlive(tvec1);
		GC.KeepAlive(rvec2);
		GC.KeepAlive(tvec2);
		GC.KeepAlive(rvec3);
		GC.KeepAlive(tvec3);
		GC.KeepAlive(dr3dr1);
		GC.KeepAlive(dr3dt1);
		GC.KeepAlive(dr3dr2);
		GC.KeepAlive(dr3dt2);
		GC.KeepAlive(dt3dr1);
		GC.KeepAlive(dt3dt1);
		GC.KeepAlive(dt3dr2);
		GC.KeepAlive(dt3dt2);
	}

	public static void ComposeRT(double[] rvec1, double[] tvec1, double[] rvec2, double[] tvec2, out double[] rvec3, out double[] tvec3, out double[,] dr3dr1, out double[,] dr3dt1, out double[,] dr3dr2, out double[,] dr3dt2, out double[,] dt3dr1, out double[,] dt3dt1, out double[,] dt3dr2, out double[,] dt3dt2)
	{
		if (rvec1 == null)
		{
			throw new ArgumentNullException("rvec1");
		}
		if (tvec1 == null)
		{
			throw new ArgumentNullException("tvec1");
		}
		if (rvec2 == null)
		{
			throw new ArgumentNullException("rvec2");
		}
		if (tvec2 == null)
		{
			throw new ArgumentNullException("tvec2");
		}
		using Mat mat = new Mat(3, 1, MatType.CV_64FC1, rvec1, 0L);
		using Mat mat2 = new Mat(3, 1, MatType.CV_64FC1, tvec1, 0L);
		using Mat mat3 = new Mat(3, 1, MatType.CV_64FC1, rvec2, 0L);
		using Mat mat4 = new Mat(3, 1, MatType.CV_64FC1, tvec2, 0L);
		using MatOfDouble matOfDouble = new MatOfDouble();
		using MatOfDouble matOfDouble2 = new MatOfDouble();
		using MatOfDouble matOfDouble3 = new MatOfDouble();
		using MatOfDouble matOfDouble4 = new MatOfDouble();
		using MatOfDouble matOfDouble5 = new MatOfDouble();
		using MatOfDouble matOfDouble6 = new MatOfDouble();
		using MatOfDouble matOfDouble7 = new MatOfDouble();
		using MatOfDouble matOfDouble8 = new MatOfDouble();
		using MatOfDouble matOfDouble9 = new MatOfDouble();
		using MatOfDouble matOfDouble10 = new MatOfDouble();
		NativeMethods.calib3d_composeRT_Mat(mat.CvPtr, mat2.CvPtr, mat3.CvPtr, mat4.CvPtr, matOfDouble.CvPtr, matOfDouble2.CvPtr, matOfDouble3.CvPtr, matOfDouble4.CvPtr, matOfDouble5.CvPtr, matOfDouble6.CvPtr, matOfDouble7.CvPtr, matOfDouble8.CvPtr, matOfDouble9.CvPtr, matOfDouble10.CvPtr);
		rvec3 = matOfDouble.ToArray();
		tvec3 = matOfDouble2.ToArray();
		dr3dr1 = matOfDouble3.ToRectangularArray();
		dr3dt1 = matOfDouble4.ToRectangularArray();
		dr3dr2 = matOfDouble5.ToRectangularArray();
		dr3dt2 = matOfDouble6.ToRectangularArray();
		dt3dr1 = matOfDouble7.ToRectangularArray();
		dt3dt1 = matOfDouble8.ToRectangularArray();
		dt3dr2 = matOfDouble9.ToRectangularArray();
		dt3dt2 = matOfDouble10.ToRectangularArray();
	}

	public static void ComposeRT(double[] rvec1, double[] tvec1, double[] rvec2, double[] tvec2, out double[] rvec3, out double[] tvec3)
	{
		ComposeRT(rvec1, tvec2, rvec2, tvec2, out rvec3, out tvec3, out var _, out var _, out var _, out var _, out var _, out var _, out var _, out var _);
	}

	public static void ProjectPoints(InputArray objectPoints, InputArray rvec, InputArray tvec, InputArray cameraMatrix, InputArray distCoeffs, OutputArray imagePoints, OutputArray jacobian = null, double aspectRatio = 0.0)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (rvec == null)
		{
			throw new ArgumentNullException("rvec");
		}
		if (tvec == null)
		{
			throw new ArgumentNullException("tvec");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		objectPoints.ThrowIfDisposed();
		rvec.ThrowIfDisposed();
		tvec.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		imagePoints.ThrowIfNotReady();
		if (jacobian == null)
		{
			jacobian = new Mat();
		}
		NativeMethods.calib3d_projectPoints_InputArray(objectPoints.CvPtr, rvec.CvPtr, tvec.CvPtr, cameraMatrix.CvPtr, ToPtr(distCoeffs), imagePoints.CvPtr, ToPtr(jacobian), aspectRatio);
		GC.KeepAlive(objectPoints);
		GC.KeepAlive(rvec);
		GC.KeepAlive(tvec);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		GC.KeepAlive(imagePoints);
		GC.KeepAlive(jacobian);
	}

	public static void ProjectPoints(IEnumerable<Point3f> objectPoints, double[] rvec, double[] tvec, double[,] cameraMatrix, double[] distCoeffs, out Point2f[] imagePoints, out double[,] jacobian, double aspectRatio = 0.0)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (rvec == null)
		{
			throw new ArgumentNullException("rvec");
		}
		if (rvec.Length != 3)
		{
			throw new ArgumentException("rvec.Length != 3");
		}
		if (tvec == null)
		{
			throw new ArgumentNullException("tvec");
		}
		if (tvec.Length != 3)
		{
			throw new ArgumentException("tvec.Length != 3");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
		{
			throw new ArgumentException("cameraMatrix must be double[3,3]");
		}
		Point3f[] array = EnumerableEx.ToArray(objectPoints);
		using Mat mat = new Mat(array.Length, 1, MatType.CV_32FC3, array, 0L);
		using Mat mat2 = new Mat(3, 1, MatType.CV_64FC1, rvec, 0L);
		using Mat mat3 = new Mat(3, 1, MatType.CV_64FC1, tvec, 0L);
		using Mat mat4 = new Mat(3, 3, MatType.CV_64FC1, cameraMatrix, 0L);
		using Mat mat5 = ((distCoeffs == null) ? new Mat() : new Mat(distCoeffs.Length, 1, MatType.CV_64FC1, distCoeffs, 0L));
		using MatOfPoint2f matOfPoint2f = new MatOfPoint2f();
		using MatOfDouble matOfDouble = new MatOfDouble();
		NativeMethods.calib3d_projectPoints_Mat(mat.CvPtr, mat2.CvPtr, mat3.CvPtr, mat4.CvPtr, mat5.CvPtr, matOfPoint2f.CvPtr, matOfDouble.CvPtr, aspectRatio);
		imagePoints = matOfPoint2f.ToArray();
		jacobian = matOfDouble.ToRectangularArray();
	}

	public static void SolvePnP(InputArray objectPoints, InputArray imagePoints, InputArray cameraMatrix, InputArray distCoeffs, OutputArray rvec, OutputArray tvec, bool useExtrinsicGuess = false, SolvePnPFlags flags = SolvePnPFlags.Iterative)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (rvec == null)
		{
			throw new ArgumentNullException("rvec");
		}
		if (tvec == null)
		{
			throw new ArgumentNullException("tvec");
		}
		objectPoints.ThrowIfDisposed();
		imagePoints.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		distCoeffs.ThrowIfDisposed();
		rvec.ThrowIfDisposed();
		tvec.ThrowIfDisposed();
		IntPtr distCoeffs2 = ToPtr(distCoeffs);
		NativeMethods.calib3d_solvePnP_InputArray(objectPoints.CvPtr, imagePoints.CvPtr, cameraMatrix.CvPtr, distCoeffs2, rvec.CvPtr, tvec.CvPtr, useExtrinsicGuess ? 1 : 0, (int)flags);
		rvec.Fix();
		tvec.Fix();
		GC.KeepAlive(objectPoints);
		GC.KeepAlive(imagePoints);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
	}

	public static void SolvePnP(IEnumerable<Point3f> objectPoints, IEnumerable<Point2f> imagePoints, double[,] cameraMatrix, IEnumerable<double> distCoeffs, ref double[] rvec, ref double[] tvec, bool useExtrinsicGuess = false, SolvePnPFlags flags = SolvePnPFlags.Iterative)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
		{
			throw new ArgumentException("");
		}
		Point3f[] array = EnumerableEx.ToArray(objectPoints);
		Point2f[] array2 = EnumerableEx.ToArray(imagePoints);
		double[] array3 = EnumerableEx.ToArray(distCoeffs);
		int distCoeffsLength = ((distCoeffs != null) ? array3.Length : 0);
		if (!useExtrinsicGuess)
		{
			rvec = new double[3];
			tvec = new double[3];
		}
		NativeMethods.calib3d_solvePnP_vector(array, array.Length, array2, array2.Length, cameraMatrix, array3, distCoeffsLength, rvec, tvec, useExtrinsicGuess ? 1 : 0, (int)flags);
	}

	public static void SolvePnPRansac(InputArray objectPoints, InputArray imagePoints, InputArray cameraMatrix, InputArray distCoeffs, OutputArray rvec, OutputArray tvec, bool useExtrinsicGuess = false, int iterationsCount = 100, float reprojectionError = 8f, double confidence = 0.99, OutputArray inliers = null, SolvePnPFlags flags = SolvePnPFlags.Iterative)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (rvec == null)
		{
			throw new ArgumentNullException("rvec");
		}
		if (tvec == null)
		{
			throw new ArgumentNullException("tvec");
		}
		objectPoints.ThrowIfDisposed();
		imagePoints.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		distCoeffs.ThrowIfDisposed();
		rvec.ThrowIfDisposed();
		tvec.ThrowIfDisposed();
		IntPtr distCoeffs2 = ToPtr(distCoeffs);
		NativeMethods.calib3d_solvePnPRansac_InputArray(objectPoints.CvPtr, imagePoints.CvPtr, cameraMatrix.CvPtr, distCoeffs2, rvec.CvPtr, tvec.CvPtr, useExtrinsicGuess ? 1 : 0, iterationsCount, reprojectionError, confidence, ToPtr(inliers), (int)flags);
		GC.KeepAlive(objectPoints);
		GC.KeepAlive(imagePoints);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		rvec.Fix();
		tvec.Fix();
		inliers?.Fix();
	}

	public static void SolvePnPRansac(IEnumerable<Point3f> objectPoints, IEnumerable<Point2f> imagePoints, double[,] cameraMatrix, IEnumerable<double> distCoeffs, out double[] rvec, out double[] tvec)
	{
		SolvePnPRansac(objectPoints, imagePoints, cameraMatrix, distCoeffs, out rvec, out tvec, out var _);
	}

	public static void SolvePnPRansac(IEnumerable<Point3f> objectPoints, IEnumerable<Point2f> imagePoints, double[,] cameraMatrix, IEnumerable<double> distCoeffs, out double[] rvec, out double[] tvec, out int[] inliers, bool useExtrinsicGuess = false, int iterationsCount = 100, float reprojectionError = 8f, double confidence = 0.99, SolvePnPFlags flags = SolvePnPFlags.Iterative)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
		{
			throw new ArgumentException("");
		}
		Point3f[] array = EnumerableEx.ToArray(objectPoints);
		Point2f[] array2 = EnumerableEx.ToArray(imagePoints);
		double[] array3 = EnumerableEx.ToArray(distCoeffs);
		int distCoeffsLength = ((distCoeffs != null) ? array3.Length : 0);
		rvec = new double[3];
		tvec = new double[3];
		using VectorOfInt32 vectorOfInt = new VectorOfInt32();
		NativeMethods.calib3d_solvePnPRansac_vector(array, array.Length, array2, array2.Length, cameraMatrix, array3, distCoeffsLength, rvec, tvec, useExtrinsicGuess ? 1 : 0, iterationsCount, reprojectionError, confidence, vectorOfInt.CvPtr, (int)flags);
		inliers = vectorOfInt.ToArray();
	}

	public static Mat InitCameraMatrix2D(IEnumerable<Mat> objectPoints, IEnumerable<Mat> imagePoints, Size imageSize, double aspectRatio = 1.0)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		IntPtr[] array = EnumerableEx.SelectPtrs(objectPoints);
		IntPtr[] array2 = EnumerableEx.SelectPtrs(imagePoints);
		return new Mat(NativeMethods.calib3d_initCameraMatrix2D_Mat(array, array.Length, array2, array2.Length, imageSize, aspectRatio));
	}

	public static Mat InitCameraMatrix2D(IEnumerable<IEnumerable<Point3d>> objectPoints, IEnumerable<IEnumerable<Point2d>> imagePoints, Size imageSize, double aspectRatio = 1.0)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints == null)
		{
			throw new ArgumentNullException("imagePoints");
		}
		using ArrayAddress2<Point3d> arrayAddress = new ArrayAddress2<Point3d>(objectPoints);
		using ArrayAddress2<Point2d> arrayAddress2 = new ArrayAddress2<Point2d>(imagePoints);
		return new Mat(NativeMethods.calib3d_initCameraMatrix2D_array(arrayAddress, arrayAddress.Dim1Length, arrayAddress.Dim2Lengths, arrayAddress2, arrayAddress2.Dim1Length, arrayAddress2.Dim2Lengths, imageSize, aspectRatio));
	}

	public static bool FindChessboardCorners(InputArray image, Size patternSize, OutputArray corners, ChessboardFlags flags = ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (corners == null)
		{
			throw new ArgumentNullException("corners");
		}
		image.ThrowIfDisposed();
		corners.ThrowIfNotReady();
		int num = NativeMethods.calib3d_findChessboardCorners_InputArray(image.CvPtr, patternSize, corners.CvPtr, (int)flags);
		GC.KeepAlive(image);
		corners.Fix();
		return num != 0;
	}

	public static bool FindChessboardCorners(InputArray image, Size patternSize, out Point2f[] corners, ChessboardFlags flags = ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		int num = NativeMethods.calib3d_findChessboardCorners_vector(image.CvPtr, patternSize, vectorOfPoint2f.CvPtr, (int)flags);
		GC.KeepAlive(image);
		corners = vectorOfPoint2f.ToArray();
		return num != 0;
	}

	public static bool CheckChessboard(InputArray img, Size size)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		int num = NativeMethods.calib3d_checkChessboard(img.CvPtr, size);
		GC.KeepAlive(img);
		return num != 0;
	}

	public static bool FindChessboardCornersSB(InputArray image, Size patternSize, OutputArray corners, ChessboardFlags flags = ChessboardFlags.None)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (corners == null)
		{
			throw new ArgumentNullException("corners");
		}
		image.ThrowIfDisposed();
		corners.ThrowIfNotReady();
		int num = NativeMethods.calib3d_findChessboardCornersSB_OutputArray(image.CvPtr, patternSize, corners.CvPtr, (int)flags);
		GC.KeepAlive(image);
		GC.KeepAlive(corners);
		return num != 0;
	}

	public static bool FindChessboardCornersSB(InputArray image, Size patternSize, out Point2f[] corners, ChessboardFlags flags = ChessboardFlags.None)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		int num = NativeMethods.calib3d_findChessboardCornersSB_vector(image.CvPtr, patternSize, vectorOfPoint2f.CvPtr, (int)flags);
		corners = vectorOfPoint2f.ToArray();
		GC.KeepAlive(image);
		return num != 0;
	}

	public static bool Find4QuadCornerSubpix(InputArray img, InputOutputArray corners, Size regionSize)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (corners == null)
		{
			throw new ArgumentNullException("corners");
		}
		img.ThrowIfDisposed();
		corners.ThrowIfNotReady();
		int num = NativeMethods.calib3d_find4QuadCornerSubpix_InputArray(img.CvPtr, corners.CvPtr, regionSize);
		GC.KeepAlive(img);
		corners.Fix();
		return num != 0;
	}

	public static bool Find4QuadCornerSubpix(InputArray img, [In][Out] Point2f[] corners, Size regionSize)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (corners == null)
		{
			throw new ArgumentNullException("corners");
		}
		img.ThrowIfDisposed();
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f(corners);
		int num = NativeMethods.calib3d_find4QuadCornerSubpix_vector(img.CvPtr, vectorOfPoint2f.CvPtr, regionSize);
		GC.KeepAlive(img);
		Point2f[] array = vectorOfPoint2f.ToArray();
		for (int i = 0; i < corners.Length; i++)
		{
			corners[i] = array[i];
		}
		return num != 0;
	}

	public static void DrawChessboardCorners(InputOutputArray image, Size patternSize, InputArray corners, bool patternWasFound)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (corners == null)
		{
			throw new ArgumentNullException("corners");
		}
		image.ThrowIfNotReady();
		corners.ThrowIfDisposed();
		NativeMethods.calib3d_drawChessboardCorners_InputArray(image.CvPtr, patternSize, corners.CvPtr, patternWasFound ? 1 : 0);
		GC.KeepAlive(corners);
		image.Fix();
	}

	public static void DrawChessboardCorners(InputOutputArray image, Size patternSize, IEnumerable<Point2f> corners, bool patternWasFound)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (corners == null)
		{
			throw new ArgumentNullException("corners");
		}
		image.ThrowIfNotReady();
		Point2f[] array = EnumerableEx.ToArray(corners);
		NativeMethods.calib3d_drawChessboardCorners_array(image.CvPtr, patternSize, array, array.Length, patternWasFound ? 1 : 0);
		image.Fix();
	}

	public static void DrawFrameAxes(InputOutputArray image, InputArray cameraMatrix, InputArray distCoeffs, InputArray rvec, InputArray tvec, float length, int thickness = 3)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (distCoeffs == null)
		{
			throw new ArgumentNullException("distCoeffs");
		}
		if (rvec == null)
		{
			throw new ArgumentNullException("rvec");
		}
		if (tvec == null)
		{
			throw new ArgumentNullException("tvec");
		}
		image.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		distCoeffs.ThrowIfDisposed();
		rvec.ThrowIfDisposed();
		tvec.ThrowIfDisposed();
		NativeMethods.calib3d_drawFrameAxes(image.CvPtr, cameraMatrix.CvPtr, distCoeffs.CvPtr, rvec.CvPtr, tvec.CvPtr, length, thickness);
		GC.KeepAlive(image);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		GC.KeepAlive(rvec);
		GC.KeepAlive(tvec);
	}

	public static bool FindCirclesGrid(InputArray image, Size patternSize, OutputArray centers, FindCirclesGridFlags flags = FindCirclesGridFlags.SymmetricGrid, Feature2D blobDetector = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (centers == null)
		{
			throw new ArgumentNullException("centers");
		}
		image.ThrowIfDisposed();
		centers.ThrowIfNotReady();
		int num = NativeMethods.calib3d_findCirclesGrid_InputArray(image.CvPtr, patternSize, centers.CvPtr, (int)flags, ToPtr(blobDetector));
		GC.KeepAlive(image);
		GC.KeepAlive(centers);
		GC.KeepAlive(blobDetector);
		centers.Fix();
		return num != 0;
	}

	public static bool FindCirclesGrid(InputArray image, Size patternSize, out Point2f[] centers, FindCirclesGridFlags flags = FindCirclesGridFlags.SymmetricGrid, Feature2D blobDetector = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		int num = NativeMethods.calib3d_findCirclesGrid_vector(image.CvPtr, patternSize, vectorOfPoint2f.CvPtr, (int)flags, ToPtr(blobDetector));
		GC.KeepAlive(image);
		GC.KeepAlive(blobDetector);
		centers = vectorOfPoint2f.ToArray();
		return num != 0;
	}

	public static double CalibrateCamera(IEnumerable<Mat> objectPoints, IEnumerable<Mat> imagePoints, Size imageSize, InputOutputArray cameraMatrix, InputOutputArray distCoeffs, out Mat[] rvecs, out Mat[] tvecs, CalibrationFlags flags = CalibrationFlags.None, TermCriteria? criteria = null)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (distCoeffs == null)
		{
			throw new ArgumentNullException("distCoeffs");
		}
		cameraMatrix.ThrowIfNotReady();
		distCoeffs.ThrowIfNotReady();
		TermCriteria valueOrDefault = criteria.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 30, double.Epsilon));
		IntPtr[] array = EnumerableEx.SelectPtrs(objectPoints);
		IntPtr[] imagePoints2 = EnumerableEx.SelectPtrs(imagePoints);
		double result;
		using (VectorOfMat vectorOfMat = new VectorOfMat())
		{
			using VectorOfMat vectorOfMat2 = new VectorOfMat();
			result = NativeMethods.calib3d_calibrateCamera_InputArray(array, array.Length, imagePoints2, array.Length, imageSize, cameraMatrix.CvPtr, distCoeffs.CvPtr, vectorOfMat.CvPtr, vectorOfMat2.CvPtr, (int)flags, valueOrDefault);
			GC.KeepAlive(cameraMatrix);
			GC.KeepAlive(distCoeffs);
			GC.KeepAlive(objectPoints);
			GC.KeepAlive(imagePoints);
			rvecs = vectorOfMat.ToArray();
			tvecs = vectorOfMat2.ToArray();
		}
		cameraMatrix.Fix();
		distCoeffs.Fix();
		return result;
	}

	public static double CalibrateCamera(IEnumerable<IEnumerable<Point3f>> objectPoints, IEnumerable<IEnumerable<Point2f>> imagePoints, Size imageSize, double[,] cameraMatrix, double[] distCoeffs, out Vec3d[] rvecs, out Vec3d[] tvecs, CalibrationFlags flags = CalibrationFlags.None, TermCriteria? criteria = null)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (distCoeffs == null)
		{
			throw new ArgumentNullException("distCoeffs");
		}
		TermCriteria valueOrDefault = criteria.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 30, double.Epsilon));
		using ArrayAddress2<Point3f> arrayAddress = new ArrayAddress2<Point3f>(objectPoints);
		using ArrayAddress2<Point2f> arrayAddress2 = new ArrayAddress2<Point2f>(imagePoints);
		using VectorOfMat vectorOfMat = new VectorOfMat();
		using VectorOfMat vectorOfMat2 = new VectorOfMat();
		double result = NativeMethods.calib3d_calibrateCamera_vector(arrayAddress.Pointer, arrayAddress.Dim1Length, arrayAddress.Dim2Lengths, arrayAddress2.Pointer, arrayAddress2.Dim1Length, arrayAddress2.Dim2Lengths, imageSize, cameraMatrix, distCoeffs, distCoeffs.Length, vectorOfMat.CvPtr, vectorOfMat2.CvPtr, (int)flags, valueOrDefault);
		Mat[] enumerable = vectorOfMat.ToArray();
		Mat[] enumerable2 = vectorOfMat2.ToArray();
		rvecs = EnumerableEx.SelectToArray(enumerable, (Mat m) => m.Get<Vec3d>(0));
		tvecs = EnumerableEx.SelectToArray(enumerable2, (Mat m) => m.Get<Vec3d>(0));
		return result;
	}

	public static void CalibrationMatrixValues(InputArray cameraMatrix, Size imageSize, double apertureWidth, double apertureHeight, out double fovx, out double fovy, out double focalLength, out Point2d principalPoint, out double aspectRatio)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		cameraMatrix.ThrowIfDisposed();
		NativeMethods.calib3d_calibrationMatrixValues_InputArray(cameraMatrix.CvPtr, imageSize, apertureWidth, apertureHeight, out fovx, out fovy, out focalLength, out principalPoint, out aspectRatio);
		GC.KeepAlive(cameraMatrix);
	}

	public static void CalibrationMatrixValues(double[,] cameraMatrix, Size imageSize, double apertureWidth, double apertureHeight, out double fovx, out double fovy, out double focalLength, out Point2d principalPoint, out double aspectRatio)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
		{
			throw new ArgumentException("cameraMatrix must be 3x3");
		}
		NativeMethods.calib3d_calibrationMatrixValues_array(cameraMatrix, imageSize, apertureWidth, apertureHeight, out fovx, out fovy, out focalLength, out principalPoint, out aspectRatio);
	}

	public static double StereoCalibrate(IEnumerable<InputArray> objectPoints, IEnumerable<InputArray> imagePoints1, IEnumerable<InputArray> imagePoints2, InputOutputArray cameraMatrix1, InputOutputArray distCoeffs1, InputOutputArray cameraMatrix2, InputOutputArray distCoeffs2, Size imageSize, OutputArray R, OutputArray T, OutputArray E, OutputArray F, CalibrationFlags flags = CalibrationFlags.FixIntrinsic, TermCriteria? criteria = null)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints1 == null)
		{
			throw new ArgumentNullException("imagePoints1");
		}
		if (imagePoints2 == null)
		{
			throw new ArgumentNullException("imagePoints2");
		}
		if (cameraMatrix1 == null)
		{
			throw new ArgumentNullException("cameraMatrix1");
		}
		if (distCoeffs1 == null)
		{
			throw new ArgumentNullException("distCoeffs1");
		}
		if (cameraMatrix2 == null)
		{
			throw new ArgumentNullException("cameraMatrix2");
		}
		if (distCoeffs2 == null)
		{
			throw new ArgumentNullException("distCoeffs2");
		}
		cameraMatrix1.ThrowIfDisposed();
		distCoeffs1.ThrowIfDisposed();
		cameraMatrix2.ThrowIfDisposed();
		distCoeffs2.ThrowIfDisposed();
		cameraMatrix1.ThrowIfNotReady();
		cameraMatrix2.ThrowIfNotReady();
		distCoeffs1.ThrowIfNotReady();
		distCoeffs2.ThrowIfNotReady();
		IntPtr[] array = EnumerableEx.SelectPtrs(objectPoints);
		IntPtr[] array2 = EnumerableEx.SelectPtrs(imagePoints1);
		IntPtr[] array3 = EnumerableEx.SelectPtrs(imagePoints2);
		double result = NativeMethods.calib3d_stereoCalibrate_InputArray(criteria: criteria.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 30, 1E-06)), objectPoints: array, opSize: array.Length, imagePoints1: array2, ip1Size: array2.Length, imagePoints2: array3, ip2Size: array3.Length, cameraMatrix1: cameraMatrix1.CvPtr, distCoeffs1: distCoeffs1.CvPtr, cameraMatrix2: cameraMatrix2.CvPtr, distCoeffs2: distCoeffs2.CvPtr, imageSize: imageSize, R: ToPtr(R), T: ToPtr(T), E: ToPtr(E), F: ToPtr(F), flags: (int)flags);
		GC.KeepAlive(cameraMatrix1);
		GC.KeepAlive(distCoeffs1);
		GC.KeepAlive(cameraMatrix2);
		GC.KeepAlive(distCoeffs2);
		GC.KeepAlive(R);
		GC.KeepAlive(T);
		GC.KeepAlive(E);
		GC.KeepAlive(F);
		GC.KeepAlive(objectPoints);
		GC.KeepAlive(imagePoints1);
		GC.KeepAlive(imagePoints2);
		cameraMatrix1.Fix();
		distCoeffs1.Fix();
		cameraMatrix2.Fix();
		distCoeffs2.Fix();
		R?.Fix();
		T?.Fix();
		E?.Fix();
		F?.Fix();
		return result;
	}

	public static double StereoCalibrate(IEnumerable<IEnumerable<Point3f>> objectPoints, IEnumerable<IEnumerable<Point2f>> imagePoints1, IEnumerable<IEnumerable<Point2f>> imagePoints2, double[,] cameraMatrix1, double[] distCoeffs1, double[,] cameraMatrix2, double[] distCoeffs2, Size imageSize, OutputArray R, OutputArray T, OutputArray E, OutputArray F, CalibrationFlags flags = CalibrationFlags.FixIntrinsic, TermCriteria? criteria = null)
	{
		if (objectPoints == null)
		{
			throw new ArgumentNullException("objectPoints");
		}
		if (imagePoints1 == null)
		{
			throw new ArgumentNullException("imagePoints1");
		}
		if (imagePoints2 == null)
		{
			throw new ArgumentNullException("imagePoints2");
		}
		if (cameraMatrix1 == null)
		{
			throw new ArgumentNullException("cameraMatrix1");
		}
		if (distCoeffs1 == null)
		{
			throw new ArgumentNullException("distCoeffs1");
		}
		if (cameraMatrix2 == null)
		{
			throw new ArgumentNullException("cameraMatrix2");
		}
		if (distCoeffs2 == null)
		{
			throw new ArgumentNullException("distCoeffs2");
		}
		TermCriteria valueOrDefault = criteria.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 30, 1E-06));
		using ArrayAddress2<Point3f> arrayAddress = new ArrayAddress2<Point3f>(objectPoints);
		using ArrayAddress2<Point2f> arrayAddress2 = new ArrayAddress2<Point2f>(imagePoints1);
		using ArrayAddress2<Point2f> arrayAddress3 = new ArrayAddress2<Point2f>(imagePoints2);
		double result = NativeMethods.calib3d_stereoCalibrate_array(arrayAddress.Pointer, arrayAddress.Dim1Length, arrayAddress.Dim2Lengths, arrayAddress2.Pointer, arrayAddress2.Dim1Length, arrayAddress2.Dim2Lengths, arrayAddress3.Pointer, arrayAddress3.Dim1Length, arrayAddress3.Dim2Lengths, cameraMatrix1, distCoeffs1, distCoeffs1.Length, cameraMatrix2, distCoeffs2, distCoeffs2.Length, imageSize, ToPtr(R), ToPtr(T), ToPtr(E), ToPtr(F), (int)flags, valueOrDefault);
		GC.KeepAlive(R);
		GC.KeepAlive(T);
		GC.KeepAlive(E);
		GC.KeepAlive(F);
		return result;
	}

	public static void StereoRectify(InputArray cameraMatrix1, InputArray distCoeffs1, InputArray cameraMatrix2, InputArray distCoeffs2, Size imageSize, InputArray R, InputArray T, OutputArray R1, OutputArray R2, OutputArray P1, OutputArray P2, OutputArray Q, StereoRectificationFlags flags = StereoRectificationFlags.ZeroDisparity, double alpha = -1.0, Size? newImageSize = null)
	{
		Size valueOrDefault = newImageSize.GetValueOrDefault(new Size(0, 0));
		StereoRectify(cameraMatrix1, distCoeffs1, cameraMatrix2, distCoeffs2, imageSize, R, T, R1, R2, P1, P2, Q, flags, alpha, valueOrDefault, out var _, out var _);
	}

	public static void StereoRectify(InputArray cameraMatrix1, InputArray distCoeffs1, InputArray cameraMatrix2, InputArray distCoeffs2, Size imageSize, InputArray R, InputArray T, OutputArray R1, OutputArray R2, OutputArray P1, OutputArray P2, OutputArray Q, StereoRectificationFlags flags, double alpha, Size newImageSize, out Rect validPixROI1, out Rect validPixROI2)
	{
		if (cameraMatrix1 == null)
		{
			throw new ArgumentNullException("cameraMatrix1");
		}
		if (distCoeffs1 == null)
		{
			throw new ArgumentNullException("distCoeffs1");
		}
		if (cameraMatrix2 == null)
		{
			throw new ArgumentNullException("cameraMatrix2");
		}
		if (distCoeffs2 == null)
		{
			throw new ArgumentNullException("distCoeffs2");
		}
		if (R == null)
		{
			throw new ArgumentNullException("R");
		}
		if (T == null)
		{
			throw new ArgumentNullException("T");
		}
		if (R1 == null)
		{
			throw new ArgumentNullException("R1");
		}
		if (R2 == null)
		{
			throw new ArgumentNullException("R2");
		}
		if (P1 == null)
		{
			throw new ArgumentNullException("P1");
		}
		if (P2 == null)
		{
			throw new ArgumentNullException("P2");
		}
		if (Q == null)
		{
			throw new ArgumentNullException("Q");
		}
		cameraMatrix1.ThrowIfDisposed();
		distCoeffs1.ThrowIfDisposed();
		cameraMatrix2.ThrowIfDisposed();
		distCoeffs2.ThrowIfDisposed();
		R.ThrowIfDisposed();
		T.ThrowIfDisposed();
		R1.ThrowIfNotReady();
		R2.ThrowIfNotReady();
		P1.ThrowIfNotReady();
		P2.ThrowIfNotReady();
		Q.ThrowIfNotReady();
		NativeMethods.calib3d_stereoRectify_InputArray(cameraMatrix1.CvPtr, distCoeffs1.CvPtr, cameraMatrix2.CvPtr, distCoeffs2.CvPtr, imageSize, R.CvPtr, T.CvPtr, R1.CvPtr, R2.CvPtr, P1.CvPtr, P2.CvPtr, Q.CvPtr, (int)flags, alpha, newImageSize, out validPixROI1, out validPixROI2);
		GC.KeepAlive(cameraMatrix1);
		GC.KeepAlive(distCoeffs1);
		GC.KeepAlive(cameraMatrix2);
		GC.KeepAlive(distCoeffs2);
		GC.KeepAlive(R);
		GC.KeepAlive(T);
		GC.KeepAlive(R1);
		GC.KeepAlive(R2);
		GC.KeepAlive(P1);
		GC.KeepAlive(P2);
		GC.KeepAlive(Q);
		R1.Fix();
		R2.Fix();
		P1.Fix();
		P2.Fix();
		Q.Fix();
	}

	public static void StereoRectify(double[,] cameraMatrix1, double[] distCoeffs1, double[,] cameraMatrix2, double[] distCoeffs2, Size imageSize, double[,] R, double[] T, out double[,] R1, out double[,] R2, out double[,] P1, out double[,] P2, out double[,] Q, StereoRectificationFlags flags = StereoRectificationFlags.ZeroDisparity, double alpha = -1.0, Size? newImageSize = null)
	{
		Size valueOrDefault = newImageSize.GetValueOrDefault(new Size(0, 0));
		StereoRectify(cameraMatrix1, distCoeffs1, cameraMatrix2, distCoeffs2, imageSize, R, T, out R1, out R2, out P1, out P2, out Q, flags, alpha, valueOrDefault, out var _, out var _);
	}

	public static void StereoRectify(double[,] cameraMatrix1, double[] distCoeffs1, double[,] cameraMatrix2, double[] distCoeffs2, Size imageSize, double[,] R, double[] T, out double[,] R1, out double[,] R2, out double[,] P1, out double[,] P2, out double[,] Q, StereoRectificationFlags flags, double alpha, Size newImageSize, out Rect validPixROI1, out Rect validPixROI2)
	{
		if (cameraMatrix1 == null)
		{
			throw new ArgumentNullException("cameraMatrix1");
		}
		if (distCoeffs1 == null)
		{
			throw new ArgumentNullException("distCoeffs1");
		}
		if (cameraMatrix2 == null)
		{
			throw new ArgumentNullException("cameraMatrix2");
		}
		if (distCoeffs2 == null)
		{
			throw new ArgumentNullException("distCoeffs2");
		}
		if (R == null)
		{
			throw new ArgumentNullException("R");
		}
		if (T == null)
		{
			throw new ArgumentNullException("T");
		}
		R1 = new double[3, 3];
		R2 = new double[3, 3];
		P1 = new double[3, 4];
		P2 = new double[3, 4];
		Q = new double[4, 4];
		NativeMethods.calib3d_stereoRectify_array(cameraMatrix1, distCoeffs1, distCoeffs1.Length, cameraMatrix2, distCoeffs2, distCoeffs2.Length, imageSize, R, T, R1, R2, P1, P2, Q, (int)flags, alpha, newImageSize, out validPixROI1, out validPixROI2);
	}

	public static bool StereoRectifyUncalibrated(InputArray points1, InputArray points2, InputArray F, Size imgSize, OutputArray H1, OutputArray H2, double threshold = 5.0)
	{
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (H1 == null)
		{
			throw new ArgumentNullException("H1");
		}
		if (H2 == null)
		{
			throw new ArgumentNullException("H2");
		}
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		F.ThrowIfDisposed();
		H1.ThrowIfNotReady();
		H2.ThrowIfNotReady();
		int num = NativeMethods.calib3d_stereoRectifyUncalibrated_InputArray(points1.CvPtr, points2.CvPtr, F.CvPtr, imgSize, H1.CvPtr, H2.CvPtr, threshold);
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		GC.KeepAlive(F);
		GC.KeepAlive(H1);
		GC.KeepAlive(H2);
		H1.Fix();
		H2.Fix();
		return num != 0;
	}

	public static bool StereoRectifyUncalibrated(IEnumerable<Point2d> points1, IEnumerable<Point2d> points2, double[,] F, Size imgSize, out double[,] H1, out double[,] H2, double threshold = 5.0)
	{
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (F.GetLength(0) != 3 || F.GetLength(1) != 3)
		{
			throw new ArgumentException("F != double[3,3]");
		}
		Point2d[] array = EnumerableEx.ToArray(points1);
		Point2d[] array2 = EnumerableEx.ToArray(points2);
		H1 = new double[3, 3];
		H2 = new double[3, 3];
		return NativeMethods.calib3d_stereoRectifyUncalibrated_array(array, array.Length, array2, array2.Length, F, imgSize, H1, H2, threshold) != 0;
	}

	public static float Rectify3Collinear(InputArray cameraMatrix1, InputArray distCoeffs1, InputArray cameraMatrix2, InputArray distCoeffs2, InputArray cameraMatrix3, InputArray distCoeffs3, IEnumerable<InputArray> imgpt1, IEnumerable<InputArray> imgpt3, Size imageSize, InputArray R12, InputArray T12, InputArray R13, InputArray T13, OutputArray R1, OutputArray R2, OutputArray R3, OutputArray P1, OutputArray P2, OutputArray P3, OutputArray Q, double alpha, Size newImgSize, out Rect roi1, out Rect roi2, StereoRectificationFlags flags)
	{
		if (cameraMatrix1 == null)
		{
			throw new ArgumentNullException("cameraMatrix1");
		}
		if (distCoeffs1 == null)
		{
			throw new ArgumentNullException("distCoeffs1");
		}
		if (cameraMatrix2 == null)
		{
			throw new ArgumentNullException("cameraMatrix2");
		}
		if (distCoeffs2 == null)
		{
			throw new ArgumentNullException("distCoeffs2");
		}
		if (cameraMatrix3 == null)
		{
			throw new ArgumentNullException("cameraMatrix3");
		}
		if (distCoeffs3 == null)
		{
			throw new ArgumentNullException("distCoeffs3");
		}
		if (imgpt1 == null)
		{
			throw new ArgumentNullException("imgpt1");
		}
		if (imgpt3 == null)
		{
			throw new ArgumentNullException("imgpt3");
		}
		if (R12 == null)
		{
			throw new ArgumentNullException("R12");
		}
		if (T12 == null)
		{
			throw new ArgumentNullException("T12");
		}
		if (R13 == null)
		{
			throw new ArgumentNullException("R13");
		}
		if (T13 == null)
		{
			throw new ArgumentNullException("T13");
		}
		if (R1 == null)
		{
			throw new ArgumentNullException("R1");
		}
		if (R2 == null)
		{
			throw new ArgumentNullException("R2");
		}
		if (R3 == null)
		{
			throw new ArgumentNullException("R3");
		}
		if (P1 == null)
		{
			throw new ArgumentNullException("P1");
		}
		if (P2 == null)
		{
			throw new ArgumentNullException("P2");
		}
		if (P3 == null)
		{
			throw new ArgumentNullException("P3");
		}
		if (Q == null)
		{
			throw new ArgumentNullException("Q");
		}
		cameraMatrix1.ThrowIfDisposed();
		distCoeffs1.ThrowIfDisposed();
		cameraMatrix2.ThrowIfDisposed();
		distCoeffs2.ThrowIfDisposed();
		cameraMatrix3.ThrowIfDisposed();
		distCoeffs3.ThrowIfDisposed();
		R12.ThrowIfDisposed();
		T12.ThrowIfDisposed();
		R13.ThrowIfDisposed();
		T13.ThrowIfDisposed();
		R1.ThrowIfNotReady();
		R2.ThrowIfNotReady();
		R3.ThrowIfNotReady();
		P1.ThrowIfNotReady();
		P2.ThrowIfNotReady();
		P3.ThrowIfNotReady();
		Q.ThrowIfNotReady();
		IntPtr[] array = EnumerableEx.SelectPtrs(imgpt1);
		IntPtr[] array2 = EnumerableEx.SelectPtrs(imgpt3);
		float result = NativeMethods.calib3d_rectify3Collinear_InputArray(cameraMatrix1.CvPtr, distCoeffs1.CvPtr, cameraMatrix2.CvPtr, distCoeffs2.CvPtr, cameraMatrix3.CvPtr, distCoeffs3.CvPtr, array, array.Length, array2, array2.Length, imageSize, R12.CvPtr, T12.CvPtr, R13.CvPtr, T13.CvPtr, R1.CvPtr, R2.CvPtr, R3.CvPtr, P1.CvPtr, P2.CvPtr, P3.CvPtr, Q.CvPtr, alpha, newImgSize, out roi1, out roi2, (int)flags);
		GC.KeepAlive(cameraMatrix1);
		GC.KeepAlive(distCoeffs1);
		GC.KeepAlive(cameraMatrix2);
		GC.KeepAlive(distCoeffs2);
		GC.KeepAlive(cameraMatrix3);
		GC.KeepAlive(distCoeffs3);
		GC.KeepAlive(imgpt1);
		GC.KeepAlive(imgpt3);
		GC.KeepAlive(R12);
		GC.KeepAlive(T12);
		GC.KeepAlive(R13);
		GC.KeepAlive(T13);
		GC.KeepAlive(R1);
		GC.KeepAlive(R2);
		GC.KeepAlive(R3);
		GC.KeepAlive(P1);
		GC.KeepAlive(P2);
		GC.KeepAlive(P3);
		GC.KeepAlive(Q);
		R1.Fix();
		R2.Fix();
		R3.Fix();
		P1.Fix();
		P2.Fix();
		P3.Fix();
		Q.Fix();
		return result;
	}

	public static Mat GetOptimalNewCameraMatrix(InputArray cameraMatrix, InputArray distCoeffs, Size imageSize, double alpha, Size newImgSize, out Rect validPixROI, bool centerPrincipalPoint = false)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException();
		}
		cameraMatrix.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.calib3d_getOptimalNewCameraMatrix_InputArray(cameraMatrix.CvPtr, ToPtr(distCoeffs), imageSize, alpha, newImgSize, out validPixROI, centerPrincipalPoint ? 1 : 0);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		return new Mat(ptr);
	}

	public static double[,] GetOptimalNewCameraMatrix(double[,] cameraMatrix, double[] distCoeffs, Size imageSize, double alpha, Size newImgSize, out Rect validPixROI, bool centerPrincipalPoint = false)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException();
		}
		IntPtr intPtr = NativeMethods.calib3d_getOptimalNewCameraMatrix_array(cameraMatrix, distCoeffs, distCoeffs.Length, imageSize, alpha, newImgSize, out validPixROI, centerPrincipalPoint ? 1 : 0);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		using MatOfDouble matOfDouble = new MatOfDouble(intPtr);
		return matOfDouble.ToRectangularArray();
	}

	public static void ConvertPointsToHomogeneous(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.calib3d_convertPointsToHomogeneous_InputArray(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static Vec3f[] ConvertPointsToHomogeneous(IEnumerable<Vec2f> src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		Vec2f[] array = EnumerableEx.ToArray(src);
		Vec3f[] array2 = new Vec3f[array.Length];
		NativeMethods.calib3d_convertPointsToHomogeneous_array1(array, array2, array.Length);
		return array2;
	}

	public static Vec4f[] ConvertPointsToHomogeneous(IEnumerable<Vec3f> src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		Vec3f[] array = EnumerableEx.ToArray(src);
		Vec4f[] array2 = new Vec4f[array.Length];
		NativeMethods.calib3d_convertPointsToHomogeneous_array2(array, array2, array.Length);
		return array2;
	}

	public static void ConvertPointsFromHomogeneous(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.calib3d_convertPointsFromHomogeneous_InputArray(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		dst.Fix();
	}

	public static Vec2f[] ConvertPointsFromHomogeneous(IEnumerable<Vec3f> src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		Vec3f[] array = EnumerableEx.ToArray(src);
		Vec2f[] array2 = new Vec2f[array.Length];
		NativeMethods.calib3d_convertPointsFromHomogeneous_array1(array, array2, array.Length);
		return array2;
	}

	public static Vec3f[] ConvertPointsFromHomogeneous(IEnumerable<Vec4f> src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		Vec4f[] array = EnumerableEx.ToArray(src);
		Vec3f[] array2 = new Vec3f[array.Length];
		NativeMethods.calib3d_convertPointsFromHomogeneous_array2(array, array2, array.Length);
		return array2;
	}

	public static void ConvertPointsHomogeneous(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.calib3d_convertPointsHomogeneous(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		dst.Fix();
	}

	public static Mat FindFundamentalMat(InputArray points1, InputArray points2, FundamentalMatMethod method = FundamentalMatMethod.Ransac, double param1 = 3.0, double param2 = 0.99, OutputArray mask = null)
	{
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.calib3d_findFundamentalMat_InputArray(points1.CvPtr, points2.CvPtr, (int)method, param1, param2, ToPtr(mask));
		mask?.Fix();
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		return new Mat(ptr);
	}

	public static Mat FindFundamentalMat(IEnumerable<Point2d> points1, IEnumerable<Point2d> points2, FundamentalMatMethod method = FundamentalMatMethod.Ransac, double param1 = 3.0, double param2 = 0.99, OutputArray mask = null)
	{
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		Point2d[] array = EnumerableEx.ToArray(points1);
		Point2d[] array2 = EnumerableEx.ToArray(points2);
		IntPtr ptr = NativeMethods.calib3d_findFundamentalMat_array(array, array.Length, array2, array2.Length, (int)method, param1, param2, ToPtr(mask));
		mask?.Fix();
		return new Mat(ptr);
	}

	public static void ComputeCorrespondEpilines(InputArray points, int whichImage, InputArray F, OutputArray lines)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (lines == null)
		{
			throw new ArgumentNullException("lines");
		}
		points.ThrowIfDisposed();
		F.ThrowIfDisposed();
		lines.ThrowIfNotReady();
		NativeMethods.calib3d_computeCorrespondEpilines_InputArray(points.CvPtr, whichImage, F.CvPtr, lines.CvPtr);
		GC.KeepAlive(F);
		GC.KeepAlive(points);
		lines.Fix();
	}

	public static Point3f[] ComputeCorrespondEpilines(IEnumerable<Point2d> points, int whichImage, double[,] F)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (F.GetLength(0) != 3 && F.GetLength(1) != 3)
		{
			throw new ArgumentException("F != double[3,3]");
		}
		Point2d[] array = EnumerableEx.ToArray(points);
		Point3f[] array2 = new Point3f[array.Length];
		NativeMethods.calib3d_computeCorrespondEpilines_array2d(array, array.Length, whichImage, F, array2);
		return array2;
	}

	public static Point3f[] ComputeCorrespondEpilines(IEnumerable<Point3d> points, int whichImage, double[,] F)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (F.GetLength(0) != 3 && F.GetLength(1) != 3)
		{
			throw new ArgumentException("F != double[3,3]");
		}
		Point3d[] array = EnumerableEx.ToArray(points);
		Point3f[] array2 = new Point3f[array.Length];
		NativeMethods.calib3d_computeCorrespondEpilines_array3d(array, array.Length, whichImage, F, array2);
		return array2;
	}

	public static void TriangulatePoints(InputArray projMatr1, InputArray projMatr2, InputArray projPoints1, InputArray projPoints2, OutputArray points4D)
	{
		if (projMatr1 == null)
		{
			throw new ArgumentNullException("projMatr1");
		}
		if (projMatr2 == null)
		{
			throw new ArgumentNullException("projMatr2");
		}
		if (projPoints1 == null)
		{
			throw new ArgumentNullException("projPoints1");
		}
		if (projPoints2 == null)
		{
			throw new ArgumentNullException("projPoints2");
		}
		if (points4D == null)
		{
			throw new ArgumentNullException("points4D");
		}
		projMatr1.ThrowIfDisposed();
		projMatr2.ThrowIfDisposed();
		projPoints1.ThrowIfDisposed();
		projPoints2.ThrowIfDisposed();
		points4D.ThrowIfNotReady();
		NativeMethods.calib3d_triangulatePoints_InputArray(projMatr1.CvPtr, projMatr2.CvPtr, projPoints1.CvPtr, projPoints2.CvPtr, points4D.CvPtr);
		GC.KeepAlive(projMatr1);
		GC.KeepAlive(projMatr2);
		GC.KeepAlive(projPoints1);
		GC.KeepAlive(projPoints2);
		points4D.Fix();
	}

	public static Vec4d[] TriangulatePoints(double[,] projMatr1, double[,] projMatr2, IEnumerable<Point2d> projPoints1, IEnumerable<Point2d> projPoints2)
	{
		if (projMatr1 == null)
		{
			throw new ArgumentNullException("projMatr1");
		}
		if (projMatr2 == null)
		{
			throw new ArgumentNullException("projMatr2");
		}
		if (projPoints1 == null)
		{
			throw new ArgumentNullException("projPoints1");
		}
		if (projPoints2 == null)
		{
			throw new ArgumentNullException("projPoints2");
		}
		if (projMatr1.GetLength(0) != 3 && projMatr1.GetLength(1) != 4)
		{
			throw new ArgumentException("projMatr1 != double[3,4]");
		}
		if (projMatr2.GetLength(0) != 3 && projMatr2.GetLength(1) != 4)
		{
			throw new ArgumentException("projMatr2 != double[3,4]");
		}
		Point2d[] array = EnumerableEx.ToArray(projPoints1);
		Point2d[] array2 = EnumerableEx.ToArray(projPoints2);
		Vec4d[] array3 = new Vec4d[array.Length];
		NativeMethods.calib3d_triangulatePoints_array(projMatr1, projMatr2, array, array.Length, array2, array2.Length, array3);
		return array3;
	}

	public static void CorrectMatches(InputArray F, InputArray points1, InputArray points2, OutputArray newPoints1, OutputArray newPoints2)
	{
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (newPoints1 == null)
		{
			throw new ArgumentNullException("newPoints1");
		}
		if (newPoints2 == null)
		{
			throw new ArgumentNullException("newPoints2");
		}
		F.ThrowIfDisposed();
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		newPoints1.ThrowIfNotReady();
		newPoints2.ThrowIfNotReady();
		NativeMethods.calib3d_correctMatches_InputArray(F.CvPtr, points1.CvPtr, points2.CvPtr, newPoints1.CvPtr, newPoints2.CvPtr);
		GC.KeepAlive(F);
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		newPoints1.Fix();
		newPoints2.Fix();
	}

	public static void CorrectMatches(double[,] F, IEnumerable<Point2d> points1, IEnumerable<Point2d> points2, out Point2d[] newPoints1, out Point2d[] newPoints2)
	{
		if (F == null)
		{
			throw new ArgumentNullException("F");
		}
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		Point2d[] array = EnumerableEx.ToArray(points1);
		Point2d[] array2 = EnumerableEx.ToArray(points2);
		newPoints1 = new Point2d[array.Length];
		newPoints2 = new Point2d[array2.Length];
		NativeMethods.calib3d_correctMatches_array(F, array, array.Length, array2, array2.Length, newPoints1, newPoints2);
	}

	public static int RecoverPose(InputArray E, InputArray points1, InputArray points2, InputArray cameraMatrix, OutputArray R, OutputArray t, InputOutputArray mask = null)
	{
		if (E == null)
		{
			throw new ArgumentNullException("E");
		}
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (R == null)
		{
			throw new ArgumentNullException("R");
		}
		if (t == null)
		{
			throw new ArgumentNullException("t");
		}
		E.ThrowIfDisposed();
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		R.ThrowIfNotReady();
		t.ThrowIfNotReady();
		int result = NativeMethods.calib3d_recoverPose_InputArray1(E.CvPtr, points1.CvPtr, points2.CvPtr, cameraMatrix.CvPtr, R.CvPtr, t.CvPtr, ToPtr(mask));
		GC.KeepAlive(E);
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		GC.KeepAlive(cameraMatrix);
		R.Fix();
		t.Fix();
		mask?.Fix();
		return result;
	}

	public static int RecoverPose(InputArray E, InputArray points1, InputArray points2, OutputArray R, OutputArray t, double focal, Point2d pp, InputOutputArray mask = null)
	{
		if (E == null)
		{
			throw new ArgumentNullException("E");
		}
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (R == null)
		{
			throw new ArgumentNullException("R");
		}
		if (t == null)
		{
			throw new ArgumentNullException("t");
		}
		E.ThrowIfDisposed();
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		R.ThrowIfNotReady();
		t.ThrowIfNotReady();
		int result = NativeMethods.calib3d_recoverPose_InputArray2(E.CvPtr, points1.CvPtr, points2.CvPtr, R.CvPtr, t.CvPtr, focal, new StructurePointer<Point2d>(pp), ToPtr(mask));
		GC.KeepAlive(E);
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		GC.KeepAlive(pp);
		R.Fix();
		t.Fix();
		mask?.Fix();
		return result;
	}

	public static int RecoverPose(InputArray E, InputArray points1, InputArray points2, InputArray cameraMatrix, OutputArray R, OutputArray t, double distanceTresh, InputOutputArray mask = null, OutputArray triangulatedPoints = null)
	{
		if (E == null)
		{
			throw new ArgumentNullException("E");
		}
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (R == null)
		{
			throw new ArgumentNullException("R");
		}
		if (t == null)
		{
			throw new ArgumentNullException("t");
		}
		E.ThrowIfDisposed();
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		R.ThrowIfNotReady();
		t.ThrowIfNotReady();
		int result = NativeMethods.calib3d_recoverPose_InputArray3(E.CvPtr, points1.CvPtr, points2.CvPtr, cameraMatrix.CvPtr, R.CvPtr, t.CvPtr, distanceTresh, ToPtr(mask), ToPtr(triangulatedPoints));
		GC.KeepAlive(E);
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		GC.KeepAlive(cameraMatrix);
		R.Fix();
		t.Fix();
		mask?.Fix();
		triangulatedPoints?.Fix();
		return result;
	}

	public static Mat FindEssentialMat(InputArray points1, InputArray points2, InputArray cameraMatrix, EssentialMatMethod method = EssentialMatMethod.Ransac, double prob = 0.999, double threshold = 1.0, OutputArray mask = null)
	{
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		cameraMatrix.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.calib3d_findEssentialMat_InputArray1(points1.CvPtr, points2.CvPtr, cameraMatrix.CvPtr, (int)method, prob, threshold, ToPtr(mask));
		mask?.Fix();
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		GC.KeepAlive(cameraMatrix);
		return new Mat(ptr);
	}

	public static Mat FindEssentialMat(InputArray points1, InputArray points2, double focal, Point2d pp, EssentialMatMethod method = EssentialMatMethod.Ransac, double prob = 0.999, double threshold = 1.0, OutputArray mask = null)
	{
		if (points1 == null)
		{
			throw new ArgumentNullException("points1");
		}
		if (points2 == null)
		{
			throw new ArgumentNullException("points2");
		}
		points1.ThrowIfDisposed();
		points2.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.calib3d_findEssentialMat_InputArray2(points1.CvPtr, points2.CvPtr, focal, new StructurePointer<Point2d>(pp), (int)method, prob, threshold, ToPtr(mask));
		mask?.Fix();
		GC.KeepAlive(points1);
		GC.KeepAlive(points2);
		return new Mat(ptr);
	}

	public static void FilterSpeckles(InputOutputArray img, double newVal, int maxSpeckleSize, double maxDiff, InputOutputArray buf = null)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfNotReady();
		NativeMethods.calib3d_filterSpeckles(img.CvPtr, newVal, maxSpeckleSize, maxDiff, ToPtr(buf));
		GC.KeepAlive(img);
		GC.KeepAlive(buf);
		img.Fix();
	}

	public static Rect GetValidDisparityROI(Rect roi1, Rect roi2, int minDisparity, int numberOfDisparities, int SADWindowSize)
	{
		return NativeMethods.calib3d_getValidDisparityROI(roi1, roi2, minDisparity, numberOfDisparities, SADWindowSize);
	}

	public static void ValidateDisparity(InputOutputArray disparity, InputArray cost, int minDisparity, int numberOfDisparities, int disp12MaxDisp = 1)
	{
		if (disparity == null)
		{
			throw new ArgumentNullException("disparity");
		}
		if (cost == null)
		{
			throw new ArgumentNullException("cost");
		}
		disparity.ThrowIfNotReady();
		cost.ThrowIfDisposed();
		NativeMethods.calib3d_validateDisparity(disparity.CvPtr, cost.CvPtr, minDisparity, numberOfDisparities, disp12MaxDisp);
		disparity.Fix();
		GC.KeepAlive(disparity);
		GC.KeepAlive(cost);
	}

	public static void ReprojectImageTo3D(InputArray disparity, OutputArray _3dImage, InputArray Q, bool handleMissingValues = false, int ddepth = -1)
	{
		if (disparity == null)
		{
			throw new ArgumentNullException("disparity");
		}
		if (_3dImage == null)
		{
			throw new ArgumentNullException("_3dImage");
		}
		if (Q == null)
		{
			throw new ArgumentNullException("Q");
		}
		disparity.ThrowIfDisposed();
		_3dImage.ThrowIfNotReady();
		Q.ThrowIfDisposed();
		NativeMethods.calib3d_reprojectImageTo3D(disparity.CvPtr, _3dImage.CvPtr, Q.CvPtr, handleMissingValues ? 1 : 0, ddepth);
		_3dImage.Fix();
		GC.KeepAlive(disparity);
		GC.KeepAlive(_3dImage);
		GC.KeepAlive(Q);
	}

	public static int EstimateAffine3D(InputArray src, InputArray dst, OutputArray outVal, OutputArray inliers, double ransacThreshold = 3.0, double confidence = 0.99)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (outVal == null)
		{
			throw new ArgumentNullException("outVal");
		}
		if (inliers == null)
		{
			throw new ArgumentNullException("inliers");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		outVal.ThrowIfNotReady();
		inliers.ThrowIfNotReady();
		int result = NativeMethods.calib3d_estimateAffine3D(src.CvPtr, dst.CvPtr, outVal.CvPtr, inliers.CvPtr, ransacThreshold, confidence);
		outVal.Fix();
		inliers.Fix();
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		return result;
	}

	public static double SampsonDistance(InputArray pt1, InputArray pt2, InputArray f)
	{
		if (pt1 == null)
		{
			throw new ArgumentNullException("pt1");
		}
		if (pt2 == null)
		{
			throw new ArgumentNullException("pt2");
		}
		if (f == null)
		{
			throw new ArgumentNullException("f");
		}
		pt1.ThrowIfDisposed();
		pt2.ThrowIfDisposed();
		f.ThrowIfDisposed();
		double result = NativeMethods.calib3d_sampsonDistance_InputArray(pt1.CvPtr, pt2.CvPtr, f.CvPtr);
		GC.KeepAlive(pt1);
		GC.KeepAlive(pt2);
		return result;
	}

	public static double SampsonDistance(Point3d pt1, Point3d pt2, double[,] f)
	{
		if (f == null)
		{
			throw new ArgumentNullException("f");
		}
		if (f.GetLength(0) != 3 || f.GetLength(1) != 3)
		{
			throw new ArgumentException("f should be 3x3 matrix", "f");
		}
		double result = NativeMethods.calib3d_sampsonDistance_Point3d(pt1, pt2, f);
		GC.KeepAlive(f);
		return result;
	}

	public static Mat EstimateAffine2D(InputArray from, InputArray to, OutputArray inliers = null, RobustEstimationAlgorithms method = RobustEstimationAlgorithms.RANSAC, double ransacReprojThreshold = 3.0, ulong maxIters = 2000uL, double confidence = 0.99, ulong refineIters = 10uL)
	{
		if (from == null)
		{
			throw new ArgumentNullException("from");
		}
		if (to == null)
		{
			throw new ArgumentNullException("to");
		}
		from.ThrowIfDisposed();
		to.ThrowIfDisposed();
		inliers?.ThrowIfNotReady();
		IntPtr intPtr = NativeMethods.calib3d_estimateAffine2D(from.CvPtr, to.CvPtr, ToPtr(inliers), (int)method, ransacReprojThreshold, maxIters, confidence, refineIters);
		GC.KeepAlive(inliers);
		GC.KeepAlive(inliers);
		GC.KeepAlive(inliers);
		if (!(intPtr == IntPtr.Zero))
		{
			return new Mat(intPtr);
		}
		return null;
	}

	public static Mat EstimateAffinePartial2D(InputArray from, InputArray to, OutputArray inliers = null, RobustEstimationAlgorithms method = RobustEstimationAlgorithms.RANSAC, double ransacReprojThreshold = 3.0, ulong maxIters = 2000uL, double confidence = 0.99, ulong refineIters = 10uL)
	{
		if (from == null)
		{
			throw new ArgumentNullException("from");
		}
		if (to == null)
		{
			throw new ArgumentNullException("to");
		}
		from.ThrowIfDisposed();
		to.ThrowIfDisposed();
		inliers?.ThrowIfNotReady();
		IntPtr intPtr = NativeMethods.calib3d_estimateAffinePartial2D(from.CvPtr, to.CvPtr, ToPtr(inliers), (int)method, ransacReprojThreshold, maxIters, confidence, refineIters);
		GC.KeepAlive(inliers);
		GC.KeepAlive(inliers);
		GC.KeepAlive(inliers);
		if (!(intPtr == IntPtr.Zero))
		{
			return new Mat(intPtr);
		}
		return null;
	}

	public static int DecomposeHomographyMat(InputArray h, InputArray k, IEnumerable<Mat> rotations, IEnumerable<Mat> translations, IEnumerable<Mat> normals)
	{
		if (h == null)
		{
			throw new ArgumentNullException("h");
		}
		if (k == null)
		{
			throw new ArgumentNullException("k");
		}
		if (rotations == null)
		{
			throw new ArgumentNullException("rotations");
		}
		if (translations == null)
		{
			throw new ArgumentNullException("translations");
		}
		if (normals == null)
		{
			throw new ArgumentNullException("normals");
		}
		h.ThrowIfDisposed();
		k.ThrowIfDisposed();
		int result;
		using (VectorOfMat vectorOfMat = new VectorOfMat(rotations))
		{
			using VectorOfMat vectorOfMat2 = new VectorOfMat(translations);
			using VectorOfMat vectorOfMat3 = new VectorOfMat(normals);
			result = NativeMethods.calib3d_decomposeHomographyMat(h.CvPtr, k.CvPtr, vectorOfMat.CvPtr, vectorOfMat2.CvPtr, vectorOfMat3.CvPtr);
		}
		GC.KeepAlive(h);
		GC.KeepAlive(k);
		GC.KeepAlive(rotations);
		GC.KeepAlive(translations);
		GC.KeepAlive(normals);
		return result;
	}

	public static void FilterHomographyDecompByVisibleRefpoints(IEnumerable<Mat> rotations, IEnumerable<Mat> normals, InputArray beforePoints, InputArray afterPoints, OutputArray possibleSolutions, InputArray pointsMask = null)
	{
		if (rotations == null)
		{
			throw new ArgumentNullException("rotations");
		}
		if (normals == null)
		{
			throw new ArgumentNullException("normals");
		}
		if (beforePoints == null)
		{
			throw new ArgumentNullException("beforePoints");
		}
		if (afterPoints == null)
		{
			throw new ArgumentNullException("afterPoints");
		}
		if (possibleSolutions == null)
		{
			throw new ArgumentNullException("possibleSolutions");
		}
		beforePoints.ThrowIfDisposed();
		afterPoints.ThrowIfDisposed();
		possibleSolutions.ThrowIfNotReady();
		pointsMask?.ThrowIfDisposed();
		using (VectorOfMat vectorOfMat = new VectorOfMat(rotations))
		{
			using VectorOfMat vectorOfMat2 = new VectorOfMat(normals);
			NativeMethods.calib3d_filterHomographyDecompByVisibleRefpoints(vectorOfMat.CvPtr, vectorOfMat2.CvPtr, beforePoints.CvPtr, afterPoints.CvPtr, possibleSolutions.CvPtr, ToPtr(pointsMask));
		}
		GC.KeepAlive(rotations);
		GC.KeepAlive(normals);
		GC.KeepAlive(beforePoints);
		GC.KeepAlive(afterPoints);
		GC.KeepAlive(possibleSolutions);
		GC.KeepAlive(pointsMask);
	}

	public static void Undistort(InputArray src, OutputArray dst, InputArray cameraMatrix, InputArray distCoeffs, InputArray newCameraMatrix = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		cameraMatrix.ThrowIfDisposed();
		NativeMethods.calib3d_undistort(src.CvPtr, dst.CvPtr, cameraMatrix.CvPtr, ToPtr(distCoeffs), ToPtr(newCameraMatrix));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		GC.KeepAlive(newCameraMatrix);
		dst.Fix();
	}

	public static void InitUndistortRectifyMap(InputArray cameraMatrix, InputArray distCoeffs, InputArray r, InputArray newCameraMatrix, Size size, MatType m1Type, OutputArray map1, OutputArray map2)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (distCoeffs == null)
		{
			throw new ArgumentNullException("distCoeffs");
		}
		if (r == null)
		{
			throw new ArgumentNullException("r");
		}
		if (newCameraMatrix == null)
		{
			throw new ArgumentNullException("newCameraMatrix");
		}
		if (map1 == null)
		{
			throw new ArgumentNullException("map1");
		}
		if (map2 == null)
		{
			throw new ArgumentNullException("map2");
		}
		cameraMatrix.ThrowIfDisposed();
		distCoeffs.ThrowIfDisposed();
		r.ThrowIfDisposed();
		newCameraMatrix.ThrowIfDisposed();
		map1.ThrowIfNotReady();
		map2.ThrowIfNotReady();
		NativeMethods.calib3d_initUndistortRectifyMap(cameraMatrix.CvPtr, distCoeffs.CvPtr, r.CvPtr, newCameraMatrix.CvPtr, size, m1Type, map1.CvPtr, map2.CvPtr);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		GC.KeepAlive(r);
		GC.KeepAlive(newCameraMatrix);
		GC.KeepAlive(map1);
		GC.KeepAlive(map2);
		map1.Fix();
		map2.Fix();
	}

	public static float InitWideAngleProjMap(InputArray cameraMatrix, InputArray distCoeffs, Size imageSize, int destImageWidth, MatType m1Type, OutputArray map1, OutputArray map2, ProjectionType projType, double alpha = 0.0)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		if (distCoeffs == null)
		{
			throw new ArgumentNullException("distCoeffs");
		}
		if (map1 == null)
		{
			throw new ArgumentNullException("map1");
		}
		if (map2 == null)
		{
			throw new ArgumentNullException("map2");
		}
		cameraMatrix.ThrowIfDisposed();
		distCoeffs.ThrowIfDisposed();
		map1.ThrowIfNotReady();
		map2.ThrowIfNotReady();
		float result = NativeMethods.calib3d_initWideAngleProjMap(cameraMatrix.CvPtr, distCoeffs.CvPtr, imageSize, destImageWidth, m1Type, map1.CvPtr, map2.CvPtr, (int)projType, alpha);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		GC.KeepAlive(map1);
		GC.KeepAlive(map2);
		map1.Fix();
		map2.Fix();
		return result;
	}

	public static Mat GetDefaultNewCameraMatrix(InputArray cameraMatrix, Size? imgSize = null, bool centerPrincipalPoint = false)
	{
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		cameraMatrix.ThrowIfDisposed();
		Size size = default;
		Size imgsize = imgSize ?? size;
		IntPtr ptr = NativeMethods.calib3d_getDefaultNewCameraMatrix(cameraMatrix.CvPtr, imgsize, centerPrincipalPoint ? 1 : 0);
		GC.KeepAlive(cameraMatrix);
		return new Mat(ptr);
	}

	public static void UndistortPoints(InputArray src, OutputArray dst, InputArray cameraMatrix, InputArray distCoeffs, InputArray r = null, InputArray p = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (cameraMatrix == null)
		{
			throw new ArgumentNullException("cameraMatrix");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		cameraMatrix.ThrowIfDisposed();
		NativeMethods.calib3d_undistortPoints(src.CvPtr, dst.CvPtr, cameraMatrix.CvPtr, ToPtr(distCoeffs), ToPtr(r), ToPtr(p));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(cameraMatrix);
		GC.KeepAlive(distCoeffs);
		GC.KeepAlive(r);
		GC.KeepAlive(p);
		dst.Fix();
	}

	public static void SetNumThreads(int nThreads)
	{
		NativeMethods.core_setNumThreads(nThreads);
	}

	public static int GetNumThreads()
	{
		return NativeMethods.core_getNumThreads();
	}

	public static int GetThreadNum()
	{
		return NativeMethods.core_getThreadNum();
	}

	public static string GetBuildInformation()
	{
		StringBuilder stringBuilder = new StringBuilder(NativeMethods.core_getBuildInformation_length() + 1);
		NativeMethods.core_getBuildInformation(stringBuilder, stringBuilder.Capacity);
		return stringBuilder.ToString();
	}

	public static string GetVersionString()
	{
		StringBuilder stringBuilder = new StringBuilder(129);
		NativeMethods.core_getVersionString(stringBuilder, stringBuilder.Capacity);
		return stringBuilder.ToString();
	}

	public static int GetVersionMajor()
	{
		return NativeMethods.core_getVersionMajor();
	}

	public static int GetVersionMinor()
	{
		return NativeMethods.core_getVersionMinor();
	}

	public static int GetVersionRevision()
	{
		return NativeMethods.core_getVersionRevision();
	}

	public static long GetTickCount()
	{
		return NativeMethods.core_getTickCount();
	}

	public static double GetTickFrequency()
	{
		return NativeMethods.core_getTickFrequency();
	}

	public static long GetCpuTickCount()
	{
		return NativeMethods.core_getCPUTickCount();
	}

	public static bool CheckHardwareSupport(CpuFeatures feature)
	{
		return NativeMethods.core_checkHardwareSupport((int)feature) != 0;
	}

	public static string GetHardwareFeatureName(int feature)
	{
		StringBuilder stringBuilder = new StringBuilder(129);
		NativeMethods.core_getHardwareFeatureName(feature, stringBuilder, stringBuilder.Capacity);
		return stringBuilder.ToString();
	}

	public static string GetCpuFeaturesLine()
	{
		StringBuilder stringBuilder = new StringBuilder(513);
		NativeMethods.core_getCPUFeaturesLine(stringBuilder, stringBuilder.Capacity);
		return stringBuilder.ToString();
	}

	public static int GetNumberOfCpus()
	{
		return NativeMethods.core_getNumberOfCPUs();
	}

	public static IntPtr FastMalloc(long bufSize)
	{
		return NativeMethods.core_fastMalloc(new IntPtr(bufSize));
	}

	public static void FastFree(IntPtr ptr)
	{
		NativeMethods.core_fastFree(ptr);
	}

	public static void SetUseOptimized(bool onoff)
	{
		NativeMethods.core_setUseOptimized(onoff ? 1 : 0);
	}

	public static bool UseOptimized()
	{
		return NativeMethods.core_useOptimized() != 0;
	}

	public static int AlignSize(int sz, int n)
	{
		if ((n & (n - 1)) != 0)
		{
			throw new ArgumentException();
		}
		return (sz + n - 1) & -n;
	}

	public static string[] Glob(string pattern, bool recursive = false)
	{
		if (pattern == null)
		{
			throw new ArgumentNullException("pattern");
		}
		using VectorOfString vectorOfString = new VectorOfString();
		NativeMethods.core_glob(pattern, vectorOfString.CvPtr, recursive ? 1 : 0);
		return vectorOfString.ToArray();
	}

	public static bool SetBreakOnError(bool flag)
	{
		return NativeMethods.core_setBreakOnError(flag ? 1 : 0) != 0;
	}

	public unsafe static string Format(InputArray mtx, FormatType format = FormatType.Default)
	{
		if (mtx == null)
		{
			throw new ArgumentNullException("mtx");
		}
		sbyte* ptr = null;
		try
		{
			ptr = NativeMethods.core_format(mtx.CvPtr, (int)format);
			return StringHelper.PtrToStringAnsi(ptr);
		}
		finally
		{
			if (ptr != null)
			{
				NativeMethods.core_char_delete(ptr);
			}
			GC.KeepAlive(mtx);
		}
	}

	public static MatExpr Abs(Mat src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.core_abs_Mat(src.CvPtr);
		GC.KeepAlive(src);
		return new MatExpr(ptr);
	}

	public static MatExpr Abs(MatExpr src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.core_abs_MatExpr(src.CvPtr);
		GC.KeepAlive(src);
		return new MatExpr(ptr);
	}

	public static void Add(InputArray src1, InputArray src2, OutputArray dst, InputArray mask = null, int dtype = -1)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_add(src1.CvPtr, src2.CvPtr, dst.CvPtr, ToPtr(mask), dtype);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void Subtract(InputArray src1, InputArray src2, OutputArray dst, InputArray mask = null, int dtype = -1)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_subtract_InputArray2(src1.CvPtr, src2.CvPtr, dst.CvPtr, ToPtr(mask), dtype);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
		GC.KeepAlive(mask);
	}

	public static void Subtract(InputArray src1, Scalar src2, OutputArray dst, InputArray mask = null, int dtype = -1)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_subtract_InputArrayScalar(src1.CvPtr, src2, dst.CvPtr, ToPtr(mask), dtype);
		GC.KeepAlive(src1);
		GC.KeepAlive(dst);
		dst.Fix();
		GC.KeepAlive(mask);
	}

	public static void Subtract(Scalar src1, InputArray src2, OutputArray dst, InputArray mask = null, int dtype = -1)
	{
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_subtract_ScalarInputArray(src1, src2.CvPtr, dst.CvPtr, ToPtr(mask), dtype);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
		GC.KeepAlive(mask);
	}

	public static void Multiply(InputArray src1, InputArray src2, OutputArray dst, double scale = 1.0, int dtype = -1)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_multiply(src1.CvPtr, src2.CvPtr, dst.CvPtr, scale, dtype);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Divide(InputArray src1, InputArray src2, OutputArray dst, double scale = 1.0, int dtype = -1)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_divide2(src1.CvPtr, src2.CvPtr, dst.CvPtr, scale, dtype);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Divide(double scale, InputArray src2, OutputArray dst, int dtype = -1)
	{
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_divide1(scale, src2.CvPtr, dst.CvPtr, dtype);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void ScaleAdd(InputArray src1, double alpha, InputArray src2, OutputArray dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_scaleAdd(src1.CvPtr, alpha, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void AddWeighted(InputArray src1, double alpha, InputArray src2, double beta, double gamma, OutputArray dst, int dtype = -1)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_addWeighted(src1.CvPtr, alpha, src2.CvPtr, beta, gamma, dst.CvPtr, dtype);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static int BorderInterpolate(int p, int len, BorderTypes borderType)
	{
		return NativeMethods.core_borderInterpolate(p, len, (int)borderType);
	}

	public static void CopyMakeBorder(InputArray src, OutputArray dst, int top, int bottom, int left, int right, BorderTypes borderType, Scalar? value = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Scalar scalar = default;
		Scalar value2 = value ?? scalar;
		NativeMethods.core_copyMakeBorder(src.CvPtr, dst.CvPtr, top, bottom, left, right, (int)borderType, value2);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void ConvertScaleAbs(InputArray src, OutputArray dst, double alpha = 1.0, double beta = 0.0)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_convertScaleAbs(src.CvPtr, dst.CvPtr, alpha, beta);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void LUT(InputArray src, InputArray lut, OutputArray dst, int interpolation = 0)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (lut == null)
		{
			throw new ArgumentNullException("lut");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		lut.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_LUT(src.CvPtr, lut.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(lut);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void LUT(InputArray src, byte[] lut, OutputArray dst, int interpolation = 0)
	{
		if (lut == null)
		{
			throw new ArgumentNullException("lut");
		}
		if (lut.Length != 256)
		{
			throw new ArgumentException("lut.Length != 256");
		}
		using Mat mat = new Mat(256, 1, MatType.CV_8UC1, lut, 0L);
		LUT(src, mat, dst, interpolation);
	}

	public static Scalar Sum(InputArray src)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		Scalar result = NativeMethods.core_sum(src.CvPtr);
		GC.KeepAlive(src);
		return result;
	}

	public static int CountNonZero(InputArray mtx)
	{
		if (mtx == null)
		{
			throw new ArgumentNullException("mtx");
		}
		mtx.ThrowIfDisposed();
		int result = NativeMethods.core_countNonZero(mtx.CvPtr);
		GC.KeepAlive(mtx);
		return result;
	}

	public static void FindNonZero(InputArray src, OutputArray idx)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (idx == null)
		{
			throw new ArgumentNullException("idx");
		}
		src.ThrowIfDisposed();
		idx.ThrowIfNotReady();
		NativeMethods.core_findNonZero(src.CvPtr, idx.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(idx);
		idx.Fix();
	}

	public static Scalar Mean(InputArray src, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		Scalar result = NativeMethods.core_mean(src.CvPtr, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
		return result;
	}

	public static void MeanStdDev(InputArray src, OutputArray mean, OutputArray stddev, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		if (stddev == null)
		{
			throw new ArgumentNullException("stddev");
		}
		src.ThrowIfDisposed();
		mean.ThrowIfNotReady();
		stddev.ThrowIfNotReady();
		NativeMethods.core_meanStdDev_OutputArray(src.CvPtr, mean.CvPtr, stddev.CvPtr, ToPtr(mask));
		mean.Fix();
		stddev.Fix();
		GC.KeepAlive(src);
		GC.KeepAlive(mean);
		GC.KeepAlive(stddev);
		GC.KeepAlive(mask);
	}

	public static void MeanStdDev(InputArray src, out Scalar mean, out Scalar stddev, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		NativeMethods.core_meanStdDev_Scalar(src.CvPtr, out mean, out stddev, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
	}

	public static double Norm(InputArray src1, NormTypes normType = NormTypes.L2, InputArray mask = null)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		src1.ThrowIfDisposed();
		double result = NativeMethods.core_norm1(src1.CvPtr, (int)normType, ToPtr(mask));
		GC.KeepAlive(src1);
		GC.KeepAlive(mask);
		return result;
	}

	public static double Norm(InputArray src1, InputArray src2, NormTypes normType = NormTypes.L2, InputArray mask = null)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		double result = NativeMethods.core_norm2(src1.CvPtr, src2.CvPtr, (int)normType, ToPtr(mask));
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(mask);
		return result;
	}

	public static void BatchDistance(InputArray src1, InputArray src2, OutputArray dist, int dtype, OutputArray nidx, NormTypes normType = NormTypes.L2, int k = 0, InputArray mask = null, int update = 0, bool crosscheck = false)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dist == null)
		{
			throw new ArgumentNullException("dist");
		}
		if (nidx == null)
		{
			throw new ArgumentNullException("nidx");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dist.ThrowIfNotReady();
		nidx.ThrowIfNotReady();
		NativeMethods.core_batchDistance(src1.CvPtr, src2.CvPtr, dist.CvPtr, dtype, nidx.CvPtr, (int)normType, k, ToPtr(mask), update, crosscheck ? 1 : 0);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dist);
		GC.KeepAlive(nidx);
		dist.Fix();
		nidx.Fix();
		GC.KeepAlive(mask);
	}

	public static void Normalize(InputArray src, InputOutputArray dst, double alpha = 1.0, double beta = 0.0, NormTypes normType = NormTypes.L2, int dtype = -1, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_normalize(src.CvPtr, dst.CvPtr, alpha, beta, (int)normType, dtype, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
		GC.KeepAlive(mask);
	}

	public static void MinMaxLoc(InputArray src, out double minVal, out double maxVal)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		NativeMethods.core_minMaxLoc1(src.CvPtr, out minVal, out maxVal);
		GC.KeepAlive(src);
	}

	public static void MinMaxLoc(InputArray src, out Point minLoc, out Point maxLoc)
	{
		MinMaxLoc(src, out var _, out var _, out minLoc, out maxLoc);
	}

	public static void MinMaxLoc(InputArray src, out double minVal, out double maxVal, out Point minLoc, out Point maxLoc, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		NativeMethods.core_minMaxLoc2(src.CvPtr, out minVal, out maxVal, out minLoc, out maxLoc, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
	}

	public static void MinMaxIdx(InputArray src, out double minVal, out double maxVal)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		NativeMethods.core_minMaxIdx1(src.CvPtr, out minVal, out maxVal);
		GC.KeepAlive(src);
	}

	public static void MinMaxIdx(InputArray src, int[] minIdx, int[] maxIdx)
	{
		MinMaxIdx(src, out var _, out var _, minIdx, maxIdx);
	}

	public static void MinMaxIdx(InputArray src, out double minVal, out double maxVal, int[] minIdx, int[] maxIdx, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (minIdx == null)
		{
			throw new ArgumentNullException("minIdx");
		}
		if (maxIdx == null)
		{
			throw new ArgumentNullException("maxIdx");
		}
		src.ThrowIfDisposed();
		NativeMethods.core_minMaxIdx2(src.CvPtr, out minVal, out maxVal, minIdx, maxIdx, ToPtr(mask));
		GC.KeepAlive(src);
	}

	public static void Reduce(InputArray src, OutputArray dst, ReduceDimension dim, ReduceTypes rtype, int dtype)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_reduce(src.CvPtr, dst.CvPtr, (int)dim, (int)rtype, dtype);
		dst.Fix();
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
	}

	public static void Merge(Mat[] mv, Mat dst)
	{
		if (mv == null)
		{
			throw new ArgumentNullException("mv");
		}
		if (mv.Length == 0)
		{
			throw new ArgumentException("mv.Length == 0");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		for (int i = 0; i < mv.Length; i++)
		{
			(mv[i] ?? throw new ArgumentException("mv contains null element")).ThrowIfDisposed();
		}
		dst.ThrowIfDisposed();
		IntPtr[] array = new IntPtr[mv.Length];
		for (int j = 0; j < mv.Length; j++)
		{
			array[j] = mv[j].CvPtr;
		}
		NativeMethods.core_merge(array, (uint)array.Length, dst.CvPtr);
		GC.KeepAlive(mv);
		GC.KeepAlive(dst);
	}

	public static void Split(Mat src, out Mat[] mv)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		NativeMethods.core_split(src.CvPtr, out var mv2);
		using (VectorOfMat vectorOfMat = new VectorOfMat(mv2))
		{
			mv = vectorOfMat.ToArray();
		}
		GC.KeepAlive(src);
	}

	public static Mat[] Split(Mat src)
	{
		Split(src, out var mv);
		return mv;
	}

	public static void MixChannels(Mat[] src, Mat[] dst, int[] fromTo)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (fromTo == null)
		{
			throw new ArgumentNullException("fromTo");
		}
		if (src.Length == 0)
		{
			throw new ArgumentException("src.Length == 0");
		}
		if (dst.Length == 0)
		{
			throw new ArgumentException("dst.Length == 0");
		}
		if (fromTo.Length == 0 || fromTo.Length % 2 != 0)
		{
			throw new ArgumentException("fromTo.Length == 0");
		}
		IntPtr[] array = new IntPtr[src.Length];
		IntPtr[] array2 = new IntPtr[dst.Length];
		for (int i = 0; i < src.Length; i++)
		{
			src[i].ThrowIfDisposed();
			array[i] = src[i].CvPtr;
		}
		for (int j = 0; j < dst.Length; j++)
		{
			dst[j].ThrowIfDisposed();
			array2[j] = dst[j].CvPtr;
		}
		NativeMethods.core_mixChannels(array, (uint)src.Length, array2, (uint)dst.Length, fromTo, (uint)(fromTo.Length / 2));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
	}

	public static void ExtractChannel(InputArray src, OutputArray dst, int coi)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_extractChannel(src.CvPtr, dst.CvPtr, coi);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void InsertChannel(InputArray src, InputOutputArray dst, int coi)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_insertChannel(src.CvPtr, dst.CvPtr, coi);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Flip(InputArray src, OutputArray dst, FlipMode flipCode)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_flip(src.CvPtr, dst.CvPtr, (int)flipCode);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Repeat(InputArray src, int ny, int nx, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_repeat1(src.CvPtr, ny, nx, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static Mat Repeat(Mat src, int ny, int nx)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.core_repeat2(src.CvPtr, ny, nx);
		GC.KeepAlive(src);
		return new Mat(ptr);
	}

	public static void HConcat(Mat[] src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (src.Length == 0)
		{
			throw new ArgumentException("src.Length == 0");
		}
		IntPtr[] array = new IntPtr[src.Length];
		for (int i = 0; i < src.Length; i++)
		{
			src[i].ThrowIfDisposed();
			array[i] = src[i].CvPtr;
		}
		NativeMethods.core_hconcat1(array, (uint)src.Length, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void HConcat(InputArray src1, InputArray src2, OutputArray dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_hconcat2(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void VConcat(Mat[] src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (src.Length == 0)
		{
			throw new ArgumentException("src.Length == 0");
		}
		IntPtr[] array = new IntPtr[src.Length];
		for (int i = 0; i < src.Length; i++)
		{
			src[i].ThrowIfDisposed();
			array[i] = src[i].CvPtr;
		}
		NativeMethods.core_vconcat1(array, (uint)src.Length, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void VConcat(InputArray src1, InputArray src2, OutputArray dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_vconcat2(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void BitwiseAnd(InputArray src1, InputArray src2, OutputArray dst, InputArray mask = null)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_bitwise_and(src1.CvPtr, src2.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
		GC.KeepAlive(mask);
	}

	public static void BitwiseOr(InputArray src1, InputArray src2, OutputArray dst, InputArray mask = null)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_bitwise_or(src1.CvPtr, src2.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void BitwiseXor(InputArray src1, InputArray src2, OutputArray dst, InputArray mask = null)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_bitwise_xor(src1.CvPtr, src2.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void BitwiseNot(InputArray src, OutputArray dst, InputArray mask = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_bitwise_not(src.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void Absdiff(InputArray src1, InputArray src2, OutputArray dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_absdiff(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void InRange(InputArray src, InputArray lowerb, InputArray upperb, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (lowerb == null)
		{
			throw new ArgumentNullException("lowerb");
		}
		if (upperb == null)
		{
			throw new ArgumentNullException("upperb");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		lowerb.ThrowIfDisposed();
		upperb.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_inRange_InputArray(src.CvPtr, lowerb.CvPtr, upperb.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(lowerb);
		GC.KeepAlive(upperb);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void InRange(InputArray src, Scalar lowerb, Scalar upperb, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_inRange_Scalar(src.CvPtr, lowerb, upperb, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Compare(InputArray src1, InputArray src2, OutputArray dst, CmpTypes cmpop)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_compare(src1.CvPtr, src2.CvPtr, dst.CvPtr, (int)cmpop);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Min(InputArray src1, InputArray src2, OutputArray dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_min1(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Min(Mat src1, Mat src2, Mat dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		NativeMethods.core_min_MatMat(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
	}

	public static void Min(Mat src1, double src2, Mat dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		NativeMethods.core_min_MatDouble(src1.CvPtr, src2, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(dst);
	}

	public static void Max(InputArray src1, InputArray src2, OutputArray dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_max1(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Max(Mat src1, Mat src2, Mat dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		NativeMethods.core_max_MatMat(src1.CvPtr, src2.CvPtr, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
	}

	public static void Max(Mat src1, double src2, Mat dst)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		NativeMethods.core_max_MatDouble(src1.CvPtr, src2, dst.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(dst);
	}

	public static void Sqrt(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_sqrt(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Pow(InputArray src, double power, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_pow_Mat(src.CvPtr, power, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Exp(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_exp_Mat(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Log(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_log_Mat(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static float CubeRoot(float val)
	{
		return NativeMethods.core_cubeRoot(val);
	}

	public static float FastAtan2(float y, float x)
	{
		return NativeMethods.core_fastAtan2(y, x);
	}

	public static void PolarToCart(InputArray magnitude, InputArray angle, OutputArray x, OutputArray y, bool angleInDegrees = false)
	{
		if (magnitude == null)
		{
			throw new ArgumentNullException("magnitude");
		}
		if (angle == null)
		{
			throw new ArgumentNullException("angle");
		}
		if (x == null)
		{
			throw new ArgumentNullException("x");
		}
		if (y == null)
		{
			throw new ArgumentNullException("y");
		}
		magnitude.ThrowIfDisposed();
		angle.ThrowIfDisposed();
		x.ThrowIfNotReady();
		y.ThrowIfNotReady();
		NativeMethods.core_polarToCart(magnitude.CvPtr, angle.CvPtr, x.CvPtr, y.CvPtr, angleInDegrees ? 1 : 0);
		GC.KeepAlive(magnitude);
		GC.KeepAlive(angle);
		GC.KeepAlive(x);
		GC.KeepAlive(y);
		x.Fix();
		y.Fix();
	}

	public static void CartToPolar(InputArray x, InputArray y, OutputArray magnitude, OutputArray angle, bool angleInDegrees = false)
	{
		if (x == null)
		{
			throw new ArgumentNullException("x");
		}
		if (y == null)
		{
			throw new ArgumentNullException("y");
		}
		if (magnitude == null)
		{
			throw new ArgumentNullException("magnitude");
		}
		if (angle == null)
		{
			throw new ArgumentNullException("angle");
		}
		x.ThrowIfDisposed();
		y.ThrowIfDisposed();
		magnitude.ThrowIfNotReady();
		angle.ThrowIfNotReady();
		NativeMethods.core_cartToPolar(x.CvPtr, y.CvPtr, magnitude.CvPtr, angle.CvPtr, angleInDegrees ? 1 : 0);
		GC.KeepAlive(x);
		GC.KeepAlive(y);
		GC.KeepAlive(magnitude);
		GC.KeepAlive(angle);
		magnitude.Fix();
		angle.Fix();
	}

	public static void Phase(InputArray x, InputArray y, OutputArray angle, bool angleInDegrees = false)
	{
		if (x == null)
		{
			throw new ArgumentNullException("x");
		}
		if (y == null)
		{
			throw new ArgumentNullException("y");
		}
		if (angle == null)
		{
			throw new ArgumentNullException("angle");
		}
		x.ThrowIfDisposed();
		y.ThrowIfDisposed();
		angle.ThrowIfNotReady();
		NativeMethods.core_phase(x.CvPtr, y.CvPtr, angle.CvPtr, angleInDegrees ? 1 : 0);
		GC.KeepAlive(x);
		GC.KeepAlive(y);
		GC.KeepAlive(angle);
		angle.Fix();
	}

	public static void Magnitude(InputArray x, InputArray y, OutputArray magnitude)
	{
		if (x == null)
		{
			throw new ArgumentNullException("x");
		}
		if (y == null)
		{
			throw new ArgumentNullException("y");
		}
		if (magnitude == null)
		{
			throw new ArgumentNullException("magnitude");
		}
		x.ThrowIfDisposed();
		y.ThrowIfDisposed();
		magnitude.ThrowIfNotReady();
		NativeMethods.core_magnitude_Mat(x.CvPtr, y.CvPtr, magnitude.CvPtr);
		GC.KeepAlive(x);
		GC.KeepAlive(y);
		GC.KeepAlive(magnitude);
		magnitude.Fix();
	}

	public static bool CheckRange(InputArray src, bool quiet = true)
	{
		Point pos;
		return CheckRange(src, quiet, out pos);
	}

	public static bool CheckRange(InputArray src, bool quiet, out Point pos, double minVal = double.MinValue, double maxVal = double.MaxValue)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		int num = NativeMethods.core_checkRange(src.CvPtr, quiet ? 1 : 0, out pos, minVal, maxVal);
		GC.KeepAlive(src);
		return num != 0;
	}

	public static void PatchNaNs(InputOutputArray a, double val = 0.0)
	{
		if (a == null)
		{
			throw new ArgumentNullException("a");
		}
		a.ThrowIfNotReady();
		NativeMethods.core_patchNaNs(a.CvPtr, val);
		GC.KeepAlive(a);
	}

	public static void Gemm(InputArray src1, InputArray src2, double alpha, InputArray src3, double gamma, OutputArray dst, GemmFlags flags = GemmFlags.None)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (src3 == null)
		{
			throw new ArgumentNullException("src3");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		src3.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_gemm(src1.CvPtr, src2.CvPtr, alpha, src3.CvPtr, gamma, dst.CvPtr, (int)flags);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(src3);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void MulTransposed(InputArray src, OutputArray dst, bool aTa, InputArray delta = null, double scale = 1.0, int dtype = -1)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_mulTransposed(src.CvPtr, dst.CvPtr, aTa ? 1 : 0, ToPtr(delta), scale, dtype);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(delta);
		dst.Fix();
	}

	public static void Transpose(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_transpose(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Transform(InputArray src, OutputArray dst, InputArray m)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		m.ThrowIfDisposed();
		NativeMethods.core_transform(src.CvPtr, dst.CvPtr, m.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(m);
		dst.Fix();
	}

	public static void PerspectiveTransform(InputArray src, OutputArray dst, InputArray m)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		m.ThrowIfDisposed();
		NativeMethods.core_perspectiveTransform(src.CvPtr, dst.CvPtr, m.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(m);
		dst.Fix();
	}

	public static Point2f[] PerspectiveTransform(IEnumerable<Point2f> src, Mat m)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		using MatOfPoint2f matOfPoint2f = MatOfPoint2f.FromArray(src);
		using MatOfPoint2f matOfPoint2f2 = new MatOfPoint2f();
		NativeMethods.core_perspectiveTransform_Mat(matOfPoint2f.CvPtr, matOfPoint2f2.CvPtr, m.CvPtr);
		GC.KeepAlive(m);
		return matOfPoint2f2.ToArray();
	}

	public static Point2d[] PerspectiveTransform(IEnumerable<Point2d> src, Mat m)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		using MatOfPoint2d matOfPoint2d = MatOfPoint2d.FromArray(src);
		using MatOfPoint2d matOfPoint2d2 = new MatOfPoint2d();
		NativeMethods.core_perspectiveTransform_Mat(matOfPoint2d.CvPtr, matOfPoint2d2.CvPtr, m.CvPtr);
		GC.KeepAlive(m);
		return matOfPoint2d2.ToArray();
	}

	public static Point3f[] PerspectiveTransform(IEnumerable<Point3f> src, Mat m)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		using MatOfPoint3f matOfPoint3f = MatOfPoint3f.FromArray(src);
		using MatOfPoint3f matOfPoint3f2 = new MatOfPoint3f();
		NativeMethods.core_perspectiveTransform_Mat(matOfPoint3f.CvPtr, matOfPoint3f2.CvPtr, m.CvPtr);
		GC.KeepAlive(m);
		return matOfPoint3f2.ToArray();
	}

	public static Point3d[] PerspectiveTransform(IEnumerable<Point3d> src, Mat m)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		using MatOfPoint3d matOfPoint3d = MatOfPoint3d.FromArray(src);
		using MatOfPoint3d matOfPoint3d2 = new MatOfPoint3d();
		NativeMethods.core_perspectiveTransform_Mat(matOfPoint3d.CvPtr, matOfPoint3d2.CvPtr, m.CvPtr);
		GC.KeepAlive(m);
		return matOfPoint3d2.ToArray();
	}

	public static void CompleteSymm(InputOutputArray mtx, bool lowerToUpper = false)
	{
		if (mtx == null)
		{
			throw new ArgumentNullException("mtx");
		}
		mtx.ThrowIfNotReady();
		NativeMethods.core_completeSymm(mtx.CvPtr, lowerToUpper ? 1 : 0);
		GC.KeepAlive(mtx);
		mtx.Fix();
	}

	public static void SetIdentity(InputOutputArray mtx, Scalar? s = null)
	{
		if (mtx == null)
		{
			throw new ArgumentNullException("mtx");
		}
		mtx.ThrowIfNotReady();
		Scalar valueOrDefault = s.GetValueOrDefault(new Scalar(1.0));
		NativeMethods.core_setIdentity(mtx.CvPtr, valueOrDefault);
		GC.KeepAlive(mtx);
		mtx.Fix();
	}

	public static double Determinant(InputArray mtx)
	{
		if (mtx == null)
		{
			throw new ArgumentNullException("mtx");
		}
		mtx.ThrowIfDisposed();
		double result = NativeMethods.core_determinant(mtx.CvPtr);
		GC.KeepAlive(mtx);
		return result;
	}

	public static Scalar Trace(InputArray mtx)
	{
		if (mtx == null)
		{
			throw new ArgumentNullException("mtx");
		}
		mtx.ThrowIfDisposed();
		Scalar result = NativeMethods.core_trace(mtx.CvPtr);
		GC.KeepAlive(mtx);
		return result;
	}

	public static double Invert(InputArray src, OutputArray dst, DecompTypes flags = DecompTypes.LU)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		double result = NativeMethods.core_invert(src.CvPtr, dst.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
		return result;
	}

	public static bool Solve(InputArray src1, InputArray src2, OutputArray dst, DecompTypes flags = DecompTypes.LU)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		int num = NativeMethods.core_solve(src1.CvPtr, src2.CvPtr, dst.CvPtr, (int)flags);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		dst.Fix();
		return num != 0;
	}

	public static SolveLPResult SolveLP(Mat func, Mat constr, Mat z)
	{
		if (func == null)
		{
			throw new ArgumentNullException("func");
		}
		if (constr == null)
		{
			throw new ArgumentNullException("constr");
		}
		if (z == null)
		{
			throw new ArgumentNullException("z");
		}
		int result = NativeMethods.core_solveLP(func.CvPtr, constr.CvPtr, z.CvPtr);
		GC.KeepAlive(func);
		GC.KeepAlive(constr);
		GC.KeepAlive(z);
		return (SolveLPResult)result;
	}

	public static void Sort(InputArray src, OutputArray dst, SortFlags flags)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_sort(src.CvPtr, dst.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void SortIdx(InputArray src, OutputArray dst, SortFlags flags)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_sortIdx(src.CvPtr, dst.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static int SolveCubic(InputArray coeffs, OutputArray roots)
	{
		if (coeffs == null)
		{
			throw new ArgumentNullException("coeffs");
		}
		if (roots == null)
		{
			throw new ArgumentNullException("roots");
		}
		coeffs.ThrowIfDisposed();
		roots.ThrowIfNotReady();
		int result = NativeMethods.core_solveCubic(coeffs.CvPtr, roots.CvPtr);
		GC.KeepAlive(coeffs);
		GC.KeepAlive(roots);
		roots.Fix();
		return result;
	}

	public static double SolvePoly(InputArray coeffs, OutputArray roots, int maxIters = 300)
	{
		if (coeffs == null)
		{
			throw new ArgumentNullException("coeffs");
		}
		if (roots == null)
		{
			throw new ArgumentNullException("roots");
		}
		coeffs.ThrowIfDisposed();
		roots.ThrowIfNotReady();
		double result = NativeMethods.core_solvePoly(coeffs.CvPtr, roots.CvPtr, maxIters);
		GC.KeepAlive(coeffs);
		GC.KeepAlive(roots);
		roots.Fix();
		return result;
	}

	public static bool Eigen(InputArray src, OutputArray eigenvalues, OutputArray eigenvectors)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (eigenvalues == null)
		{
			throw new ArgumentNullException("eigenvalues");
		}
		if (eigenvectors == null)
		{
			throw new ArgumentNullException("eigenvectors");
		}
		src.ThrowIfDisposed();
		eigenvalues.ThrowIfNotReady();
		eigenvectors.ThrowIfNotReady();
		int num = NativeMethods.core_eigen(src.CvPtr, eigenvalues.CvPtr, eigenvectors.CvPtr);
		eigenvalues.Fix();
		eigenvectors.Fix();
		GC.KeepAlive(src);
		GC.KeepAlive(eigenvalues);
		GC.KeepAlive(eigenvectors);
		return num != 0;
	}

	public static void CalcCovarMatrix(Mat[] samples, Mat covar, Mat mean, CovarFlags flags)
	{
		CalcCovarMatrix(samples, covar, mean, flags, 6);
	}

	public static void CalcCovarMatrix(Mat[] samples, Mat covar, Mat mean, CovarFlags flags, MatType ctype)
	{
		if (samples == null)
		{
			throw new ArgumentNullException("samples");
		}
		if (covar == null)
		{
			throw new ArgumentNullException("covar");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		covar.ThrowIfDisposed();
		mean.ThrowIfDisposed();
		NativeMethods.core_calcCovarMatrix_Mat(EnumerableEx.SelectPtrs(samples), samples.Length, covar.CvPtr, mean.CvPtr, (int)flags, ctype);
		GC.KeepAlive(samples);
		GC.KeepAlive(covar);
		GC.KeepAlive(mean);
	}

	public static void CalcCovarMatrix(InputArray samples, OutputArray covar, InputOutputArray mean, CovarFlags flags)
	{
		CalcCovarMatrix(samples, covar, mean, flags, 6);
	}

	public static void CalcCovarMatrix(InputArray samples, OutputArray covar, InputOutputArray mean, CovarFlags flags, MatType ctype)
	{
		if (samples == null)
		{
			throw new ArgumentNullException("samples");
		}
		if (covar == null)
		{
			throw new ArgumentNullException("covar");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		samples.ThrowIfDisposed();
		covar.ThrowIfNotReady();
		mean.ThrowIfNotReady();
		NativeMethods.core_calcCovarMatrix_InputArray(samples.CvPtr, covar.CvPtr, mean.CvPtr, (int)flags, ctype);
		GC.KeepAlive(samples);
		GC.KeepAlive(covar);
		GC.KeepAlive(mean);
		covar.Fix();
		mean.Fix();
	}

	public static void PCACompute(InputArray data, InputOutputArray mean, OutputArray eigenvectors, int maxComponents = 0)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		if (eigenvectors == null)
		{
			throw new ArgumentNullException("eigenvectors");
		}
		data.ThrowIfDisposed();
		mean.ThrowIfNotReady();
		eigenvectors.ThrowIfNotReady();
		NativeMethods.core_PCACompute(data.CvPtr, mean.CvPtr, eigenvectors.CvPtr, maxComponents);
		GC.KeepAlive(data);
		GC.KeepAlive(mean);
		GC.KeepAlive(eigenvectors);
		mean.Fix();
		eigenvectors.Fix();
	}

	public static void PCAComputeVar(InputArray data, InputOutputArray mean, OutputArray eigenvectors, double retainedVariance)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		if (eigenvectors == null)
		{
			throw new ArgumentNullException("eigenvectors");
		}
		data.ThrowIfDisposed();
		mean.ThrowIfNotReady();
		eigenvectors.ThrowIfNotReady();
		NativeMethods.core_PCAComputeVar(data.CvPtr, mean.CvPtr, eigenvectors.CvPtr, retainedVariance);
		GC.KeepAlive(data);
		GC.KeepAlive(mean);
		GC.KeepAlive(eigenvectors);
		mean.Fix();
		eigenvectors.Fix();
	}

	public static void PCAProject(InputArray data, InputArray mean, InputArray eigenvectors, OutputArray result)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		if (eigenvectors == null)
		{
			throw new ArgumentNullException("eigenvectors");
		}
		if (result == null)
		{
			throw new ArgumentNullException("result");
		}
		data.ThrowIfDisposed();
		mean.ThrowIfDisposed();
		eigenvectors.ThrowIfDisposed();
		result.ThrowIfNotReady();
		NativeMethods.core_PCAProject(data.CvPtr, mean.CvPtr, eigenvectors.CvPtr, result.CvPtr);
		GC.KeepAlive(data);
		GC.KeepAlive(mean);
		GC.KeepAlive(eigenvectors);
		GC.KeepAlive(result);
		result.Fix();
	}

	public static void PCABackProject(InputArray data, InputArray mean, InputArray eigenvectors, OutputArray result)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		if (eigenvectors == null)
		{
			throw new ArgumentNullException("eigenvectors");
		}
		if (result == null)
		{
			throw new ArgumentNullException("result");
		}
		data.ThrowIfDisposed();
		mean.ThrowIfDisposed();
		eigenvectors.ThrowIfDisposed();
		result.ThrowIfNotReady();
		NativeMethods.core_PCABackProject(data.CvPtr, mean.CvPtr, eigenvectors.CvPtr, result.CvPtr);
		GC.KeepAlive(data);
		GC.KeepAlive(mean);
		GC.KeepAlive(eigenvectors);
		GC.KeepAlive(result);
		result.Fix();
	}

	public static void SVDecomp(InputArray src, OutputArray w, OutputArray u, OutputArray vt, SVD.Flags flags = SVD.Flags.None)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (w == null)
		{
			throw new ArgumentNullException("w");
		}
		if (u == null)
		{
			throw new ArgumentNullException("u");
		}
		if (vt == null)
		{
			throw new ArgumentNullException("vt");
		}
		src.ThrowIfDisposed();
		w.ThrowIfNotReady();
		u.ThrowIfNotReady();
		vt.ThrowIfNotReady();
		NativeMethods.core_SVDecomp(src.CvPtr, w.CvPtr, u.CvPtr, vt.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(w);
		GC.KeepAlive(u);
		GC.KeepAlive(vt);
		w.Fix();
		u.Fix();
		vt.Fix();
	}

	public static void SVBackSubst(InputArray w, InputArray u, InputArray vt, InputArray rhs, OutputArray dst)
	{
		if (w == null)
		{
			throw new ArgumentNullException("w");
		}
		if (u == null)
		{
			throw new ArgumentNullException("u");
		}
		if (vt == null)
		{
			throw new ArgumentNullException("vt");
		}
		if (rhs == null)
		{
			throw new ArgumentNullException("rhs");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		w.ThrowIfDisposed();
		u.ThrowIfDisposed();
		vt.ThrowIfDisposed();
		rhs.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_SVBackSubst(w.CvPtr, u.CvPtr, vt.CvPtr, rhs.CvPtr, dst.CvPtr);
		GC.KeepAlive(w);
		GC.KeepAlive(u);
		GC.KeepAlive(vt);
		GC.KeepAlive(rhs);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static double Mahalanobis(InputArray v1, InputArray v2, InputArray icovar)
	{
		if (v1 == null)
		{
			throw new ArgumentNullException("v1");
		}
		if (v2 == null)
		{
			throw new ArgumentNullException("v2");
		}
		if (icovar == null)
		{
			throw new ArgumentNullException("icovar");
		}
		v1.ThrowIfDisposed();
		v2.ThrowIfDisposed();
		icovar.ThrowIfDisposed();
		double result = NativeMethods.core_Mahalanobis(v1.CvPtr, v2.CvPtr, icovar.CvPtr);
		GC.KeepAlive(v1);
		GC.KeepAlive(v2);
		GC.KeepAlive(icovar);
		return result;
	}

	public static double Mahalonobis(InputArray v1, InputArray v2, InputArray icovar)
	{
		return Mahalanobis(v1, v2, icovar);
	}

	public static void Dft(InputArray src, OutputArray dst, DftFlags flags = DftFlags.None, int nonzeroRows = 0)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_dft(src.CvPtr, dst.CvPtr, (int)flags, nonzeroRows);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Idft(InputArray src, OutputArray dst, DftFlags flags = DftFlags.None, int nonzeroRows = 0)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_idft(src.CvPtr, dst.CvPtr, (int)flags, nonzeroRows);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Dct(InputArray src, OutputArray dst, DctFlags flags = DctFlags.None)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_dct(src.CvPtr, dst.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Idct(InputArray src, OutputArray dst, DctFlags flags = DctFlags.None)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.core_idct(src.CvPtr, dst.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void MulSpectrums(InputArray a, InputArray b, OutputArray c, DftFlags flags, bool conjB = false)
	{
		if (a == null)
		{
			throw new ArgumentNullException("a");
		}
		if (b == null)
		{
			throw new ArgumentNullException("b");
		}
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		a.ThrowIfDisposed();
		b.ThrowIfDisposed();
		c.ThrowIfNotReady();
		NativeMethods.core_mulSpectrums(a.CvPtr, b.CvPtr, c.CvPtr, (int)flags, conjB ? 1 : 0);
		GC.KeepAlive(a);
		GC.KeepAlive(b);
		GC.KeepAlive(c);
		c.Fix();
	}

	public static int GetOptimalDFTSize(int vecsize)
	{
		return NativeMethods.core_getOptimalDFTSize(vecsize);
	}

	public static double Kmeans(InputArray data, int k, InputOutputArray bestLabels, TermCriteria criteria, int attempts, KMeansFlags flags, OutputArray centers = null)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (bestLabels == null)
		{
			throw new ArgumentNullException("bestLabels");
		}
		data.ThrowIfDisposed();
		bestLabels.ThrowIfDisposed();
		double result = NativeMethods.core_kmeans(data.CvPtr, k, bestLabels.CvPtr, criteria, attempts, (int)flags, ToPtr(centers));
		bestLabels.Fix();
		centers?.Fix();
		GC.KeepAlive(data);
		GC.KeepAlive(bestLabels);
		GC.KeepAlive(centers);
		return result;
	}

	public static RNG TheRNG()
	{
		return new RNG(NativeMethods.core_theRNG());
	}

	public static void Randu(InputOutputArray dst, InputArray low, InputArray high)
	{
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (low == null)
		{
			throw new ArgumentNullException("low");
		}
		if (high == null)
		{
			throw new ArgumentNullException("high");
		}
		dst.ThrowIfNotReady();
		low.ThrowIfDisposed();
		high.ThrowIfDisposed();
		NativeMethods.core_randu_InputArray(dst.CvPtr, low.CvPtr, high.CvPtr);
		GC.KeepAlive(dst);
		GC.KeepAlive(low);
		GC.KeepAlive(high);
		dst.Fix();
	}

	public static void Randu(InputOutputArray dst, Scalar low, Scalar high)
	{
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		dst.ThrowIfNotReady();
		NativeMethods.core_randu_Scalar(dst.CvPtr, low, high);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Randn(InputOutputArray dst, InputArray mean, InputArray stddev)
	{
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (mean == null)
		{
			throw new ArgumentNullException("mean");
		}
		if (stddev == null)
		{
			throw new ArgumentNullException("stddev");
		}
		dst.ThrowIfNotReady();
		mean.ThrowIfDisposed();
		stddev.ThrowIfDisposed();
		NativeMethods.core_randn_InputArray(dst.CvPtr, mean.CvPtr, stddev.CvPtr);
		GC.KeepAlive(dst);
		GC.KeepAlive(mean);
		GC.KeepAlive(stddev);
		dst.Fix();
	}

	public static void Randn(InputOutputArray dst, Scalar mean, Scalar stddev)
	{
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		dst.ThrowIfNotReady();
		NativeMethods.core_randn_Scalar(dst.CvPtr, mean, stddev);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void RandShuffle(InputOutputArray dst, double iterFactor, RNG rng = null)
	{
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		dst.ThrowIfNotReady();
		if (rng == null)
		{
			NativeMethods.core_randShuffle(dst.CvPtr, iterFactor, IntPtr.Zero);
		}
		else
		{
			ulong rng2 = rng.State;
			NativeMethods.core_randShuffle(dst.CvPtr, iterFactor, ref rng2);
			rng.State = rng2;
		}
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Write(FileStorage fs, string name, int value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, float value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, double value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, string value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, Mat value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, SparseMat value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, IEnumerable<KeyPoint> value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void Write(FileStorage fs, string name, IEnumerable<DMatch> value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.Write(name, value);
	}

	public static void WriteScalar(FileStorage fs, int value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.WriteScalar(value);
	}

	public static void WriteScalar(FileStorage fs, float value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.WriteScalar(value);
	}

	public static void WriteScalar(FileStorage fs, double value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.WriteScalar(value);
	}

	public static void WriteScalar(FileStorage fs, string value)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.WriteScalar(value);
	}

	public static int ReadInt(FileNode node, int defaultValue = 0)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadInt(defaultValue);
	}

	public static float ReadFloat(FileNode node, float defaultValue = 0f)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadFloat(defaultValue);
	}

	public static double ReadDouble(FileNode node, double defaultValue = 0.0)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadDouble(defaultValue);
	}

	public static string ReadString(FileNode node, string defaultValue = null)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadString(defaultValue);
	}

	public static Mat ReadMat(FileNode node, Mat defaultMat = null)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadMat(defaultMat);
	}

	public static SparseMat ReadSparseMat(FileNode node, SparseMat defaultMat = null)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadSparseMat(defaultMat);
	}

	public static KeyPoint[] ReadKeyPoints(FileNode node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadKeyPoints();
	}

	public static DMatch[] ReadDMatches(FileNode node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ReadDMatches();
	}

	public static int Partition<T>(IEnumerable<T> vec, out int[] labels, PartitionPredicate<T> predicate)
	{
		T[] array = EnumerableEx.ToArray(vec);
		labels = new int[array.Length];
		List<T> list = new List<T>();
		int num = 0;
		T[] array2 = array;
		foreach (T val in array2)
		{
			bool flag = false;
			int num2 = 0;
			foreach (T item in list)
			{
				if (predicate(item, val))
				{
					labels[num] = num2;
					flag = true;
					break;
				}
				num2++;
			}
			if (!flag)
			{
				labels[num] = list.Count;
				list.Add(val);
			}
			num++;
		}
		return list.Count;
	}

	public static KeyPoint[] FAST(InputArray image, int threshold, bool nonmaxSupression = true)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfKeyPoint vectorOfKeyPoint = new VectorOfKeyPoint();
		NativeMethods.features2d_FAST1(image.CvPtr, vectorOfKeyPoint.CvPtr, threshold, nonmaxSupression ? 1 : 0);
		GC.KeepAlive(image);
		return vectorOfKeyPoint.ToArray();
	}

	public static KeyPoint[] FAST(InputArray image, int threshold, bool nonmaxSupression, FASTType type)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfKeyPoint vectorOfKeyPoint = new VectorOfKeyPoint();
		NativeMethods.features2d_FAST2(image.CvPtr, vectorOfKeyPoint.CvPtr, threshold, nonmaxSupression ? 1 : 0, (int)type);
		GC.KeepAlive(image);
		return vectorOfKeyPoint.ToArray();
	}

	public static KeyPoint[] AGAST(InputArray image, int threshold, bool nonmaxSuppression, AgastFeatureDetector.DetectorType type)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfKeyPoint vectorOfKeyPoint = new VectorOfKeyPoint();
		NativeMethods.features2d_AGAST(image.CvPtr, vectorOfKeyPoint.CvPtr, threshold, nonmaxSuppression ? 1 : 0, (int)type);
		GC.KeepAlive(image);
		return vectorOfKeyPoint.ToArray();
	}

	public static void DrawKeypoints(Mat image, IEnumerable<KeyPoint> keypoints, Mat outImage, Scalar? color = null, DrawMatchesFlags flags = DrawMatchesFlags.Default)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (outImage == null)
		{
			throw new ArgumentNullException("outImage");
		}
		if (keypoints == null)
		{
			throw new ArgumentNullException("keypoints");
		}
		image.ThrowIfDisposed();
		outImage.ThrowIfDisposed();
		KeyPoint[] array = EnumerableEx.ToArray(keypoints);
		Scalar valueOrDefault = color.GetValueOrDefault(Scalar.All(-1.0));
		NativeMethods.features2d_drawKeypoints(image.CvPtr, array, array.Length, outImage.CvPtr, valueOrDefault, (int)flags);
		GC.KeepAlive(image);
		GC.KeepAlive(outImage);
	}

	public static void DrawMatches(Mat img1, IEnumerable<KeyPoint> keypoints1, Mat img2, IEnumerable<KeyPoint> keypoints2, IEnumerable<DMatch> matches1To2, Mat outImg, Scalar? matchColor = null, Scalar? singlePointColor = null, IEnumerable<byte> matchesMask = null, DrawMatchesFlags flags = DrawMatchesFlags.Default)
	{
		if (img1 == null)
		{
			throw new ArgumentNullException("img1");
		}
		if (img2 == null)
		{
			throw new ArgumentNullException("img2");
		}
		if (outImg == null)
		{
			throw new ArgumentNullException("outImg");
		}
		if (keypoints1 == null)
		{
			throw new ArgumentNullException("keypoints1");
		}
		if (keypoints2 == null)
		{
			throw new ArgumentNullException("keypoints2");
		}
		if (matches1To2 == null)
		{
			throw new ArgumentNullException("matches1To2");
		}
		img1.ThrowIfDisposed();
		img2.ThrowIfDisposed();
		outImg.ThrowIfDisposed();
		KeyPoint[] array = EnumerableEx.ToArray(keypoints1);
		KeyPoint[] array2 = EnumerableEx.ToArray(keypoints2);
		DMatch[] array3 = EnumerableEx.ToArray(matches1To2);
		Scalar valueOrDefault = matchColor.GetValueOrDefault(Scalar.All(-1.0));
		Scalar valueOrDefault2 = singlePointColor.GetValueOrDefault(Scalar.All(-1.0));
		byte[] array4 = null;
		int matchesMaskLength = 0;
		if (matchesMask != null)
		{
			array4 = EnumerableEx.ToArray(matchesMask);
			matchesMaskLength = array4.Length;
		}
		NativeMethods.features2d_drawMatches1(img1.CvPtr, array, array.Length, img2.CvPtr, array2, array2.Length, array3, array3.Length, outImg.CvPtr, valueOrDefault, valueOrDefault2, array4, matchesMaskLength, (int)flags);
		GC.KeepAlive(img1);
		GC.KeepAlive(img2);
		GC.KeepAlive(outImg);
	}

	public static void DrawMatches(Mat img1, IEnumerable<KeyPoint> keypoints1, Mat img2, IEnumerable<KeyPoint> keypoints2, IEnumerable<IEnumerable<DMatch>> matches1To2, Mat outImg, Scalar? matchColor = null, Scalar? singlePointColor = null, IEnumerable<IEnumerable<byte>> matchesMask = null, DrawMatchesFlags flags = DrawMatchesFlags.Default)
	{
		if (img1 == null)
		{
			throw new ArgumentNullException("img1");
		}
		if (img2 == null)
		{
			throw new ArgumentNullException("img2");
		}
		if (outImg == null)
		{
			throw new ArgumentNullException("outImg");
		}
		if (keypoints1 == null)
		{
			throw new ArgumentNullException("keypoints1");
		}
		if (keypoints2 == null)
		{
			throw new ArgumentNullException("keypoints2");
		}
		if (matches1To2 == null)
		{
			throw new ArgumentNullException("matches1To2");
		}
		img1.ThrowIfDisposed();
		img2.ThrowIfDisposed();
		outImg.ThrowIfDisposed();
		KeyPoint[] array = EnumerableEx.ToArray(keypoints1);
		KeyPoint[] array2 = EnumerableEx.ToArray(keypoints2);
		DMatch[][] array3 = EnumerableEx.SelectToArray(matches1To2, EnumerableEx.ToArray);
		int matches1to2Size = array3.Length;
		int[] matches1to2Size2 = EnumerableEx.SelectToArray(array3, (DMatch[] dm) => dm.Length);
		Scalar valueOrDefault = matchColor.GetValueOrDefault(Scalar.All(-1.0));
		Scalar valueOrDefault2 = singlePointColor.GetValueOrDefault(Scalar.All(-1.0));
		using ArrayAddress2<DMatch> arrayAddress = new ArrayAddress2<DMatch>(array3);
		if (matchesMask == null)
		{
			NativeMethods.features2d_drawMatches2(img1.CvPtr, array, array.Length, img2.CvPtr, array2, array2.Length, arrayAddress, matches1to2Size, matches1to2Size2, outImg.CvPtr, valueOrDefault, valueOrDefault2, null, 0, null, (int)flags);
		}
		else
		{
			byte[][] array4 = EnumerableEx.SelectToArray(matchesMask, EnumerableEx.ToArray);
			int matchesMaskSize = array3.Length;
			int[] matchesMaskSize2 = EnumerableEx.SelectToArray(array4, (byte[] dm) => dm.Length);
			using ArrayAddress2<byte> arrayAddress2 = new ArrayAddress2<byte>(array4);
			NativeMethods.features2d_drawMatches2(img1.CvPtr, array, array.Length, img2.CvPtr, array2, array2.Length, arrayAddress.Pointer, matches1to2Size, matches1to2Size2, outImg.CvPtr, valueOrDefault, valueOrDefault2, arrayAddress2, matchesMaskSize, matchesMaskSize2, (int)flags);
		}
		GC.KeepAlive(img1);
		GC.KeepAlive(img2);
		GC.KeepAlive(outImg);
	}

	public static void EvaluateFeatureDetector(Mat img1, Mat img2, Mat H1to2, ref KeyPoint[] keypoints1, ref KeyPoint[] keypoints2, out float repeatability, out int correspCount)
	{
		if (img1 == null)
		{
			throw new ArgumentNullException("img1");
		}
		if (img2 == null)
		{
			throw new ArgumentNullException("img2");
		}
		if (H1to2 == null)
		{
			throw new ArgumentNullException("H1to2");
		}
		if (keypoints1 == null)
		{
			throw new ArgumentNullException("keypoints1");
		}
		if (keypoints2 == null)
		{
			throw new ArgumentNullException("keypoints2");
		}
		using VectorOfKeyPoint vectorOfKeyPoint = new VectorOfKeyPoint(keypoints1);
		using VectorOfKeyPoint vectorOfKeyPoint2 = new VectorOfKeyPoint(keypoints2);
		NativeMethods.features2d_evaluateFeatureDetector(img1.CvPtr, img2.CvPtr, H1to2.CvPtr, vectorOfKeyPoint.CvPtr, vectorOfKeyPoint2.CvPtr, out repeatability, out correspCount);
		GC.KeepAlive(img1);
		GC.KeepAlive(img2);
		GC.KeepAlive(H1to2);
		keypoints1 = vectorOfKeyPoint.ToArray();
		keypoints2 = vectorOfKeyPoint2.ToArray();
	}

	public static Point2f[] ComputeRecallPrecisionCurve(DMatch[][] matches1to2, byte[][] correctMatches1to2Mask)
	{
		if (matches1to2 == null)
		{
			throw new ArgumentNullException("matches1to2");
		}
		if (correctMatches1to2Mask == null)
		{
			throw new ArgumentNullException("correctMatches1to2Mask");
		}
		using ArrayAddress2<DMatch> arrayAddress = new ArrayAddress2<DMatch>(matches1to2);
		using ArrayAddress2<byte> arrayAddress2 = new ArrayAddress2<byte>(correctMatches1to2Mask);
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		NativeMethods.features2d_computeRecallPrecisionCurve(arrayAddress.Pointer, arrayAddress.Dim1Length, arrayAddress.Dim2Lengths, arrayAddress2.Pointer, arrayAddress2.Dim1Length, arrayAddress2.Dim2Lengths, vectorOfPoint2f.CvPtr);
		return vectorOfPoint2f.ToArray();
	}

	public static float GetRecall(IEnumerable<Point2f> recallPrecisionCurve, float lPrecision)
	{
		if (recallPrecisionCurve == null)
		{
			throw new ArgumentNullException("recallPrecisionCurve");
		}
		Point2f[] array = EnumerableEx.ToArray(recallPrecisionCurve);
		return NativeMethods.features2d_getRecall(array, array.Length, lPrecision);
	}

	public static int GetNearestPoint(IEnumerable<Point2f> recallPrecisionCurve, float lPrecision)
	{
		if (recallPrecisionCurve == null)
		{
			throw new ArgumentNullException("recallPrecisionCurve");
		}
		Point2f[] array = EnumerableEx.ToArray(recallPrecisionCurve);
		return NativeMethods.features2d_getNearestPoint(array, array.Length, lPrecision);
	}

	public static void NamedWindow(string winname)
	{
		NamedWindow(winname, WindowMode.Normal);
	}

	public static void NamedWindow(string winname, WindowMode flags)
	{
		if (string.IsNullOrEmpty(winname))
		{
			throw new ArgumentNullException("winname");
		}
		try
		{
			NativeMethods.highgui_namedWindow(winname, (int)flags);
		}
		catch (BadImageFormatException ex)
		{
			throw PInvokeHelper.CreateException(ex);
		}
	}

	public static void DestroyWindow(string winName)
	{
		if (string.IsNullOrEmpty("winName"))
		{
			throw new ArgumentNullException("winName");
		}
		NativeMethods.highgui_destroyWindow(winName);
	}

	public static void DestroyAllWindows()
	{
		NativeMethods.highgui_destroyAllWindows();
	}

	public static int StartWindowThread()
	{
		return NativeMethods.highgui_startWindowThread();
	}

	public static int WaitKey(int delay = 0)
	{
		try
		{
			return NativeMethods.highgui_waitKey(delay);
		}
		catch (BadImageFormatException ex)
		{
			throw PInvokeHelper.CreateException(ex);
		}
	}

	public static int WaitKeyEx(int delay = 0)
	{
		try
		{
			return NativeMethods.highgui_waitKeyEx(delay);
		}
		catch (BadImageFormatException ex)
		{
			throw PInvokeHelper.CreateException(ex);
		}
	}

	public static void ResizeWindow(string winName, int width, int height)
	{
		if (string.IsNullOrEmpty(winName))
		{
			throw new ArgumentNullException("winName");
		}
		NativeMethods.highgui_resizeWindow(winName, width, height);
	}

	public static void MoveWindow(string winName, int x, int y)
	{
		if (string.IsNullOrEmpty(winName))
		{
			throw new ArgumentNullException("winName");
		}
		NativeMethods.highgui_moveWindow(winName, x, y);
	}

	public static void SetWindowProperty(string winName, WindowProperty propId, double propValue)
	{
		if (string.IsNullOrEmpty(winName))
		{
			throw new ArgumentNullException("winName");
		}
		NativeMethods.highgui_setWindowProperty(winName, (int)propId, propValue);
	}

	public static void SetWindowTitle(string winname, string title)
	{
		if (string.IsNullOrEmpty(winname))
		{
			throw new ArgumentNullException("winname");
		}
		if (string.IsNullOrEmpty(title))
		{
			throw new ArgumentNullException("title");
		}
		NativeMethods.highgui_setWindowTitle(winname, title);
	}

	public static double GetWindowProperty(string winName, WindowProperty propId)
	{
		if (string.IsNullOrEmpty(winName))
		{
			throw new ArgumentNullException("winName");
		}
		return NativeMethods.highgui_getWindowProperty(winName, (int)propId);
	}

	public static void SetMouseCallback(string windowName, CvMouseCallback onMouse, IntPtr userdata = default(IntPtr))
	{
		if (string.IsNullOrEmpty(windowName))
		{
			throw new ArgumentNullException("windowName");
		}
		if (onMouse == null)
		{
			throw new ArgumentNullException("onMouse");
		}
		NativeMethods.highgui_setMouseCallback(windowName, onMouse, userdata);
	}

	public static int GetMouseWheelDelta(MouseEvent flags)
	{
		return NativeMethods.highgui_getMouseWheelDelta((int)flags);
	}

	public static int CreateTrackbar(string trackbarName, string winName, ref int value, int count, CvTrackbarCallback2 onChange = null, IntPtr userdata = default(IntPtr))
	{
		if (trackbarName == null)
		{
			throw new ArgumentNullException("trackbarName");
		}
		if (winName == null)
		{
			throw new ArgumentNullException("winName");
		}
		if (onChange == null)
		{
			return NativeMethods.highgui_createTrackbar(trackbarName, winName, ref value, count, IntPtr.Zero, userdata);
		}
		return NativeMethods.highgui_createTrackbar(trackbarName, winName, ref value, count, onChange, userdata);
	}

	public static int GetTrackbarPos(string trackbarName, string winName)
	{
		if (trackbarName == null)
		{
			throw new ArgumentNullException("trackbarName");
		}
		return NativeMethods.highgui_getTrackbarPos(trackbarName, winName);
	}

	public static void SetTrackbarPos(string trackbarName, string winName, int pos)
	{
		if (trackbarName == null)
		{
			throw new ArgumentNullException("trackbarName");
		}
		NativeMethods.highgui_setTrackbarPos(trackbarName, winName, pos);
	}

	public static void SetTrackbarMax(string trackbarname, string winname, int maxval)
	{
		if (trackbarname == null)
		{
			throw new ArgumentNullException("trackbarname");
		}
		NativeMethods.highgui_setTrackbarMax(trackbarname, winname, maxval);
	}

	public static void SetTrackbarMin(string trackbarname, string winname, int minval)
	{
		if (trackbarname == null)
		{
			throw new ArgumentNullException("trackbarname");
		}
		NativeMethods.highgui_setTrackbarMin(trackbarname, winname, minval);
	}

	public static void ImShow(string winname, Mat mat)
	{
		if (string.IsNullOrEmpty(winname))
		{
			throw new ArgumentNullException("winname");
		}
		if (mat == null)
		{
			throw new ArgumentNullException("mat");
		}
		try
		{
			NativeMethods.highgui_imshow(winname, mat.CvPtr);
			GC.KeepAlive(mat);
		}
		catch (BadImageFormatException ex)
		{
			throw PInvokeHelper.CreateException(ex);
		}
	}

	public static Mat ImRead(string fileName, ImreadModes flags = ImreadModes.Color)
	{
		return new Mat(fileName, flags);
	}

	public static bool ImReadMulti(string filename, out Mat[] mats, ImreadModes flags = ImreadModes.AnyColor)
	{
		if (filename == null)
		{
			throw new ArgumentNullException("filename");
		}
		using VectorOfMat vectorOfMat = new VectorOfMat();
		int num = NativeMethods.imgcodecs_imreadmulti(filename, vectorOfMat.CvPtr, (int)flags);
		mats = vectorOfMat.ToArray();
		return num != 0;
	}

	public static bool ImWrite(string fileName, Mat img, int[] prms = null)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			throw new ArgumentNullException("fileName");
		}
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (prms == null)
		{
			prms = new int[0];
		}
		bool result = NativeMethods.imgcodecs_imwrite(fileName, img.CvPtr, prms, prms.Length) != 0;
		GC.KeepAlive(img);
		return result;
	}

	public static bool ImWrite(string fileName, Mat img, params ImageEncodingParam[] prms)
	{
		if (prms != null && prms.Length != 0)
		{
			List<int> list = new List<int>();
			foreach (ImageEncodingParam imageEncodingParam in prms)
			{
				list.Add((int)imageEncodingParam.EncodingId);
				list.Add(imageEncodingParam.Value);
			}
			return ImWrite(fileName, img, list.ToArray());
		}
		return ImWrite(fileName, img);
	}

	public static bool ImWrite(string fileName, IEnumerable<Mat> img, int[] prms = null)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			throw new ArgumentNullException("fileName");
		}
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (prms == null)
		{
			prms = new int[0];
		}
		using VectorOfMat vectorOfMat = new VectorOfMat(img);
		bool result = NativeMethods.imgcodecs_imwrite_multi(fileName, vectorOfMat.CvPtr, prms, prms.Length) != 0;
		GC.KeepAlive(img);
		return result;
	}

	public static bool ImWrite(string fileName, IEnumerable<Mat> img, params ImageEncodingParam[] prms)
	{
		if (prms != null && prms.Length != 0)
		{
			List<int> list = new List<int>();
			foreach (ImageEncodingParam imageEncodingParam in prms)
			{
				list.Add((int)imageEncodingParam.EncodingId);
				list.Add(imageEncodingParam.Value);
			}
			return ImWrite(fileName, img, list.ToArray());
		}
		return ImWrite(fileName, img);
	}

	public static Mat ImDecode(Mat buf, ImreadModes flags)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		buf.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.imgcodecs_imdecode_Mat(buf.CvPtr, (int)flags);
		GC.KeepAlive(buf);
		return new Mat(ptr);
	}

	public static Mat ImDecode(InputArray buf, ImreadModes flags)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		buf.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.imgcodecs_imdecode_InputArray(buf.CvPtr, (int)flags);
		GC.KeepAlive(buf);
		return new Mat(ptr);
	}

	public static Mat ImDecode(byte[] buf, ImreadModes flags)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		return new Mat(NativeMethods.imgcodecs_imdecode_vector(buf, new IntPtr(buf.Length), (int)flags));
	}

	public static bool ImEncode(string ext, InputArray img, out byte[] buf, int[] prms = null)
	{
		if (string.IsNullOrEmpty(ext))
		{
			throw new ArgumentNullException("ext");
		}
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (prms == null)
		{
			prms = new int[0];
		}
		img.ThrowIfDisposed();
		using VectorOfByte vectorOfByte = new VectorOfByte();
		int num = NativeMethods.imgcodecs_imencode_vector(ext, img.CvPtr, vectorOfByte.CvPtr, prms, prms.Length);
		GC.KeepAlive(img);
		buf = vectorOfByte.ToArray();
		return num != 0;
	}

	public static void ImEncode(string ext, InputArray img, out byte[] buf, params ImageEncodingParam[] prms)
	{
		if (prms != null)
		{
			List<int> list = new List<int>();
			foreach (ImageEncodingParam imageEncodingParam in prms)
			{
				list.Add((int)imageEncodingParam.EncodingId);
				list.Add(imageEncodingParam.Value);
			}
			ImEncode(ext, img, out buf, list.ToArray());
		}
		else
		{
			ImEncode(ext, img, out buf);
		}
	}

	public static bool HaveImageReader(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		return NativeMethods.imgcodecs_haveImageReader(fileName) != 0;
	}

	public static bool HaveImageWriter(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		return NativeMethods.imgcodecs_haveImageWriter(fileName) != 0;
	}

	public static Mat GetGaussianKernel(int ksize, double sigma, MatType? ktype = null)
	{
		MatType valueOrDefault = ktype.GetValueOrDefault(6);
		IntPtr intPtr = NativeMethods.imgproc_getGaussianKernel(ksize, sigma, valueOrDefault);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		return new Mat(intPtr);
	}

	public static void GetDerivKernels(OutputArray kx, OutputArray ky, int dx, int dy, int ksize, bool normalize = false, MatType? ktype = null)
	{
		if (kx == null)
		{
			throw new ArgumentNullException("kx");
		}
		if (ky == null)
		{
			throw new ArgumentNullException("ky");
		}
		kx.ThrowIfNotReady();
		ky.ThrowIfNotReady();
		MatType valueOrDefault = ktype.GetValueOrDefault(5);
		NativeMethods.imgproc_getDerivKernels(kx.CvPtr, ky.CvPtr, dx, dy, ksize, normalize ? 1 : 0, valueOrDefault);
		GC.KeepAlive(kx);
		GC.KeepAlive(ky);
		kx.Fix();
		ky.Fix();
	}

	public static Mat GetGaborKernel(Size ksize, double sigma, double theta, double lambd, double gamma, double psi, int ktype)
	{
		return new Mat(NativeMethods.imgproc_getGaborKernel(ksize, sigma, theta, lambd, gamma, psi, ktype));
	}

	public static Mat GetStructuringElement(MorphShapes shape, Size ksize)
	{
		return GetStructuringElement(shape, ksize, new Point(-1, -1));
	}

	public static Mat GetStructuringElement(MorphShapes shape, Size ksize, Point anchor)
	{
		return new Mat(NativeMethods.imgproc_getStructuringElement((int)shape, ksize, anchor));
	}

	public static void MedianBlur(InputArray src, OutputArray dst, int ksize)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_medianBlur(src.CvPtr, dst.CvPtr, ksize);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void GaussianBlur(InputArray src, OutputArray dst, Size ksize, double sigmaX, double sigmaY = 0.0, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_GaussianBlur(src.CvPtr, dst.CvPtr, ksize, sigmaX, sigmaY, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void BilateralFilter(InputArray src, OutputArray dst, int d, double sigmaColor, double sigmaSpace, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_bilateralFilter(src.CvPtr, dst.CvPtr, d, sigmaColor, sigmaSpace, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void BoxFilter(InputArray src, OutputArray dst, MatType ddepth, Size ksize, Point? anchor = null, bool normalize = true, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		NativeMethods.imgproc_boxFilter(src.CvPtr, dst.CvPtr, ddepth, ksize, valueOrDefault, normalize ? 1 : 0, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Blur(InputArray src, OutputArray dst, Size ksize, Point? anchor = null, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		NativeMethods.imgproc_blur(src.CvPtr, dst.CvPtr, ksize, valueOrDefault, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Filter2D(InputArray src, OutputArray dst, MatType ddepth, InputArray kernel, Point? anchor = null, double delta = 0.0, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (kernel == null)
		{
			throw new ArgumentNullException("kernel");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		kernel.ThrowIfDisposed();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		NativeMethods.imgproc_filter2D(src.CvPtr, dst.CvPtr, ddepth, kernel.CvPtr, valueOrDefault, delta, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(kernel);
		dst.Fix();
	}

	public static void SepFilter2D(InputArray src, OutputArray dst, MatType ddepth, InputArray kernelX, InputArray kernelY, Point? anchor = null, double delta = 0.0, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (kernelX == null)
		{
			throw new ArgumentNullException("kernelX");
		}
		if (kernelY == null)
		{
			throw new ArgumentNullException("kernelY");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		kernelX.ThrowIfDisposed();
		kernelY.ThrowIfDisposed();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		NativeMethods.imgproc_sepFilter2D(src.CvPtr, dst.CvPtr, ddepth, kernelX.CvPtr, kernelY.CvPtr, valueOrDefault, delta, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(kernelX);
		GC.KeepAlive(kernelY);
		dst.Fix();
	}

	public static void Sobel(InputArray src, OutputArray dst, MatType ddepth, int xorder, int yorder, int ksize = 3, double scale = 1.0, double delta = 0.0, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_Sobel(src.CvPtr, dst.CvPtr, ddepth, xorder, yorder, ksize, scale, delta, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Scharr(InputArray src, OutputArray dst, MatType ddepth, int xorder, int yorder, double scale = 1.0, double delta = 0.0, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_Scharr(src.CvPtr, dst.CvPtr, ddepth, xorder, yorder, scale, delta, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Laplacian(InputArray src, OutputArray dst, MatType ddepth, int ksize = 1, double scale = 1.0, double delta = 0.0, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_Laplacian(src.CvPtr, dst.CvPtr, ddepth, ksize, scale, delta, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Canny(InputArray src, OutputArray edges, double threshold1, double threshold2, int apertureSize = 3, bool L2gradient = false)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (edges == null)
		{
			throw new ArgumentNullException("edges");
		}
		src.ThrowIfDisposed();
		edges.ThrowIfNotReady();
		NativeMethods.imgproc_Canny(src.CvPtr, edges.CvPtr, threshold1, threshold2, apertureSize, L2gradient ? 1 : 0);
		GC.KeepAlive(src);
		GC.KeepAlive(edges);
		edges.Fix();
	}

	public static void CornerEigenValsAndVecs(InputArray src, OutputArray dst, int blockSize, int ksize, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_cornerEigenValsAndVecs(src.CvPtr, dst.CvPtr, blockSize, ksize, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void PreCornerDetect(InputArray src, OutputArray dst, int ksize, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_preCornerDetect(src.CvPtr, dst.CvPtr, ksize, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static Point2f[] CornerSubPix(InputArray image, IEnumerable<Point2f> inputCorners, Size winSize, Size zeroZone, TermCriteria criteria)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (inputCorners == null)
		{
			throw new ArgumentNullException("inputCorners");
		}
		image.ThrowIfDisposed();
		Point2f[] array = EnumerableEx.ToArray(inputCorners);
		Point2f[] array2 = new Point2f[array.Length];
		Array.Copy(array, array2, array.Length);
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f(array2);
		NativeMethods.imgproc_cornerSubPix(image.CvPtr, vectorOfPoint2f.CvPtr, winSize, zeroZone, criteria);
		GC.KeepAlive(image);
		return vectorOfPoint2f.ToArray();
	}

	public static Point2f[] GoodFeaturesToTrack(InputArray src, int maxCorners, double qualityLevel, double minDistance, InputArray mask, int blockSize, bool useHarrisDetector, double k)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		src.ThrowIfDisposed();
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		IntPtr mask2 = ToPtr(mask);
		NativeMethods.imgproc_goodFeaturesToTrack(src.CvPtr, vectorOfPoint2f.CvPtr, maxCorners, qualityLevel, minDistance, mask2, blockSize, (!useHarrisDetector) ? 1 : 0, k);
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
		return vectorOfPoint2f.ToArray();
	}

	public static LineSegmentPolar[] HoughLines(InputArray image, double rho, double theta, int threshold, double srn = 0.0, double stn = 0.0)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		using VectorOfVec2f vectorOfVec2f = new VectorOfVec2f();
		NativeMethods.imgproc_HoughLines(image.CvPtr, vectorOfVec2f.CvPtr, rho, theta, threshold, srn, stn);
		GC.KeepAlive(image);
		return vectorOfVec2f.ToArray<LineSegmentPolar>();
	}

	public static LineSegmentPoint[] HoughLinesP(InputArray image, double rho, double theta, int threshold, double minLineLength = 0.0, double maxLineGap = 0.0)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfVec4i vectorOfVec4i = new VectorOfVec4i();
		NativeMethods.imgproc_HoughLinesP(image.CvPtr, vectorOfVec4i.CvPtr, rho, theta, threshold, minLineLength, maxLineGap);
		GC.KeepAlive(image);
		return vectorOfVec4i.ToArray<LineSegmentPoint>();
	}

	public static CircleSegment[] HoughCircles(InputArray image, HoughMethods method, double dp, double minDist, double param1 = 100.0, double param2 = 100.0, int minRadius = 0, int maxRadius = 0)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfDisposed();
		using VectorOfVec3f vectorOfVec3f = new VectorOfVec3f();
		NativeMethods.imgproc_HoughCircles(image.CvPtr, vectorOfVec3f.CvPtr, (int)method, dp, minDist, param1, param2, minRadius, maxRadius);
		GC.KeepAlive(image);
		return vectorOfVec3f.ToArray<CircleSegment>();
	}

	public static Scalar MorphologyDefaultBorderValue()
	{
		return Scalar.All(double.MaxValue);
	}

	public static void Dilate(InputArray src, OutputArray dst, InputArray element, Point? anchor = null, int iterations = 1, BorderTypes borderType = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		Scalar valueOrDefault2 = borderValue.GetValueOrDefault(MorphologyDefaultBorderValue());
		IntPtr kernel = ToPtr(element);
		NativeMethods.imgproc_dilate(src.CvPtr, dst.CvPtr, kernel, valueOrDefault, iterations, (int)borderType, valueOrDefault2);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(element);
		dst.Fix();
	}

	public static void Erode(InputArray src, OutputArray dst, InputArray element, Point? anchor = null, int iterations = 1, BorderTypes borderType = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		Scalar valueOrDefault2 = borderValue.GetValueOrDefault(MorphologyDefaultBorderValue());
		IntPtr kernel = ToPtr(element);
		NativeMethods.imgproc_erode(src.CvPtr, dst.CvPtr, kernel, valueOrDefault, iterations, (int)borderType, valueOrDefault2);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(element);
		dst.Fix();
	}

	public static void MorphologyEx(InputArray src, OutputArray dst, MorphTypes op, InputArray element, Point? anchor = null, int iterations = 1, BorderTypes borderType = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Point valueOrDefault = anchor.GetValueOrDefault(new Point(-1, -1));
		Scalar valueOrDefault2 = borderValue.GetValueOrDefault(MorphologyDefaultBorderValue());
		IntPtr kernel = ToPtr(element);
		NativeMethods.imgproc_morphologyEx(src.CvPtr, dst.CvPtr, (int)op, kernel, valueOrDefault, iterations, (int)borderType, valueOrDefault2);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(element);
		dst.Fix();
	}

	public static void Resize(InputArray src, OutputArray dst, Size dsize, double fx = 0.0, double fy = 0.0, InterpolationFlags interpolation = InterpolationFlags.Linear)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_resize(src.CvPtr, dst.CvPtr, dsize, fx, fy, (int)interpolation);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void WarpAffine(InputArray src, OutputArray dst, InputArray m, Size dsize, InterpolationFlags flags = InterpolationFlags.Linear, BorderTypes borderMode = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		m.ThrowIfDisposed();
		Scalar valueOrDefault = borderValue.GetValueOrDefault(Scalar.All(0.0));
		NativeMethods.imgproc_warpAffine(src.CvPtr, dst.CvPtr, m.CvPtr, dsize, (int)flags, (int)borderMode, valueOrDefault);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(m);
		dst.Fix();
	}

	public static void WarpPerspective(InputArray src, OutputArray dst, InputArray m, Size dsize, InterpolationFlags flags = InterpolationFlags.Linear, BorderTypes borderMode = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		m.ThrowIfDisposed();
		Scalar valueOrDefault = borderValue.GetValueOrDefault(Scalar.All(0.0));
		NativeMethods.imgproc_warpPerspective_MisInputArray(src.CvPtr, dst.CvPtr, m.CvPtr, dsize, (int)flags, (int)borderMode, valueOrDefault);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(m);
		dst.Fix();
	}

	public static void WarpPerspective(InputArray src, OutputArray dst, float[,] m, Size dsize, InterpolationFlags flags = InterpolationFlags.Linear, BorderTypes borderMode = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		Scalar valueOrDefault = borderValue.GetValueOrDefault(Scalar.All(0.0));
		int length = m.GetLength(0);
		int length2 = m.GetLength(1);
		NativeMethods.imgproc_warpPerspective_MisArray(src.CvPtr, dst.CvPtr, m, length, length2, dsize, (int)flags, (int)borderMode, valueOrDefault);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Remap(InputArray src, OutputArray dst, InputArray map1, InputArray map2, InterpolationFlags interpolation = InterpolationFlags.Linear, BorderTypes borderMode = BorderTypes.Constant, Scalar? borderValue = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (map1 == null)
		{
			throw new ArgumentNullException("map1");
		}
		if (map2 == null)
		{
			throw new ArgumentNullException("map2");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		map1.ThrowIfDisposed();
		map2.ThrowIfDisposed();
		Scalar valueOrDefault = borderValue.GetValueOrDefault(Scalar.All(0.0));
		NativeMethods.imgproc_remap(src.CvPtr, dst.CvPtr, map1.CvPtr, map2.CvPtr, (int)interpolation, (int)borderMode, valueOrDefault);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
		GC.KeepAlive(map1);
		GC.KeepAlive(map2);
	}

	public static void ConvertMaps(InputArray map1, InputArray map2, OutputArray dstmap1, OutputArray dstmap2, MatType dstmap1Type, bool nnInterpolation = false)
	{
		if (map1 == null)
		{
			throw new ArgumentNullException("map1");
		}
		if (map2 == null)
		{
			throw new ArgumentNullException("map2");
		}
		if (dstmap1 == null)
		{
			throw new ArgumentNullException("dstmap1");
		}
		if (dstmap2 == null)
		{
			throw new ArgumentNullException("dstmap2");
		}
		map1.ThrowIfDisposed();
		map2.ThrowIfDisposed();
		dstmap1.ThrowIfDisposed();
		dstmap2.ThrowIfDisposed();
		NativeMethods.imgproc_convertMaps(map1.CvPtr, map2.CvPtr, dstmap1.CvPtr, dstmap2.CvPtr, dstmap1Type, nnInterpolation ? 1 : 0);
		GC.KeepAlive(map1);
		GC.KeepAlive(map2);
		GC.KeepAlive(dstmap1);
		GC.KeepAlive(dstmap2);
		dstmap1.Fix();
		dstmap2.Fix();
	}

	public static Mat GetRotationMatrix2D(Point2f center, double angle, double scale)
	{
		return new Mat(NativeMethods.imgproc_getRotationMatrix2D(center, angle, scale));
	}

	public static void InvertAffineTransform(InputArray m, OutputArray im)
	{
		if (m == null)
		{
			throw new ArgumentNullException("m");
		}
		if (im == null)
		{
			throw new ArgumentNullException("im");
		}
		m.ThrowIfDisposed();
		im.ThrowIfNotReady();
		NativeMethods.imgproc_invertAffineTransform(m.CvPtr, im.CvPtr);
		GC.KeepAlive(m);
		GC.KeepAlive(im);
		im.Fix();
	}

	public static Mat GetPerspectiveTransform(IEnumerable<Point2f> src, IEnumerable<Point2f> dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		Point2f[] src2 = EnumerableEx.ToArray(src);
		Point2f[] dst2 = EnumerableEx.ToArray(dst);
		return new Mat(NativeMethods.imgproc_getPerspectiveTransform1(src2, dst2));
	}

	public static Mat GetPerspectiveTransform(InputArray src, InputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.imgproc_getPerspectiveTransform2(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		return new Mat(ptr);
	}

	public static Mat GetAffineTransform(IEnumerable<Point2f> src, IEnumerable<Point2f> dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		Point2f[] src2 = EnumerableEx.ToArray(src);
		Point2f[] dst2 = EnumerableEx.ToArray(dst);
		return new Mat(NativeMethods.imgproc_getAffineTransform1(src2, dst2));
	}

	public static Mat GetAffineTransform(InputArray src, InputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		IntPtr ptr = NativeMethods.imgproc_getAffineTransform2(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		return new Mat(ptr);
	}

	public static void GetRectSubPix(InputArray image, Size patchSize, Point2f center, OutputArray patch, int patchType = -1)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (patch == null)
		{
			throw new ArgumentNullException("patch");
		}
		image.ThrowIfDisposed();
		patch.ThrowIfNotReady();
		NativeMethods.imgproc_getRectSubPix(image.CvPtr, patchSize, center, patch.CvPtr, patchType);
		GC.KeepAlive(image);
		GC.KeepAlive(patch);
		patch.Fix();
	}

	public static void LogPolar(InputArray src, OutputArray dst, Point2f center, double m, InterpolationFlags flags)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_logPolar(src.CvPtr, dst.CvPtr, center, m, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void LinearPolar(InputArray src, OutputArray dst, Point2f center, double maxRadius, InterpolationFlags flags)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_linearPolar(src.CvPtr, dst.CvPtr, center, maxRadius, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Integral(InputArray src, OutputArray sum, int sdepth = -1)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (sum == null)
		{
			throw new ArgumentNullException("sum");
		}
		src.ThrowIfDisposed();
		sum.ThrowIfNotReady();
		NativeMethods.imgproc_integral1(src.CvPtr, sum.CvPtr, sdepth);
		GC.KeepAlive(src);
		GC.KeepAlive(sum);
		sum.Fix();
	}

	public static void Integral(InputArray src, OutputArray sum, OutputArray sqsum, int sdepth = -1)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (sum == null)
		{
			throw new ArgumentNullException("sum");
		}
		if (sqsum == null)
		{
			throw new ArgumentNullException("sqsum");
		}
		src.ThrowIfDisposed();
		sum.ThrowIfNotReady();
		sqsum.ThrowIfNotReady();
		NativeMethods.imgproc_integral2(src.CvPtr, sum.CvPtr, sqsum.CvPtr, sdepth);
		GC.KeepAlive(src);
		GC.KeepAlive(sum);
		GC.KeepAlive(sqsum);
		sum.Fix();
		sqsum.Fix();
	}

	public static void Integral(InputArray src, OutputArray sum, OutputArray sqsum, OutputArray tilted, int sdepth = -1)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (sum == null)
		{
			throw new ArgumentNullException("sum");
		}
		if (sqsum == null)
		{
			throw new ArgumentNullException("sqsum");
		}
		if (tilted == null)
		{
			throw new ArgumentNullException("tilted");
		}
		src.ThrowIfDisposed();
		sum.ThrowIfNotReady();
		sqsum.ThrowIfNotReady();
		tilted.ThrowIfNotReady();
		NativeMethods.imgproc_integral3(src.CvPtr, sum.CvPtr, sqsum.CvPtr, tilted.CvPtr, sdepth);
		GC.KeepAlive(src);
		GC.KeepAlive(sum);
		GC.KeepAlive(sqsum);
		GC.KeepAlive(tilted);
		sum.Fix();
		sqsum.Fix();
		tilted.Fix();
	}

	public static void Accumulate(InputArray src, InputOutputArray dst, InputArray mask)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_accumulate(src.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void AccumulateSquare(InputArray src, InputOutputArray dst, InputArray mask)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_accumulateSquare(src.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void AccumulateProduct(InputArray src1, InputArray src2, InputOutputArray dst, InputArray mask)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_accumulateProduct(src1.CvPtr, src2.CvPtr, dst.CvPtr, ToPtr(mask));
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void AccumulateWeighted(InputArray src, InputOutputArray dst, double alpha, InputArray mask)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_accumulateWeighted(src.CvPtr, dst.CvPtr, alpha, ToPtr(mask));
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static double PSNR(InputArray src1, InputArray src2)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		double result = NativeMethods.imgproc_PSNR(src1.CvPtr, src2.CvPtr);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		return result;
	}

	public static Point2d PhaseCorrelate(InputArray src1, InputArray src2, InputArray window = null)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		Point2d result = NativeMethods.imgproc_phaseCorrelate(src1.CvPtr, src2.CvPtr, ToPtr(window));
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(window);
		return result;
	}

	public static Point2d PhaseCorrelateRes(InputArray src1, InputArray src2, InputArray window)
	{
		double response;
		return PhaseCorrelateRes(src1, src2, window, out response);
	}

	public static Point2d PhaseCorrelateRes(InputArray src1, InputArray src2, InputArray window, out double response)
	{
		if (src1 == null)
		{
			throw new ArgumentNullException("src1");
		}
		if (src2 == null)
		{
			throw new ArgumentNullException("src2");
		}
		if (window == null)
		{
			throw new ArgumentNullException("src2");
		}
		src1.ThrowIfDisposed();
		src2.ThrowIfDisposed();
		window.ThrowIfDisposed();
		Point2d result = NativeMethods.imgproc_phaseCorrelateRes(src1.CvPtr, src2.CvPtr, window.CvPtr, out response);
		GC.KeepAlive(src1);
		GC.KeepAlive(src2);
		GC.KeepAlive(window);
		return result;
	}

	public static void CreateHanningWindow(InputOutputArray dst, Size winSize, MatType type)
	{
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_createHanningWindow(dst.CvPtr, winSize, type);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static double Threshold(InputArray src, OutputArray dst, double thresh, double maxval, ThresholdTypes type)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		double result = NativeMethods.imgproc_threshold(src.CvPtr, dst.CvPtr, thresh, maxval, (int)type);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
		return result;
	}

	public static void AdaptiveThreshold(InputArray src, OutputArray dst, double maxValue, AdaptiveThresholdTypes adaptiveMethod, ThresholdTypes thresholdType, int blockSize, double c)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_adaptiveThreshold(src.CvPtr, dst.CvPtr, maxValue, (int)adaptiveMethod, (int)thresholdType, blockSize, c);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void PyrDown(InputArray src, OutputArray dst, Size? dstSize = null, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Size size = default;
		Size dstsize = dstSize ?? size;
		NativeMethods.imgproc_pyrDown(src.CvPtr, dst.CvPtr, dstsize, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void PyrUp(InputArray src, OutputArray dst, Size? dstSize = null, BorderTypes borderType = BorderTypes.Reflect101)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		Size size = default;
		Size dstsize = dstSize ?? size;
		NativeMethods.imgproc_pyrUp(src.CvPtr, dst.CvPtr, dstsize, (int)borderType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void CalcHist(Mat[] images, int[] channels, InputArray mask, OutputArray hist, int dims, int[] histSize, Rangef[] ranges, bool uniform = true, bool accumulate = false)
	{
		if (ranges == null)
		{
			throw new ArgumentNullException("ranges");
		}
		float[][] ranges2 = EnumerableEx.SelectToArray(ranges, (Rangef r) => new float[2] { r.Start, r.End });
		CalcHist(images, channels, mask, hist, dims, histSize, ranges2, uniform, accumulate);
	}

	public static void CalcHist(Mat[] images, int[] channels, InputArray mask, OutputArray hist, int dims, int[] histSize, float[][] ranges, bool uniform = true, bool accumulate = false)
	{
		if (images == null)
		{
			throw new ArgumentNullException("images");
		}
		if (channels == null)
		{
			throw new ArgumentNullException("channels");
		}
		if (hist == null)
		{
			throw new ArgumentNullException("hist");
		}
		if (histSize == null)
		{
			throw new ArgumentNullException("histSize");
		}
		if (ranges == null)
		{
			throw new ArgumentNullException("ranges");
		}
		hist.ThrowIfNotReady();
		IntPtr[] images2 = EnumerableEx.SelectPtrs(images);
		using (ArrayAddress2<float> arrayAddress = new ArrayAddress2<float>(ranges))
		{
			NativeMethods.imgproc_calcHist1(images2, images.Length, channels, ToPtr(mask), hist.CvPtr, dims, histSize, arrayAddress, uniform ? 1 : 0, accumulate ? 1 : 0);
		}
		GC.KeepAlive(images);
		GC.KeepAlive(mask);
		GC.KeepAlive(hist);
		hist.Fix();
	}

	public static void CalcBackProject(Mat[] images, int[] channels, InputArray hist, OutputArray backProject, Rangef[] ranges, bool uniform = true)
	{
		if (images == null)
		{
			throw new ArgumentNullException("images");
		}
		if (channels == null)
		{
			throw new ArgumentNullException("channels");
		}
		if (hist == null)
		{
			throw new ArgumentNullException("hist");
		}
		if (backProject == null)
		{
			throw new ArgumentNullException("backProject");
		}
		if (ranges == null)
		{
			throw new ArgumentNullException("ranges");
		}
		hist.ThrowIfDisposed();
		backProject.ThrowIfNotReady();
		IntPtr[] images2 = EnumerableEx.SelectPtrs(images);
		using (ArrayAddress2<float> arrayAddress = new ArrayAddress2<float>(EnumerableEx.SelectToArray(ranges, (Rangef r) => new float[2] { r.Start, r.End })))
		{
			NativeMethods.imgproc_calcBackProject(images2, images.Length, channels, hist.CvPtr, backProject.CvPtr, arrayAddress, uniform ? 1 : 0);
		}
		GC.KeepAlive(images);
		GC.KeepAlive(hist);
		GC.KeepAlive(backProject);
		backProject.Fix();
	}

	public static double CompareHist(InputArray h1, InputArray h2, HistCompMethods method)
	{
		if (h1 == null)
		{
			throw new ArgumentNullException("h1");
		}
		if (h2 == null)
		{
			throw new ArgumentNullException("h2");
		}
		h1.ThrowIfDisposed();
		h2.ThrowIfDisposed();
		double result = NativeMethods.imgproc_compareHist1(h1.CvPtr, h2.CvPtr, (int)method);
		GC.KeepAlive(h1);
		GC.KeepAlive(h2);
		return result;
	}

	public static void EqualizeHist(InputArray src, OutputArray dst)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_equalizeHist(src.CvPtr, dst.CvPtr);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static CLAHE CreateCLAHE(double clipLimit = 40.0, Size? tileGridSize = null)
	{
		return CLAHE.Create(clipLimit, tileGridSize);
	}

	public static float EMD(InputArray signature1, InputArray signature2, DistanceTypes distType)
	{
		float lowerBound;
		return EMD(signature1, signature1, distType, null, out lowerBound, null);
	}

	public static float EMD(InputArray signature1, InputArray signature2, DistanceTypes distType, InputArray cost)
	{
		float lowerBound;
		return EMD(signature1, signature1, distType, cost, out lowerBound, null);
	}

	public static float EMD(InputArray signature1, InputArray signature2, DistanceTypes distType, InputArray cost, out float lowerBound)
	{
		return EMD(signature1, signature1, distType, cost, out lowerBound, null);
	}

	public static float EMD(InputArray signature1, InputArray signature2, DistanceTypes distType, InputArray cost, out float lowerBound, OutputArray flow)
	{
		if (signature1 == null)
		{
			throw new ArgumentNullException("signature1");
		}
		if (signature2 == null)
		{
			throw new ArgumentNullException("signature2");
		}
		signature1.ThrowIfDisposed();
		signature2.ThrowIfDisposed();
		float result = NativeMethods.imgproc_EMD(signature1.CvPtr, signature2.CvPtr, (int)distType, ToPtr(cost), out lowerBound, ToPtr(flow));
		GC.KeepAlive(signature1);
		GC.KeepAlive(signature2);
		GC.KeepAlive(cost);
		GC.KeepAlive(flow);
		flow?.Fix();
		return result;
	}

	public static void Watershed(InputArray image, InputOutputArray markers)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (markers == null)
		{
			throw new ArgumentNullException("markers");
		}
		image.ThrowIfDisposed();
		markers.ThrowIfNotReady();
		NativeMethods.imgproc_watershed(image.CvPtr, markers.CvPtr);
		GC.KeepAlive(image);
		GC.KeepAlive(markers);
		markers.Fix();
	}

	public static void PyrMeanShiftFiltering(InputArray src, OutputArray dst, double sp, double sr, int maxLevel = 1, TermCriteria? termcrit = null)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		TermCriteria valueOrDefault = termcrit.GetValueOrDefault(new TermCriteria(CriteriaType.Count | CriteriaType.Eps, 5, 1.0));
		NativeMethods.imgproc_pyrMeanShiftFiltering(src.CvPtr, dst.CvPtr, sp, sr, maxLevel, valueOrDefault);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void GrabCut(InputArray img, InputOutputArray mask, Rect rect, InputOutputArray bgdModel, InputOutputArray fgdModel, int iterCount, GrabCutModes mode)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (mask == null)
		{
			throw new ArgumentNullException("mask");
		}
		if (bgdModel == null)
		{
			throw new ArgumentNullException("bgdModel");
		}
		if (fgdModel == null)
		{
			throw new ArgumentNullException("fgdModel");
		}
		img.ThrowIfDisposed();
		mask.ThrowIfNotReady();
		bgdModel.ThrowIfNotReady();
		fgdModel.ThrowIfNotReady();
		NativeMethods.imgproc_grabCut(img.CvPtr, mask.CvPtr, rect, bgdModel.CvPtr, fgdModel.CvPtr, iterCount, (int)mode);
		GC.KeepAlive(img);
		GC.KeepAlive(mask);
		GC.KeepAlive(bgdModel);
		GC.KeepAlive(fgdModel);
		mask.Fix();
		bgdModel.Fix();
		fgdModel.Fix();
	}

	public static void DistanceTransformWithLabels(InputArray src, OutputArray dst, OutputArray labels, DistanceTypes distanceType, DistanceMaskSize maskSize, DistanceTransformLabelTypes labelType = DistanceTransformLabelTypes.CComp)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (labels == null)
		{
			throw new ArgumentNullException("labels");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		labels.ThrowIfNotReady();
		NativeMethods.imgproc_distanceTransformWithLabels(src.CvPtr, dst.CvPtr, labels.CvPtr, (int)distanceType, (int)maskSize, (int)labelType);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(labels);
		dst.Fix();
		labels.Fix();
	}

	public static void DistanceTransform(InputArray src, OutputArray dst, DistanceTypes distanceType, DistanceMaskSize maskSize)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_distanceTransform(src.CvPtr, dst.CvPtr, (int)distanceType, (int)maskSize);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static int FloodFill(InputOutputArray image, Point seedPoint, Scalar newVal)
	{
		Rect rect;
		return FloodFill(image, seedPoint, newVal, out rect, null, null, FloodFillFlags.Link4);
	}

	public static int FloodFill(InputOutputArray image, Point seedPoint, Scalar newVal, out Rect rect, Scalar? loDiff = null, Scalar? upDiff = null, FloodFillFlags flags = FloodFillFlags.Link4)
	{
		return FloodFill(image, seedPoint, newVal, out rect, loDiff, upDiff, (int)flags);
	}

	public static int FloodFill(InputOutputArray image, Point seedPoint, Scalar newVal, out Rect rect, Scalar? loDiff = null, Scalar? upDiff = null, int flags = 4)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfNotReady();
		Scalar scalar = default;
		Scalar loDiff2 = loDiff ?? scalar;
		scalar = default;
		Scalar upDiff2 = upDiff ?? scalar;
		int result = NativeMethods.imgproc_floodFill1(image.CvPtr, seedPoint, newVal, out rect, loDiff2, upDiff2, flags);
		GC.KeepAlive(image);
		image.Fix();
		return result;
	}

	public static int FloodFill(InputOutputArray image, InputOutputArray mask, Point seedPoint, Scalar newVal)
	{
		Rect rect;
		return FloodFill(image, mask, seedPoint, newVal, out rect, null, null, FloodFillFlags.Link4);
	}

	public static int FloodFill(InputOutputArray image, InputOutputArray mask, Point seedPoint, Scalar newVal, out Rect rect, Scalar? loDiff = null, Scalar? upDiff = null, FloodFillFlags flags = FloodFillFlags.Link4)
	{
		return FloodFill(image, mask, seedPoint, newVal, out rect, loDiff, upDiff, (int)flags);
	}

	public static int FloodFill(InputOutputArray image, InputOutputArray mask, Point seedPoint, Scalar newVal, out Rect rect, Scalar? loDiff = null, Scalar? upDiff = null, int flags = 4)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (mask == null)
		{
			throw new ArgumentNullException("mask");
		}
		image.ThrowIfNotReady();
		mask.ThrowIfNotReady();
		Scalar scalar = default;
		Scalar loDiff2 = loDiff ?? scalar;
		scalar = default;
		Scalar upDiff2 = upDiff ?? scalar;
		int result = NativeMethods.imgproc_floodFill2(image.CvPtr, mask.CvPtr, seedPoint, newVal, out rect, loDiff2, upDiff2, flags);
		GC.KeepAlive(image);
		GC.KeepAlive(mask);
		image.Fix();
		mask.Fix();
		return result;
	}

	public static void CvtColor(InputArray src, OutputArray dst, ColorConversionCodes code, int dstCn = 0)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_cvtColor(src.CvPtr, dst.CvPtr, (int)code, dstCn);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static Moments Moments(InputArray array, bool binaryImage = false)
	{
		return new Moments(array, binaryImage);
	}

	public static Moments Moments(byte[,] array, bool binaryImage = false)
	{
		return new Moments(array, binaryImage);
	}

	public static Moments Moments(float[,] array, bool binaryImage = false)
	{
		return new Moments(array, binaryImage);
	}

	public static Moments Moments(IEnumerable<Point> array, bool binaryImage = false)
	{
		return new Moments(array, binaryImage);
	}

	public static Moments Moments(IEnumerable<Point2f> array, bool binaryImage = false)
	{
		return new Moments(array, binaryImage);
	}

	public static void MatchTemplate(InputArray image, InputArray templ, OutputArray result, TemplateMatchModes method, InputArray mask = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (templ == null)
		{
			throw new ArgumentNullException("templ");
		}
		if (result == null)
		{
			throw new ArgumentNullException("result");
		}
		image.ThrowIfDisposed();
		templ.ThrowIfDisposed();
		result.ThrowIfNotReady();
		mask?.ThrowIfDisposed();
		NativeMethods.imgproc_matchTemplate(image.CvPtr, templ.CvPtr, result.CvPtr, (int)method, ToPtr(mask));
		GC.KeepAlive(image);
		GC.KeepAlive(templ);
		GC.KeepAlive(result);
		result.Fix();
		GC.KeepAlive(mask);
	}

	public static int ConnectedComponents(InputArray image, OutputArray labels, PixelConnectivity connectivity = PixelConnectivity.Connectivity8)
	{
		return ConnectedComponents(image, labels, connectivity, 4);
	}

	public static int ConnectedComponents(InputArray image, OutputArray labels, PixelConnectivity connectivity, MatType ltype)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (labels == null)
		{
			throw new ArgumentNullException("labels");
		}
		image.ThrowIfDisposed();
		labels.ThrowIfNotReady();
		int result = NativeMethods.imgproc_connectedComponents(image.CvPtr, labels.CvPtr, (int)connectivity, ltype);
		GC.KeepAlive(image);
		GC.KeepAlive(labels);
		labels.Fix();
		return result;
	}

	public static int ConnectedComponents(InputArray image, out int[,] labels, PixelConnectivity connectivity)
	{
		using MatOfInt matOfInt = new MatOfInt();
		int result = ConnectedComponents(image, matOfInt, connectivity, 4);
		labels = matOfInt.ToRectangularArray();
		return result;
	}

	public static int ConnectedComponentsWithStats(InputArray image, OutputArray labels, OutputArray stats, OutputArray centroids, PixelConnectivity connectivity = PixelConnectivity.Connectivity8)
	{
		return ConnectedComponentsWithStats(image, labels, stats, centroids, connectivity, 4);
	}

	public static int ConnectedComponentsWithStats(InputArray image, OutputArray labels, OutputArray stats, OutputArray centroids, PixelConnectivity connectivity, MatType ltype)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (labels == null)
		{
			throw new ArgumentNullException("labels");
		}
		if (stats == null)
		{
			throw new ArgumentNullException("stats");
		}
		if (centroids == null)
		{
			throw new ArgumentNullException("centroids");
		}
		image.ThrowIfDisposed();
		labels.ThrowIfNotReady();
		stats.ThrowIfNotReady();
		centroids.ThrowIfNotReady();
		int result = NativeMethods.imgproc_connectedComponentsWithStats(image.CvPtr, labels.CvPtr, stats.CvPtr, centroids.CvPtr, (int)connectivity, ltype);
		GC.KeepAlive(image);
		GC.KeepAlive(labels);
		GC.KeepAlive(stats);
		GC.KeepAlive(centroids);
		labels.Fix();
		stats.Fix();
		centroids.Fix();
		return result;
	}

	public static ConnectedComponents ConnectedComponentsEx(InputArray image, PixelConnectivity connectivity = PixelConnectivity.Connectivity8)
	{
		using MatOfInt matOfInt = new MatOfInt();
		using MatOfInt matOfInt2 = new MatOfInt();
		using MatOfDouble matOfDouble = new MatOfDouble();
		int num = ConnectedComponentsWithStats(image, matOfInt, matOfInt2, matOfDouble, connectivity, 4);
		int[,] labels = matOfInt.ToRectangularArray();
		int[,] array = matOfInt2.ToRectangularArray();
		double[,] array2 = matOfDouble.ToRectangularArray();
		ConnectedComponents.Blob[] array3 = new ConnectedComponents.Blob[num];
		for (int i = 0; i < num; i++)
		{
			array3[i] = new ConnectedComponents.Blob
			{
				Label = i,
				Left = array[i, 0],
				Top = array[i, 1],
				Width = array[i, 2],
				Height = array[i, 3],
				Area = array[i, 4],
				Centroid = new Point2d(array2[i, 0], array2[i, 1])
			};
		}
		return new ConnectedComponents(array3, labels, num);
	}

	public static void FindContours(InputOutputArray image, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes mode, ContourApproximationModes method, Point? offset = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfNotReady();
		Point point = default;
		Point offset2 = offset ?? point;
		NativeMethods.imgproc_findContours1_vector(image.CvPtr, out var contours2, out var hierarchy2, (int)mode, (int)method, offset2);
		using (VectorOfVectorPoint vectorOfVectorPoint = new VectorOfVectorPoint(contours2))
		{
			using VectorOfVec4i vectorOfVec4i = new VectorOfVec4i(hierarchy2);
			contours = vectorOfVectorPoint.ToArray();
			Vec4i[] enumerable = vectorOfVec4i.ToArray();
			hierarchy = EnumerableEx.SelectToArray(enumerable, HierarchyIndex.FromVec4i);
		}
		image.Fix();
		GC.KeepAlive(image);
	}

	public static void FindContours(InputOutputArray image, out Mat[] contours, OutputArray hierarchy, RetrievalModes mode, ContourApproximationModes method, Point? offset = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (hierarchy == null)
		{
			throw new ArgumentNullException("hierarchy");
		}
		image.ThrowIfNotReady();
		hierarchy.ThrowIfNotReady();
		Point point = default;
		Point offset2 = offset ?? point;
		NativeMethods.imgproc_findContours1_OutputArray(image.CvPtr, out var contours2, hierarchy.CvPtr, (int)mode, (int)method, offset2);
		using (VectorOfMat vectorOfMat = new VectorOfMat(contours2))
		{
			contours = vectorOfMat.ToArray();
		}
		image.Fix();
		hierarchy.Fix();
		GC.KeepAlive(image);
		GC.KeepAlive(hierarchy);
	}

	public static Point[][] FindContoursAsArray(InputOutputArray image, RetrievalModes mode, ContourApproximationModes method, Point? offset = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfNotReady();
		Point point = default;
		Point offset2 = offset ?? point;
		NativeMethods.imgproc_findContours2_vector(image.CvPtr, out var contours, (int)mode, (int)method, offset2);
		image.Fix();
		GC.KeepAlive(image);
		using VectorOfVectorPoint vectorOfVectorPoint = new VectorOfVectorPoint(contours);
		return vectorOfVectorPoint.ToArray();
	}

	public static MatOfPoint[] FindContoursAsMat(InputOutputArray image, RetrievalModes mode, ContourApproximationModes method, Point? offset = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		image.ThrowIfNotReady();
		Point point = default;
		Point offset2 = offset ?? point;
		NativeMethods.imgproc_findContours2_OutputArray(image.CvPtr, out var contours, (int)mode, (int)method, offset2);
		image.Fix();
		GC.KeepAlive(image);
		using VectorOfMat vectorOfMat = new VectorOfMat(contours);
		return vectorOfMat.ToArray<MatOfPoint>();
	}

	public static void ApproxPolyDP(InputArray curve, OutputArray approxCurve, double epsilon, bool closed)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		if (approxCurve == null)
		{
			throw new ArgumentNullException("approxCurve");
		}
		curve.ThrowIfDisposed();
		approxCurve.ThrowIfNotReady();
		NativeMethods.imgproc_approxPolyDP_InputArray(curve.CvPtr, approxCurve.CvPtr, epsilon, closed ? 1 : 0);
		GC.KeepAlive(curve);
		GC.KeepAlive(approxCurve);
		approxCurve.Fix();
	}

	public static Point[] ApproxPolyDP(IEnumerable<Point> curve, double epsilon, bool closed)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		Point[] array = EnumerableEx.ToArray(curve);
		NativeMethods.imgproc_approxPolyDP_Point(array, array.Length, out var approxCurve, epsilon, closed ? 1 : 0);
		using VectorOfPoint vectorOfPoint = new VectorOfPoint(approxCurve);
		return vectorOfPoint.ToArray();
	}

	public static Point2f[] ApproxPolyDP(IEnumerable<Point2f> curve, double epsilon, bool closed)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		Point2f[] array = EnumerableEx.ToArray(curve);
		NativeMethods.imgproc_approxPolyDP_Point2f(array, array.Length, out var approxCurve, epsilon, closed ? 1 : 0);
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f(approxCurve);
		return vectorOfPoint2f.ToArray();
	}

	public static double ArcLength(InputArray curve, bool closed)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		curve.ThrowIfDisposed();
		double result = NativeMethods.imgproc_arcLength_InputArray(curve.CvPtr, closed ? 1 : 0);
		GC.KeepAlive(curve);
		return result;
	}

	public static double ArcLength(IEnumerable<Point> curve, bool closed)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		Point[] array = EnumerableEx.ToArray(curve);
		return NativeMethods.imgproc_arcLength_Point(array, array.Length, closed ? 1 : 0);
	}

	public static double ArcLength(IEnumerable<Point2f> curve, bool closed)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		Point2f[] array = EnumerableEx.ToArray(curve);
		return NativeMethods.imgproc_arcLength_Point2f(array, array.Length, closed ? 1 : 0);
	}

	public static Rect BoundingRect(InputArray curve)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		curve.ThrowIfDisposed();
		Rect result = NativeMethods.imgproc_boundingRect_InputArray(curve.CvPtr);
		GC.KeepAlive(curve);
		return result;
	}

	public static Rect BoundingRect(IEnumerable<Point> curve)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		Point[] array = EnumerableEx.ToArray(curve);
		return NativeMethods.imgproc_boundingRect_Point(array, array.Length);
	}

	public static Rect BoundingRect(IEnumerable<Point2f> curve)
	{
		if (curve == null)
		{
			throw new ArgumentNullException("curve");
		}
		Point2f[] array = EnumerableEx.ToArray(curve);
		return NativeMethods.imgproc_boundingRect_Point2f(array, array.Length);
	}

	public static double ContourArea(InputArray contour, bool oriented = false)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		contour.ThrowIfDisposed();
		double result = NativeMethods.imgproc_contourArea_InputArray(contour.CvPtr, oriented ? 1 : 0);
		GC.KeepAlive(contour);
		return result;
	}

	public static double ContourArea(IEnumerable<Point> contour, bool oriented = false)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		Point[] array = EnumerableEx.ToArray(contour);
		return NativeMethods.imgproc_contourArea_Point(array, array.Length, oriented ? 1 : 0);
	}

	public static double ContourArea(IEnumerable<Point2f> contour, bool oriented = false)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		Point2f[] array = EnumerableEx.ToArray(contour);
		return NativeMethods.imgproc_contourArea_Point2f(array, array.Length, oriented ? 1 : 0);
	}

	public static RotatedRect MinAreaRect(InputArray points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		points.ThrowIfDisposed();
		RotatedRect result = NativeMethods.imgproc_minAreaRect_InputArray(points.CvPtr);
		GC.KeepAlive(points);
		return result;
	}

	public static RotatedRect MinAreaRect(IEnumerable<Point> points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point[] array = EnumerableEx.ToArray(points);
		return NativeMethods.imgproc_minAreaRect_Point(array, array.Length);
	}

	public static RotatedRect MinAreaRect(IEnumerable<Point2f> points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point2f[] array = EnumerableEx.ToArray(points);
		return NativeMethods.imgproc_minAreaRect_Point2f(array, array.Length);
	}

	public static void MinEnclosingCircle(InputArray points, out Point2f center, out float radius)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		points.ThrowIfDisposed();
		NativeMethods.imgproc_minEnclosingCircle_InputArray(points.CvPtr, out center, out radius);
		GC.KeepAlive(points);
	}

	public static void MinEnclosingCircle(IEnumerable<Point> points, out Point2f center, out float radius)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point[] array = EnumerableEx.ToArray(points);
		NativeMethods.imgproc_minEnclosingCircle_Point(array, array.Length, out center, out radius);
	}

	public static void MinEnclosingCircle(IEnumerable<Point2f> points, out Point2f center, out float radius)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point2f[] array = EnumerableEx.ToArray(points);
		NativeMethods.imgproc_minEnclosingCircle_Point2f(array, array.Length, out center, out radius);
	}

	public static double MatchShapes(InputArray contour1, InputArray contour2, ShapeMatchModes method, double parameter = 0.0)
	{
		if (contour1 == null)
		{
			throw new ArgumentNullException("contour1");
		}
		if (contour2 == null)
		{
			throw new ArgumentNullException("contour2");
		}
		double result = NativeMethods.imgproc_matchShapes_InputArray(contour1.CvPtr, contour2.CvPtr, (int)method, parameter);
		GC.KeepAlive(contour1);
		GC.KeepAlive(contour2);
		return result;
	}

	public static double MatchShapes(IEnumerable<Point> contour1, IEnumerable<Point> contour2, ShapeMatchModes method, double parameter = 0.0)
	{
		if (contour1 == null)
		{
			throw new ArgumentNullException("contour1");
		}
		if (contour2 == null)
		{
			throw new ArgumentNullException("contour2");
		}
		Point[] array = EnumerableEx.ToArray(contour1);
		Point[] array2 = EnumerableEx.ToArray(contour2);
		return NativeMethods.imgproc_matchShapes_Point(array, array.Length, array2, array2.Length, (int)method, parameter);
	}

	public static void ConvexHull(InputArray points, OutputArray hull, bool clockwise = false, bool returnPoints = true)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (hull == null)
		{
			throw new ArgumentNullException("hull");
		}
		points.ThrowIfDisposed();
		hull.ThrowIfNotReady();
		NativeMethods.imgproc_convexHull_InputArray(points.CvPtr, hull.CvPtr, clockwise ? 1 : 0, returnPoints ? 1 : 0);
		GC.KeepAlive(points);
		GC.KeepAlive(hull);
		hull.Fix();
	}

	public static Point[] ConvexHull(IEnumerable<Point> points, bool clockwise = false)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point[] array = EnumerableEx.ToArray(points);
		NativeMethods.imgproc_convexHull_Point_ReturnsPoints(array, array.Length, out var hull, clockwise ? 1 : 0);
		using VectorOfPoint vectorOfPoint = new VectorOfPoint(hull);
		return vectorOfPoint.ToArray();
	}

	public static Point2f[] ConvexHull(IEnumerable<Point2f> points, bool clockwise = false)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point2f[] array = EnumerableEx.ToArray(points);
		NativeMethods.imgproc_convexHull_Point2f_ReturnsPoints(array, array.Length, out var hull, clockwise ? 1 : 0);
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f(hull);
		return vectorOfPoint2f.ToArray();
	}

	public static int[] ConvexHullIndices(IEnumerable<Point> points, bool clockwise = false)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point[] array = EnumerableEx.ToArray(points);
		NativeMethods.imgproc_convexHull_Point_ReturnsIndices(array, array.Length, out var hull, clockwise ? 1 : 0);
		using VectorOfInt32 vectorOfInt = new VectorOfInt32(hull);
		return vectorOfInt.ToArray();
	}

	public static int[] ConvexHullIndices(IEnumerable<Point2f> points, bool clockwise = false)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point2f[] array = EnumerableEx.ToArray(points);
		NativeMethods.imgproc_convexHull_Point2f_ReturnsIndices(array, array.Length, out var hull, clockwise ? 1 : 0);
		using VectorOfInt32 vectorOfInt = new VectorOfInt32(hull);
		return vectorOfInt.ToArray();
	}

	public static void ConvexityDefects(InputArray contour, InputArray convexHull, OutputArray convexityDefects)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		if (convexHull == null)
		{
			throw new ArgumentNullException("convexHull");
		}
		if (convexityDefects == null)
		{
			throw new ArgumentNullException("convexityDefects");
		}
		contour.ThrowIfDisposed();
		convexHull.ThrowIfDisposed();
		convexityDefects.ThrowIfNotReady();
		NativeMethods.imgproc_convexityDefects_InputArray(contour.CvPtr, convexHull.CvPtr, convexityDefects.CvPtr);
		GC.KeepAlive(contour);
		GC.KeepAlive(convexHull);
		GC.KeepAlive(convexityDefects);
		convexityDefects.Fix();
	}

	public static Vec4i[] ConvexityDefects(IEnumerable<Point> contour, IEnumerable<int> convexHull)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		if (convexHull == null)
		{
			throw new ArgumentNullException("convexHull");
		}
		Point[] array = EnumerableEx.ToArray(contour);
		int[] array2 = EnumerableEx.ToArray(convexHull);
		NativeMethods.imgproc_convexityDefects_Point(array, array.Length, array2, array2.Length, out var convexityDefects);
		using VectorOfVec4i vectorOfVec4i = new VectorOfVec4i(convexityDefects);
		return vectorOfVec4i.ToArray();
	}

	public static Vec4i[] ConvexityDefects(IEnumerable<Point2f> contour, IEnumerable<int> convexHull)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		if (convexHull == null)
		{
			throw new ArgumentNullException("convexHull");
		}
		Point2f[] array = EnumerableEx.ToArray(contour);
		int[] array2 = EnumerableEx.ToArray(convexHull);
		NativeMethods.imgproc_convexityDefects_Point2f(array, array.Length, array2, array2.Length, out var convexityDefects);
		using VectorOfVec4i vectorOfVec4i = new VectorOfVec4i(convexityDefects);
		return vectorOfVec4i.ToArray();
	}

	public static bool IsContourConvex(InputArray contour)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		contour.ThrowIfDisposed();
		int num = NativeMethods.imgproc_isContourConvex_InputArray(contour.CvPtr);
		GC.KeepAlive(contour);
		return num != 0;
	}

	public static bool IsContourConvex(IEnumerable<Point> contour)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		Point[] array = EnumerableEx.ToArray(contour);
		return NativeMethods.imgproc_isContourConvex_Point(array, array.Length) != 0;
	}

	public static bool IsContourConvex(IEnumerable<Point2f> contour)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		Point2f[] array = EnumerableEx.ToArray(contour);
		return NativeMethods.imgproc_isContourConvex_Point2f(array, array.Length) != 0;
	}

	public static float IntersectConvexConvex(InputArray p1, InputArray p2, OutputArray p12, bool handleNested = true)
	{
		if (p1 == null)
		{
			throw new ArgumentNullException("p1");
		}
		if (p2 == null)
		{
			throw new ArgumentNullException("p2");
		}
		if (p12 == null)
		{
			throw new ArgumentNullException("p12");
		}
		p1.ThrowIfDisposed();
		p2.ThrowIfDisposed();
		p12.ThrowIfNotReady();
		float result = NativeMethods.imgproc_intersectConvexConvex_InputArray(p1.CvPtr, p2.CvPtr, p12.CvPtr, handleNested ? 1 : 0);
		GC.KeepAlive(p1);
		GC.KeepAlive(p2);
		GC.KeepAlive(p12);
		p12.Fix();
		return result;
	}

	public static float IntersectConvexConvex(IEnumerable<Point> p1, IEnumerable<Point> p2, out Point[] p12, bool handleNested = true)
	{
		if (p1 == null)
		{
			throw new ArgumentNullException("p1");
		}
		if (p2 == null)
		{
			throw new ArgumentNullException("p2");
		}
		Point[] array = EnumerableEx.ToArray(p1);
		Point[] array2 = EnumerableEx.ToArray(p2);
		float result = NativeMethods.imgproc_intersectConvexConvex_Point(array, array.Length, array2, array2.Length, out var p13, handleNested ? 1 : 0);
		using VectorOfPoint vectorOfPoint = new VectorOfPoint(p13);
		p12 = vectorOfPoint.ToArray();
		return result;
	}

	public static float IntersectConvexConvex(IEnumerable<Point2f> p1, IEnumerable<Point2f> p2, out Point2f[] p12, bool handleNested = true)
	{
		if (p1 == null)
		{
			throw new ArgumentNullException("p1");
		}
		if (p2 == null)
		{
			throw new ArgumentNullException("p2");
		}
		Point2f[] array = EnumerableEx.ToArray(p1);
		Point2f[] array2 = EnumerableEx.ToArray(p2);
		float result = NativeMethods.imgproc_intersectConvexConvex_Point2f(array, array.Length, array2, array2.Length, out var p13, handleNested ? 1 : 0);
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f(p13);
		p12 = vectorOfPoint2f.ToArray();
		return result;
	}

	public static RotatedRect FitEllipse(InputArray points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		points.ThrowIfDisposed();
		RotatedRect result = NativeMethods.imgproc_fitEllipse_InputArray(points.CvPtr);
		GC.KeepAlive(points);
		return result;
	}

	public static RotatedRect FitEllipse(IEnumerable<Point> points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point[] array = EnumerableEx.ToArray(points);
		return NativeMethods.imgproc_fitEllipse_Point(array, array.Length);
	}

	public static RotatedRect FitEllipse(IEnumerable<Point2f> points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point2f[] array = EnumerableEx.ToArray(points);
		return NativeMethods.imgproc_fitEllipse_Point2f(array, array.Length);
	}

	public static void FitLine(InputArray points, OutputArray line, DistanceTypes distType, double param, double reps, double aeps)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (line == null)
		{
			throw new ArgumentNullException("line");
		}
		points.ThrowIfDisposed();
		line.ThrowIfNotReady();
		NativeMethods.imgproc_fitLine_InputArray(points.CvPtr, line.CvPtr, (int)distType, param, reps, aeps);
		GC.KeepAlive(points);
		GC.KeepAlive(line);
		line.Fix();
	}

	public static Line2D FitLine(IEnumerable<Point> points, DistanceTypes distType, double param, double reps, double aeps)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point[] array = EnumerableEx.ToArray(points);
		float[] line = new float[4];
		NativeMethods.imgproc_fitLine_Point(array, array.Length, line, (int)distType, param, reps, aeps);
		return new Line2D(line);
	}

	public static Line2D FitLine(IEnumerable<Point2f> points, DistanceTypes distType, double param, double reps, double aeps)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point2f[] array = EnumerableEx.ToArray(points);
		float[] line = new float[4];
		NativeMethods.imgproc_fitLine_Point2f(array, array.Length, line, (int)distType, param, reps, aeps);
		return new Line2D(line);
	}

	public static Line3D FitLine(IEnumerable<Point3i> points, DistanceTypes distType, double param, double reps, double aeps)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point3i[] array = EnumerableEx.ToArray(points);
		float[] line = new float[6];
		NativeMethods.imgproc_fitLine_Point3i(array, array.Length, line, (int)distType, param, reps, aeps);
		return new Line3D(line);
	}

	public static Line3D FitLine(IEnumerable<Point3f> points, DistanceTypes distType, double param, double reps, double aeps)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Point3f[] array = EnumerableEx.ToArray(points);
		float[] line = new float[6];
		NativeMethods.imgproc_fitLine_Point3f(array, array.Length, line, (int)distType, param, reps, aeps);
		return new Line3D(line);
	}

	public static double PointPolygonTest(InputArray contour, Point2f pt, bool measureDist)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		contour.ThrowIfDisposed();
		double result = NativeMethods.imgproc_pointPolygonTest_InputArray(contour.CvPtr, pt, measureDist ? 1 : 0);
		GC.KeepAlive(contour);
		return result;
	}

	public static double PointPolygonTest(IEnumerable<Point> contour, Point2f pt, bool measureDist)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		Point[] array = EnumerableEx.ToArray(contour);
		return NativeMethods.imgproc_pointPolygonTest_Point(array, array.Length, pt, measureDist ? 1 : 0);
	}

	public static double PointPolygonTest(IEnumerable<Point2f> contour, Point2f pt, bool measureDist)
	{
		if (contour == null)
		{
			throw new ArgumentNullException("contour");
		}
		Point2f[] array = EnumerableEx.ToArray(contour);
		return NativeMethods.imgproc_pointPolygonTest_Point2f(array, array.Length, pt, measureDist ? 1 : 0);
	}

	public static RectanglesIntersectTypes RotatedRectangleIntersection(RotatedRect rect1, RotatedRect rect2, OutputArray intersectingRegion)
	{
		if (intersectingRegion == null)
		{
			throw new ArgumentNullException("intersectingRegion");
		}
		intersectingRegion.ThrowIfNotReady();
		int result = NativeMethods.imgproc_rotatedRectangleIntersection_OutputArray(rect1, rect2, intersectingRegion.CvPtr);
		GC.KeepAlive(intersectingRegion);
		intersectingRegion.Fix();
		return (RectanglesIntersectTypes)result;
	}

	public static RectanglesIntersectTypes RotatedRectangleIntersection(RotatedRect rect1, RotatedRect rect2, out Point2f[] intersectingRegion)
	{
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		int result = NativeMethods.imgproc_rotatedRectangleIntersection_vector(rect1, rect2, vectorOfPoint2f.CvPtr);
		intersectingRegion = vectorOfPoint2f.ToArray();
		return (RectanglesIntersectTypes)result;
	}

	public static void ApplyColorMap(InputArray src, OutputArray dst, ColormapTypes colormap)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.imgproc_applyColorMap(src.CvPtr, dst.CvPtr, (int)colormap);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		dst.Fix();
	}

	public static void Line(InputOutputArray img, int pt1X, int pt1Y, int pt2X, int pt2Y, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		Line(img, new Point(pt1X, pt1Y), new Point(pt2X, pt2Y), color, thickness, lineType, shift);
	}

	public static void Line(InputOutputArray img, Point pt1, Point pt2, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfNotReady();
		NativeMethods.imgproc_line(img.CvPtr, pt1, pt2, color, thickness, (int)lineType, shift);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static void ArrowedLine(InputOutputArray img, Point pt1, Point pt2, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0, double tipLength = 0.1)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfNotReady();
		NativeMethods.imgproc_arrowedLine(img.CvPtr, pt1, pt2, color, thickness, (int)lineType, shift, tipLength);
		GC.KeepAlive(img);
		img.Fix();
	}

	public static void Rectangle(InputOutputArray img, Point pt1, Point pt2, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		NativeMethods.imgproc_rectangle_InputOutputArray(img.CvPtr, pt1, pt2, color, thickness, (int)lineType, shift);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static void Rectangle(InputOutputArray img, Rect rect, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		NativeMethods.imgproc_rectangle_InputOutputArray(img.CvPtr, rect.TopLeft, rect.BottomRight, color, thickness, (int)lineType, shift);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static void Rectangle(Mat img, Point pt1, Point pt2, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		Rect rect = Rect.FromLTRB(pt1.X, pt1.Y, pt2.X, pt2.Y);
		NativeMethods.imgproc_rectangle_Mat(img.CvPtr, rect, color, thickness, (int)lineType, shift);
		GC.KeepAlive(img);
	}

	public static void Rectangle(Mat img, Rect rect, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		NativeMethods.imgproc_rectangle_Mat(img.CvPtr, rect, color, thickness, (int)lineType, shift);
		GC.KeepAlive(img);
	}

	public static void Circle(InputOutputArray img, int centerX, int centerY, int radius, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		Circle(img, new Point(centerX, centerY), radius, color, thickness, lineType, shift);
	}

	public static void Circle(InputOutputArray img, Point center, int radius, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		NativeMethods.imgproc_circle(img.CvPtr, center, radius, color, thickness, (int)lineType, shift);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static void Ellipse(InputOutputArray img, Point center, Size axes, double angle, double startAngle, double endAngle, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfNotReady();
		NativeMethods.imgproc_ellipse1(img.CvPtr, center, axes, angle, startAngle, endAngle, color, thickness, (int)lineType, shift);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static void Ellipse(InputOutputArray img, RotatedRect box, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		NativeMethods.imgproc_ellipse2(img.CvPtr, box, color, thickness, (int)lineType);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static void FillConvexPoly(Mat img, IEnumerable<Point> pts, Scalar color, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		Point[] array = EnumerableEx.ToArray(pts);
		NativeMethods.imgproc_fillConvexPoly_Mat(img.CvPtr, array, array.Length, color, (int)lineType, shift);
		GC.KeepAlive(img);
	}

	public static void FillConvexPoly(InputOutputArray img, InputArray pts, Scalar color, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		img.ThrowIfDisposed();
		pts.ThrowIfDisposed();
		NativeMethods.imgproc_fillConvexPoly_InputOutputArray(img.CvPtr, pts.CvPtr, color, (int)lineType, shift);
		GC.KeepAlive(img);
		GC.KeepAlive(pts);
	}

	public static void FillPoly(Mat img, IEnumerable<IEnumerable<Point>> pts, Scalar color, LineTypes lineType = LineTypes.Link8, int shift = 0, Point? offset = null)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		Point point = default;
		Point offset2 = offset ?? point;
		List<Point[]> list = new List<Point[]>();
		List<int> list2 = new List<int>();
		foreach (IEnumerable<Point> pt in pts)
		{
			Point[] array = EnumerableEx.ToArray(pt);
			list.Add(array);
			list2.Add(array.Length);
		}
		Point[][] array2 = list.ToArray();
		int[] npts = list2.ToArray();
		int ncontours = array2.Length;
		using (ArrayAddress2<Point> arrayAddress = new ArrayAddress2<Point>(array2))
		{
			NativeMethods.imgproc_fillPoly_Mat(img.CvPtr, arrayAddress.Pointer, npts, ncontours, color, (int)lineType, shift, offset2);
		}
		GC.KeepAlive(img);
	}

	public static void FillPoly(InputOutputArray img, InputArray pts, Scalar color, LineTypes lineType = LineTypes.Link8, int shift = 0, Point? offset = null)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		img.ThrowIfDisposed();
		pts.ThrowIfDisposed();
		Point point = default;
		Point offset2 = offset ?? point;
		NativeMethods.imgproc_fillPoly_InputOutputArray(img.CvPtr, pts.CvPtr, color, (int)lineType, shift, offset2);
		GC.KeepAlive(img);
		GC.KeepAlive(pts);
		img.Fix();
	}

	public static void Polylines(Mat img, IEnumerable<IEnumerable<Point>> pts, bool isClosed, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		List<Point[]> list = new List<Point[]>();
		List<int> list2 = new List<int>();
		foreach (IEnumerable<Point> pt in pts)
		{
			Point[] array = EnumerableEx.ToArray(pt);
			list.Add(array);
			list2.Add(array.Length);
		}
		Point[][] array2 = list.ToArray();
		int[] npts = list2.ToArray();
		int ncontours = array2.Length;
		using ArrayAddress2<Point> arrayAddress = new ArrayAddress2<Point>(array2);
		NativeMethods.imgproc_polylines_Mat(img.CvPtr, arrayAddress.Pointer, npts, ncontours, isClosed ? 1 : 0, color, thickness, (int)lineType, shift);
		GC.KeepAlive(img);
	}

	public static void Polylines(InputOutputArray img, InputArray pts, bool isClosed, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, int shift = 0)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		img.ThrowIfDisposed();
		pts.ThrowIfDisposed();
		NativeMethods.imgproc_polylines_InputOutputArray(img.CvPtr, pts.CvPtr, isClosed ? 1 : 0, color, thickness, (int)lineType, shift);
		GC.KeepAlive(img);
		img.Fix();
		GC.KeepAlive(pts);
	}

	public static void DrawContours(InputOutputArray image, IEnumerable<IEnumerable<Point>> contours, int contourIdx, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, IEnumerable<HierarchyIndex> hierarchy = null, int maxLevel = int.MaxValue, Point? offset = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (contours == null)
		{
			throw new ArgumentNullException("contours");
		}
		image.ThrowIfNotReady();
		Point point = default;
		Point offset2 = offset ?? point;
		Point[][] array = EnumerableEx.SelectToArray(contours, EnumerableEx.ToArray);
		int[] contoursSize = EnumerableEx.SelectToArray(array, (Point[] pts) => pts.Length);
		using (ArrayAddress2<Point> arrayAddress = new ArrayAddress2<Point>(array))
		{
			if (hierarchy == null)
			{
				NativeMethods.imgproc_drawContours_vector(image.CvPtr, arrayAddress.Pointer, array.Length, contoursSize, contourIdx, color, thickness, (int)lineType, IntPtr.Zero, 0, maxLevel, offset2);
			}
			else
			{
				Vec4i[] array2 = EnumerableEx.SelectToArray(hierarchy, (HierarchyIndex hi) => hi.ToVec4i());
				NativeMethods.imgproc_drawContours_vector(image.CvPtr, arrayAddress.Pointer, array.Length, contoursSize, contourIdx, color, thickness, (int)lineType, array2, array2.Length, maxLevel, offset2);
			}
		}
		GC.KeepAlive(image);
		image.Fix();
	}

	public static void DrawContours(InputOutputArray image, IEnumerable<Mat> contours, int contourIdx, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, Mat hierarchy = null, int maxLevel = int.MaxValue, Point? offset = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (contours == null)
		{
			throw new ArgumentNullException("contours");
		}
		image.ThrowIfNotReady();
		Point point = default;
		Point offset2 = offset ?? point;
		IntPtr[] array = EnumerableEx.SelectPtrs(contours);
		NativeMethods.imgproc_drawContours_InputArray(image.CvPtr, array, array.Length, contourIdx, color, thickness, (int)lineType, ToPtr(hierarchy), maxLevel, offset2);
		image.Fix();
		GC.KeepAlive(image);
		GC.KeepAlive(contours);
		GC.KeepAlive(hierarchy);
	}

	public static bool ClipLine(Size imgSize, ref Point pt1, ref Point pt2)
	{
		return NativeMethods.imgproc_clipLine1(imgSize, ref pt1, ref pt2) != 0;
	}

	public static bool ClipLine(Rect imgRect, ref Point pt1, ref Point pt2)
	{
		return NativeMethods.imgproc_clipLine2(imgRect, ref pt1, ref pt2) != 0;
	}

	public static Point[] Ellipse2Poly(Point center, Size axes, int angle, int arcStart, int arcEnd, int delta)
	{
		using VectorOfPoint vectorOfPoint = new VectorOfPoint();
		NativeMethods.imgproc_ellipse2Poly(center, axes, angle, arcStart, arcEnd, delta, vectorOfPoint.CvPtr);
		return vectorOfPoint.ToArray();
	}

	public static void PutText(InputOutputArray img, string text, Point org, HersheyFonts fontFace, double fontScale, Scalar color, int thickness = 1, LineTypes lineType = LineTypes.Link8, bool bottomLeftOrigin = false)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (string.IsNullOrEmpty(text))
		{
			throw new ArgumentNullException(text);
		}
		img.ThrowIfDisposed();
		NativeMethods.core_putText(img.CvPtr, text, org, (int)fontFace, fontScale, color, thickness, (int)lineType, bottomLeftOrigin ? 1 : 0);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static Size GetTextSize(string text, HersheyFonts fontFace, double fontScale, int thickness, out int baseLine)
	{
		if (string.IsNullOrEmpty(text))
		{
			throw new ArgumentNullException(text);
		}
		return NativeMethods.core_getTextSize(text, (int)fontFace, fontScale, thickness, out baseLine);
	}

	public static void GroupRectangles(IList<Rect> rectList, int groupThreshold, double eps = 0.2)
	{
		if (rectList == null)
		{
			throw new ArgumentNullException("rectList");
		}
		using VectorOfRect vectorOfRect = new VectorOfRect(rectList);
		NativeMethods.objdetect_groupRectangles1(vectorOfRect.CvPtr, groupThreshold, eps);
		ClearAndAddRange(rectList, vectorOfRect.ToArray());
	}

	public static void GroupRectangles(IList<Rect> rectList, out int[] weights, int groupThreshold, double eps = 0.2)
	{
		if (rectList == null)
		{
			throw new ArgumentNullException("rectList");
		}
		using VectorOfRect vectorOfRect = new VectorOfRect(rectList);
		using VectorOfInt32 vectorOfInt = new VectorOfInt32();
		NativeMethods.objdetect_groupRectangles2(vectorOfRect.CvPtr, vectorOfInt.CvPtr, groupThreshold, eps);
		ClearAndAddRange(rectList, vectorOfRect.ToArray());
		weights = vectorOfInt.ToArray();
	}

	public static void GroupRectangles(IList<Rect> rectList, int groupThreshold, double eps, out int[] weights, out double[] levelWeights)
	{
		if (rectList == null)
		{
			throw new ArgumentNullException("rectList");
		}
		using VectorOfRect vectorOfRect = new VectorOfRect(rectList);
		using VectorOfInt32 vectorOfInt = new VectorOfInt32();
		using VectorOfDouble vectorOfDouble = new VectorOfDouble();
		NativeMethods.objdetect_groupRectangles3(vectorOfRect.CvPtr, groupThreshold, eps, vectorOfInt.CvPtr, vectorOfDouble.CvPtr);
		ClearAndAddRange(rectList, vectorOfRect.ToArray());
		weights = vectorOfInt.ToArray();
		levelWeights = vectorOfDouble.ToArray();
	}

	public static void GroupRectangles(IList<Rect> rectList, out int[] rejectLevels, out double[] levelWeights, int groupThreshold, double eps = 0.2)
	{
		if (rectList == null)
		{
			throw new ArgumentNullException("rectList");
		}
		using VectorOfRect vectorOfRect = new VectorOfRect(rectList);
		using VectorOfInt32 vectorOfInt = new VectorOfInt32();
		using VectorOfDouble vectorOfDouble = new VectorOfDouble();
		NativeMethods.objdetect_groupRectangles4(vectorOfRect.CvPtr, vectorOfInt.CvPtr, vectorOfDouble.CvPtr, groupThreshold, eps);
		ClearAndAddRange(rectList, vectorOfRect.ToArray());
		rejectLevels = vectorOfInt.ToArray();
		levelWeights = vectorOfDouble.ToArray();
	}

	public static void GroupRectanglesMeanshift(IList<Rect> rectList, out double[] foundWeights, out double[] foundScales, double detectThreshold = 0.0, Size? winDetSize = null)
	{
		if (rectList == null)
		{
			throw new ArgumentNullException("rectList");
		}
		Size valueOrDefault = winDetSize.GetValueOrDefault(new Size(64, 128));
		using VectorOfRect vectorOfRect = new VectorOfRect(rectList);
		using VectorOfDouble vectorOfDouble = new VectorOfDouble();
		using VectorOfDouble vectorOfDouble2 = new VectorOfDouble();
		NativeMethods.objdetect_groupRectangles_meanshift(vectorOfRect.CvPtr, vectorOfDouble.CvPtr, vectorOfDouble2.CvPtr, detectThreshold, valueOrDefault);
		ClearAndAddRange(rectList, vectorOfRect.ToArray());
		foundWeights = vectorOfDouble.ToArray();
		foundScales = vectorOfDouble2.ToArray();
	}

	private static void ClearAndAddRange<T>(IList<T> list, IEnumerable<T> values)
	{
		list.Clear();
		foreach (T value in values)
		{
			list.Add(value);
		}
	}

	public static void Inpaint(InputArray src, InputArray inpaintMask, OutputArray dst, double inpaintRadius, InpaintMethod flags)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (inpaintMask == null)
		{
			throw new ArgumentNullException("inpaintMask");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		inpaintMask.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.photo_inpaint(src.CvPtr, inpaintMask.CvPtr, dst.CvPtr, inpaintRadius, (int)flags);
		dst.Fix();
		GC.KeepAlive(src);
		GC.KeepAlive(inpaintMask);
	}

	public static void FastNlMeansDenoising(InputArray src, OutputArray dst, float h = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.photo_fastNlMeansDenoising(src.CvPtr, dst.CvPtr, h, templateWindowSize, searchWindowSize);
		dst.Fix();
		GC.KeepAlive(src);
	}

	public static void FastNlMeansDenoisingColored(InputArray src, OutputArray dst, float h = 3f, float hColor = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.photo_fastNlMeansDenoisingColored(src.CvPtr, dst.CvPtr, h, hColor, templateWindowSize, searchWindowSize);
		dst.Fix();
		GC.KeepAlive(src);
	}

	public static void FastNlMeansDenoisingMulti(IEnumerable<InputArray> srcImgs, OutputArray dst, int imgToDenoiseIndex, int temporalWindowSize, float h = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
	{
		if (srcImgs == null)
		{
			throw new ArgumentNullException("srcImgs");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		dst.ThrowIfNotReady();
		IntPtr[] array = EnumerableEx.SelectPtrs(srcImgs);
		NativeMethods.photo_fastNlMeansDenoisingMulti(array, array.Length, dst.CvPtr, imgToDenoiseIndex, templateWindowSize, h, templateWindowSize, searchWindowSize);
		dst.Fix();
		GC.KeepAlive(srcImgs);
	}

	public static void FastNlMeansDenoisingMulti(IEnumerable<Mat> srcImgs, OutputArray dst, int imgToDenoiseIndex, int temporalWindowSize, float h = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
	{
		FastNlMeansDenoisingMulti(EnumerableEx.Select(srcImgs, (Mat m) => new InputArray(m)), dst, imgToDenoiseIndex, templateWindowSize, h, templateWindowSize, searchWindowSize);
	}

	public static void FastNlMeansDenoisingColoredMulti(IEnumerable<InputArray> srcImgs, OutputArray dst, int imgToDenoiseIndex, int temporalWindowSize, float h = 3f, float hColor = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
	{
		if (srcImgs == null)
		{
			throw new ArgumentNullException("srcImgs");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		dst.ThrowIfNotReady();
		IntPtr[] array = EnumerableEx.SelectPtrs(srcImgs);
		NativeMethods.photo_fastNlMeansDenoisingColoredMulti(array, array.Length, dst.CvPtr, imgToDenoiseIndex, templateWindowSize, h, hColor, templateWindowSize, searchWindowSize);
		dst.Fix();
		GC.KeepAlive(srcImgs);
	}

	public static void FastNlMeansDenoisingColoredMulti(IEnumerable<Mat> srcImgs, OutputArray dst, int imgToDenoiseIndex, int temporalWindowSize, float h = 3f, float hColor = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
	{
		FastNlMeansDenoisingColoredMulti(EnumerableEx.Select(srcImgs, (Mat m) => new InputArray(m)), dst, imgToDenoiseIndex, templateWindowSize, h, hColor, templateWindowSize, searchWindowSize);
	}

	public static void DenoiseTVL1(IEnumerable<Mat> observations, Mat result, double lambda = 1.0, int niters = 30)
	{
		if (observations == null)
		{
			throw new ArgumentNullException("observations");
		}
		if (result == null)
		{
			throw new ArgumentNullException("result");
		}
		IntPtr[] array = EnumerableEx.SelectPtrs(observations);
		NativeMethods.photo_denoise_TVL1(array, array.Length, result.CvPtr, lambda, niters);
		GC.KeepAlive(observations);
		GC.KeepAlive(result);
	}

	public static void Decolor(InputArray src, OutputArray grayscale, OutputArray colorBoost)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (grayscale == null)
		{
			throw new ArgumentNullException("grayscale");
		}
		if (colorBoost == null)
		{
			throw new ArgumentNullException("colorBoost");
		}
		src.ThrowIfDisposed();
		grayscale.ThrowIfNotReady();
		colorBoost.ThrowIfNotReady();
		NativeMethods.photo_decolor(src.CvPtr, grayscale.CvPtr, colorBoost.CvPtr);
		GC.KeepAlive(src);
		grayscale.Fix();
		colorBoost.Fix();
	}

	public static void SeamlessClone(InputArray src, InputArray dst, InputArray mask, Point p, OutputArray blend, SeamlessCloneMethods flags)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (blend == null)
		{
			throw new ArgumentNullException("blend");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfDisposed();
		mask?.ThrowIfDisposed();
		blend.ThrowIfNotReady();
		NativeMethods.photo_seamlessClone(src.CvPtr, dst.CvPtr, ToPtr(mask), p, blend.CvPtr, (int)flags);
		GC.KeepAlive(src);
		GC.KeepAlive(dst);
		GC.KeepAlive(mask);
		blend.Fix();
	}

	public static void ColorChange(InputArray src, InputArray mask, OutputArray dst, float redMul = 1f, float greenMul = 1f, float blueMul = 1f)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		mask?.ThrowIfDisposed();
		NativeMethods.photo_colorChange(src.CvPtr, ToPtr(mask), dst.CvPtr, redMul, greenMul, blueMul);
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void IlluminationChange(InputArray src, InputArray mask, OutputArray dst, float alpha = 0.2f, float beta = 0.4f)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		mask?.ThrowIfDisposed();
		NativeMethods.photo_illuminationChange(src.CvPtr, ToPtr(mask), dst.CvPtr, alpha, beta);
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void TextureFlattening(InputArray src, InputArray mask, OutputArray dst, float lowThreshold = 30f, float highThreshold = 45f, int kernelSize = 3)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		mask?.ThrowIfDisposed();
		NativeMethods.photo_textureFlattening(src.CvPtr, ToPtr(mask), dst.CvPtr, lowThreshold, highThreshold, kernelSize);
		GC.KeepAlive(src);
		GC.KeepAlive(mask);
		dst.Fix();
	}

	public static void EdgePreservingFilter(InputArray src, OutputArray dst, EdgePreservingMethods flags = EdgePreservingMethods.RecursFilter, float sigmaS = 60f, float sigmaR = 0.4f)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.photo_edgePreservingFilter(src.CvPtr, dst.CvPtr, (int)flags, sigmaS, sigmaR);
		GC.KeepAlive(src);
		dst.Fix();
	}

	public static void DetailEnhance(InputArray src, OutputArray dst, float sigmaS = 10f, float sigmaR = 0.15f)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.photo_detailEnhance(src.CvPtr, dst.CvPtr, sigmaS, sigmaR);
		GC.KeepAlive(src);
		dst.Fix();
	}

	public static void PencilSketch(InputArray src, OutputArray dst1, OutputArray dst2, float sigmaS = 60f, float sigmaR = 0.07f, float shadeFactor = 0.02f)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst1 == null)
		{
			throw new ArgumentNullException("dst1");
		}
		if (dst2 == null)
		{
			throw new ArgumentNullException("dst2");
		}
		src.ThrowIfDisposed();
		dst1.ThrowIfNotReady();
		dst2.ThrowIfNotReady();
		NativeMethods.photo_pencilSketch(src.CvPtr, dst1.CvPtr, dst2.CvPtr, sigmaS, sigmaR, shadeFactor);
		GC.KeepAlive(src);
		dst1.Fix();
		dst2.Fix();
	}

	public static void Stylization(InputArray src, OutputArray dst, float sigmaS = 60f, float sigmaR = 0.45f)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		src.ThrowIfDisposed();
		dst.ThrowIfNotReady();
		NativeMethods.photo_stylization(src.CvPtr, dst.CvPtr, sigmaS, sigmaR);
		GC.KeepAlive(src);
		dst.Fix();
	}

	public static void PutTextZh(InputOutputArray img, string text, Point org, int fontScale, Scalar color, string font = "Arial", bool italic = false, bool underline = false)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (string.IsNullOrEmpty(text))
		{
			throw new ArgumentNullException(text);
		}
		img.ThrowIfDisposed();
		NativeMethods.core_putTextZh(img.CvPtr, text, org, fontScale, color, font, italic ? 1 : 0, underline ? 1 : 0);
		img.Fix();
		GC.KeepAlive(img);
	}

	public static FrameSource CreateFrameSource_Empty()
	{
		return FrameSource.CreateFrameSource_Empty();
	}

	public static FrameSource CreateFrameSource_Video(string fileName)
	{
		return FrameSource.CreateFrameSource_Video(fileName);
	}

	public static FrameSource CreateFrameSource_Video_CUDA(string fileName)
	{
		return FrameSource.CreateFrameSource_Video_CUDA(fileName);
	}

	public static FrameSource CreateFrameSource_Camera(int deviceId)
	{
		return FrameSource.CreateFrameSource_Camera(deviceId);
	}

	public static SuperResolution CreateSuperResolution_BTVL1()
	{
		return SuperResolution.CreateBTVL1();
	}

	public static SuperResolution CreateSuperResolution_BTVL1_CUDA()
	{
		return SuperResolution.CreateBTVL1_CUDA();
	}

	public static DenseOpticalFlowExt CreateOptFlow_Farneback()
	{
		return DenseOpticalFlowExt.CreateFarneback();
	}

	public static DenseOpticalFlowExt CreateOptFlow_Farneback_GPU()
	{
		return DenseOpticalFlowExt.CreateFarneback_CUDA();
	}

	public static DenseOpticalFlowExt CreateOptFlow_DualTVL1_GPU()
	{
		return DenseOpticalFlowExt.CreateDualTVL1_CUDA();
	}

	public static DenseOpticalFlowExt CreateOptFlow_Brox_GPU()
	{
		return DenseOpticalFlowExt.CreateBrox_CUDA();
	}

	public static DenseOpticalFlowExt CreateOptFlow_PyrLK_GPU()
	{
		return DenseOpticalFlowExt.CreatePyrLK_CUDA();
	}

	public static RotatedRect CamShift(InputArray probImage, ref Rect window, TermCriteria criteria)
	{
		if (probImage == null)
		{
			throw new ArgumentNullException("probImage");
		}
		probImage.ThrowIfDisposed();
		RotatedRect result = NativeMethods.video_CamShift(probImage.CvPtr, ref window, criteria);
		GC.KeepAlive(probImage);
		return result;
	}

	public static int MeanShift(InputArray probImage, ref Rect window, TermCriteria criteria)
	{
		if (probImage == null)
		{
			throw new ArgumentNullException("probImage");
		}
		probImage.ThrowIfDisposed();
		int result = NativeMethods.video_meanShift(probImage.CvPtr, ref window, criteria);
		GC.KeepAlive(probImage);
		return result;
	}

	public static int BuildOpticalFlowPyramid(InputArray img, OutputArray pyramid, Size winSize, int maxLevel, bool withDerivatives = true, BorderTypes pyrBorder = BorderTypes.Reflect101, BorderTypes derivBorder = BorderTypes.Constant, bool tryReuseInputImage = true)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		if (pyramid == null)
		{
			throw new ArgumentNullException("pyramid");
		}
		img.ThrowIfDisposed();
		pyramid.ThrowIfNotReady();
		int result = NativeMethods.video_buildOpticalFlowPyramid1(img.CvPtr, pyramid.CvPtr, winSize, maxLevel, withDerivatives ? 1 : 0, (int)pyrBorder, (int)derivBorder, tryReuseInputImage ? 1 : 0);
		pyramid.Fix();
		GC.KeepAlive(img);
		return result;
	}

	public static int BuildOpticalFlowPyramid(InputArray img, out Mat[] pyramid, Size winSize, int maxLevel, bool withDerivatives = true, BorderTypes pyrBorder = BorderTypes.Reflect101, BorderTypes derivBorder = BorderTypes.Constant, bool tryReuseInputImage = true)
	{
		if (img == null)
		{
			throw new ArgumentNullException("img");
		}
		img.ThrowIfDisposed();
		using VectorOfMat vectorOfMat = new VectorOfMat();
		int result = NativeMethods.video_buildOpticalFlowPyramid2(img.CvPtr, vectorOfMat.CvPtr, winSize, maxLevel, withDerivatives ? 1 : 0, (int)pyrBorder, (int)derivBorder, tryReuseInputImage ? 1 : 0);
		GC.KeepAlive(img);
		pyramid = vectorOfMat.ToArray();
		return result;
	}

	public static void CalcOpticalFlowPyrLK(InputArray prevImg, InputArray nextImg, InputArray prevPts, InputOutputArray nextPts, OutputArray status, OutputArray err, Size? winSize = null, int maxLevel = 3, TermCriteria? criteria = null, OpticalFlowFlags flags = OpticalFlowFlags.None, double minEigThreshold = 0.0001)
	{
		if (prevImg == null)
		{
			throw new ArgumentNullException("prevImg");
		}
		if (nextImg == null)
		{
			throw new ArgumentNullException("nextImg");
		}
		if (prevPts == null)
		{
			throw new ArgumentNullException("prevPts");
		}
		if (nextPts == null)
		{
			throw new ArgumentNullException("nextPts");
		}
		if (status == null)
		{
			throw new ArgumentNullException("status");
		}
		if (err == null)
		{
			throw new ArgumentNullException("err");
		}
		prevImg.ThrowIfDisposed();
		nextImg.ThrowIfDisposed();
		prevPts.ThrowIfDisposed();
		nextPts.ThrowIfNotReady();
		status.ThrowIfNotReady();
		err.ThrowIfNotReady();
		Size valueOrDefault = winSize.GetValueOrDefault(new Size(21, 21));
		TermCriteria valueOrDefault2 = criteria.GetValueOrDefault(TermCriteria.Both(30, 0.01));
		NativeMethods.video_calcOpticalFlowPyrLK_InputArray(prevImg.CvPtr, nextImg.CvPtr, prevPts.CvPtr, nextPts.CvPtr, status.CvPtr, err.CvPtr, valueOrDefault, maxLevel, valueOrDefault2, (int)flags, minEigThreshold);
		GC.KeepAlive(prevImg);
		GC.KeepAlive(nextImg);
		GC.KeepAlive(prevPts);
		nextPts.Fix();
		status.Fix();
		err.Fix();
	}

	public static void CalcOpticalFlowPyrLK(InputArray prevImg, InputArray nextImg, Point2f[] prevPts, ref Point2f[] nextPts, out byte[] status, out float[] err, Size? winSize = null, int maxLevel = 3, TermCriteria? criteria = null, OpticalFlowFlags flags = OpticalFlowFlags.None, double minEigThreshold = 0.0001)
	{
		if (prevImg == null)
		{
			throw new ArgumentNullException("prevImg");
		}
		if (nextImg == null)
		{
			throw new ArgumentNullException("nextImg");
		}
		if (prevPts == null)
		{
			throw new ArgumentNullException("prevPts");
		}
		if (nextPts == null)
		{
			throw new ArgumentNullException("nextPts");
		}
		prevImg.ThrowIfDisposed();
		nextImg.ThrowIfDisposed();
		Size valueOrDefault = winSize.GetValueOrDefault(new Size(21, 21));
		TermCriteria valueOrDefault2 = criteria.GetValueOrDefault(TermCriteria.Both(30, 0.01));
		using VectorOfPoint2f vectorOfPoint2f = new VectorOfPoint2f();
		using VectorOfByte vectorOfByte = new VectorOfByte();
		using VectorOfFloat vectorOfFloat = new VectorOfFloat();
		NativeMethods.video_calcOpticalFlowPyrLK_vector(prevImg.CvPtr, nextImg.CvPtr, prevPts, prevPts.Length, vectorOfPoint2f.CvPtr, vectorOfByte.CvPtr, vectorOfFloat.CvPtr, valueOrDefault, maxLevel, valueOrDefault2, (int)flags, minEigThreshold);
		GC.KeepAlive(prevImg);
		GC.KeepAlive(nextImg);
		nextPts = vectorOfPoint2f.ToArray();
		status = vectorOfByte.ToArray();
		err = vectorOfFloat.ToArray();
	}

	public static void CalcOpticalFlowFarneback(InputArray prev, InputArray next, InputOutputArray flow, double pyrScale, int levels, int winsize, int iterations, int polyN, double polySigma, OpticalFlowFlags flags)
	{
		if (prev == null)
		{
			throw new ArgumentNullException("prev");
		}
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (flow == null)
		{
			throw new ArgumentNullException("flow");
		}
		prev.ThrowIfDisposed();
		next.ThrowIfDisposed();
		flow.ThrowIfNotReady();
		NativeMethods.video_calcOpticalFlowFarneback(prev.CvPtr, next.CvPtr, flow.CvPtr, pyrScale, levels, winsize, iterations, polyN, polySigma, (int)flags);
		GC.KeepAlive(prev);
		GC.KeepAlive(next);
		flow.Fix();
	}
}
