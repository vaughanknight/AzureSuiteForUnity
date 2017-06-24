using AzureSuiteForUnity.CognitiveServices.Face;
using AzureSuiteForUnity.CognitiveServices.Face.Model;
using mattatz.Triangulation2DSystem;
using Newtonsoft.Json;
using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestWarpImage : MonoBehaviour
{
    public Texture2D sourceImage;
    public Texture2D warpToImage;
    public Texture2D finalImage;
    public GameObject target;
    //public string sourceImageFace;
    //public string warpToFace;

    private DetectResponse.RootObject sourceFace;
    private DetectResponse.RootObject warpToFace;


    void Start()
    {
        MergeFaces();
    }
    
    public void MergeFaces()
    {
        Debug.Log(FaceAPIManager.Instance);
        FaceAPIManager.Instance.OnDetect += OnDetectSource;
        sourceFace = null;
        warpToFace = null;
        FaceAPIManager.Instance.Detect(sourceImage);
        FaceAPIManager.Instance.Detect(warpToImage);
    }

    private void OnDetectSource(object sender, FaceAPIEventArgs args)
    {
        if(args.OriginalImage == sourceImage)
        {
            sourceFace = args.FacesDetected[0];
        }
        else
        {
            warpToFace = args.FacesDetected[0];
        }
        
        // If we have both face dimensions, let's warp them!
        if(sourceFace != null && warpToFace != null)
        {
            WarpFaces();
        }
    }

    private void WarpFaces()
    {
        // Create the image matrices
        Mat imgSrc = CreateSourceImageMat();
        Mat imgDest = CreateDestImageMat(imgSrc);

        // Generate the face data (in this case, load the parameters)
        //var sourceFace = JsonConvert.DeserializeObject<RootObject>(sourceImageFace);
        //var warpToFace = JsonConvert.DeserializeObject<RootObject>(this.warpToFace);

        // Create the landmark point lists
        var pointList1 = new List<Point>();
        var pointList2 = new List<Point>();
        AddMatchingLandmarks(sourceFace.faceLandmarks, warpToFace.faceLandmarks, pointList1, pointList2);

        if (pointList1.Count != pointList2.Count)
        {
            Debug.LogErrorFormat("Counts are not equal.  Lists ahve {0} and {1} elements.", pointList1.Count, pointList2.Count);
        }

        // Declare the int source width/height
        int sw = (int)imgSrc.size().width;
        int sh = (int)imgSrc.size().height;
        int dw = warpToImage.width;
        int dh = warpToImage.height;

        // Create a subdivision for the source image
        var sourceSubDiv = new Subdiv2D(new OpenCVForUnity.Rect(0, 0, sw, sh));

        // Add border points to the subdivision
        // and add all the other points
        Add8PointBorder(sourceSubDiv, sw, sh);
        foreach (var p in pointList1)
        {
            sourceSubDiv.insert(p);
        }

        // Create a list of triangles
        // from the subdivision
        MatOfFloat6 sourceTriMat = new MatOfFloat6();
        sourceSubDiv.getTriangleList(sourceTriMat);
        var sourceTriList = new List<float>(sourceTriMat.toArray());

        // OpenCV for Unity subdiv has a bug and we need to ensure 
        // every triangle is inside the bounds of the source
        CapToMax(sourceTriList, sw - 1, sh - 1);

        // Copy it
        var destTriList = new List<float>();
        foreach (float f in sourceTriList) destTriList.Add(f);

        // Find the points from the source, and 
        // move them to the destination this way 
        // the triangles correlate to each other
        MovePointsInList(sourceFace.faceLandmarks.FaceLandmarkList(), warpToFace.faceLandmarks.FaceLandmarkList(), destTriList);
        MovePointsInList(Get8PointBorder(sw, sh), Get8PointBorder(dw, dh), destTriList);

        // Now we have source triangles and positions
        // and destination triangles and positions
        // warp the source triangles ot the destination
        WarpImages(imgSrc, imgDest, sourceTriList, destTriList);

        // Everything is warped, so now create the texture from
        // the matrix
        Texture2D texture = MatToTexture2D(imgDest);

        // Assign the texture
        target.GetComponent<Renderer>().material.mainTexture = texture;
    }

    /// <summary>
    /// Converts an image Mat to a Texture2D.
    /// </summary>
    /// <param name="imgDest">Mat that is an image</param>
    /// <returns></returns>
    private static Texture2D MatToTexture2D(Mat imgDest)
    {
        Texture2D texture = new Texture2D((int)imgDest.size().width, (int)imgDest.size().height);
        var matOfByte = new MatOfByte();
        Imgcodecs.imencode(".png", imgDest, matOfByte);
        texture.LoadImage(matOfByte.toArray());
        return texture;
    }


    #region Manual Test 

    //var a1 = new Point(0, 0);
    //var b1 = new Point(499, 0);
    //var c1 = new Point(499, 499);
    //var d1 = new Point(0, 499);
    //var e1 = new Point(250, 250);
    //var e2 = new Point(300, 250);

    //var pointList1 = new List<Point>() { a1, b1, c1, d1, e1 };
    //var pointList2 = new List<Point>() { a1, b1, c1, d1, e2 };

    //Subdiv2D subman = new Subdiv2D(new OpenCVForUnity.Rect(0, 0, 500, 500));
    //foreach (var p in pointList1) subman.insert(p);

    //subman.getTriangleList(tri1);

    //triList1 = new List<float>(tri1.toArray());
    //CapToMax(triList1, 0, 499);

    //// Copy it
    //triList2 = new List<float>();
    //foreach (float f in triList1) triList2.Add(f);

    //MovePointsInList(pointList1, pointList2, triList2);

    #endregion

    /// <summary>
    /// Creates the destination Mat based on the 
    /// source images type, and the target images size
    /// </summary>
    /// <param name="imgSrc">Image source mat</param>
    /// <returns></returns>
    private Mat CreateDestImageMat(Mat imgSrc)
    {
        var destImageSize = new Size(warpToImage.width, warpToImage.height);
        Mat imgDest = Mat.zeros(destImageSize, imgSrc.type());
        return imgDest;
    }

    /// <summary>
    /// Creates the source image mat from the 
    /// source texture.  Assumes RGB 8 bit per channel.
    /// </summary>
    /// <returns></returns>
    private Mat CreateSourceImageMat()
    {
        var sourceBytes = sourceImage.EncodeToJPG();
        Mat rawData = new Mat(1, sourceBytes.Length, CvType.CV_8UC3);
        rawData.put(0, 0, sourceBytes);
        var imgSrc = OpenCVForUnity.Imgcodecs.imdecode(rawData, 1);
        return imgSrc;
    }

    /// <summary>
    /// Warps the source image, into the destination image
    /// using the source triangles, warping to the destination
    /// triangles.
    /// </summary>
    /// <param name="imgSrc"></param>
    /// <param name="imgDest"></param>
    /// <param name="triListSrc"></param>
    /// <param name="triListDest"></param>
    private void WarpImages(Mat imgSrc, Mat imgDest, List<float> triListSrc, List<float> triListDest)
    {
        for (var i = 0; i < triListSrc.Count; i += 6)
        {
            var point1 = new Point(triListSrc[i], triListSrc[i + 1]);
            var point2 = new Point(triListSrc[i + 2], triListSrc[i + 3]);
            var point3 = new Point(triListSrc[i + 4], triListSrc[i + 5]);
            var pointArray1 = new Point[] { point1, point2, point3 };
            var mat1 = new MatOfPoint(pointArray1);

            var point4 = new Point(triListDest[i], triListDest[i + 1]);
            var point5 = new Point(triListDest[i + 2], triListDest[i + 3]);
            var point6 = new Point(triListDest[i + 4], triListDest[i + 5]);
            var pointArray2 = new Point[] { point4, point5, point6 };
            var mat2 = new MatOfPoint(pointArray2);

            WarpTriangle(imgSrc, imgDest, mat1, mat2);
        }
    }

    private void MovePointsInList(List<DetectResponse.FaceLandmark> landList1, List<DetectResponse.FaceLandmark> landList2, List<float> triList2)
    {
        for(var i = 0; i < landList1.Count(); i++)
        {
            var lm1 = landList1[i];
            var lm2 = landList2[i];

            UpdateCoordinates(lm1.X, lm2.X, lm1.Y, lm2.Y, triList2);
        }
    }

    private void MovePointsInList(List<Point> pointList1, List<Point> pointList2, List<float> triList2)
    {
        for (var i = 0; i < pointList1.Count(); i++)
        {
            var lm1 = pointList1[i];
            var lm2 = pointList2[i];

            UpdateCoordinates((float)lm1.x, (float)lm2.x, (float)lm1.y, (float)lm2.y, triList2);
        }
    }


    private void UpdateCoordinates(float oldX, float newX, float oldY, float newY, List<float> targetList)
    {
        for (var i = 0; i < targetList.Count(); i += 2)
        {
            var x = targetList[i];
            var y = targetList[i + 1];

            var xabs = Math.Abs(x - oldX);
            var yabs = Math.Abs(y - oldY);


            //if (x == oldX && y == oldY)
            if(xabs < 0.01f && yabs < 0.01f)
            {
                targetList[i] = newX;
                targetList[i+1] = newY;
            }
        }
    }


    private static void AddAll(Triangle2D[] triangles, List<float> trilist)
    {
        foreach (var t in triangles)
        {
            trilist.Add(t.a.Coordinate.x);
            trilist.Add(t.a.Coordinate.y);
            trilist.Add(t.b.Coordinate.x);
            trilist.Add(t.b.Coordinate.y);
            trilist.Add(t.c.Coordinate.x);
            trilist.Add(t.c.Coordinate.y);
        }
    }


    private static void CapToMax(List<float> l, int maxX, int maxY)
    {
        for (var i = 0; i < l.Count; i+=2)
        {
            l[i] = Mathf.Min(l[i], maxX);
            l[i] = Mathf.Max(l[i], 0);
            
            l[i + 1] = Mathf.Min(l[i + 1], maxY);
            l[i + 1] = Mathf.Max(l[i + 1], 0);
        }
    }

    private static List<Point> Get8PointBorder(int width, int height)
    {
        var h2 = height / 2;
        var h = height - 1;
        var w2 = width / 2;
        var w = width - 1;

        var list = new List<Point>() {
            new Point(0, 0),
            new Point(0, h2),
            new Point(0, h),
            new Point(w2, h),
            new Point(w, h),
            new Point(w, h2),
            new Point(w, 0),
            new Point(w2, 0)
        };

        return list;
    }
    private static void Add8PointBorder(Subdiv2D s, int width, int height)
    {
        var list = Get8PointBorder(width, height);

        foreach(var p in list)
        {
            s.insert(p);
        }
    }

    public void WarpTriangle(Mat imgSrc, Mat imgDest, MatOfPoint triPointsSource, MatOfPoint triPointsDest)
    {
        
        // These are the points (from p1 to p2) 
        var triDestArray = triPointsDest.toArray();
        var triSourceArray = triPointsSource.toArray();
        
        // This is the bounding rects (from r1 to r)
        var boundingRectDestination = Imgproc.boundingRect(triPointsDest);
        var boundingRectSource = Imgproc.boundingRect(triPointsSource);
        
        // Offset points by left top corner of the respective rectangles
        // r1 and t1, r and t
        var triSourceOffset = new List<Point>();
        var triDestOffset = new List<Point>();
        var triDestOffsetInt = new List<Point>();

        for (int i = 0; i < 3; i++)
        {
            triDestOffset.Add(new Point(triDestArray[i].x - boundingRectDestination.x, triDestArray[i].y - boundingRectDestination.y));
            triDestOffsetInt.Add(new Point(Math.Round(triDestArray[i].x - boundingRectDestination.x), Math.Round(triDestArray[i].y - boundingRectDestination.y))); // for fillConvexPoly
            triSourceOffset.Add(new Point(triSourceArray[i].x - boundingRectSource.x, triSourceArray[i].y - boundingRectSource.y));
        }

        var matTriDestOffsetInt = new MatOfPoint(triDestOffsetInt.ToArray());

        // Get mask by filling triangle
        Mat mask = Mat.zeros(boundingRectDestination.height, boundingRectDestination.width, CvType.CV_8UC1);
        Imgproc.fillConvexPoly(mask, matTriDestOffsetInt, new Scalar(1, 1, 1));

        // Image rect
        Mat img1Rect = new Mat(imgSrc, boundingRectSource);
        
        // Target image is in r/destination triangle bounds
        Mat warpImage1 = Mat.zeros(boundingRectDestination.height, boundingRectDestination.width, img1Rect.type());
        
        //Do the affine transform warp
        applyAffineTransform(warpImage1, img1Rect, new MatOfPoint2f(triSourceOffset.ToArray()), new MatOfPoint2f(triDestOffset.ToArray()));

        // Copy triangular region of the rectangular patch to the output image
        Core.multiply(warpImage1, mask, warpImage1);
        var sub = imgDest.submat(boundingRectDestination);
        warpImage1.copyTo(sub, mask);
    }

    void applyAffineTransform(Mat destination, Mat source, MatOfPoint2f srcTri, MatOfPoint2f dstTri)
    {
        // Given a pair of triangles, find the affine transform.
        Mat warpMat = Imgproc.getAffineTransform(srcTri, dstTri);

        // Apply the Affine Transform just found to the src image
        Imgproc.warpAffine(source, destination, warpMat, destination.size(), Imgproc.INTER_LINEAR, Core.BORDER_REFLECT_101, new Scalar(1));
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void AddMatchingLandmarks(DetectResponse.FaceLandmarks l1, DetectResponse.FaceLandmarks l2, List<Point> p1, List<Point> p2)
    {
        var landList1 = l1.FaceLandmarkList();
        var landList2 = l2.FaceLandmarkList();

        for(var i = 0; i < landList1.Count(); i++)
        {
           

            var lm1 = landList1[i];
            var lm2 = landList2[i];

            AddFeatureIfExists(lm1, lm2, p1, p2);
        }
    }

    public void AddFeatureIfExists(DetectResponse.FaceLandmark feature1, DetectResponse.FaceLandmark feature2, List<Point> p1, List<Point> p2)
    {
        if (feature1 != null && feature2 != null)
        {
            p1.Add(feature1.ToPoint());
            p2.Add(feature2.ToPoint());
        }
    }


}
