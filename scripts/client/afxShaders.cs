
//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~~//
// Arcane-FX - Shared AFX Shaders
//
// All of the non-zodiac shaders support alpha-fading.
//
// Copyright (C) Faust Logic, Inc.
//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~~//


//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~~//
// Zodiac Shaders

singleton ShaderData(afxZodiacTerrainShader)
{
   DXVertexShaderFile = "shaders/AFX/afxZodiac_Terrain_V.hlsl";
   DXPixelShaderFile = "shaders/AFX/afxZodiac_Terrain_P.hlsl";

   OGLVertexShaderFile = "shaders/AFX/gl/afxZodiac_Terrain_V.glsl";
   OGLPixelShaderFile = "shaders/AFX/gl/afxZodiac_Terrain_P.glsl";

   samplerNames[0] = "$zodiacMap";
   
   pixVersion = 1.1;
};

singleton ShaderData(afxZodiacInteriorShader)
{
   DXVertexShaderFile = "shaders/AFX/afxZodiac_Interior_V.hlsl";
   DXPixelShaderFile = "shaders/AFX/afxZodiac_Interior_P.hlsl";

   OGLVertexShaderFile = "shaders/AFX/gl/afxZodiac_Interior_V.glsl";
   OGLPixelShaderFile = "shaders/AFX/gl/afxZodiac_Interior_P.glsl";

   samplerNames[0] = "$zodiacMap";
   
   pixVersion = 1.1;
};

singleton ShaderData(afxZodiacPolysoupShader)
{
   DXVertexShaderFile = "shaders/AFX/afxZodiac_Polysoup_V.hlsl";
   DXPixelShaderFile = "shaders/AFX/afxZodiac_Polysoup_P.hlsl";

   OGLVertexShaderFile = "shaders/AFX/gl/afxZodiac_Polysoup_V.glsl";
   OGLPixelShaderFile = "shaders/AFX/gl/afxZodiac_Polysoup_P.glsl";

   samplerNames[0] = "$zodiacMap";
   
   pixVersion = 1.1;
};

//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~~//
//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~~//
// This Post-Effect is adapted from the resource,
// "Silhoute selection via postFX for Torque3D" posted by Konrad Kiss.
// http://www.garagegames.com/community/resources/view/17821
//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~//~~~~~~~~~~~~~~~~~~~~~//

singleton ShaderData( PFX_afxHighlightShader )
{
   DXVertexShaderFile   = "shaders/common/postFX/postFxV.hlsl";
   DXPixelShaderFile    = "shaders/AFX/afxPostFX_Highlight_P.hlsl";

   //OGLVertexShaderFile  = "shaders/common/postFX/gl//postFxV.glsl";
   //OGLPixelShaderFile   = "shaders/common/postFX/gl/passthruP.glsl";

   samplerNames[0] = "$inputTex";

   pixVersion = 2.0;
};  

singleton GFXStateBlockData( PFX_afxDefaultHighlightStateBlock )
{  
   zDefined = true;
   zEnable = false;
   zWriteEnable = false;

   samplersDefined = true;
   samplerStates[0] = SamplerClampLinear;
};

singleton PostEffect( afxHighlightPostFX )
{
   // Do not allow the selection effect to work in reflection 
   // passes by default so we don't do the extra drawing.
   allowReflectPass = false;
                  
   renderTime = "PFXAfterDiffuse";
   renderBin = "HighlightBin";
   renderPriority = 1;
   enabled = true;

   shader = PFX_afxHighlightShader;
   stateBlock = PFX_afxDefaultHighlightStateBlock;
   texture[0] = "#highlight";
   texture[1] = "$backBuffer";
   target = "$backBuffer";
};
