# Terrain-Interaction-Toolkit
Purpose:
A toolkit made for making details and trees on existing Unity terrains interactable. Includes STP integration.

First off, what is a terrain detail? It's the grass, bushes, rocks, and any gameObjects you use Unity's Terrain Tools to place down onto the terrain in your scene. This toolkit will allow you to transform existing terrain details into GameObjects from the editor or create placeholders at runtime and the existing details will be removed as you interact with them. Existing Terrains don't allow much change to the original prefab of the objects you set up to paint onto the terrain. In many cases the changes don't translate to the actual terrain detail or tree objects or the changes break the setup of your terrain detail or tree object altogether which makes it stop showing up on the map.

Why would you want to do this? Many types of games available today, specifically the survival genre, require you to interact with objects in the player's environment, like chopping down trees, picking up rocks, or harvesting berries from a bush. To do this you may need to turn those details back into GameObjects.
This code is meant to make changes to your TerrainData. This can break your terrain. It is highly recommended that you know what you're doing.
Back up your terrains. Use at your own risk.

The Toolkit offers two methods making the terrain object interactable:
Method 1 - GameObject replacement:
Using a very accurate placement system, the toolkit will place the CURRENT version of the detail or tree prototype (the prefabs you used to set up the details or tree object) on the map at the same position and rotation as it's original painted counterpart. This means any changes you've made to the prefab will now be present on the newly placed GamneObject. After ensuring the placement is correct you are able to remove the original painted details or trees (so that if you remove or move the newly placed GameObject there isn't a ghost object left behind). This method _should_ _not_ be used during runtime. This is of course the most accurate and 1:1 method. It also has the potential to fill your scene with a lot of new GameObjects. There are a few ways provided to accomplish this to make sure you can always achieve the desired results.

Method 2 - Placeholders:
The placement algorithm remains the same, so you still get high accuracy placement, but instead of placing a copy of the original prefab a GameObject that you specify will be placed instead. This should be an object that has no renderer or any kind of visible parts. Just colliders and interaction scripts should be present on the placeholder GameObject and you will still be able to see the original painted detail objects. The painted details are removed if the placeholder triggers the WasInteracted event and has Destroy() called on it. Because we are affecting pizels on the detail map, this method of removal is slightly less accurate and has the potential to remove more than one object at a time, especially if the objects are overlapping. However this has the major upside of using Unity's Terrain system to render the objects, which will feel highly optmized compared to many placed objects.

I myself use a combination of the two systems. GameObject trees, and placeholder details. It works very well.

Directions:
Step 1) Initial setup: First, select each terrain in your scene and add "Terrain Detail Converter". If you use additive scenes, they do not need to be in the active scene.

![image](https://github.com/eorvedal/Terrain-Interaction-Toolkit/assets/44689074/4af5d4f4-d7cb-4704-b120-d34440dadd07)

Place an empty GameObject in your _active_ scene and name it. Add the "Terrain Detail Converter Manager" script to it.

![image](https://github.com/eorvedal/Terrain-Interaction-Toolkit/assets/44689074/1ea00bbd-0c43-4aed-9da6-dc720b61a86e)

You're ready to make your terrain objects interactable.
Again laziness strikes me, I meant to give you a button that does this. Roadmap. 

GameObject Replacement Method: First you need to decide which layers you are going to replace. The detail layers are setup as an array that uses a zero based numbering system. This means if you are trying to replace layers 6, 7, and 8 you would need to enter this as 5, 6, and 7 into the "New Detail Layers To Replace" (this is no longer the new method, I'll fix the wording later).

![image](https://github.com/eorvedal/Terrain-Interaction-Toolkit/assets/44689074/191273f1-d877-478b-a77c-ebbd6afef985)

![image](https://github.com/eorvedal/Terrain-Interaction-Toolkit/assets/44689074/71b6fc1c-4cf6-41a6-acf6-e17d1636e8f9)

Now, make any changes you need to the original prefabs you used to set up your terrain detail layers. If you need to be reminded which prefabs they were exactly you can click on the prototype in the Edit Details like so:
![image](https://github.com/eorvedal/Terrain-Interaction-Toolkit/assets/44689074/a7a07fe8-6cbc-4fce-af9c-34fdae0fd4e7)

Once your prefabs are set up with your changes (interactivity scripts, pickup objects scripts, etc.) you are ready to replace the terrain details with these modified prefabs. In the manager's context menu choose "Convert _Customized_ Terrain Details to GameObjects (New Method)".
![image](https://github.com/eorvedal/Terrain-Interaction-Toolkit/assets/44689074/78c92d47-64a1-460d-81ce-694988a8d2b6)
Under each terrain a child object will appear called "DetailContainer". This will hold all of the spawned GameObjects. The script will now replace the instances of each layer you specified with it's original GameObjects. Save your scene(s). Mission complete!





