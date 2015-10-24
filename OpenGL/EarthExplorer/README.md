# EarthExplorer

**EarthExplorer** uses OpenGL to render terrain on globe from NASA elevation data files (*.hgt).  

This repository contains a few HGT files, but you can find more here: http://www.viewfinderpanoramas.org/Coverage%20map%20viewfinderpanoramas_org3.htm

## Features
- two cameras (press F to switch, fly with WSAD)
- two modes - map and fly mode (press V to switch)
- level of details - you can set details level (1-9 keys) or set auto (0 key)

## Remarks
This project was created as an excercise on my university. It's rather a simple implementation, just to learn how to use OpenGL. It's quite slow, regardless implemented culling.

## Compilation
To compile unzip `lib.zip` first. To launch the application copy `EarthExplorer\Shaders` and `EarthExplorer\glew32.dll` to the directory with `EarthExplorer.exe`.

Sample command:  
`EarthExplorer.exe -mdir ..\hgt_files`
