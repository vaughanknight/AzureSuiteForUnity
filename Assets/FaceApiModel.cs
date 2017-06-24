using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace WarpImage.FaceModel
//{

//    public class FaceRectangle
//    {
//        public int Top { get; set; }
//        public int Left { get; set; }
//        public int Width { get; set; }
//        public int Height { get; set; }
//    }

//    public class HeadPose
//    {
//        public float Pitch { get; set; }
//        public float Roll { get; set; }
//        public float Yaw { get; set; }
//    }

//    public class FacialHair
//    {
//        public float Moustache { get; set; }
//        public float Beard { get; set; }
//        public float Sideburns { get; set; }
//    }

//    public class Emotion
//    {
//        public float Anger { get; set; }
//        public float Contempt { get; set; }
//        public float Disgust { get; set; }
//        public float Fear { get; set; }
//        public float Happiness { get; set; }
//        public float Neutral { get; set; }
//        public float Sadness { get; set; }
//        public float Surprise { get; set; }
//    }

//    public class FaceAttributes
//    {
//        public float Smile { get; set; }
//        public HeadPose HeadPose { get; set; }
//        public string Gender { get; set; }
//        public float Age { get; set; }
//        public FacialHair FacialHair { get; set; }
//        public string Glasses { get; set; }
//        public Emotion Emotion { get; set; }
//    }

//    public class FaceLandmark
//    {
//        public float X { get; set; }
//        public float Y { get; set; }

//        public FaceLandmark() { }
//        public FaceLandmark(int x, int y)
//        {
//            X = x;
//            Y = y;
//        }

//        public Point ToPoint()
//        {
//            return new Point(X, Y);
//        }
//    }

//    public class PupilLeft : FaceLandmark 
//    {
       
//    }

//    public class PupilRight : FaceLandmark
//    {

//    }

//    public class NoseTip : FaceLandmark
//    {

//    }

//    public class MouthLeft : FaceLandmark
//    {

//    }

//    public class MouthRight : FaceLandmark
//    {

//    }

//    public class EyebrowLeftOuter : FaceLandmark
//    {

//    }

//    public class EyebrowLeftInner : FaceLandmark
//    {

//    }

//    public class EyeLeftOuter : FaceLandmark
//    {

//    }
//    public class EyeLeftTop : FaceLandmark
//    {

//    }

//    public class EyeLeftBottom : FaceLandmark
//    {

//    }

//    public class EyeLeftInner : FaceLandmark
//    {

//    }

//    public class EyebrowRightInner : FaceLandmark
//    {

//    }

//    public class EyebrowRightOuter : FaceLandmark
//    {

//    }

//    public class EyeRightInner : FaceLandmark
//    {

//    }

//    public class EyeRightTop : FaceLandmark
//    {

//    }

//    public class EyeRightBottom : FaceLandmark
//    {

//    }

//    public class EyeRightOuter : FaceLandmark
//    {

//    }

//    public class NoseRootLeft : FaceLandmark
//    {

//    }
//    public class NoseRootRight : FaceLandmark
//    {

//    }

//    public class NoseLeftAlarTop : FaceLandmark
//    {

//    }

//    public class NoseRightAlarTop : FaceLandmark
//    {

//    }

//    public class NoseLeftAlarOutTip : FaceLandmark
//    {

//    }

//    public class NoseRightAlarOutTip : FaceLandmark
//    {

//    }

//    public class UpperLipTop : FaceLandmark
//    {

//    }

//    public class UpperLipBottom : FaceLandmark
//    {

//    }

//    public class UnderLipTop : FaceLandmark
//    {

//    }

//    public class UnderLipBottom : FaceLandmark
//    {

//    }

//    public class FaceLandmarks
//    {
//        public List<FaceLandmark> FaceLandmarkList()
//        {
//            var list = new List<FaceLandmark>()
//            {
//                PupilLeft, PupilRight,
//                NoseTip, MouthLeft, MouthRight,
//                EyebrowLeftOuter, EyebrowLeftInner,
//                EyeLeftOuter, EyeLeftTop, 
//                EyeLeftBottom, EyeLeftInner,
//                EyebrowRightOuter, EyebrowRightInner,
//                EyeRightOuter, EyeRightTop,
//                EyeRightBottom, EyeRightInner,
//                NoseLeftAlarOutTip, NoseLeftAlarTop, NoseRightAlarOutTip, NoseRightAlarTop,
//                NoseRootLeft, NoseRootRight, NoseTip,
//                UpperLipBottom, UpperLipTop,
//                UnderLipBottom, UnderLipTop
//            };

//            return list;
//        }

//        public PupilLeft PupilLeft { get; set; }
//        public PupilRight PupilRight { get; set; }
//        public NoseTip NoseTip { get; set; }
//        public MouthLeft MouthLeft { get; set; }
//        public MouthRight MouthRight { get; set; }
//        public EyebrowLeftOuter EyebrowLeftOuter { get; set; }
//        public EyebrowLeftInner EyebrowLeftInner { get; set; }
//        public EyeLeftOuter EyeLeftOuter { get; set; }
//        public EyeLeftTop EyeLeftTop { get; set; }
//        public EyeLeftBottom EyeLeftBottom { get; set; }
//        public EyeLeftInner EyeLeftInner { get; set; }
//        public EyebrowRightInner EyebrowRightInner { get; set; }
//        public EyebrowRightOuter EyebrowRightOuter { get; set; }
//        public EyeRightInner EyeRightInner { get; set; }
//        public EyeRightTop EyeRightTop { get; set; }
//        public EyeRightBottom EyeRightBottom { get; set; }
//        public EyeRightOuter EyeRightOuter { get; set; }

//        public NoseRootLeft NoseRootLeft { get; set; }
//        public NoseRootRight NoseRootRight { get; set; }
//        public NoseLeftAlarTop NoseLeftAlarTop { get; set; }
//        public NoseRightAlarTop NoseRightAlarTop { get; set; }
//        public NoseLeftAlarOutTip NoseLeftAlarOutTip { get; set; }
//        public NoseRightAlarOutTip NoseRightAlarOutTip { get; set; }
//        public UpperLipTop UpperLipTop { get; set; }
//        public UpperLipBottom UpperLipBottom { get; set; }
//        public UnderLipTop UnderLipTop { get; set; }
//        public UnderLipBottom UnderLipBottom { get; set; }
//    }

//    public class RootObject
//    {
//        public string FaceId { get; set; }
//        public FaceRectangle FaceRectangle { get; set; }
//        public FaceAttributes FaceAttributes { get; set; }
//        public FaceLandmarks FaceLandmarks { get; set; }
//    }

//}
