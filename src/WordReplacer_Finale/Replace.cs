using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Text.RegularExpressions;
namespace WordReplacer_Finale
{
    class Replace
    {

        WordprocessingDocument word_file;
         
        string filePath;


        public Replace(string file)
        {
            filePath = file;

        }

        public void ReplaceWord(string change_text, string replacement, RegexOptions regexOptions)
        {
            string pattern = @"\b" + change_text + @"\b";//replacment pattren(\b will replace the whole word)

            word_file = WordprocessingDocument.Open(filePath, true);//Open the file

            //text = Regex.Replace(text, pattern, replacement);
            StreamReader sr = new StreamReader(word_file.MainDocumentPart.GetStream());//Read the text of the file
            string file_text = sr.ReadToEnd();//Insert the text to a variable
            sr.Close();//close stream

            file_text = Regex.Replace(file_text, pattern, replacement, regexOptions);
            

            StreamWriter sw = new StreamWriter(word_file.MainDocumentPart.GetStream(FileMode.Create));

            sw.Write(file_text);//write the new text(after replacing)

            sw.Close();



            try
            {
                word_file.MainDocumentPart.Document.Save();//save file
            }
            catch (System.Xml.XmlException)
            {

            }
            word_file.Close();


        }







    }
}
