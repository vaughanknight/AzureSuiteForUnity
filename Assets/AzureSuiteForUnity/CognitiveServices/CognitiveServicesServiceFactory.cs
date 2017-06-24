using AzureSuiteForUnity.CognitiveServices;
using AzureSuiteForUnity.CognitiveServices.BingSpeech;
using AzureSuiteForUnity.CognitiveServices.ComputerVision;
using AzureSuiteForUnity.CognitiveServices.Emotion;
using AzureSuiteForUnity.CognitiveServices.Face;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace AzureSuiteForUnity.CognitiveServices
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IComputerVisionAPI>()
                                    .ImplementedBy<ComputerVisionAPI>()
                                    //.Interceptors<LoggingInterceptor>()
                                    .LifeStyle.Transient,
                               Component.For<IEmotionAPI>()
                                    .ImplementedBy<EmotionAPI>()
                                    //.Interceptors<LoggingInterceptor>()
                                    .LifeStyle.Transient,
                               Component.For<IFaceAPI>()
                                    .ImplementedBy<FaceAPI>()
                                    //.Interceptors<LoggingInterceptor>()
                                    .LifeStyle.Transient,
                               Component.For<MonoBehaviour>()
                                    .Instance(CognitiveServicesServiceFactory.Instance),
                               Component.For<IBingSpeechAPI>()
                                    .ImplementedBy<BingSpeechAPI>()
                                    .LifeStyle.Transient,
                               Component.For<LoggingInterceptor>().LifeStyle.Transient);
        }
    }

    public class CognitiveServicesServiceFactory : SelfInstantiatingSingletonBehaviour<CognitiveServicesServiceFactory>
    {
        private WindsorContainer container;
        
        public new void Awake()
        {
            base.Awake();
            container = new WindsorContainer();
            container.Install(FromAssembly.This());
        }

        public IBingSpeechAPI GetBingSpeechAPI(string APIKey)
        {
            //ProxyGenerator generator = new ProxyGenerator();
            //BingSpeechAPI service = null;// new BingSpeechAPIService();
                                                //IService proxyService = generator.CreateInterfaceProxyWithTarget<IService>(service, new KeyInterceptor());
                                                //IBingSpeechAPIService proxyService = generator.CreateInterfaceProxyWithTarget<IBingSpeechAPIService>(service, new LoggingInterceptor());

            var api = container.Resolve<IBingSpeechAPI>();
            api.APIKey = APIKey;

            return api;
        }

        public IComputerVisionAPI GetComputerVisionAPI(string APIKey)
        {
            var api = container.Resolve<IComputerVisionAPI>();
            api.APIKey = APIKey;
            return api;
        }

        public IFaceAPI GetFaceAPI(string APIKey)
        {
            var api = container.Resolve<IFaceAPI>();
            api.APIKey = APIKey;
            return api;
        }

        public IEmotionAPI GetEmotionAPI(string APIKey)
        {
            var api = container.Resolve<IEmotionAPI>();
            api.APIKey = APIKey;
            return api;
        }
    }
}
