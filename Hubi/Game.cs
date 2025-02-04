﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hubi
{
    public class Game
    {
        private int level;

        private List<Vector> boardMesh;

        private List<Field> board;

        private List<Player> players;

        private Player startPlayer;

        private Player currentPlayer;

        private MediaPlayer media;

        private Hubi hubi;

        private int rounds;

        private bool cheatBoard;

        private int clock;
		
		//creo una lista per salvare gli elementi audio da eventualmente ripetere successivamente con il pulsante R:
		public List<string> lista_audio_da_ripetere;

        public Game(bool cheatBoard, int level, string playerstr)
        {
            this.level = level;
            this.cheatBoard = cheatBoard;
            this.players = new List<Player>();
            this.rounds = 0;
            media = new MediaPlayer();
			lista_audio_da_ripetere = new List<string>();

            foreach(var c in playerstr)
            {
                var player = Player.CreatePlayer(c);
                if(player != null)
                {
                    this.players.Add(player);
                }
            }
            startPlayer = players.First();
            if (level == 1)
                clock = -1;
            else if (level == 2)
                clock = 7; //in origine era 9, ma nel gioco originale non mi sembra vi sia un countdown...
            else if (level == 3)
                clock = 5; //in origine era 7, ma nel gioco originale non mi sembra vi sia un countdown...

            hubi = new Hubi(6 - level);
        }

        public void Init()
        {
            //media.AddSound("intro"); //per debug commento altrimenti impazzisco
			//media.AddSound("saluto_gufo"); //lo aggiungo all'interno del turno del giocatore

            bool isMeshValid = false;

            BuildBoardMatrix();
            BuildBoardMesh(level);

            // AwakeHubi();

            PrintMesh(BuildPrintMesh());

            currentPlayer = startPlayer;

        }

        public bool Loop()
        {
            foreach(var player in players)
            {
                if (PlayerTurn(player) == true)
                {
                    //media.Play();
					Console.WriteLine($"Round giocatore: {rounds}");
					Console.WriteLine(string.Join(" - ", lista_audio_da_ripetere));
					//non sembra entrare in questo blocco durante il turno del giocatore....
					foreach (string audiofile in lista_audio_da_ripetere)
					{
						Program.PlayAndStopAudio(audiofile);
					}
                    return true;
                }
                PrintMesh(BuildPrintMesh());
                rounds++;
                Console.WriteLine($"Round: {rounds}");
            }
            return false;
        }

        public bool PlayerTurn(Player player)
        {
            Console.WriteLine($"Current player: {player.PlayerType}, {player.PlayerColor}, {player.ViewChar}");
            Console.WriteLine($"Position: X={player.PosX}, Y={player.PosY}");
            //media.AddSound("player"); //superfluo
			
			//provo a recuperare su quale animale si trova il giocatore per suonare il suo saluto:
			var X_player = player.PosX;
			var Y_player = player.PosY;
			var playerField = board.Single(f => f.x == X_player && f.y == Y_player);
            Console.WriteLine($"Posizione del giocatore: {playerField.FieldType}, {playerField.FieldColor}");
			//media.Clear(); // Svuota la coda prima di aggiungere un nuovo suono - già commentato in origine
			
			//creo una lista per salvare gli elementi audio da eventualmente ripetere successivamente con il pulsante R:
			//public List<string> lista_audio_da_ripetere;
			//lista_audio_da_ripetere = new List<string>();
			
			switch(playerField.FieldType)
            {
                case FieldType.Owl:
                    //media.AddSound("saluto_gufo");
					lista_audio_da_ripetere.Add("sound\\saluto_gufo.m4a");
                    break;
                case FieldType.Worm:
                    //media.AddSound("saluto_verme");
					lista_audio_da_ripetere.Add("sound\\saluto_verme.m4a");
                    break;
                case FieldType.Frog:
                    //media.AddSound("saluto_rana");
					lista_audio_da_ripetere.Add("sound\\saluto_rana.m4a");
                    break;
                case FieldType.Bat:
                    //media.AddSound("saluto_pipistrello");
					lista_audio_da_ripetere.Add("sound\\saluto_pipistrello.m4a");
                    break;
            }

            switch(player.PlayerColor)
            {
                case PlayerColor.Blue:
                    //media.AddSound("blue");
					lista_audio_da_ripetere.Add("sound\\blue.m4a");
                    break;
                case PlayerColor.Red:
                    //media.AddSound("red");
					lista_audio_da_ripetere.Add("sound\\red.m4a");
                    break;
                case PlayerColor.Yellow:
                    //media.AddSound("yellow");
					lista_audio_da_ripetere.Add("sound\\yellow.m4a");
                    break;
                case PlayerColor.Green:
                    //media.AddSound("green");
					lista_audio_da_ripetere.Add("sound\\green.m4a");
                    break;
            }
            //media.AddSound("turn"); //commento dall'inizio in quanto priva di senso

            var validCommand = false;
            var xDir = 0;
            var yDir = 0;
            bool extraMove = false;
            bool hubiMove = false;
            int nexthubiMove = -1;
			//aggiungo un contatore continuo per le extramove perche' nel gioco originale fa muovere al massimo 2 volte di seguito
			int count_extramove = 0;

            while (!validCommand || extraMove)
            {
                if (extraMove == true && count_extramove<1)
                {
                    Console.WriteLine("Extra move");
                    //media.AddSound("extramove");
					lista_audio_da_ripetere.Add("sound\\extramove.m4a");
                    PrintMesh(BuildPrintMesh());
                    extraMove = false;
					count_extramove++;
                }

                //media.AddSound("move");
                //media.Play();
				lista_audio_da_ripetere.Add("sound\\move.m4a");
				Console.WriteLine(string.Join(", ", lista_audio_da_ripetere));
				foreach (string audiofile in lista_audio_da_ripetere)
				{
					Program.PlayAndStopAudio(audiofile);
				}
                Console.Write("Your Move (H=Help, R=Repeat) ? >");
				
                var command = Console.ReadKey().Key;
                while(command == ConsoleKey.R)
                {
                    //media.Play();
					//ciclo dentro la lsta per far ripetere i comandi fino a qui
					foreach (string audiofile in lista_audio_da_ripetere)
					{
						Program.PlayAndStopAudio(audiofile);
					}
                    Console.Write("Your Move (H=Help, R=Repeat) ? >");
                    command = Console.ReadKey().Key;
                }
                //media.Clear();
				lista_audio_da_ripetere.Clear();
				
				/*
				//provo a sollecitare l'utente dopo 20 secondi di inattività -- funziona ma si accavalla con gli altri file audio
				// Crea un token per annullare il timer se l'utente risponde in tempo
				var cts = new CancellationTokenSource();
				// Avvia un task asincrono per sollecitare l'utente dopo 20 secondi
				var reminderTask = Task.Run(async () =>
				{
					try
					{
						await Task.Delay(20000, cts.Token); // Attendi 20 secondi
						if (!cts.Token.IsCancellationRequested)
						{
							Console.WriteLine("\nSei ancora lì? Scrivi qualcosa per continuare!");
							switch(player.PlayerColor)
							{
								case PlayerColor.Blue:
									//media.AddSound("blue");
									Program.PlayAndStopAudio("sound\\blue.m4a");
									break;
								case PlayerColor.Red:
									//media.AddSound("red");
									Program.PlayAndStopAudio("sound\\red.m4a");
									break;
								case PlayerColor.Yellow:
									//media.AddSound("yellow");
									Program.PlayAndStopAudio("sound\\yellow.m4a");
									break;
								case PlayerColor.Green:
									//media.AddSound("green");
									Program.PlayAndStopAudio("sound\\green.m4a");
									break;
							}
							//media.AddSound("sollecito_inattivita");
							//media.Play();
							Program.PlayAndStopAudio("sound\\sollecito_inattivita.m4a");
						}
					}
					catch (TaskCanceledException)
					{
						// Il task è stato annullato, non fare nulla
					}
				});
				*/

                //media.Stop(); //penso che questa riga fosse già commentata in origine
                Console.WriteLine();
                switch (command)
                {
                    case ConsoleKey.UpArrow:
                        xDir = 0;
                        yDir = -1;
                        break;
                    case ConsoleKey.DownArrow:
                        xDir = 0;
                        yDir = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        xDir = -1;
                        yDir = 0;
                        break;
                    case ConsoleKey.RightArrow:
                        xDir = 1;
                        yDir = 0;
                        break;
                    case ConsoleKey.H:
                        xDir = 0;
                        yDir = 0;
                        break;
                    default:
                        //media.AddSound("failmove1");
						lista_audio_da_ripetere.Add("sound\\failmove1.m4a");
                        Console.WriteLine("Invalid Command");
                        validCommand = false;
                        continue;                        
                }
                validCommand = true;
                if(command == ConsoleKey.H)
                {
                    if (!PrintHelp(player.PosX, player.PosY, hubi.IsAwake))
                    {
                        validCommand = false;
                    }; 
                    continue;
                }

                var wantedX = player.PosX + xDir;
                var wantedY = player.PosY + yDir;
                if (wantedX < 0 || wantedX > 3 || wantedY < 0 || wantedY > 3)
                {
                    //media.AddSound("failmove2");
					lista_audio_da_ripetere.Add("sound\\failmove2.m4a");
                    Console.WriteLine("Move not allowed.");
                    validCommand = false;
                    continue;
                }
                var wayVector = boardMesh.Single(x => ((x.X1 == player.PosX && x.Y1 == player.PosY) && (x.X2 == wantedX && x.Y2 == wantedY)) ||
                                ((x.X1 == wantedX && x.Y1 == wantedY) && (x.X2 == player.PosX && x.Y2 == player.PosY)));
                switch (wayVector.WallType)
                {
                    case WallType.Open:
                        //media.AddSound("openwall");
                        //media.AddSound("pass");
						lista_audio_da_ripetere.Add("sound\\openwall.m4a");
						lista_audio_da_ripetere.Add("sound\\pass.m4a");

                        Console.WriteLine("Open, can pass");
                        break;
                    case WallType.Full:
                        //media.AddSound("fullwall");
                        //media.AddSound("nopass");
						lista_audio_da_ripetere.Add("sound\\fullwall.m4a");
						lista_audio_da_ripetere.Add("sound\\nopass.m4a");
                        Console.WriteLine("Wall, cannot pass");
                        xDir = 0;
                        yDir = 0;
                        break;
                    case WallType.Mouse:
                        //media.AddSound("mousewall");
						lista_audio_da_ripetere.Add("sound\\mousewall.m4a");
                        if (player.PlayerType == PlayerType.Mouse)
                        {
                            Console.WriteLine("Mouse wall, can pass");
                            //media.AddSound("pass");
							lista_audio_da_ripetere.Add("sound\\pass.m4a");
                        }
                        else
                        {
                            Console.WriteLine("Mouse wall, can not pass");
                            //media.AddSound("nopass");
							lista_audio_da_ripetere.Add("sound\\nopass.m4a");
                            xDir = 0;
                            yDir = 0;
                        }
                        break;
                    case WallType.Rabbit:
                        //media.AddSound("rabbitwall");
						lista_audio_da_ripetere.Add("sound\\rabbitwall.m4a");
                        if (player.PlayerType == PlayerType.Rabbit)
                        {
                            Console.WriteLine("Rabbit wall, can pass");
                            //media.AddSound("pass");
							lista_audio_da_ripetere.Add("sound\\pass.m4a");
                        }
                        else
                        {
                            Console.WriteLine("Rabbit wall, can not pass");
                            //media.AddSound("nopass");
							lista_audio_da_ripetere.Add("sound\\nopass.m4a");
                            xDir = 0;
                            yDir = 0;
                        }
                        break;
                    case WallType.MagicClosed:
                        var magic1player = players.FirstOrDefault(p => p.PosX == wayVector.X1 && p.PosY == wayVector.Y1);
                        var magic2player = players.FirstOrDefault(p => p.PosX == wayVector.X2 && p.PosY == wayVector.Y2);

                        if (magic1player != null && magic2player != null)
                        {
                            //media.AddSound("opendoor");
                            //media.AddSound("pass");
							lista_audio_da_ripetere.Add("sound\\opendoor.m4a");
							lista_audio_da_ripetere.Add("sound\\pass.m4a");
                            wayVector.WallType = WallType.MagicOpen;
                            Console.WriteLine("Magic Door is open, can pass");
                            var magicDoors = boardMesh.Where(v => v.WallType == WallType.MagicClosed);
                            if(magicDoors.Any())
                            {
                                Console.WriteLine("Still closed magic doors ");
                                //media.AddSound("stillclosedmagicdoors");
								lista_audio_da_ripetere.Add("sound\\stillclosedmagicdoors.m4a");
                            }
                            else
                            {
                                Console.WriteLine("Hubi is awake ");
                                //media.AddSound("hubiawake_caverna");
								lista_audio_da_ripetere.Add("sound\\hubiawake_caverna.m4a");
                                AwakeHubi();
                                PrintHelp(player.PosX, player.PosY, hubi.IsAwake);
                            }

                            Console.WriteLine("Magic Door is open, can pass");
                            wayVector.WallType = WallType.MagicOpen;
                            extraMove = true;
                        }
                        else
                        {
                            //media.AddSound("closemagicwall");
                            //media.AddSound("nopass");
							lista_audio_da_ripetere.Add("sound\\closemagicwall.m4a");
							lista_audio_da_ripetere.Add("sound\\nopass.m4a");

                            Console.WriteLine("Closed Magic Door, cannot pass");
                            xDir = 0;
                            yDir = 0;
                        }
                        break;
                    case WallType.MagicOpen:
						//media.AddSound("magicdoor_opening");
                        //media.AddSound("openmagicwall");
                        //media.AddSound("pass");
						lista_audio_da_ripetere.Add("sound\\magicdoor_opening.m4a");
						lista_audio_da_ripetere.Add("sound\\openmagicwall.m4a");
						lista_audio_da_ripetere.Add("sound\\pass.m4a");
                        Console.WriteLine("Open Magic Door, can pass");
                        break;
                    default:
                        Console.WriteLine("Wall type unkown");
                        xDir = 0;
                        yDir = 0;
                        break;
                }
                if (xDir != 0 || yDir != 0)
                {
                    player.PosX += xDir;
                    player.PosY += yDir;
					//blocco if originale
					/*if (player.AddVisitedField(player.PosX, player.PosY))
                    {
                        extraMove = true;
                    }*/
					//nel gioco originale fa muovere al massimo 2 volte di seguito
					if (player.AddVisitedField(player.PosX, player.PosY))
                    {
						if(count_extramove==1) {extraMove = false;}
                        else {extraMove = true;}
                    }
                    var playerWithHubi = CheckFinish();
                    if(playerWithHubi >= 2)
                    {
                        Console.WriteLine("You have won");
                        //media.AddSound("win");
						//lo scompongo
						//media.AddSound("win_hubi_caverna");
						//media.AddSound("win_jingle1");
						//media.AddSound("win_narrator");
						//media.AddSound("win_hubi2_caverna");
						//media.AddSound("win_narrator2");
						//media.AddSound("win_jingle2");
						lista_audio_da_ripetere.Add("sound\\win_hubi_caverna.m4a");
						lista_audio_da_ripetere.Add("sound\\win_jingle1.m4a");
						lista_audio_da_ripetere.Add("sound\\win_narrator.m4a");
						lista_audio_da_ripetere.Add("sound\\win_hubi2_caverna.m4a");
						lista_audio_da_ripetere.Add("sound\\win_narrator2.m4a");
						lista_audio_da_ripetere.Add("sound\\win_jingle2.m4a");
                        return true;
                    }
                    else if(playerWithHubi == 1)
                    {
                        Console.WriteLine("Hubi is here");
                        //media.AddSound("visithubi_caverna");
						//media.AddSound("hubi_check");
						lista_audio_da_ripetere.Add("sound\\visithubi_caverna.m4a");
						lista_audio_da_ripetere.Add("sound\\hubi_check.m4a");
                        if(nexthubiMove == -1)
                            nexthubiMove = 0;
                    }
                }
                if (hubi.IsAwake)
                {
                    if ((rounds+1) % hubi.Speed == 0 || nexthubiMove == 3)
                    {
                        nexthubiMove = -1;
                        hubiMove = true;
                    }
                    if (nexthubiMove >= 0)
                        nexthubiMove++;
                }
                if (hubiMove)
                {
                    Console.WriteLine("Hubi is moved");
                    //media.AddSound("hubimove_caverna");
					lista_audio_da_ripetere.Add("sound\\hubimove_caverna.m4a");
                    PrintHelp(player.PosX, player.PosY, hubi.IsAwake);
                    MoveHubi();
                    PrintMesh(BuildPrintMesh());
                }
                if(clock == 0)
                {
                    Console.WriteLine("Clock is over, you lost");
                    //media.AddSound("lost");
					lista_audio_da_ripetere.Add("sound\\lost.m4a");
                    return true;

                }
                if (clock != -1 && (rounds +1) % 8 == 0)
                {
                    Console.WriteLine($"Clock is {clock}");
                    //media.AddSound(clock.ToString());
					lista_audio_da_ripetere.Add("sound\\pendolo_cattedrale_cutted_fadeout.m4a");
					string concatenatedString = "sound\\" + clock.ToString() + ".m4a";
					lista_audio_da_ripetere.Add(concatenatedString);
                    clock--;
                }
				if (clock == -1 && (rounds +1) == 8) //aggiungo questa sollecitazione dopo 8 mosse. In realtà andrebbe messa dopo circa 6 mosse da quando Hubi si sveglia (mi pare)... La metto solo nel livello di difficoltà 1 per il momento
                {
                    Console.WriteLine($"Sono passati {rounds} round");
                    //media.AddSound("tramonto_cattedrale");
					lista_audio_da_ripetere.Add("sound\\pendolo_cattedrale_cutted_fadeout.m4a");
					lista_audio_da_ripetere.Add("sound\\tramonto_cattedrale.m4a");
                    //clock--;
                }

            }
            return false;
        }

        private int CheckFinish()
        {
            if (!hubi.IsAwake)
                return 0;

            int c = 0;

            foreach(var p in players)
            {
                if (p.PosX == hubi.Pos.X && p.PosY == hubi.Pos.Y)
                    c++;
            }
            return c;
        }

        private void MoveHubi()
        {
            var freeFields = new List<Field>();
            var sorFields = new List<Field>();
            int[] sx = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] sy = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < 8; i++)
            {
                var sf = board.SingleOrDefault(f => f.x == hubi.Pos.X + sx[i] && f.y == hubi.Pos.Y + sy[i]);
                if (sf != null)
                {
                    sorFields.Add(sf);
                }
            }


            foreach (var field in sorFields)
            {
                bool includeField = true;
                foreach (var p in players)
                {
                    if (p.PosX == field.x && p.PosY == field.y)
                    {
                        includeField = false;
                        break;
                    }
                }
                if (includeField)
                {
                    freeFields.Add(field);
                }
            }
            int c = freeFields.Count();
            if (c > 0)
            { 
                int r = new Random().Next(0, c);
                hubi.Pos.X = freeFields.ElementAt(r).x;
                hubi.Pos.Y = freeFields.ElementAt(r).y;
            }
        }

        private void AwakeHubi()
        {
            hubi.IsAwake = true;

            var freeFields = new List<Field>();
            foreach(var field in board)
            {
                bool includeField = true;
                foreach (var p in players)
                {
                    if(p.PosX == field.x && p.PosY == field.y)
                    {
                        includeField = false;
                        break;
                    }
                }
                if (includeField)
                {
                    freeFields.Add(field);
                }
            }

            int c = freeFields.Count();
            int r = new Random().Next(0, c);
            hubi.Pos.X = freeFields.ElementAt(r).x;
            hubi.Pos.Y = freeFields.ElementAt(r).y;
        }

        private bool PrintHelp(int posX, int posY, bool forHubi)
        {
            var field = board.Single(f => f.x == posX && f.y == posY);
			//Console.WriteLine($"Field: {field}"); //non mi aiuta, dice Field: Hubi.Field
            if(!forHubi)
            {
				Console.WriteLine(field.hasGivenHint); //non viene gestito mi pare
                if (field.hasGivenHint)
                {
                    //media.AddSound("alreadyanimalhint");
					lista_audio_da_ripetere.Add("sound\\alreadyanimalhint.m4a");
                    Console.WriteLine("Animal has already given a hint");
                    return false;
                }

                var magicDoors = boardMesh.Where(v => v.WallType == WallType.MagicClosed);

                var c = magicDoors.Count();
                if(c == 0)
                {
                    //media.AddSound("failmove1");
					lista_audio_da_ripetere.Add("sound\\failmove1.m4a");
                    Console.WriteLine("No hint possible");
                    return false;
                }
                var r = new Random().Next(0, c);
                var magicDoor = magicDoors.ElementAt(r);
                var magic1 = board.Single(f => f.x == magicDoor.X1 && f.y == magicDoor.Y1);
                var magic2 = board.Single(f => f.x == magicDoor.X2 && f.y == magicDoor.Y2);
                var shift = new Random().Next(0, 2);
                if(level == 1)
                {
                    if(shift == 0)
                    {
                        Console.WriteLine($"The magic door is between {magic1.FieldColor},{magic1.FieldType} and {magic2.FieldColor},{magic2.FieldType}");
                        /*media.AddSound("staywithme");
						media.AddSound("magicbetween");
						//inverto prima l'animale e poi il colore, per l'italiano
                        media.AddSound(magic1.FieldType.ToString().ToLower());
						media.AddSound(magic1.FieldColor.ToString().ToLower());
                        media.AddSound("and");
						media.AddSound(magic2.FieldType.ToString().ToLower());
                        media.AddSound(magic2.FieldColor.ToString().ToLower());*/
						//uso il metodo che interrompe l'audio quando viene premuto un pulsante d'azione
						lista_audio_da_ripetere.Add("sound\\staywithme.m4a");
						lista_audio_da_ripetere.Add("sound\\magicbetween.m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic1.FieldType.ToString().ToLower() + ".m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic1.FieldColor.ToString().ToLower() + ".m4a");
						lista_audio_da_ripetere.Add("sound\\and.m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic2.FieldType.ToString().ToLower() + ".m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic2.FieldColor.ToString().ToLower() + ".m4a");
                    }
                    else
                    {
                        Console.WriteLine($"The magic door is between {magic2.FieldColor},{magic2.FieldType} and {magic1.FieldColor},{magic1.FieldType}");
						/*media.AddSound("staywithme");
                        media.AddSound("magicbetween");
                        media.AddSound(magic2.FieldType.ToString().ToLower());
						media.AddSound(magic2.FieldColor.ToString().ToLower());
                        media.AddSound("and");
                        media.AddSound(magic1.FieldType.ToString().ToLower());
						media.AddSound(magic1.FieldColor.ToString().ToLower());*/
						//uso il metodo che interrompe l'audio quando viene premuto un pulsante d'azione
						lista_audio_da_ripetere.Add("sound\\staywithme.m4a");
						lista_audio_da_ripetere.Add("sound\\magicbetween.m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic2.FieldType.ToString().ToLower() + ".m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic2.FieldColor.ToString().ToLower() + ".m4a");
						lista_audio_da_ripetere.Add("sound\\and.m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic1.FieldType.ToString().ToLower() + ".m4a");
						lista_audio_da_ripetere.Add("sound\\" + magic1.FieldColor.ToString().ToLower() + ".m4a");
                    }
                }
                else if (level == 2)
                {
                    /* nel gioco originale al livello 2 viene ancora data la posizione della porta tra 2 animali
					Field magic = GetMagicFieldForHint(magic1, magic2, shift);
                    Console.WriteLine($"The magic door is next to {magic.FieldColor},{magic.FieldType}");
                    media.AddSound("staywithme");
					media.AddSound("magicnext");
					//inverto ordine colore-animale per italiano:
					media.AddSound(magic.FieldType.ToString().ToLower());
                    media.AddSound(magic.FieldColor.ToString().ToLower());
					*/
					Console.WriteLine($"The magic door is between {magic1.FieldColor},{magic1.FieldType} and {magic2.FieldColor},{magic2.FieldType}");
                    /*media.AddSound("staywithme");
					media.AddSound("magicbetween");
					//inverto prima l'animale e poi il colore, per l'italiano
                    media.AddSound(magic1.FieldType.ToString().ToLower());
					media.AddSound(magic1.FieldColor.ToString().ToLower());
                    media.AddSound("and");
					media.AddSound(magic2.FieldType.ToString().ToLower());
                    media.AddSound(magic2.FieldColor.ToString().ToLower());*/
					//uso il metodo che interrompe l'audio quando viene premuto un pulsante d'azione
					lista_audio_da_ripetere.Add("sound\\staywithme.m4a");
					lista_audio_da_ripetere.Add("sound\\magicbetween.m4a");
					lista_audio_da_ripetere.Add("sound\\" + magic1.FieldType.ToString().ToLower() + ".m4a");
					lista_audio_da_ripetere.Add("sound\\" + magic1.FieldColor.ToString().ToLower() + ".m4a");
					lista_audio_da_ripetere.Add("sound\\and.m4a");
					lista_audio_da_ripetere.Add("sound\\" + magic2.FieldType.ToString().ToLower() + ".m4a");
					lista_audio_da_ripetere.Add("sound\\" + magic2.FieldColor.ToString().ToLower() + ".m4a");
                }
                else if (level == 3)
                {
					//in realtà anche al livello 3 del gioco originale viene data la posizione della porta magica tra 2 animali, solo che le porte sono 3
                    Field magic = GetMagicFieldForHint(magic1, magic2, shift);

                    Console.WriteLine($"The magic door is next to {magic1.FieldType}");
                    /*media.AddSound("staywithme");
					media.AddSound("magicnext");
                    media.AddSound(magic1.FieldType.ToString().ToLower());*/
					//uso il metodo che interrompe l'audio quando viene premuto un pulsante d'azione
					lista_audio_da_ripetere.Add("sound\\staywithme.m4a");
					lista_audio_da_ripetere.Add("sound\\magicnext.m4a");
					lista_audio_da_ripetere.Add("sound\\" + magic1.FieldType.ToString().ToLower() + ".m4a");
                }
				//provo a gestire il fatto che si sia già dato un aiuto:
				field.hasGivenHint = true;
            }
            else
            {
                var hubiField = board.Single(f => f.x == hubi.Pos.X && f.y == hubi.Pos.Y);
                Console.WriteLine($"Hubi is seen: {hubiField.FieldColor},{hubiField.FieldType}");
                media.AddSound("staywithme");
				media.AddSound("hubiseen");
                media.AddSound(hubiField.FieldColor.ToString().ToLower());
                media.AddSound(hubiField.FieldType.ToString().ToLower());
            }
            return true;
        }

        private static Field GetMagicFieldForHint(Field magic1, Field magic2, int shift)
        {
            Field magic = null;
            if (shift == 0)
                magic = magic1;
            else
                magic = magic2;
 
            return magic;
        }

        private void BuildBoardMatrix()
        {
            board = new List<Field>();
            board.Add(new Field(0, 0, FieldColor.White, FieldType.Owl));
            board.Add(new Field(0, 3, FieldColor.Black, FieldType.Owl));
            board.Add(new Field(3, 0, FieldColor.Black, FieldType.Owl));
            board.Add(new Field(3, 3, FieldColor.White, FieldType.Owl));

            board.Add(new Field(1, 0, FieldColor.Black, FieldType.Worm));
            board.Add(new Field(2, 0, FieldColor.White, FieldType.Frog));

            board.Add(new Field(0, 1, FieldColor.Black, FieldType.Frog));
            board.Add(new Field(1, 1, FieldColor.White, FieldType.Bat));
            board.Add(new Field(2, 1, FieldColor.Black, FieldType.Bat));
            board.Add(new Field(3, 1, FieldColor.White, FieldType.Worm));

            board.Add(new Field(0, 2, FieldColor.White, FieldType.Worm));
            board.Add(new Field(1, 2, FieldColor.Black, FieldType.Bat));
            board.Add(new Field(2, 2, FieldColor.White, FieldType.Bat));
            board.Add(new Field(3, 2, FieldColor.Black, FieldType.Frog));

            board.Add(new Field(1, 3, FieldColor.White, FieldType.Frog));
            board.Add(new Field(2, 3, FieldColor.Black, FieldType.Worm));
        }

        private void BuildBoardMesh(int magicWalls)
        {
            boardMesh = InitMesh();

            int[,] xMain = { { 0, 1, 0, 2 }, { 1, 1, 1, 2 }, { 2, 1, 2, 2 }, { 3, 1, 3, 2 } };
            int[,] yMain = { { 1, 0, 2, 0 }, { 1, 1, 2, 1 }, { 1, 2, 2, 2 }, { 1, 3, 2, 3 } };
            int[,] lt = { { 0, 0, 1, 0 }, { 0, 1, 1, 1 }, { 0, 0, 0, 1 }, { 1, 0, 1, 1 } };
            int[,] rt = { { 2, 0, 3, 0 }, { 2, 1, 3, 1 }, { 2, 0, 2, 1 }, { 3, 0, 3, 1 } };
            int[,] lb = { { 0, 2, 0, 3 }, { 0, 3, 1, 3 }, { 0, 2, 0, 3 }, { 1, 2, 1, 3 } }; ;
            int[,] rb = { { 2, 2, 3, 2 }, { 2, 3, 3, 3 }, { 2, 2, 2, 3 }, { 3, 2, 3, 3 } }; ;

            OpenDoors(xMain, true);
            OpenDoors(yMain, true);
            OpenDoors(lt, false);
            OpenDoors(rt, false);
            OpenDoors(lb, false);
            OpenDoors(rb, false);

            ExtraDoors(4, WallType.Mouse);
            ExtraDoors(4, WallType.Rabbit);
            int fullWalls = 3 - magicWalls;
            ExtraDoors(fullWalls, WallType.Full);
            ExtraDoors(magicWalls, WallType.MagicClosed);


        }

        private void ExtraDoors(int num, WallType wallType)
        {
            for (int i = 0; i < num; i++)
            {
                var undef = boardMesh.FindAll(x => x.WallType == WallType.Undefined);
                if (undef.Any())
                { 
                    var r = new Random().Next(0, undef.Count);
                    var wall = undef.ElementAt(r);
                    wall.WallType = wallType;
                }
            }
        }

        private void OpenDoors(int[,] vlist, bool single)
        {
            int[] vOpen = { -1, -1, -1 };
            int r = new Random().Next(0, 4);
            if (single)
            {
                vOpen[0] = r;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < 4; i++)
                {
                    if(i != r)
                    {
                        vOpen[j++] = i;
                    }
                }
            }

            foreach(var v in vOpen)
            {
                if (v == -1)
                    continue;
                var x1 = vlist[v, 0];
                var y1 = vlist[v, 1];
                var x2 = vlist[v, 2];
                var y2 = vlist[v, 3];

                var vector = boardMesh.FirstOrDefault(x => (x.X1 == x1 && x.Y1 == y1 && x.X2 == x2 && x.Y2 == y2) ||
                    (x.X2 == x1 && x.Y2 == y1 && x.X1 == x2 && x.Y1 == y2));
                if (vector != null)
                {
                    vector.WallType = WallType.Open;
                }
            }
        }

        private List<Vector> InitMesh()
        {
            var wallMesh = new List<Vector>();

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    var vector1 = Vector.CreateVector(x, y, x + 1, y);
                    if(vector1 != null)
                    {
                        vector1.WallType = WallType.Undefined;
                        wallMesh.Add(vector1);
                    }
                    var vector2 = Vector.CreateVector(x, y, x, y+1 );
                    if (vector2 != null)
                    {
                        vector2.WallType = WallType.Undefined;
                        wallMesh.Add(vector2);
                    }
                }
            }
            return wallMesh;
        }

        private Dictionary<Direction,Vector> FindNeighbors(int x, int y)
        {
            var res = new Dictionary<Direction, Vector>();

            var neighbors = boardMesh.FindAll(n => (n.X1 == x && n.Y1 == y) || (n.X2 == x && n.Y2 == y));
            foreach(var n in neighbors)
            {
                if(n.X1 == x && n.Y1 == y)
                {
                    SetDirection(x, y, n.X2, n.Y2, res, n);
                }
                else if (n.X2 == x && n.Y2 == y)
                {
                    SetDirection(x, y, n.X1, n.Y1, res, n);
                }

            }
            return res;
        }

        private static void SetDirection(int x, int y, int dx, int dy, Dictionary<Direction, Vector> res, Vector n)
        {
            if (dx == x && dy > y)
            {
                res.Add(Direction.South, n);
            }
            if (dx == x && dy < y)
            {
                res.Add(Direction.North, n);
            }
            if (dx > x && dy == y)
            {
                res.Add(Direction.East, n);
            }
            if (dx < x && dy == y)
            {
                res.Add(Direction.West, n);
            }
        }

        private void PrintMesh(char[,] printmesh)
        {
            if (cheatBoard == false)
                return;

            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    var color = ConsoleColor.Yellow;

                    int xi = (int)(x / 3);
                    int yi = (int)(y / 3);

                    if ((xi == 0 || xi == 2) && (yi == 0 || yi == 2))
                    {
                        color = ConsoleColor.DarkBlue;
                    }
                    if ((xi == 1 || xi == 3) && (yi == 1 || yi == 3))
                    {
                        color = ConsoleColor.Red;
                    }

                    Console.BackgroundColor = color;
                    Console.Write(Convert.ToString(printmesh[x, y]),color);
                }
                Console.WriteLine();
            }
        }

        private char[,] BuildPrintMesh()
        {
            var printmatrix = new char[12, 12];

            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    printmatrix[x, y] = '.';
                }
            }

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int posx = x * 3;
                    int posy = y * 3;

                    var field = board.Single(a => a.x == x && a.y == y);
                    printmatrix[posx + 1, posy + 1] = GetFieldChar(field);

                    if (hubi.IsAwake && hubi.Pos.X == x && hubi.Pos.Y == y)
                    {
                        printmatrix[posx + 1, posy + 1] = 'ß';
                    }


                    var vectors = FindNeighbors(x,y);
                    foreach(var v in vectors)
                    {
                        switch(v.Key)
                        {
                            case Direction.North:
                                printmatrix[posx + 1, posy] = GetWallChar(v.Key, v.Value.WallType);
                                break;
                            case Direction.South:
                                printmatrix[posx + 1, posy+2] = GetWallChar(v.Key, v.Value.WallType);
                                break;
                            case Direction.West:
                                printmatrix[posx , posy+1] = GetWallChar(v.Key, v.Value.WallType);
                                break;
                            case Direction.East:
                                printmatrix[posx +2 , posy+1] = GetWallChar(v.Key, v.Value.WallType);
                                break;
                        }
                    }
                }
                foreach(var player in players)
                {
                    var posx = player.PosX * 3 + (player.StartPosX / 3 == 1 ? 2 : 0);
                    var posy = player.PosY * 3 + (player.StartPosY / 3 == 1 ? 2 : 0); 
                    printmatrix[posx, posy] = player.ViewChar;
                }
            }
            return printmatrix;
        }

        private char GetFieldChar(Field field)
        {
            char elem=' ';

            if(field.FieldType == FieldType.Bat)
            {
                elem = 'B';
            }
            if (field.FieldType == FieldType.Frog)
            {
                elem = 'F';
            }
            if (field.FieldType == FieldType.Worm)
            {
                elem = 'W';
            }
            if (field.FieldType == FieldType.Owl)
            {
                elem = 'O';
            }

            if (field.FieldColor == FieldColor.White)
                elem =  elem.ToString().ToLower()[0];
            return elem;
        }

        private char GetWallChar(Direction dir, WallType wallType)
        {
            switch(wallType)
            {
                case WallType.Full:
                    if (dir == Direction.North || dir == Direction.South)
                        return '-';
                    else
                        return '|';
                case WallType.Rabbit:
                    return 'R';
                case WallType.Undefined:
                    return '?';
                case WallType.Open:
                    return 'O';
                case WallType.Mouse:
                    return 'm';
                case WallType.MagicClosed:
                    return '*';
                case WallType.MagicOpen:
                    return '#';
            }
            return ' ';
        }
    }
}
