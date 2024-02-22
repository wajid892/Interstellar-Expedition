# __Cosmos skybox shader documentation__

__The package contains two shaders:__
- Cosmos Animated (Skybox/Cosmos Animated)
- Cosmos Animated Mesh (Unlit/Cosmos Animated Mesh)

## __Cosmos Animated__ shader
### Properties:
- ***Color*** - tint color
- ***Main Texture*** - Main skybox texture (Cubemap)
- ***Colorize*** - saturation of skybox (0 is BW, 1 is Colorful)
<br><br>
- ***Animation*** - Enable animation of the skybox (turning off can increase the performance)
<br><br>
- ***Detail textures*** - Additive 2D detail textures
- ***Intensity*** - Influence of texture (0 is no influence)
- ***Scale*** - Scale of detail texture
- ***Distortion*** - Distortion of detail texture
- ***Speed*** - Animation speed

## __Cosmos Animated Mesh__ shader
### It is same as __Cosmos Animated__ shader but for meshes