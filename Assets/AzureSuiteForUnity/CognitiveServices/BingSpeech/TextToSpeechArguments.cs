using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace AzureSuiteForUnity.CognitiveServices.BingSpeech
{
    public class TextToSpeechHeaders
    {
        public const string X_MICROSOFT_OUTPUTFORMAT = "X-Microsoft-OutputFormat";
        public const string X_SEARCH_APP_ID = "X-Search-AppId";
        public const string X_SEARCH_CLIENT_ID = "X-Search-ClientID";
        //public const string USER_AGENT = "User-Agent";

        public static string EnumToHyphenString(Enum e)
        {
            return e.ToString().Replace('_', '-');
        }

        public static Dictionary<string, string> CreateHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add(X_SEARCH_APP_ID, Guid.NewGuid().ToString());
            headers.Add(X_SEARCH_CLIENT_ID, Guid.NewGuid().ToString());

            return headers;
        }
    }

    public class TextToSpeechParameters
    {
        public struct Param
        {
            public const string VOICE_TYPE = "VoiceType";
            public const string VOICE_NAME = "VoiceName";
            public const string LOCALE = "Locale";
            public const string OUTPUT_FORMAT = "OutputFormat";
            public const string REQUEST_URI = "RequestUri";
            public const string AUTHORIZATION_TOKEN = "AuthorizationToken";
            public const string TEXT = "Text";
        }

        private VoiceType _voiceType;
        private string _locale;
        private OutputFormatParameterValues _outputFormat;
        private string _requestUri;
        private string _authToken;
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        public TextToSpeechParameters(VoiceType voiceType, OutputFormatParameterValues outputFormat,
            string locale, string requestUri, string authToken, string text)
        {
            _voiceType = voiceType;
            _outputFormat = outputFormat;
            _locale = locale;
            _requestUri = requestUri;
            _authToken = authToken;
            _text = text;
        }

        public static string EnumToHyphenString(Enum e)
        {
            return e.ToString().Replace('_', '-');
        }

        public TextToSpeechParameters(VoiceType voiceType, OutputFormatParameterValues outputFormat,
            Locale locale, string requestUri, string authToken, string text)
        {
            _voiceType = voiceType;
            _outputFormat = outputFormat;
            _locale = EnumToHyphenString(locale);
            _requestUri = requestUri;
            _authToken = authToken;
            _text = text;
        }

        public string ToQueryString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Param.VOICE_TYPE);
            sb.Append("={0}&");
            sb.Append(Param.OUTPUT_FORMAT);
            sb.Append("={1}&");
            sb.Append(Param.LOCALE);
            sb.Append("={2}&");
            sb.Append(Param.AUTHORIZATION_TOKEN);
            sb.Append("={3}&");
            sb.Append(Param.TEXT);
            sb.Append("={4}");

            var paramFormat = sb.ToString();
            var paramUrlString = String.Format(paramFormat,
                WWW.EscapeURL(_voiceType.ToString()),
                WWW.EscapeURL(_outputFormat.ToString()),
                WWW.EscapeURL(_locale),
                WWW.EscapeURL(_authToken),
                WWW.EscapeURL(_text)
                );

            return paramUrlString;
        }

        public enum VoiceType
        {
            Female,
            Male
        }

        /// <summary>
        /// Supported locales for easier reference, however
        /// can be replaced by 
        /// </summary>
        public enum Locale
        {
            ar_EG,
            ar_SA,
            ca_ES,
            cs_CZ,
            da_DK,
            de_AT,
            de_CH,
            de_DE,
            el_GR,
            en_AU,
            en_CA,
            en_GB,
            en_IE,
            en_IN,
            en_US,
            es_ES,
            es_MX,
            fi_FI,
            fr_CA,
            fr_CH,
            fr_FR,
            he_IL,
            hi_IN,
            hu_HU,
            id_ID,
            it_IT,
            ja_JP,
            ko_KR,
            nb_NO,
            nl_NL,
            pl_PL,
            pt_BR,
            pt_PT,
            ro_RO,
            ru_RU,
            sk_SK,
            sv_SE,
            th_TH,
            tr_TR,
            zh_CN,
            zh_HK,
            zh_TW
        }

        /// <summary>
        /// Use the defualts for now as there is a fairly long list
        /// and for most countries it's just 1 voice per locale/voiceType combo
        /// anyway
        /// </summary>
        public enum VoiceName
        {
            None
        }
    }

    public enum OutputFormatParameterValues
    {
        SSML16Khz16BitMonoTTS,
        Raw16khz16bitMonoPCM,
        Audio16khz16kbpsMonoSiren,
        Riff16khz16kbpsMonoSiren,
        Riff16khz16bitMonoPcm,
        Audio16khz128kbitrateMonoMp3,
        Audio16khz64kbitrateMonoMp3,
        Audio16khz32kbitrateMonoMp3
    }

    public enum XMicrosoftOutputFormatHeaderValues
    {
        ssml_16khz_16bit_mono_tts,
        raw_16khz_16bit_mono_pcm,
        audio_16khz_16kbps_mono_siren,
        riff_16khz_16kbps_mono_siren,
        riff_16khz_16bit_mono_pcm,
        audio_16khz_128kbitrate_mono_mp3,
        audio_16khz_64kbitrate_mono_mp3,
        audio_16khz_32kbitrate_mono_mp3
    }
}
