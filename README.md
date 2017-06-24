# Azure Suite for Unity
This project was once known as Azure Cognitive Services for Unity, however many othe Azure capabilities overlap, so it is now known as Azure Suite for Unity.

This is a suite of code I've been using to plug Unity into Azure and Azure Cognitive Services.  The aim is to have a full suite of services, firstly accessible through code, but secondly accessible in a more Unity/Beavhiour driven way.

Some of this is simple such as sending text to services, others are more complex, such as sending images, audio, or video, and having them encoded the right way.

I've also put a [Azure Cognitive Services and Unity](https://vaughanknight.com/2017/04/29/azure-cognitive-services-and-unity/) post up on the blog.  I plan on putting most of the verbage over there, and migrate the final stuff back to here.

## The Aim
The aim of this project is two fold:
* Editor Support - Allow developers to leverage Azure Cognitive Services in their tool chain, bringing AI to game development.
* In Game Support - Allow develoeprs to leverage Azure Cognitive Serviecs easily in their games.  In some cases near zero-code integration if possible.  
* For both - Simply import the same package.

None of the code is presuming to be the 'best way'.  So if you know a better way, feel free to contribute.  Azure and Azure Cognitive Services has had new features at a rate that if it continues, I'm going to need help keeping this up to date.

## In Progress
Right now the focus is on Cognitive Services.  Some features implemented incldue:
* FaceAPI
    * Detect 
* VisionAPI
    * Describe
* BingSpeechAPI
    * Audio to Speech

## Additional features
The other aim isn't just to create a client for the Azure services, but to do some of the auto converstion and capture.
* Face API 
    * Detect faces in the current screenshot
    * Work with Texture2D
* VisionAPI
    * Desribe the current screenshot
    * Work with Texture2D
* Bing Speech API
    * Record audio to recognise
    *  Automatic AudioClip to PCM conversion

## Final Notes
There is a high chance nothing will ever be 100% complete due to the dynamic nature of the services and new features being released all the time.  If you need something that's not there, add it, and submit a pull request. 
