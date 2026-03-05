//------------------------------------------------------------------------------
// LoadFileDlg
// DTS/DAE Preview set position with OpenDialogPreviewTS.camPos = "50 50 50";
//------------------------------------------------------------------------------

function LoadFileDlg::onWake(%this)
{

   %this.shape = "";
   %this.sun   = "";
   if (!isObject(ClientMissionCleanup))
   {
       %obj = new SimGroup(ClientMissionCleanup);
       
      //XXTH added CoreCleanUp
      if (!isObject(CoreCleanUp))
            new SimSet(CoreCleanUp);
       
       
       CoreCleanUp.add(%obj);
   }
}


function LoadFileDlg::onSleep(%this)
{
   if (isObject(%this.shape))
      %this.shape.delete();
      
   if (isObject(%this.Sun))
      %this.Sun.delete();

   %this.shape = "";
   %this.sun   = "";
      
}


//------------------------------------------------------------------------------
function OpenDialogPreviewTS::zoom(%this, %value)
{
   if (!isObject(LoadFileDlg.shape))
      return;
      
   %pos = LoadFileDlg.shape.getPosition();  
   %y   = getword(%pos,1) * 1 + %value;
   %pos = setWord(%pos,1,%y);
   LoadFileDlg.shape.setPosition(%pos);
} 
function OpenDialogPreviewTS::rotate(%this, %value)
{
   if (!isObject(LoadFileDlg.shape))
      return;
      
   %rot = LoadFileDlg.shape.rotation;  
   %a   = getword(%rot,3) * 1 + %value ;
   %newRot = setWord(%rot,3,%a);
   LoadFileDlg.shape.rotation = %newRot;

//   echo(" ROT=" SPC %rot 
//          SPC "a=" SPC %a
//          SPC "new Rot = " SPC %newRot SPC "\n"    
//          SPC "final = " SPC LoadFileDlg.shape.rotation
//         ); 
   
   
} 
//------------------------------------------------------------------------------
// ex: getLoadFilename("~/stuff/*.*", openStuff);
//     -- calls 'openStuff(%filename)' on dblclick or ok
//------------------------------------------------------------------------------
function getLoadFilename(%filespec, %callback, %currentFile)
{
  $GuiLoadDialogCallback = %callback;
  dEcho("getLoadFilename FileSpec = " SPC %filespec SPC "currentFile=" SPC %currentFile);
  
  //clear preview
   LoadBitmapPreview.bitmap="";
   
  LoadFileWindowObject.text = "Open File (" SPC %filespec SPC ")";
  
  $LoadFileExFileSpec = "";
   
  if( %filespec !$= "" )
  {
    if (strPos(%filespec,"|") > 0)
    {
       //handle DingsFile|*.dings
       %tab = "" TAB "";
       %tmpStr = strreplace(%filespec,"|",%tab);
       %cnt = getFieldCount(%tmpStr);
       if (%cnt > 1) 
       {
      //"Image Files (*.png, *.jpg, *.dds, *.bmp, *.gif, *.jng. *.tga)|*.png;*.jpg;*.dds;*.bmp;*.gif;*.jng;*.tga|All Files (*.*)|*.*|";
          //dEcho("COUNT:" SPC %cnt SPC "tmpStr=" SPC %tmpStr);
          //need . 1 . 3 . 
          for (%i = 1; %i < %cnt; %i+=2)
          {
            %fld = getField(%tmpStr, %i);
            if (strPos(%fld,";") > 0)
               %fld = strreplace(%fld,";","" TAB "");
            if (%fld !$= "") 
            {
               $LoadFileExFileSpec = $LoadFileExFileSpec TAB %fld;
            }
          }
          $LoadFileExFileSpec = trim ($LoadFileExFileSpec);
       } else {
         //fallback to old style: 
         $LoadFileExFileSpec = %filespec;
       }
    } else {
         //fallback to old style: 
         $LoadFileExFileSpec = %filespec;
    
    }
    //dEcho("$LoadFileExFileSpec >>" @ $LoadFileExFileSpec @ "<<"); 
    
  } //if( %filespec !$= "" )
  else {
    $LoadFileExFileSpec = "*.*";
  }
   
   
   LoadBitmapPreview.visible = false;
   OpenDialogPreviewTS.visible = false;

  Canvas.pushDialog(LoadFileDlg, 99);

  $LoadFileExFile = %currentFile;

   // If we have a current path, set the tree to it
   if( filePath( %currentFile ) !$= "" )
      LoadDirTreeEx.setSelectedPath( filePath( %currentFile ) );
      
      
   // Update our file view to reflect the changes
   LoadFileListEx.setPath( LoadDirTreeEx.getSelectedPath(), $LoadFileExFileSpec );
}


