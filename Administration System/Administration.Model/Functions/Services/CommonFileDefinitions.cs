using System.IO;
using System.Reflection;
using File = Model.Types.File;
using IOFile = System.IO.File;

namespace Model.Functions.Services
{
    public static class CommonFileDefinitions
    {

        internal static List<File> defs = new List<File>
        {         
            CreateNewDef(LangId.Python,ContentType.Wrapper,  "00A71062-B69E-4BED-B3ED-BCC21CDA3BBF", "Default_wrapper.txt"),
            CreateNewDef( LangId.Python,ContentType.Wrapper, "616A0BF0-507D-48DE-ADC6-9B7C683748A0","QA_wrapper.txt"),
            CreateNewDef(LangId.Python,ContentType.Helpers,  "9403985E-92C1-4939-92E1-9C39406C486D","Default_helpers.txt"),
            CreateNewDef(LangId.Python,ContentType.RegExRules, "C21A3A82-0C7C-4933-8E22-E199A9C91AD8","Functions_regex.json"),

            CreateNewDef(LangId.CSharp,ContentType.Wrapper,  "4EC89C87-9DC5-4E0C-BD6D-49473A3F827C","Default_wrapper.txt"),
            CreateNewDef(LangId.CSharp, ContentType.Wrapper, "5C68C4A1-FCD0-4F25-9E45-19DB2D3EA247", "QA_wrapper.txt"),
            CreateNewDef(LangId.CSharp, ContentType.Helpers, "73136A41-156F-404C-B0EC-E84B874D08A2","Default_helpers.txt"),
            CreateNewDef(LangId.CSharp,ContentType.RegExRules,  "8D0309A9-CCF4-487E-8FC4-5CDCA3C0ABE4", "Functions_regex.json"),
            
            CreateNewDef( LangId.VB, ContentType.Wrapper, "E4A35362-818D-4790-B3D9-605A6A88A590","Default_helpers.txt"),
            CreateNewDef( LangId.VB,ContentType.Wrapper, "00A049BF-66A9-4E69-B087-24C662C2A863","QA_wrapper.txt"),
            CreateNewDef(LangId.VB,ContentType.Helpers,  "CA064B27-0EFD-4ADB-BD49-D784241EF60E","Default_helpers.txt"),
            CreateNewDef( LangId.VB,ContentType.RegExRules, "584B498E-5781-476E-A95B-08D880D2E7BC","Functions_regex.json"),
                        
            CreateNewDef(LangId.Other,ContentType.RegExRules,  "EA6DDBEC-4B8B-4C3B-B464-54A939E4F1A4","QA_regex.json"),

            CreateNewDef(LangId.Haskell,ContentType.Wrapper,  "D0FBC0FF-B889-45EF-8E8F-165A8F98CDC3","Default_wrapper.txt"),
            CreateNewDef(LangId.Haskell,ContentType.Helpers , "E5159AAF-16A5-4423-9664-22C2DCE3E51C","Default_helpers.txt"),

            CreateNewDef(LangId.PythonTyped,ContentType.RegExRules,  "A8DD28BF-C583-43FF-AF7F-106A830DFDAB","Typed_Functions_regex.json"),
        };

        private static File CreateNewDef(LangId langId, ContentType contentType, string guidStr, string fileName) =>
          new File()
          {
              Name = fileName,
              ContentType = contentType,
              LanguageId = ((int)langId).ToString(),
              Content = IOFile.ReadAllBytes($"{PathToCommonFiles(langId)}{fileName}"),
              AuthorId = 1, //Root author
              UniqueRef = new Guid(guidStr)
          };

        public static string PathToCommonFiles(LangId langId)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var dirs = currentDir.Split('\\');
            var i = Array.IndexOf(dirs, "Administration System");
            return $"{string.Join('\\', dirs[..i])}\\CommonFiles\\{LangDir(langId)}\\";
        }

        private static string LangDir(LangId langId) => langId switch
        {
            LangId.ZERO_DONT_USE => throw new NotImplementedException(),
            LangId.Python => "Common_Files_PY",
            LangId.CSharp => "Common_Files_CS",
            LangId.VB => "Common_Files_VB",
            LangId.Java => throw new NotImplementedException(),
            LangId.ARM => throw new NotImplementedException(),
            LangId.Other => "Common_Files_Other",
            LangId.Haskell => "Common_Files_HS",
            LangId.PythonTyped => "Common_Files_PY",
            _ => throw new NotImplementedException(),
        };
    }
}

