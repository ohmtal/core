//------------------------------------------------------------------------------
// Common file function d
// (c) t.huehn 2009
// * movemap needs "useFunction" hack to work
// * nocalls needs bugfix (http://www.garagegames.com/community/forums/viewthread/40031)
//------------------------------------------------------------------------------
// prefs
//------------------------------------------------------------------------------
function SafeIsFile(%filename)
{
   
   %file = new FileObject();
   if (!%file.openforRead(%filename)) {
     %file.delete();
     return false;
   }
   %file.close();%file.delete();
   return true;

}


//param usual with cs extension but only works on dso and if NO cs exists!
function DSOExec(%file, %nocalls)
{
  %dsoFile=%file @ ".dso";    
  //CHECK for cs file
  if (!SafeIsFile(%dsoFile)) {
        //we have no dso maybe fresh install ? bail out here
        return false;
  }
  if (SafeIsFile(%file)) {
      //we have a dso but also a cs which should not allowed here!
      return false;
  }          
          
  exec(%file,%noCalls);
  return true;
}
//------------------------------------------------------------------------------
function SafeExport(%wildcard,%filename)
{

   %file = new FileObject();
   if (%file.openforWrite(%filename)) {
     %file.writeLine( "function load_exported_func() {" );
   }  else {
     error("Can't save file!" SPC %filename);
     %file.delete();
     return false;
   }
   %file.close();%file.delete();
   
  export(%wildcard,%filename,true);

   %file = new FileObject();
   if (%file.openforAppend(%filename)) {
     %file.writeLine( "}" );
   }
   %file.close();%file.delete();

  compile(%filename);
  fileDelete(%filename); //XXTH FIXME BREAKSANDBOX?! , true);

  return true;
}
//------------------------------------------------------------------------------
function SafeImport(%filename)
{
  if (!DSOExec(%filename, true)) {
        return false;
  }
  if (isFunction("load_exported_func"))
        load_exported_func();
}
//------------------------------------------------------------------------------
// Action map
//------------------------------------------------------------------------------
function SafeExportActionMap(%objectName, %filename)
{
  %objectName.save(%filename, false, true);
  compile(%filename);
  fileDelete(%filename, true);

  return true;

}
//------------------------------------------------------------------------------
function SafeImportActionMap( %filename)
{
  if (!DSOExec(%filename, true)) {
        return false;
  }
  if (isFunction("load_exported_func"))
        load_exported_func();

}
//------------------------------------------------------------------------------
// Load / Save mission - not usable with safe functions :(
//------------------------------------------------------------------------------
function SinglePlayer_SaveMission(%name) {
  if ($Globals::MissionSavePath $= "") 
  {
    error ("NO SavePath set!");
    return;
  }
        

 //save mission as compiled script
  %csFile = $Globals::MissionSavePath @ %name @ ".cs";
  %misFile = $Globals::MissionSavePath @ %name @ ".mis";
  // Save the State to a file...
  MissionGroup.save(%csFile);
  compile(%csFile);
  fileDelete(%csFile, true);

  //save fake mission
   %file = new FileObject();
   if (%file.openforWrite(%misFile)) {
     %file.writeLine( "exec(\"./" @ %name @ ".cs\");" );
   } else {
     error ("SinglePlayer_SaveMission - cant open file " @ %misFile );
     return 0;
   }
   %file.close();
   %file.delete();
   return 1;
}
//------------------------------------------------------------------------------
function SinglePlayer_LoadMission(%name)
{
  if ($Globals::MissionSavePath $= "") 
  {
    error ("NO SavePath set!");
    return;
  }
   if (playGui.isAwake()) {
      disconnect();
      schedule(1000,0,SinglePlayer_LoadMission,%name);
      return;
   }

   %mission = $Globals::MissionSavePath @ %name @ ".mis";
   %serverType = "SinglePlayer";

   createServer(%serverType, %mission);
   %conn = new GameConnection(ServerConnection);
   RootGroup.add(ServerConnection);
   %conn.connectLocal();
}
//------------------------------------------------------------------------------
function RTS_SinglePlayer_LoadMission(%name)
{
  if ($Globals::MissionSavePath $= "") 
  {
    error ("NO SavePath set!");
    return;
  }
   if (playGui.isAwake()) {
      disconnect();
      schedule(1000,0,RTS_SinglePlayer_LoadMission,%name);
      return;
   }

   %mission = $Globals::MissionSavePath @ %name @ ".mis";
   %serverType = "SinglePlayer";

   createServer(%serverType, %mission);
   %conn = new RTSConnection(ServerConnection);
   RootGroup.add(ServerConnection);
   %conn.connectLocal();
}
//------------------------------------------------------------------------------


