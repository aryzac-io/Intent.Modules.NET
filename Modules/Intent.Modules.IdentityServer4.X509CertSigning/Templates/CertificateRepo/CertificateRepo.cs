// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.IdentityServer4.X509CertSigning.Templates.CertificateRepo
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.IdentityServer4.X509CertSigning\Templates\CertificateRepo\CertificateRepo.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class CertificateRepo : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.IO;\r\nusing System.Security.Cryptography.X509Certifica" +
                    "tes;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules\Modules\Intent.Modules.IdentityServer4.X509CertSigning\Templates\CertificateRepo\CertificateRepo.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// Convenient way to obtain X509 Certificates from v" +
                    "arious sources\r\n    /// </summary>\r\n    static class ");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.IdentityServer4.X509CertSigning\Templates\CertificateRepo\CertificateRepo.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        public static X509Certificate2 GetFromCertificateStore(\r\n       " +
                    "     string findType,\r\n            string findValue,\r\n            StoreName stor" +
                    "eName = StoreName.My,\r\n            StoreLocation storeLocation = StoreLocation.L" +
                    "ocalMachine)\r\n        {\r\n            X509FindType parsedFindType;\r\n            i" +
                    "f (!Enum.TryParse(findType, out parsedFindType))\r\n            {\r\n               " +
                    " throw new ArgumentException($\"Could not parse string \'{findType}\' for type \'{na" +
                    "meof(X509FindType)}\'\", nameof(findType));\r\n            }\r\n\r\n            return G" +
                    "etFromCertificateStore(\r\n                findValue: findValue,\r\n                " +
                    "findType: parsedFindType,\r\n                storeName: storeName,\r\n              " +
                    "  storeLocation: storeLocation);\r\n        }\r\n\r\n        public static X509Certifi" +
                    "cate2 GetFromCertificateStore(\r\n            X509FindType findType,\r\n            " +
                    "string findValue,\r\n            StoreName storeName = StoreName.My,\r\n            " +
                    "StoreLocation storeLocation = StoreLocation.LocalMachine)\r\n        {\r\n          " +
                    "  var store = new X509Store(storeName, storeLocation);\r\n            store.Open(O" +
                    "penFlags.ReadOnly);\r\n\r\n            var certs = store.Certificates.Find(findType," +
                    " findValue, true);\r\n            store.Close();\r\n\r\n            if (certs.Count ==" +
                    " 0)\r\n            {\r\n                throw new CertificateStoreException(\r\n      " +
                    "              message: \"Could not find any matching certificate\",\r\n             " +
                    "       findType: findType,\r\n                    findValue: findValue,\r\n         " +
                    "           storeName: storeName,\r\n                    storeLocation: storeLocati" +
                    "on);\r\n            }\r\n\r\n            if (certs.Count > 1)\r\n            {\r\n        " +
                    "        throw new CertificateStoreException(\r\n                    message: \"Foun" +
                    "d more than one matching certificate\",\r\n                    findType: findType,\r" +
                    "\n                    findValue: findValue,\r\n                    storeName: store" +
                    "Name,\r\n                    storeLocation: storeLocation);\r\n            }\r\n\r\n    " +
                    "        return certs[0];\r\n        }\r\n\r\n        public static X509Certificate2 Ge" +
                    "tEmbeddedCertificate(string resourceName, string password = null)\r\n        {\r\n  " +
                    "          var assembly = typeof(");
            
            #line 79 "C:\Dev\Intent.Modules\Modules\Intent.Modules.IdentityServer4.X509CertSigning\Templates\CertificateRepo\CertificateRepo.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(").Assembly;\r\n            using (var stream = assembly.GetManifestResourceStream(r" +
                    "esourceName))\r\n            {\r\n                return new X509Certificate2(ReadSt" +
                    "ream(stream), password);\r\n            }\r\n        }\r\n\r\n        public static X509" +
                    "Certificate2 GetFromFile(string fileName, string password = null)\r\n        {\r\n  " +
                    "          return new X509Certificate2(fileName, password);\r\n        }\r\n\r\n       " +
                    " private static byte[] ReadStream(Stream input)\r\n        {\r\n            byte[] b" +
                    "uffer = new byte[16 * 1024];\r\n            using (MemoryStream ms = new MemoryStr" +
                    "eam())\r\n            {\r\n                int read;\r\n                while ((read =" +
                    " input.Read(buffer, 0, buffer.Length)) > 0)\r\n                {\r\n                " +
                    "    ms.Write(buffer, 0, read);\r\n                }\r\n                return ms.ToA" +
                    "rray();\r\n            }\r\n        }\r\n    }\r\n\r\n    public class CertificateStoreExc" +
                    "eption : Exception\r\n    {\r\n        public CertificateStoreException() : base()\r\n" +
                    "        {\r\n        }\r\n\r\n        public CertificateStoreException(string message)" +
                    " : base(message)\r\n        {\r\n        }\r\n\r\n        public CertificateStoreExcepti" +
                    "on(string message, Exception innerException) : base(message, innerException)\r\n  " +
                    "      {\r\n        }\r\n\r\n        public CertificateStoreException(\r\n            str" +
                    "ing message,\r\n            X509FindType findType,\r\n            string findValue,\r" +
                    "\n            StoreName storeName,\r\n            StoreLocation storeLocation)\r\n   " +
                    "         : base(message)\r\n        {\r\n            FindType = findType;\r\n         " +
                    "   FindValue = findValue;\r\n            StoreName = storeName;\r\n            Store" +
                    "Location = storeLocation;\r\n        }\r\n\r\n        public X509FindType FindType { g" +
                    "et; }\r\n        public string FindValue { get; }\r\n        public StoreName StoreN" +
                    "ame { get; }\r\n        public StoreLocation StoreLocation { get; }\r\n\r\n        pub" +
                    "lic override string ToString()\r\n        {\r\n            if (string.IsNullOrEmpty(" +
                    "FindValue))\r\n            {\r\n                return base.ToString();\r\n           " +
                    " }\r\n\r\n            return $\"{Message}{Environment.NewLine}\" +\r\n                $\"" +
                    "{nameof(FindType)} = {FindType}{Environment.NewLine}\" +\r\n                $\"{name" +
                    "of(FindValue)} = {FindValue}{Environment.NewLine}\" +\r\n                $\"{nameof(" +
                    "StoreName)} = {StoreName}{Environment.NewLine}\" +\r\n                $\"{nameof(Sto" +
                    "reLocation)} = {StoreLocation}{Environment.NewLine}\" +\r\n                StackTra" +
                    "ce;\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
