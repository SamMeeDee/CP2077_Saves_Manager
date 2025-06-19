# **Cyberpunk 2077 Save Manager**

A simple Windows C#/.NET program to manage your Cyberpunk 2077 saves across different characters/playthroughs.

# Purpose

As it currently sits (06/10/2025, version 2.21), there is no way in-game to manage saves for different characters/playthroughs, or even easily determine the character/playthrough for a given save just by looking at it. I researched various solutions to this problem, such as the [Named Save](http://nexusmods.com/cyberpunk2077/mods/4521) and [Filter by LifePath](https://www.nexusmods.com/cyberpunk2077/mods/3400) mods, as well as more ad-hoc solutions, such as always making a save in the same place/time/loadout, so the thumbnail for a given character/playthrough will always be the same. Frankly, all those solutions seemed like way too much work. So I decided to solve it myself.

# Functionality

On first use, if using the launcher-enabled release, the program will ask you to identify the installation folder for the game, which is used to launch the game later. 

It will then scan all the saves in the save game folder and determine a list of available characters/playthroughs. It will also rename all the save file folders to a new uniform name format to prevent any name collisions. This is not making any changes to the save data itself. 

You can then choose which of these you wish to "load". The program will move all other saves out of your main save folder and into a backup folder, so that only the saves for the playthrough you want are visible to the game. 

In the main release, the program will close at this point. However, there is a seperate working branch where the program will the launch the game (more specifically, it will launch REDLauncher, for modding compatibility). I will be maintaining this branch along with the main branch. 

When you next run the program, it will auto-detect your previously chosen playthroughs and give you the choice to leave it as is, or go back and pick a different one. This may be useful to just sanity-check what playthroughs are currently active. 

# How it Works

The program reads the metadata.9.json file in each save folder and sorts based on the unique combination of LifePath + Body Gender + Voice Gender + Playthrough ID. Playthrough ID is an alpha-numeric string that is unique to a given character/playthrough across all its' saves. I would assume it is a hash generated at the time of the first save after starting a playthrough. It is unfortunately meaningless in determining any details about the character on its own, but I included it to handle the case of multiple playthroughs with identical combos of LifePath + Body Gender + Voice Gender (there are only 12 possible combinations after all). I may go back and just have it do the comparison based on Playthrough ID only, since assuming you don't tamper with the metadata.9.json file, it should suffice for sorting saves into different playthroughs. However, in general this should still work with modified save files, as long as you are not altering Lifepath, Body Gender, or Voice Gender. If you do modify any of those attributes, subsequent saves will be categorized as a seperate playthrough from the saves from before the modification. 

# How to Use

Download the .exe and drop it wherever you like. My plan is to put a shortcut on my desktop that I just label "Cyberpunk 2077", and use that shortcut to launch the game. 

Requires [.NET 9.0 Runtime](https://builds.dotnet.microsoft.com/dotnet/Runtime/9.0.6/dotnet-runtime-9.0.6-win-x64.exe)

I have not done any testing with how this affects cloud synced saves. I would assume if you have it turned on and make a new save, it will still sync that save as usual. If you want to be on the safe side, disable cloud saves before use. 

While this program just moves them around, I would still reccommend backing up your saves before initial use. 

# To Dos
1. Add process tracking to determine when the game has been closed, and rescan for any new saves that don't match the "loaded" character, and take some actions.
2. Addendum to #1, attempt to sort out cloud save compatibility
3. Implement this in a GUI instead of console, so that I could potentially display save thumbnails. 
4. Look into the possibility of modding CP2077 itself to add a prompt for a "character name" or "profile name" when making a new character that would be stored with each save, which would 

# License 
[Apache-2.0 © Sam Davenport](LICENSE)