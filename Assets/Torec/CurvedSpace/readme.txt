Curved Space Shader
-----------------------------------------------------------------

This Vertex shader allows to imitate 3D space with constant positive curvature (elliptic geometry) and manipulate objects through 4D rotations.
The shader transforms mesh vertices in object space using steregraphic projection from 4D sphere surface.

Texture, normal mapping and fog are currently supported.

The shader was written for Sfera sliding puzzle game: https://bntr.itch.io/sfera

-----------------------------------------------------------------

Asset contents:
    CurvedSpace
        readme.txt                  - This readme.
        Assets
            CurvedSpace.shader      - The main vertex shader.
            RotationController.cs   - Additional module for 4d rotation control by mouse/keyboard.
        Demo
            CurvedSpaceDemo.unity   - Demo scene showing all shader features.
            CurvedSpaceDemo.cs      - Demo scene component.
            CurvedSpace_*.mat       - Shader materials for demo scene objects.
            Soccer Ball             - Low Polygon Soccer Ball by Ahmet Gencoglu: https://assetstore.unity.com/packages/3d/low-polygon-soccer-ball-84382
            Venus                   - Venus 3D Model: https://archive3d.net/?a=download&id=432795f3

-----------------------------------------------------------------

Keyboard/mouse control with RotationController module:
    Mouse drag or Keyboard cursor   - rotate the space;
    Mouse wheel                     - evert the space;
    Mouse wheel + Alt               - zoom.

-----------------------------------------------------------------

Vertex transform pipeline:
    [scene]     object's world transform (once applied to mesh vertices with Utils.ApplyWorldTransform procedure)
    [shader]    additional scale (shader "_Scale" variable)
    [shader]    inverse projection into 4D unit sphere
    [shader]    4D rotation (shader "_Rotation" variable)
    [shader]    stereographic projection back to 3d space
    [scene]     orthographic camera projection to the screen

-----------------------------------------------------------------

Demo video:
    Shader Demo - shows CurvedSpaceDemo.unity scene manipulation:
        https://www.youtube.com/watch?v=oweJ4GoBHCk
    Sfera game Demo - shows a sliding puzzle gameplay using the shader:
        https://www.youtube.com/watch?v=TtxH5m7hSvU

-----------------------------------------------------------------

December 2017

Viktor Massalogin
torec.studio@gmail.com
