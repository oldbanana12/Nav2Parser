Originally forked from https://github.com/BobDoleOwndU/Nav2Parser

# MGS .nav2 File Parser

Parses information from MGS:TPP .nav2 files. Most of the fields within the files have been identified and can be parsed and displayed by this tool. Additionally, the navmesh geometry from within the file is extracted to an .obj file for inspection. See the [Nav2 Repository](https://github.com/oldbanana12/Nav2/) for more general information about the format.

## Usage

`Nav2Parser.exe \path\to\file.nav2`

.obj files representing the various pieces of the navmesh within the .nav2 file will be exported to the working directory with the same basename as the input file. You can further inspect the content of the file using the CLI interface that the program presents.