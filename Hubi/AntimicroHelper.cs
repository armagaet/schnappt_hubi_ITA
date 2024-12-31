using System;
using System.Diagnostics;

namespace Hubi.Helpers
{
    public class AntimicroHelper
    {
        private Process antimicroProcess;
		
		//avvio AntiMicro con la possibilità di riavviarlo con una nuova configurazione dei pulsanti -- NON FUNZIONA
		/*
		// Avvia AntiMicro con un file di configurazione specificato
        public void StartAntimicro(string antimicroPath, string configPath)
        {
            StopAntimicro(); // Interrompe qualsiasi processo esistente prima di avviarne uno nuovo

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = antimicroPath,
                Arguments = $"--profile \"{configPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true // Avvia in background senza mostrare una finestra
            };

            antimicroProcess = Process.Start(startInfo);
            Console.WriteLine($"AntiMicro avviato con il profilo: {configPath}");
        }

        // Interrompe AntiMicro se è in esecuzione
        public void StopAntimicro()
        {
            if (antimicroProcess != null && !antimicroProcess.HasExited)
            {
                antimicroProcess.Kill();
                antimicroProcess.WaitForExit();
                antimicroProcess = null;
                Console.WriteLine("AntiMicro interrotto.");
            }
        }
		*/
		
		
        public void StartAntimicro(string antimicroPath, string configPath)
        {
            if (antimicroProcess != null && !antimicroProcess.HasExited)
            {
                Console.WriteLine("AntiMicro è già in esecuzione.");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = antimicroPath,
                Arguments = $"--hidden --profile \"{configPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true // Impedisce l'apertura di una nuova finestra della console
            };

            try
            {
                antimicroProcess = Process.Start(startInfo);
                Console.WriteLine("AntiMicro avviato in background.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'avvio di AntiMicro: {ex.Message}");
            }
        }

		//scritta così a quanto pare una volta terminato antiMicro non riesco più a riavviarlo con una nuova configurazione...anche se mi pareva che prima funzionasse.
        /*
		public void StopAntimicro()
        {
            if (antimicroProcess != null && !antimicroProcess.HasExited)
            {
                antimicroProcess.Kill();
                antimicroProcess.Dispose();
                Console.WriteLine("AntiMicro terminato.");
            }
        }
		*/
		
		//modifico così:
		public void StopAntimicro()
        {
            if (antimicroProcess != null)
            {
                try
                {
                    if (!antimicroProcess.HasExited)
                    {
                        antimicroProcess.Kill();
                        Console.WriteLine("AntiMicro terminato.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante la chiusura di AntiMicro: {ex.Message}");
                }
                finally
                {
                    antimicroProcess.Dispose();
                    antimicroProcess = null; // Imposta il riferimento a null
                }
            }
        }
    }
}


//prima versione del file con avvio AntiMicro NON in background
/*
namespace Hubi
{
    public static class AntimicroHelper
    {
        public static void LaunchAntimicroWithProfile(string antimicroPath, string configPath)
        {
            // Verifica che i file esistano
            if (!System.IO.File.Exists(antimicroPath))
            {
                Console.WriteLine("Antimicro non trovato.");
                return;
            }
            if (!System.IO.File.Exists(configPath))
            {
                Console.WriteLine("File di configurazione non trovato.");
                return;
            }

            // Configura il processo
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = antimicroPath,
                Arguments = $"--profile \"{configPath}\"", // Passa il file di configurazione come argomento
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true // Evita di creare una nuova finestra
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    // Legge l'output standard e gli errori
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Mostra l'output
                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine("Output:\n" + output);
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Errori:\n" + error);
                    }

                    Console.WriteLine("Antimicro terminato.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'avvio di Antimicro: {ex.Message}");
            }
        }
    }
}
*/