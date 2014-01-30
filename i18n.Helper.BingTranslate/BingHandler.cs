using Microsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace i18n.Helper.BingTranslate
{
    public class BingHandler
    {
        TranslatorContainer _tc = null;
        public BingHandler()
        {
            TranslatorContainer _tc = InitializeTranslator();
        }

        public string Translate(string inputString, string sourceLanguage, string targetLanguage)
        {
            return TranslateString(_tc, inputString, sourceLanguage, targetLanguage).Text;
        }

        private Translation TranslateString(TranslatorContainer tc, string inputString, string sourceLanguage, string targetLanguage)
        {
            var translationQuery = tc.Translate(inputString, targetLanguage, sourceLanguage);

            var translationResults = translationQuery.Execute().ToList();

            if (translationResults.Count() <= 0)
            {
                return null;
            }

            var translationResult = translationResults.First();

            return translationResult;
        }

        public TranslatorContainer InitializeTranslator()
        {
            var serviceRootUri = new Uri("https://api.datamarket.azure.com/Bing/MicrosoftTranslator/");

            var accountKey = "BING_ACCOUNT_KEY";

            var tc = new TranslatorContainer(serviceRootUri);

            tc.Credentials = new NetworkCredential(accountKey, accountKey);
            return tc;
        }
    }
}
