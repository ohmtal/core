//-----------------------------------------------------------------------------
// Ohmtal Game Engine
// tom2D Sprites 
// Copyright (C) huehn-Software
//-----------------------------------------------------------------------------
function createSprite(%parentObj, %class, %imgFilename, %cells, %w, %h,  %animationFrames, %layer, %doCollision, %colRad, %tom2DCtrl) 
{

  %img = loadTexture(%imgFilename);
  %img.setParts(getWord(%cells,0),getWord(%cells,1));

  createSprite_img(%parentObj, %class, %img,  %w, %h,  %animationFrames, %layer, %doCollision, %colRad, %tom2DCtrl);
}

function createSprite_img(%parentObj, %class, %img,  %w, %h,  %animationFrames, %layer, %doCollision, %colRad, %tom2DCtrl) 
{
  if (!isObject(%parentObj.sprites)) {
    %parentObj.sprites = new SimGroup();
    GameScreen.cleanup.add(%parentObj.sprites);
  }
  
  //optional alternativ tom2dControl
  if (!isObject(%tom2DCtrl))
        %tom2DCtrl = GameScreen;
  
  %result = new tom2dSprite() {
    class  = %class;
    
    X = 0;
    Y = 0;
    Rotation = 0;
    Size = %w SPC %h;
    Layer = %layer;
    Visible = false;
    Speed  = 0;
    DirX   = 0;
    DirY   = 0;
    flipX  = false;
    flipY  = false;
    Frames = %animationFrames;
    PlayAnimation = false;
    
    SendCollision    = %doCollision;
    ReceiveCollision = %doCollision;
    CollideRadiusX = %colRad;
    CollideRadiusY = %colRad;
    
    Screen  = %tom2DCtrl;
    parent  = %parentObj;
  }; 
  %result.setTexture(%img);
  %tom2DCtrl.addSprite(%result);

  %parentObj.sprites.add(%result);
  return (%result);
}
//-----------------------------------------------------------------------------
function cleanSprites(%partenObj) {
  if (isObject(%parentObj.sprites)) {
    %parentObj.sprites.delete();
  }
}
//-----------------------------------------------------------------------------
function CloneSprite(%parentObj,%sprite, %fullClone) {

      %result = new tom2dSprite() {
          class  = %Sprite.class;
          X = 0;
          Y = 0;
          Rotation = %sprite.rotation;
          Size = %sprite.size;
          Layer = %sprite.layer;
          Speed  = 0;
          DirX   = 0;
          DirY   = 0;
          flipX  = false;
          flipY  = false;
          Frames = %sprite.Frames;
          PlayAnimation = false;
          
          SendCollision    = %sprite.SendCollision;
          ReceiveCollision = %sprite.ReceiveCollision;
          CollideRadiusX = %sprite.CollideRadiusX;
          CollideRadiusY = %sprite.CollideRadiusY;
          
          screen = %sprite.Screen; 
          parent  = %parentObj;
          
     }; 

  //clone dynamic fields and other settings
  if (%fullclone) {
    %result.X = %sprite.X;
    %result.Y = %sprite.Y;
    %result.Visible = %sprite.visible;
    %result.Speed  = %sprite.Speed;
    %result.DirX   = %sprite.DirX;
    %result.DirY   = %sprite.DirY;
    %result.flipX  = %sprite.flipX;
    %result.flipY  = %sprite.flipY;
    %result.PlayAnimation = %sprite.PlayAnimation;
    for (%i = 0; %i < %sprite.getDynamicFieldCount(); %i++)
      {
          %fieldName = %sprite.getDynamicField(%i);
          %result.setfieldValue(%fieldName,%sprite.getFieldValue(%fieldName));
      }
  }
  
  
  %result.setTexture(%sprite.getTexture());
  %sprite.Screen.addSprite(%result);


  if (isObject( %parentObj.sprites ))
      %parentObj.sprites.add(%result);
  return (%result);
}
//-----------------------------------------------------------------------------