//--------------------------------------
function DoOpenFileExCallback()
{
  %path = LoadDirTreeEx.getSelectedPath();
  %file = LoadFileListEx.getSelectedFile();
  %cat = %path @ "/" @ %file;
  
  // MEOW
  eval( $GuiLoadDialogCallback @ "(\"" @ %cat @"\");" );

  Canvas.popDialog(LoadFileDlg);
}   

//--------------------------------------
function LoadDirTreeEx::onSelectPath( %this, %path )
{
   //dEcho("LoadDirTreeEx::onSelectPath =>" SPC %path); 

   // Update our file view to reflect the changes
   LoadFileListEx.setPath( %path, $LoadFileExFileSpec  );
}

//--------------------------------------


function LoadFileListEx::onSelect(%this, %index, %itemText)
{
  

  //decho("LoadFileListEx::onSelect" SPC %index SPC %itemText SPC %file); 

  %file = LoadFileListEx.getSelectedFile();
  %path = LoadDirTreeEx.getSelectedPath();
  %ext  = getFileExt(%file);
  %fullFilename = %path @ "/" @ %file;

 //clear preview
   LoadBitmapPreview.visible = false;
   OpenDialogPreviewTS.visible = false;
   LoadBitmapPreview.bitmap="";
   if (isObject(LoadFileDlg.shape))
   {
      LoadFileDlg.shape.delete();
   }   
  
  // LoadBitmapPreview GuiBitmapCtrl  
  %previewExt = "png jpg jpeg dds bmp gif jng tga";
  if (strPos(%previewExt,%ext) >= 0)
  {
      dEcho("********** preview image loading:" SPC %fullFilename); 
       
      LoadBitmapPreview.setBitmap(%fullFilename);
      LoadBitmapPreview.visible = true;

      return;
  }
  
  %previewExt = "dts dae";
  // LoadObjectPreview GuiObjectView
  if (strPos(%previewExt,%ext) >= 0)
  {
      dEcho("********** preview object loading:" SPC %fullFilename);
      %pos = VectorAdd(OpenDialogPreviewTS.CamPos,"0 0 -1");
      LoadFileDlg.shape = new ClientTsStatic() {
         shapename = %fullFilename;
         position  = %pos;
         rotation=" 0 0 1 125";
      };
      CoreCleanUp.add(LoadFileDlg.shape); //CoreCleanup \o/
      
      if (!isObject(LoadFileDlg.Sun))
      {
         LoadFileDlg.Sun = new Sun() {
            EnvCacheClientObject = "1";
            azimuth = "0";
            elevation = "60";
            color = "0.715694 0.708376 0.708376 1";
            ambient = "0.822786 0.822786 0.838799 1";
            brightness = "2";
            castShadows = "0";
          };
         CoreCleanUp.add(LoadFileDlg.Sun); //CoreCleanup \o/
      }
      
      %box = LoadFileDlg.shape.getObjectBox();
      %size = (getWord(%box,3) + getWord(%box,4) + getWord(%box,5)) * 2;
      LoadFileDlg.shape.setPosition(setWord(LoadFileDlg.shape.position,1,
                                 getWord(LoadFileDlg.shape.position,1) + %size));
      OpenDialogPreviewTS.visible = true;
      
      return;
  }
  
  
  

}

//--------------------------------------

function LoadFileListEx::onDoubleClick(%this)
{
//sucks :(   DoOpenFileExCallback();   
}
