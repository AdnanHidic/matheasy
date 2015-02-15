using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO;

using MathEasy.src.Editing.Controls;
using MathEasy.src.LMD.Core;
using MathEasy.src.NetworkManagement;

namespace MathEasy.src.FileManagement
{
    public static class DocumentManager
    {
        private static FlowDocument lastRegistered;
        private static String fullPath;
        private static String name;

        public static void manage(FlowDocument flowDoc)
        {
            lastRegistered = cloneFlowDocument(flowDoc);
            fullPath = "";
        }

        private static FlowDocument cloneFlowDocument(FlowDocument from)
        {
            FlowDocument to = new FlowDocument();
            String fromSerialized = XamlWriter.Save(from);
            to = XamlReader.Parse(fromSerialized) as FlowDocument;
            return to;
        }

        private static Boolean areSameFlowDocuments(FlowDocument flowDoc1, FlowDocument flowDoc2)
        {
            if (flowDoc1 == null && flowDoc2 == null)
                return true;

            if (flowDoc1 == null || flowDoc2 == null)
                return false;

            String firstArgSerialized= XamlWriter.Save(flowDoc1);
            String secondArgSerialized = XamlWriter.Save(flowDoc2);

            return firstArgSerialized == secondArgSerialized;
        }

        public static String newDocumentProcedure(FlowDocument current)
        {
            if (!areSameFlowDocuments(current, lastRegistered))
            {
                Boolean shouldContinue = handleExistingDocument(current);
                if (!shouldContinue)
                    return "";
            }
            current.Blocks.Clear();
            lastRegistered = cloneFlowDocument(current);
            fullPath = "";
            return "untitled";
        }

        public static String saveProcedure(FlowDocument current)
        {
            if (fullPath == "")
            {
                return saveAsProcedure(current);
            }
            else
            {
                try
                {
                    String documentString = MathBox.getDocumentUni(current);
                    DataStream ds = new DataStream(documentString, DataStream.Type.TXT);
                    FileManager.WriteDataToFile(ds, fullPath);
                    lastRegistered = cloneFlowDocument(current);
                    return name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
                }
            }
        }

        public static String loadProcedure(FlowDocument oldDocument)
        {
            if (!areSameFlowDocuments(lastRegistered, oldDocument))
            {
                Boolean shouldContinue = handleExistingDocument(oldDocument);
                if (!shouldContinue)
                    return "";
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "TXT (editable)|*.txt";

            Nullable<bool> result = ofd.ShowDialog();
            if (result != true)
                return "";

            try
            {
                String content = FileManager.ReadDataFromFile(ofd.FileName).Content;
                fullPath = ofd.FileName;
                name = ofd.SafeFileName;
                oldDocument.Blocks.Clear();
                oldDocument.SetDocumentWithNewlines(content);
                lastRegistered = cloneFlowDocument(oldDocument);
                return name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            
        }

        /// <returns>Whether the current action should continue</returns>
        public static Boolean handleExistingDocument(FlowDocument newDocument)
        {
            if (!areSameFlowDocuments(lastRegistered, newDocument))
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the current document?", "MathEasy", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return true;
                else if (result == MessageBoxResult.Cancel)
                    return false;
                else
                {
                    if (saveAsProcedure(newDocument) == "")
                        return false;
                    return true;
                }
            }
            else
                return true;
        }

        /// <returns>Whether the current action should continue</returns>
        public static String saveAsProcedure(FlowDocument document)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "Document"; 
                sfd.DefaultExt = ".txt"; 
                sfd.Filter = "TXT (editable)|*.txt|HTML (read-only)|*.html"; 

                Nullable<bool> result = sfd.ShowDialog();
                if (result != true) 
                    return "";

                DataStream ds = null;
                int br = sfd.FilterIndex;

                String documentString = MathBox.getDocumentUni(document);

                if (br == 1)
                {
                    ds = new DataStream(documentString, DataStream.Type.TXT);
                    lastRegistered = cloneFlowDocument(document);
                    fullPath = sfd.FileName;
                    name = sfd.SafeFileName;
                }
                else
                {
                    documentString = new LMD_Document(documentString).toHTML(LMD_Document.RenderingMode.WEB);
                    ds = new DataStream(documentString, DataStream.Type.HTML);
                }

                FileManager.WriteDataToFile(ds, sfd.FileName);
                
                return name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }

        public static Boolean downloadProcedure(String aCode, FlowDocument toBeFilled)
        {
            try
            {
                if (!areSameFlowDocuments(lastRegistered, toBeFilled))
                {
                    Boolean shouldContinue = handleExistingDocument(toBeFilled);
                    if (!shouldContinue)
                        return false;
                }

                String result = NetworkManager.downloadData(aCode);

                toBeFilled.SetDocumentWithNewlines(result);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static String uploadProcedure(FlowDocument toBeUploaded)
        {
            try
            {
                String ptext = MathBox.getDocumentUni(toBeUploaded);

                String htmlBody = new LMD_Document(ptext).getHTMLBody();

                return NetworkManager.uploadData(ptext, htmlBody);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }
    }
}
