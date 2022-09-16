using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
namespace MergeHelper
{
    public class Helper : IHelper
    {
        public void MergeDocument(string newFilePath, IList<string> documentFiles)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(newFilePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
            }
            //appending all documents into one.
            AppendDocuments(newFilePath, documentFiles);
        }

        private void AppendDocuments(string newFilePath, IList<string> documentFiles)
        {
            for (int i = 0; i < documentFiles.Count; i++)
            {
                using WordprocessingDocument wpd = WordprocessingDocument.Open(newFilePath, true);
                MainDocumentPart mdp = wpd.MainDocumentPart;
                string altChunkId = $"AltChunkId{GetRandomId(10)}";
                AlternativeFormatImportPart chunk = mdp.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                using (FileStream fileStream = File.Open(documentFiles[i], FileMode.Open))
                {
                    chunk.FeedData(fileStream);
                }
                AltChunk altChunk = new AltChunk
                {
                    Id = altChunkId
                };
                mdp.Document.Body.InsertAfter(altChunk, mdp.Document.Body.Elements<Paragraph>().Last());
                AppendPageBreak(wpd);
                mdp.Document.Save();
                wpd.Close();
            }
        }
        public static string GetRandomId(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static void AppendAltChunk(WordprocessingDocument doc, string altChunkId, string html)
        {
            MainDocumentPart mainPart = doc.MainDocumentPart;
            AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart("application/xhtml+xml", altChunkId);
            using (Stream chunkStream = chunk.GetStream(FileMode.Create, FileAccess.Write))
            using (StreamWriter stringStream = new StreamWriter(chunkStream))
                stringStream.Write(html);

            AltChunk altChunk = new AltChunk();
            altChunk.Id = altChunkId;
            OpenXmlElement last = doc.MainDocumentPart.Document
                .Body
                .Elements()
                .LastOrDefault(e => e is Paragraph || e is AltChunk);
            if (last == null)
                doc.MainDocumentPart.Document.Body.InsertAt(altChunk, 0);
            else
                last.InsertAfterSelf(altChunk);
        }

        static void AppendPageBreak(WordprocessingDocument myDoc)
        {
            MainDocumentPart mainPart = myDoc.MainDocumentPart;
            OpenXmlElement last = myDoc.MainDocumentPart.Document
                .Body
                .Elements()
                .LastOrDefault(e => e is Paragraph || e is AltChunk);
            last.InsertAfterSelf(new Paragraph(
                new Run(
                    new Break() { Type = BreakValues.Page })));
        }
    }
}
