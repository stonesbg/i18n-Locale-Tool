//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Notice: Use of the service proxies that accompany this notice is subject to
//            the terms and conditions of the license agreement located at
//            http://go.microsoft.com/fwlink/?LinkID=202740&clcid=0x409
//            If you do not agree to these terms you may not use this content.
namespace Microsoft {
    using System;
    using System.Data.Services.Client;


    public partial class Translation {
        
        private String _Text;
        
        public String Text {
            get {
                return this._Text;
            }
            set {
                this._Text = value;
            }
        }
    }
    
    public partial class Language {
        
        private String _Code;
        
        public String Code {
            get {
                return this._Code;
            }
            set {
                this._Code = value;
            }
        }
    }
    
    public partial class DetectedLanguage {
        
        private String _Code;
        
        public String Code {
            get {
                return this._Code;
            }
            set {
                this._Code = value;
            }
        }
    }
    
    public partial class TranslatorContainer : System.Data.Services.Client.DataServiceContext {
        
        public TranslatorContainer(Uri serviceRoot) : 
                base(serviceRoot) {
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Text">the text to translate Sample Values : hello</param>
        /// <param name="To">the language code to translate the text into Sample Values : nl</param>
        /// <param name="From">the language code of the translation text Sample Values : en</param>
        public DataServiceQuery<Translation> Translate(String Text, String To, String From) {
            if ((Text == null)) {
                throw new System.ArgumentNullException("Text", "Text value cannot be null");
            }
            if ((To == null)) {
                throw new System.ArgumentNullException("To", "To value cannot be null");
            }
            DataServiceQuery<Translation> query;
            query = base.CreateQuery<Translation>("Translate");
            if ((Text != null)) {
                query = query.AddQueryOption("Text", string.Concat("\'", System.Uri.EscapeDataString(Text), "\'"));
            }
            if ((To != null)) {
                query = query.AddQueryOption("To", string.Concat("\'", System.Uri.EscapeDataString(To), "\'"));
            }
            if ((From != null)) {
                query = query.AddQueryOption("From", string.Concat("\'", System.Uri.EscapeDataString(From), "\'"));
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        public DataServiceQuery<Language> GetLanguagesForTranslation() {
            DataServiceQuery<Language> query;
            query = base.CreateQuery<Language>("GetLanguagesForTranslation");
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Text">the text whose language is to be identified Sample Values : hello</param>
        public DataServiceQuery<DetectedLanguage> Detect(String Text) {
            if ((Text == null)) {
                throw new System.ArgumentNullException("Text", "Text value cannot be null");
            }
            DataServiceQuery<DetectedLanguage> query;
            query = base.CreateQuery<DetectedLanguage>("Detect");
            if ((Text != null)) {
                query = query.AddQueryOption("Text", string.Concat("\'", System.Uri.EscapeDataString(Text), "\'"));
            }
            return query;
        }
    }
}
