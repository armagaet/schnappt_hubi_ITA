using System;
using System.IO;
//aggiungo questi moduli nella speranza di poter lanciare da subito la classe Media
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hubi.Helpers; //per avvio AntiMicro
using NAudio.Wave; //nel caso volessi usare questa libreria per riprodurre file audio senza richiamare vlc esterno

namespace Hubi
{
    class Program
    {
		//provo a far leggere i Media gia' a questo livello:
		private static MediaPlayer media; // Istanza del MediaPlayer
        private static bool isPlaying = false; // Flag per il controllo della riproduzione
        private static Thread playbackThread; // Thread per la riproduzione audio
			
        static void Main(string[] args)
        {

            Console.WriteLine(@"

                            Schnappt

         ('-. .-. #             #.-. .-')   #          #
        ( OO )  / #             #\  ( OO )  #          #
        ,--. ,--. # ,--. ,--.   # ;-----.\  #  ,-.-')  #
        |  | |  | # |  | |  |   # | .-.  |  #  |  |OO) #
        |   .|  | # |  | | .-') # | '-' /_) #  |  |  \ #
        |       | # |  |_|( OO )# | .-. `.  #  |  |(_/ #
        |  .-.  | # |  | | `-' /# | |  \  | # ,|  |_.' #
        |  | |  | #('  '-'(_.-' # | '--'  / #(_|  |    #
        `--' `--' #  `-----'    # `------'  #  `--'    #

");
            var cheat = args.Length == 1 && args[0] == "-c";

            if(cheat)
            {
                Console.WriteLine("--- Cheat actived! --- \n");    
            }

			/*********** AntiMicro JOYSTICK to KEYBOARD ***********/
			//Visto che usando i pulsanti come joystick vien fuori un macello, avvio AntiMicro che rimappa i pulsanti del joystick come tasti della tastiera:
			// Crea un'istanza di AntimicroHelper
            var antimicroHelper = new AntimicroHelper();
			string antimicroPath = @"C:\Program Files\AntiMicro\antimicro.exe";
            string configPath1 = $"mappatura_joystick_tastiera-123.gamecontroller.amgp";
			string configPath2 = $"mappatura_joystick_tastiera-rgby.gamecontroller.amgp";
			string configPath3 = $"mappatura_joystick_tastiera-frecce.gamecontroller.amgp";
			// Verifica che il file di configurazione esista
            if (!System.IO.File.Exists(configPath1))
            {
                Console.WriteLine($"Il file di configurazione '{configPath1}' non è stato trovato nella directory di lavoro.");
                return;
            }
            //AntimicroHelper.LaunchAntimicroWithProfile(antimicroPath, configPath1); //primo metodo con AntiMicro NON in background
			// Avvia AntiMicro in background
            antimicroHelper.StartAntimicro(antimicroPath, configPath1);
            //Console.WriteLine("Programma principale in esecuzione. Premi un tasto per terminare...");
            //Console.ReadKey();
			/*********** AntiMicro JOYSTICK to KEYBOARD ***********/
			
			
			/*********** VLC for PLAY MUSIC ***********/
			/*
			//provo a far leggere i Media gia' a questo livello:
			media = new MediaPlayer();
			//perfetto! Ora aggiungo righe per interrompere l'intro:
			//provo a capire come fermare l'audio -- non ci riesco
			// Riproduci il file audio una volta all'avvio
			StartPlayback("jingle_intro", "audio");
			//se voglio usare la classe generalizzata per fermare l'audio provo -- NON funziona
			//media.PlayAndMonitor("jingle_intro");
			
			// Monitora il processo audio e consente di interromperlo -- funziona? NO
			Console.WriteLine("Premi 's' per fermare l'audio prima che finisca.");
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
							Console.WriteLine("Audio interrotto manualmente.");
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
			Console.WriteLine("Riproduzione terminata.");
			*/
			/*********** VLC for PLAY MUSIC ***********/
			
			
			/*********** NAudio.Wave for PLAY MUSIC and STOP when press any key ***********/
			//PlayAudioAndHandleInput("sound\\jingle_intro.m4a", "Select Level of the game 1-3: ", "123");
			//PlayAudioAndHandleInput("sound\\player_select.m4a", "Select Player Order (rgby): ", "rgby");
			
			Console.Write("Select Level of the game 1-3: ");
			WaveOutEvent outputDevice = null;
			AudioFileReader audioFile = null;
			string filePath3 = "sound\\jingle_intro.m4a";
			// Stop the current playback if running
			outputDevice?.Stop();
			outputDevice?.Dispose();
			audioFile?.Dispose();
			// Load and play the new file
			audioFile = new AudioFileReader(filePath3);
			outputDevice = new WaveOutEvent();
			outputDevice.Init(audioFile);
			outputDevice.Play();
			// Interrompe se viene premuto un tasto
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                if (Console.KeyAvailable)
                {
					outputDevice?.Stop();
					outputDevice?.Dispose();
					outputDevice = null;
					audioFile?.Dispose();
					audioFile = null;
                    break;
                }
                Thread.Sleep(100); // Riduce l'utilizzo della CPU
            }
			int lev = -1;
			while (lev < 1 || lev > 3) // Assicurati che l'input sia valido
			{
				var key = Console.ReadKey();  // 'true' evita che il tasto premuto venga mostrato sulla console
				if (key.KeyChar >= '1' && key.KeyChar <= '3') // Verifica che il tasto sia tra 1 e 3
				{
					lev = Convert.ToInt32(key.KeyChar.ToString());  // Converte il tasto premuto in un intero
				}
				else
				{
					Console.WriteLine("\nInvalid input. Please press a key between 1 and 3.");
				}
			}
			Console.WriteLine($"\nYou selected Level {lev}");
			
			// Ora cambio la configurazione di AntiMicro
			antimicroHelper.StopAntimicro();
			// Verifica che il file di configurazione esista
            if (!System.IO.File.Exists(configPath2))
            {
                Console.WriteLine($"Il file di configurazione '{configPath2}' non è stato trovato nella directory di lavoro.");
                return;
            }
			// Avvia AntiMicro in background
            antimicroHelper.StartAntimicro(antimicroPath, configPath2);
			
			Console.Write("Select Player Order (rgby): ");
			WaveOutEvent outputDevice2 = null;
			AudioFileReader audioFile2 = null;
			string filePath2 = "sound\\player_select.m4a";
			// Stop the current playback if running
			outputDevice2?.Stop();
			outputDevice2?.Dispose();
			audioFile2?.Dispose();
			// Load and play the new file
			audioFile2 = new AudioFileReader(filePath2);
			outputDevice2 = new WaveOutEvent();
			outputDevice2.Init(audioFile2);
			outputDevice2.Play();
			// Interrompe se viene premuto un tasto
            while (outputDevice2.PlaybackState == PlaybackState.Playing)
            {
                if (Console.KeyAvailable)
                {
					outputDevice2?.Stop();
					outputDevice2?.Dispose();
					outputDevice2 = null;
					audioFile2?.Dispose();
					audioFile2 = null;
                    break;
                }
                Thread.Sleep(100); // Riduce l'utilizzo della CPU
            }
            //Console.Write("Player Order (rgby): ");
			//StartPlayback("player_select", "audio");
            string players = Console.ReadLine();
			/*********** NAudio.Wave for PLAY MUSIC ***********/

			/*
            //Console.Write("Level 1-3: "); //con NAudio.Wave lo faccio scegliere prima così posso interrompere l'audio se l'utente digita qualcosa da tastiera
            //int lev = Convert.ToInt32(Console.ReadLine()); //con questa riga devo aspettare invio utente
			int lev = -1;
			while (lev < 1 || lev > 3) // Assicurati che l'input sia valido
			{
				var key = Console.ReadKey();  // 'true' evita che il tasto premuto venga mostrato sulla console
				if (key.KeyChar >= '1' && key.KeyChar <= '3') // Verifica che il tasto sia tra 1 e 3
				{
					lev = Convert.ToInt32(key.KeyChar.ToString());  // Converte il tasto premuto in un intero
				}
				else
				{
					Console.WriteLine("\nInvalid input. Please press a key between 1 and 3.");
				}
			}
			Console.WriteLine($"\nYou selected Level {lev}");
			*/

            if(lev < 1 || lev > 3)
            {
                Console.WriteLine("Wrong level");
                return;
            }
            if (players.Length < 2 || players.Length > 4 )
            {
                Console.WriteLine("Wrong players");
                return;
            }

			//se tutto ok procedo con il gioco
			// Ora cambio la configurazione di AntiMicro
			antimicroHelper.StopAntimicro();
			// Verifica che il file di configurazione esista
            if (!System.IO.File.Exists(configPath3))
            {
                Console.WriteLine($"Il file di configurazione '{configPath3}' non è stato trovato nella directory di lavoro.");
                return;
            }
			// Avvia AntiMicro in background
            antimicroHelper.StartAntimicro(antimicroPath, configPath3);
			
            var game = new Game(cheat, lev,players);
            game.Init();
            var isEnd = false;
            while(!isEnd)
            {
                isEnd = game.Loop();
            }
        }
		
