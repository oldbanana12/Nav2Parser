Originally forked from https://github.com/BobDoleOwndU/Nav2Parser

# MGS .nav2 File Parser

Parses information from MGS:TPP .nav2 files. Most of the fields within the files have been identified and can be parsed and displayed by this tool. Additionally, the navmesh geometry from within the file is extracted to an .obj file for inspection. See the [Nav2 Repository](https://github.com/oldbanana12/Nav2/) for more general information about the format.

## Basic Usage

`Nav2Parser.exe \path\to\file.nav2`

.obj and .png files representing the various data structures of the navmesh within the .nav2 file will be exported to the working directory with the same basename as the input file. You can further inspect the content of the file using the CLI interface that the program presents.

## Detailed Notes

nav2 files contain up to 5 groups numbered 0-4. All .nav2 files should have group 4 at a minimum as this is the main navmesh. The other groups numbered 0-3 are small slithers of navmesh that exist on some/all of the borders of map tiles that provide the interconnect to adjacent map tiles.

For each group in the file, Nav2Parser will produce 4 files, these are as follows:

| Filename (Where `X` is the Group ID) | Description                                                                                                                                                                                                                                                                                      |
| ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `filename`_group`X`.obj              | This is the geometry of the navmesh itself and is the most "visual" representation of the navmesh.                                                                                                                                                                                               |
| `filename`_group`X`_navworld.obj     | This is a 3D representation of the __NavWorld__ graph data structure. It contains all of the points and edges that the pathfinding algorithm likely uses to traverse the mesh.                                                                                                                   |
| `filename`_group`X`_segmentGraph.obj | This is a 3D representation of the points within the __SegmentGraph__ data structure. It contains markers of the location of each point within the graph.                                                                                                                                        |
| `filename`_navworld_graph`X`.png     | This is another representation of the __NavWorld__ data structure but with edge weightings written numerically on the edges. This isn't a particularly readable format, especially from maps with geometry layered vertically, but it can be useful for looking at the relative weight of edges. |

See the [Nav2 Repository](https://github.com/oldbanana12/Nav2/) for detailed information about what these terms mean and what the data structures are for.

For most "normal" uses, you'll likely want to load `filename_group4.obj` in Unity or Blender for inspection, but the other files can be useful for further research on the format.

Additionally Nav2Parser also produces `filename_section2.obj`. This is a partially complete 3D representation of the graph data structure found in the "Section2" structure of the nav2 file. This is probably the least well understood section of the file. So this .obj file may be useful in researching what this section is for.

Lastly, if you are interested in inspecting individual "__Segments__" of the nav2, import the `filename_group4.obj` into Blender with the "Split by Group" option checked in the import file selection window. This will break the navmesh into one object per __Segment__.

## Releases
Releases are automatically built and uploaded by GitHub actions when a tag named `v{version}` is pushed to the repository. You can download the latest build [here](https://github.com/oldbanana12/Nav2Parser/releases/latest/download/Nav2ParserX64.ZIP).