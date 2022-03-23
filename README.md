# SallyProject

## Setting up for development
The unity project uses version 2022.1.0b8 so make sure you download the proper unity installation.

All unity versions can be found here:
https://unity3d.com/get-unity/download/archive

Download and use unity hub to manage all of your unity installations/projects:
https://unity3d.com/get-unity/download

In unity hub, add an existing project by clicking add and navigate to where you downloaded this repository. Make sure you're in the root directory where you can see the 'Assets' folder when you click open.



## Developments

### Creating a particle mesh of a person
Unity has a useful tool called the point cache bake tool (Window>Visual Effects>Utilities>Point Cache Bake Tool) where you input a mesh and it exports a set of points that can be used by VFX graphs.
This was used to create a point graph of a person which was imported into a custom VFX graph.
Here's a tutorial explaining most of the process: https://www.youtube.com/watch?v=j1R1Uelroco&t=101s

Note: Point cache bake tool works best with meshes from .obj files. Problems were found using it with .fbx files.

Result:
https://i.gyazo.com/57d7139cc6e2147305202bb5d32905bc.mp4
https://i.gyazo.com/11aba9749615c5e4b03e06edf9e3484e.mp4
