using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hubi
{
    public class MediaPlayer
    {
        public List<string> queue;

        public MediaPlayer()
        {
            queue = new List<string>();
        }

        public void AddSound(string sound)
        {
            queue.Add(sound);
        }


        public void Play(int sleepTime=0, string tipo_media = "audio") //aggiungo una seconda tipologia di media
        {
            string queueStr="";
            if (queue.Any())
            {
                foreach (var q in queue)
                {
					if (tipo_media=="video")
					{
						queueStr += $"video\\{q}.mp4 ";
					}
					else {
                    queueStr += $"sound\\{q}.m4a ";
					}
                }
            }
            else
                return;

            using (var process = new Process())
            {
                //process.StartInfo.FileName = @"%PROGRAMFILES%\VideoLAN\VLC\vlc.exe"; // absolute path.
				//process.StartInfo.FileName = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe"; // absolute path.
				process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES(x86)%\VideoLAN\VLC\vlc.exe");
				if (tipo_media=="video")
				{
					//per i video provo a lanciare vlc normalmente
					process.StartInfo.Arguments = $"--play-and-exit --fullscreen --qt-notification=0 --no-loop {queueStr}";
				}
				else {
                    process.StartInfo.Arguments = $"--qt-start-minimized --play-and-exit --qt-notification=0 --no-loop {queueStr}";
				}
				Console.WriteLine($"Valore di queueStr: {queueStr}");
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;

                process.Start();
            }
            Thread.Sleep(sleepTime * 1000);

        }

        public void Clear()
        {
            queue.Clear();
        }

        public void Stop()
        {
            using (var process = new Process())
            {
                //process.StartInfo.FileName = @"taskkill.exe"; // relative path. absolute path works too.
				process.StartInfo.FileName = @"C:\Windows\System32\taskkill.exe"; // Percorso completo
                process.StartInfo.Arguments = "/IM vlc.exe /F";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;

                process.Start();
            }
        }
		//provo a generalizzare la funzione per stoppare la riproduzione di un suono...difficile, non funziona da errore
		/*
		public void PlayAndMonitor(string soundFile)
		{
        StartPlayback(soundFile); // Avvia la riproduzione
        Console.WriteLine($"Riproduzione di {soundFile}. Premi 's' per fermare.");
        // Monitora il processo audio e consente di interromperlo
        Task.Run(() =>
        {
            while (isPlaying)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    if (key == ConsoleKey.S)
                    {
                        StopPlayback();
                        Console.WriteLine($"Riproduzione di {soundFile} interrotta manualmente.");
                        break;
                    }
                }
                Thread.Sleep(100); // Riduce il carico della CPU
            }
        });
        // Attendi che il file audio finisca la riproduzione
        while (isPlaying)
        {
            Thread.Sleep(500); // Controlla periodicamente lo stato della riproduzione
        }
        Console.WriteLine($"Riproduzione di {soundFile} completata.");
		}
		//funzioni per interrompere il suono
		private static void StartPlayback(string audioFileName)
        {
			// Controlla che il nome del file non sia nullo o vuoto
			if (string.IsNullOrEmpty(audioFileName))
			{
				Console.WriteLine("Nome file audio non valido.");
				return;
			}
			
            isPlaying = true;
            playbackThread = new Thread(() =>
            {
                try
                {
                    media.AddSound(audioFileName);
                    media.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante la riproduzione: {ex.Message}");
                }
                finally
                {
                    isPlaying = false; // Resetta il flag quando la riproduzione termina
                }
            });
            playbackThread.Start();
        }
        private static void StopPlayback()
        {
            if (playbackThread != null && playbackThread.IsAlive)
            {
                // Aggiungi qui la logica per interrompere il MediaPlayer
                media.Stop(); // Assicurati che MediaPlayer abbia un metodo Stop
                playbackThread.Join(); // Aspetta che il thread termini
            }
            isPlaying = false;
            Console.WriteLine("Riproduzione interrotta.");
        }*/
    }
}
