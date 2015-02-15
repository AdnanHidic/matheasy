using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathEasy.src.FileManagement;
using System.Diagnostics;

namespace MathEasy.src.ConfigurationManagement
{
    public static class ConfigurationManager
    {
        public static void loadConfiguration()
        {
            if (!loadConfigurationFromConfigurationFile())
            {
                alertUser("Configuration file not present or corrupted.\nLoading default settings.");
                loadDefaultConfiguration();
            }
        }

        public static void loadDefaultConfiguration()
        {
            URL = @"localhost/matheasy";
            Symbols = new Dictionary<string,string>
            {
                {"π", "pi"},
                {"α", "alpha"},
                {"β", "beta"},
                {"Γ", "gamma_capital"},
                {"γ", "gamma"},
                {"Δ", "delta_capital"},
                {"δ", "delta"},
                {"ε", "epsilon"},
                {"Θ", "theta_capital"},
                {"θ", "theta"},
                {"κ", "kappa"},
                {"λ", "lambda"},
                {"μ", "my"},
                {"ρ", "rho"},
                {"Σ", "sigma_capital"},
                {"Φ", "phi_capital"},
                {"φ", "phi"},
                {"ψ", "psi"},
                {"Ω", "omega_capital"},
                {"ω", "omega"},
                {"·", "dot"},
                {"→", "right"},
                {"←", "left"},
                {"↑", "up"},
                {"↓", "down"},
                {"↔", "leftright"},
                {"↕", "updown"},
                {"⇒", "implies"},
                {"∞", "infinity"}
            };

            try
            {
                String data = buildStringToSave();
                DataStream ds = new DataStream(data, DataStream.Type.TXT);
                FileManager.WriteDataToFile(ds, "matheasy.config", "");
            }
            catch (Exception)
            {
                alertUser("Saving default settings to editable file failed.\nCheck your permissions.");
            }
        }

        private static String buildStringToSave()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(URL);

            foreach (KeyValuePair<String, String> keyValuePair in Symbols)
            {
                sb.AppendLine(keyValuePair.Key + " " + keyValuePair.Value);
            }

            return sb.ToString();
        }

        public static Boolean loadConfigurationFromConfigurationFile(String fullPath = "matheasy.config.txt")
        {
            try
            {
                DataStream ds = FileManager.ReadDataFromFile(fullPath);
                String configDataFromFile = ds.Content;
                String[] linesFromFile = configDataFromFile.Split(new String[] { "\n", Environment.NewLine, "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (linesFromFile.Length == 0)
                    return false;

                URL = linesFromFile[0];

                for (int i = 1; i < linesFromFile.Length; i++)
                {
                    String[] lineContent = linesFromFile[i].Split(' ');
                    if (lineContent.Length == 2)
                        Symbols.Add(lineContent[0], lineContent[1]);
                    else
                        return false;

                }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        private static void alertUser(String message)
        {
            System.Windows.MessageBox.Show(message, "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }

        public static Dictionary<String, String> Symbols = new Dictionary<String, String>();

        public static String URL;
    }
}
