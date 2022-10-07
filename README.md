# GorillaTank
GorillaTank is a mod for Gorilla Tag that adds an interactable tank to the game.   
This mod requires the Utilla mod to function correctly, you can find it on Monke Mod Manager.

## Legal disclaimer
This product is not affiliated with Gorilla Tag or Another Axiom LLC and is not endorsed or otherwise sponsored by Another Axiom LLC. Portions of the materials contained herein are property of Another Axiom LLC. ©2021 Another Axiom LLC.

## How to use the mod
### Enter a modded lobby   
To use the mod, you need to be in a modded lobby, if it worked anywhere else I would be in monke jail right now.   
To go into a modded lobby, go to one of the gamemode selectors, go to the next page, and select one of the four modded lobbies.  

![image](https://user-images.githubusercontent.com/81720436/194441097-7cd2e7cc-abf2-4119-8206-17203eb674f5.png)

### Go to Forest
Once you have selected a modded gamemode, go to the forest map and connect to a room.   
The tank should be located to the left of the target closest to stump.

![image](https://user-images.githubusercontent.com/81720436/194441624-b4f7a726-4b7a-476c-9ed7-858d9698ed9d.png)

### MonkeMapLoader support
If you want this to work, you will need the GorillaTankMapLoader.dll file that is packaged inside the file thats in the release.   
Enter a map that has GorillaTank support and the tank will be located there. Once you leave the map it will go back in forest.   
**You will still need to be in a modded lobby for this to work.**

### Adding MonkeMapLoader support
In your map, add a GameObject called "TankArea", this will be where your tank will be located in the map.

![image](https://user-images.githubusercontent.com/81720436/194596222-a8dbab16-a5d3-4d88-b9e8-84190569878a.png)

Tip: If you put a cube inside the TankArea object that has a scale of 1.95 x 1 x 2.85, it can be used as a reference.          
The blue arrow is the direction the tank will spawn in.

![image](https://user-images.githubusercontent.com/81720436/194596709-4ee41764-ab16-44ea-8378-ff868e85633b.png)

After this, remove the reference cube and export the map. The tank will now show up in your map.

![image](https://user-images.githubusercontent.com/81720436/194597064-8477f4ab-6794-4eb8-8364-ec13f2ca44a9.png)

