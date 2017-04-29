using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class VisionAPI : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(MakeRequestBinary());
        }
    }

   
    public string SUBSCRIPTION_KEY = "d0584ebec5524eafbf5f2d8ca4fc0ae5";
    public Texture2D Image;

    

    IEnumerator MakeRequest()
    {
        //var baseUrl = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze?";
        //var queryString = "visualFeatures={0}&language={1}";
        // &details={1}
        //queryString = string.Format(queryString, "Categories", "en");
        
        var baseUrl = "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?";
        var queryString = "maxCandidates=10";
        var requestUrl = baseUrl + queryString;

        Debug.LogFormat("Request URL {0}.", requestUrl);

        var imageUrl = "http://www.strel-swimming.com/media/articles/1_45/s1a45i995sz5.jpg";
        var body = "{url: \""+imageUrl+"\"}";

        Debug.LogFormat("Body {0}.", body);

        var data = Encoding.ASCII.GetBytes(body.ToCharArray());

        Dictionary<string, string> Headers = new Dictionary<string, string>();
        Headers.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);
        Headers.Add("Content-Type", "application/json");
        var client = new WWW(requestUrl, data, Headers);

        yield return client;

        Debug.LogFormat("Response {0}.", client.text);

        var v = JsonUtility.FromJson<VisionApiResponse>(client.text);
        //var v = JsonConvert.DeserializeObject<VisionApiResponse>(client.text);
        Debug.Log(v.categories[0].name);
    }

    public Texture2D texture;

    IEnumerator MakeRequestBinary()
    {
            //Wait for graphics to render
        yield return new WaitForEndOfFrame();

        //Create a texture to pass to encoding
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        //Put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        //Split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
        yield return 0;

        var data = texture.EncodeToPNG();

        var baseUrl = "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?";
        var queryString = "maxCandidates=10";
        var requestUrl = baseUrl + queryString;

        //var baseUrl = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze?";
        //var queryString = "visualFeatures={0}&language={1}";
        //// &details={1}
        //queryString = string.Format(queryString, "Categories", "en");
        //var requestUrl = baseUrl + queryString;

        Debug.LogFormat("Request URL {0}.", requestUrl);

        //var imageUrl = "http://www.strel-swimming.com/media/articles/1_45/s1a45i995sz5.jpg";
        //var data = Image.EncodeToPNG();

        //var data = body;// Encoding.ASCII.GetBytes(body.ToCharArray());

        Dictionary<string, string> Headers = new Dictionary<string, string>();
        Headers.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);
        Headers.Add("Content-Type", "application/octet-stream");
        var client = new WWW(requestUrl, data, Headers);

        yield return client;

        Debug.LogFormat("Response {0}.", client.text);
        
        //var v = JsonUtility.FromJson<VisionApiResponse>(client.text);
        ////var v = JsonConvert.DeserializeObject<VisionApiResponse>(client.text);
        //Debug.Log(v.categories[0].name);
    }

    [Serializable]
    public class VisionApiResponse
    {
        public Category[] categories; 
    }

    [Serializable]
    public class Category
    {
        public string name;
        public float score;
    }
}
