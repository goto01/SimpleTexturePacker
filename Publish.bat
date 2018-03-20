::set texturePacker="d:/Soft/Texture packer/bin/TexturePacker"
set texturePacker=%1
::set destinationTexture="d:/Unity/SimpleTexturePacker/Simple Texture Packer/Assets/Textures/texting{n}.png"
set destinationTexture=%2
::set destinationData="d:/Unity/SimpleTexturePacker/Simple Texture Packer/Assets/Textures/texting{n}.json"
set destinationData=%3
::set sourceFolder="d:/Unity/SimpleTexturePacker/Characters/"
set sourceFolder=%4
%texturePacker% --extrude 0 --algorithm Basic --trim-mode None --png-opt-level 0 --disable-auto-alias --sheet %destinationTexture% --data %destinationData% --format json-array %sourceFolder%