		/* tutte prove per interrompere il suono ma non hanno funzionato
		//funzioni per interrompere il suono
		private static void StartPlayback(string audioFileName, string tipo_media = "audio")
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
                    media.Clear(); // Svuota la coda prima di aggiungere un nuovo suono
                    media.AddSound(audioFileName);
                    media.Play(tipo_media: tipo_media);
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
        }
		
		static void PlayAudioAndHandleInput(string filePath, string prompt, string valid_string)
		{
			WaveOutEvent outputDevice = null;
			AudioFileReader audioFile = null;
			try
			{
				if (!File.Exists(filePath))
				{
					Console.WriteLine($"Errore: il file {filePath} non esiste.");
					return;
				}
	
				// Initialize and play the audio file
				audioFile = new AudioFileReader(filePath);
				outputDevice = new WaveOutEvent();
				outputDevice.Init(audioFile);
				outputDevice.Play();
	
				Console.Write(prompt);
				
				// Wait for the user to input text and press Enter
				string userInput = Console.ReadLine();
				if (valid_string=="123")
				{
					int lev = -1;
					while (lev < 1 || lev > 3) // Assicurati che l'input sia valido
					{
						var key = Console.ReadKey();  // 'true' evita che il tasto premuto venga mostrato sulla console
						if (key.KeyChar >= '1' && key.KeyChar <= '3') // Verifica che il tasto sia tra 1 e 3
						{
							lev = Convert.ToInt32(key.KeyChar.ToString());  // Converte il tasto premuto in un intero
						}
						else
						{
							Console.WriteLine("\nInvalid input. Please press a key between 1 and 3.");
						}
					}
					Console.WriteLine($"\nYou selected Level {lev}");
				}
				else if (valid_string=="rgby")
				{
					Console.WriteLine($"\nYou selected rgbY");
				}
	
				// Interrupt playback if a key is pressed
				while (outputDevice.PlaybackState == PlaybackState.Playing)
				{
					if (Console.KeyAvailable)
					{
						Console.ReadKey(intercept: true); // Consume the key
						break;
					}
					Thread.Sleep(100); // Reduce CPU usage
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore durante la riproduzione: {ex.Message}");
			}
			finally
			{
				// Stop and release resources
				if (outputDevice != null)
				{
					outputDevice.Stop();
					outputDevice.Dispose();
					outputDevice = null;
				}
	
				if (audioFile != null)
				{
					audioFile.Dispose();
					audioFile = null;
				}
			}
		}
		*/
    }
}
