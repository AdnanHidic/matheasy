using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

using MathEasy.src.FileManagement;
using System.IO;
using MathEasy.src.LMD.Core;

namespace MathEasy.src.Rendering
{
    public partial class PreviewBox : UserControl
    {
        #region code for silencing IE

        private const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private const int SET_FEATURE_ON_THREAD = 0x00000001;
        private const int SET_FEATURE_ON_PROCESS = 0x00000002;
        private const int SET_FEATURE_IN_REGISTRY = 0x00000004;
        private const int SET_FEATURE_ON_THREAD_LOCALMACHINE = 0x00000008;
        private const int SET_FEATURE_ON_THREAD_INTRANET = 0x00000010;
        private const int SET_FEATURE_ON_THREAD_TRUSTED = 0x00000020;
        private const int SET_FEATURE_ON_THREAD_INTERNET = 0x00000040;
        private const int SET_FEATURE_ON_THREAD_RESTRICTED = 0x00000080;

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(
        int FeatureEntry,
        [MarshalAs(UnmanagedType.U4)] int dwFlags,
        bool fEnable);

        #endregion

        private String tmpFileName = "workfile";

        public PreviewBox()
        {
            InitializeComponent();

            // silencing IE clicks
            CoInternetSetFeatureEnabled(FEATURE_DISABLE_NAVIGATION_SOUNDS, SET_FEATURE_ON_PROCESS, true);
        }

        public void loadHTML(String htmlString)
        {
            if (!File.Exists(LMD_Document.localMathJaxPath)
                || !File.Exists(LMD_Document.localExCanvasPath)
                || !File.Exists(LMD_Document.localJqPlotCssPath)
                || !File.Exists(LMD_Document.localJqPlotPath)
                || !File.Exists(LMD_Document.localJqueryPath))
            {
                throw new Exception("Dependencies needed for rendering are missing!");
            }
            try
            {
                DataStream tempFile = new DataStream(htmlString,DataStream.Type.HTML);
                FileManager.WriteDataToFile(tempFile, tmpFileName, System.IO.Path.GetTempPath());
                //System.IO.Path.GetTempPath()
                webBrowser.Navigate(new Uri(System.IO.Path.GetTempPath() + tmpFileName + ".html"));
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while loading document for preview.\nError message: "+ex.Message);
            }
        }
    }
}
