//------------------------------------------------------------------------------
// ex: getSaveFilename("~/stuff/*.*", saveStuff);
//     -- calls 'saveStuff(%filename)' on ok
//------------------------------------------------------------------------------
function getSaveFilename(%filespec, %callback, %currentFile)
{
  dEcho("getSaveFilename FileSpec = " SPC %filespec SPC "currentFile=" SPC %currentFile);

  $GuiSaveDialogCallback = %callback;   
/* orig:
  if( %filespec $= "" )
   $SaveFileExFileSpec = "*.*";
  else 
    $SaveFileExFileSpec = %filespec;
*/

 SaveFileWindowObject.text = "Save File (" SPC %filespec SPC ")";


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
               $SaveFileExFileSpec = $SaveFileExFileSpec TAB %fld;
            }
          }
          $SaveFileExFileSpec = trim ($SaveFileExFileSpec);
       } else {
         //fallback to old style: 
         $SaveFileExFileSpec = %filespec;
       }
    } else {
         //fallback to old style: 
         $SaveFileExFileSpec = %filespec;
    
    }
    //dEcho("$SaveFileExFileSpec >>" @ $SaveFileExFileSpec @ "<<"); 
    
  } //if( %filespec !$= "" )
  else {
    $SaveFileExFileSpec = "*.*";
  }
   
   
  

  //<<< file spec 
  Canvas.pushDialog(SaveFileDlg, 99);

  $SaveFileExFile = %currentFile;

   // If we have a current path, set the tree to it
   if( filePath( %currentFile ) !$= "" )
      SaveDirTreeEx.setSelectedPath( filePath( %currentFile ) );
  // else if ( $pref::Constructor::lastPath !$= "" )
  //    SaveDirTreeEx.setSelectedPath( $pref::Constructor::lastPath );
      
   // Update our file view to reflect the changes
   SaveFileListEx.setPath( SaveDirTreeEx.getSelectedPath(), $SaveFileExFileSpec );

   // Update the file edit control
   SaveFileExEdit.setText(fileName($SaveFileExFile));
}


//--------------------------------------
function DoSaveFileExCallback()
{
  %path = SaveDirTreeEx.getSelectedPath();
  %file = SaveFileExEdit.getValue();
  %cat = %path @ "/" @ %file;
  
  // MEOW
  echo(%cat);
    
  eval( $GuiSaveDialogCallback @ "(\"" @ %cat @"\");" );

  Canvas.popDialog(SaveFileDlg);
}   

function SaveDirTreeEx::onSelectPath( %this, %path )
{
   // Update our file view to reflect the changes
   SaveFileListEx.setPath( %path, $SaveFileExFileSpec  );
   
   $pref::Constructor::lastPath = %path;
}

function SaveFileListEx::onSelect( %this, %listid, %file )
{
   // Update our file name to the one selected
   SaveFileExEdit.setText( %file );
}
