# Schnappt Hubi
Replacement for the electronic board game device of "Schnappt Hubi" (german)... if the batteries leaked this might help

## Overview
The board game "Schnappt Hubi" has an electronic device for directing the players on a game board. In our case the device was broken (leaked batteries). Therefore I decided to "re-engineer" the device logic in a small commandline tool. This is a quick and dirty implementation with my kids voices for fun, don't except clean code here.

# Language
Currently only the german version exists. 

# Installation
You need .NET 6 to run Hubi. For playing sounds VLC media player need to be installed, the exe must be in C:\Program Files\VideoLAN\VLC\vlc.exe

--> ho modificato a mano il file MediaPlayer.cs per accettare il percorso di vlc e plottare il valore dei file audio eseguiti

Launch the game in consolle by getting the directory:
cd C:\Users\franpala\Downloads\schnappt_hubi-main\Hubi

and then
dotnet run --project Hubi.csproj

To launch it with "cheat" option:
dotnet run --project Hubi.csproj -- -c


## Gameplay
You need the orginal board game to play, german instructions: See https://www.brettspiele-report.de/images/s/schnappt-hubi/Spielanleitung-Schnappt-Hubi.pdf

You need to select the level first (1-3), same as level selector on device.
The you need to define the players and the order, i.e. rgb means 3 players in order red, green, blue

During game:
* h = get hint
* r = repeat last sentence
* Arrow Keys = Direction

# Cheat
If you add the commandline parameter -c you see a debug board layout and location of walls and of Hubi.

## Legal 
The original game was copyright by Ravensburger. If there are any legal complains, please contact me.  
