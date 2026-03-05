//------------------------------------------------------------------------------
// EnvCache Utils
// 2009 T.Huehn 
//------------------------------------------------------------------------------
// Save envcache file to special env file.
//------------------------------------------------------------------------------




function EnvCache::addObject(%obj, %toFile) {

echo ("* " SPC %obj.getId() SPC %obj.getClassName());
  %objName = "";
$envObjCnt++;
  %tmpFile = $MyPref::Cache::TmpPath @ "tmpobj.txt";
  if (%obj.getClassName() $= "TerrainBlock")
  {
        %obj.SaveSim(%tmpFile);
        %objName = "Terrain";
  } else {
        %obj.Save(%tmpFile);
  }
  %foundCLObj = false;
  
  %file = new FileObject();
  if(%file.openForRead(%tmpfile))
  {
      //make sure name is not saved!
      %tofile.writeLine("new " @ %obj.getClassName() @ "(" SPC %objName SPC ") {");
       while(!%file.isEOF())
      {
        %tmpline=%file.readLine();
       //SKIP:
       if (%tmpline $= "//--- OBJECT WRITE BEGIN ---" 
           || %tmpline $= "//--- OBJECT WRITE END ---" 
           || getSubStr(%tmpline, 0, 4) $= "new "

          ) continue;
        
       if (%tmpLine $= "   EnvCacheClientObject = \"0\";") {
               %tmpLine = "   EnvCacheClientObject = \"1\";";
               %foundCLObj = true;
       }
       if ((%tmpLine $="};" && !%foundCLObj)) {
               %tofile.writeLine( "   EnvCacheClientObject = \"1\";" );
               %foundCLObj = true;
       }
       %tofile.writeLine(%tmpline);
      } //while
  
      %file.delete();
  } else {
    error ("EnvCache::addObject CANT OPEN TMPFILE FOR OBJECT!!! " SPC %obj.getId() SPC %obj.getClassName());
  }

}
//------------------------------------------------------------------------------
function EnvCache::ProcessGroup(%grp, %toFile) {

  %cnt = %grp.getCount(); 
  for (%i=0; %i<%cnt; %i++) {
    %obj = %grp.getObject(%i);
    switch$ (%obj.getClassName())
    {
      case "SimGroup":
                EnvCache::ProcessGroup(%obj, %toFile);
      case "TSStatic":
                EnvCache::addObject(%obj,%toFile);
/*                
      case "fxShapeReplicator":
                EnvCache::addObject(%obj,%toFile);
*/                
      case "fxFoliageReplicator":
                EnvCache::addObject(%obj,%toFile);
      case "fxSquareFoliageReplicator":
                EnvCache::addObject(%obj,%toFile);
      case "fxGrassReplicator":
                EnvCache::addObject(%obj,%toFile);
      case "fxGrassObject":
                EnvCache::addObject(%obj,%toFile);
/* eeek idk why but it sucks!!! does not detect water correct!
*/
/* 2.1 DO NEVER CACHE WATERBLOCK!!!! */
      case "Waterblock":
                EnvCache::addObject(%obj,%toFile);
              
      case "fxSunLight":
                EnvCache::addObject(%obj,%toFile);
      case "Sky":
                EnvCache::addObject(%obj,%toFile);
      case "Sun":
                EnvCache::addObject(%obj,%toFile);

      case "TerrainBlock":
                EnvCache::addObject(%obj,%toFile);
     
      case "InteriorInstance":          
                EnvCache::addObject(%obj,%toFile);
                
      case "TSDynamic":
                EnvCache::addObject(%obj,%toFile);

      case "fxPlane":
                EnvCache::addObject(%obj,%toFile);
                
/* sux!                
      case "Precipitation":
                EnvCache::addObject(%obj,%toFile);
*/                
    
    } //switch
  
  } //for

}
//------------------------------------------------------------------------------
function EnvCache::Save(%grp, %MissionFile) 
{
  if (!$OGE_ENVCACHE) 
        return;
  echo ("=========================================================");
  echo (" Export Env Objects for Mission" SPC FileBase(%MissionFile));
  echo ("=========================================================");
  
  if (!$pref::CleanMarker) {
     error("EnvCache::Save - cant save!  NO IN EDITOR MODE ! " );
     return;
  }
        
  
  $envObjCnt=0;
  %toFileName = $MyPref::Cache::Path @ FileBase(%MissionFile) @ ".env.cs";  
  %toFile = new FileObject();
  
  if(%toFile.openForWrite(%toFileName)) {
    EnvCache::ProcessGroup(%grp, %toFile);
    %toFile.close();
    compile(%toFileName);
  } else { 
        error("EnvCache::Save :: Cant open file for write:" SPC %toFileName );
  }
  %toFile.delete();
  echo (" Objects exported: " SPC $envObjCnt);
  echo ("=========================================================");
  $envObjCnt="";
 
}
//------------------------------------------------------------------------------
function EnvCache::BatchSave(%Path, %missionList)
{
  if ( $pref::client::devel !$="1" ) return;
  if ($game::running) {
        Error("EnvCache::BatchSave - CAN't do it while game is running!!!! !");
        return;
  }
  %cnt = getWordCount(%missionList);
  if (%cnt == 0 || %path $= "") {
        Error("EnvCache::BatchSave - Hey, you should give me a path and a space separated list with mission names and only the Name without suffix and path!!!");
        return;
  
  }
  $pref::CleanMarker = true;
  for (%i = 0; %i< %cnt; %i++)
  {
    $client::MissionFile = %path @ "/" @ getWord(%missionList,%i) @ ".mis";
    if (!isFile($client::MissionFile)) {
        %error = "EnvCache::BatchSave - skiping wrong mission ! mission:" SPC $client::MissionFile;
        error ( %error );
        if ($ScriptErrorHash == 0)
                $ScriptErrorHash = 1000;
        else
                $ScriptErrorHash++;
        $ScriptError=$ScriptError @ %error;
        updateConsoleErrorWindow();
        continue;
    }
    createServer("SinglePlayer", $client::MissionFile);
    EnvCache::Save(MissionGroup, $client::MissionFile);
    endMission();
  }
  


}
//------------------------------------------------------------------------------
function EnvCache::Load(%MissionFile) 
{
  if (!$OGE_ENVCACHE) {
      error("Cant load EnvCache .... not implemented!!!!"); 
      return;
  }

  if ($pref::CleanMarker) 
        return;
  echo ("=========================================================");
  echo (" Loading Enviroment cache for Mission" SPC FileBase(%MissionFile));
  echo ("=========================================================");
  
  %FileName = $MyPref::Cache::Path @ FileBase(%MissionFile) @ ".env.cs";
  
  exec(%FileName);
  
}
//------------------------------------------------------------------------------
// Client DB Cache Functions
//------------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Create and Modify an DBCache Client File on SERVER !!!
// This file must be transfered to Client... AND A CRC CHECK MUST BE DONE 
//-----------------------------------------------------------------------------
function CreateClientDBCache()
{
   if (!$Server::Dedicated) {
    error("CreateClientDBCache::Server must be dedicated!");
    return;
   }
   %filename=$MyPref::Cache::Path @ $MyPref::Cache::ClientDBFile;
   %tmpfile=$MyPref::Cache::TmpPath @ "tmpClientDB.txt";
   
   datablockgroup.save(%tmpfile);

   %file = new FileObject();
   if(%file.openForRead(%tmpfile))
   {
      %tofile = new FileObject();
      %tofile.openforWrite(%filename);
      %adddynstring = "";
     while(!%file.isEOF())
      {
      
        %tmpline=%file.readLine();
       //SKIP:
       if (%tmpline $= "//--- OBJECT WRITE BEGIN ---" ||
           %tmpline $= "new SimGroup(DataBlockGroup) {" ||
           %tmpline $= "   canSaveDynamicFields = \"1\";" ||
           %tmpline $= "//--- OBJECT WRITE END ---" ||
           %tmpline $= "};"
          ) continue;
        
       %pre=getSubStr(%tmpline,0,6);
       if (%pre $= "   new") {
       
         %objname = "";
         %needle = strpos (%tmpline,"(") + 1;
         %needleclose =  strpos (%tmpline,")");
         if (%needle >=0 && %needleclose > %needle) { 
                %objname = trim(getSubstr(%tmpline,%needle,%needleclose - %needle));
                if ($DBDynParams[%objname] !$= "") {
                   %adddynstring = $DBDynParams[%objname];
                }
                echo ("* ADDING OBJECT " SPC %objname);
         } else {
           echo("BAD LINE ?!" SPC %needle SPC %needleclose SPC %tmpline);
         }
         
         %tmpline=strreplace(%tmpline,"new","datablock");
       }

       
           
        if (%adddynstring !$= "" && trim(%tmpline) $= "};") {
           echo ("* ADDING DYNSTR FOR OBJECT " SPC %objname);        
           %tofile.writeLine(%adddynstring);
           %adddynstring = "";
        }
        
       %pre=getSubStr(%tmpline,0,9);
//XXTH 1.97 I also need the dynamic fields!        if (%pre !$= "         ")
       %tofile.writeLine(%tmpline);
           
      }
   } else {
     error ("CreateClientDBCache - cant open file " @ %tmpfile );
     return 0;
  }
   %file.close();
   %file.delete();
   
   // Create Itemdefs:
   if (!isObject(ItemListGroup)) {
     new SimGroup(ItemListGroup);
     %tmpstr="";
     for (%i=0;%i<RootGroup.getcount();%i++) {
        %obj=RootGroup.getObject(%i);
        if (isObject(%obj) && %obj.getClassName() $= "SimObject" && %obj.classname $= "CustomItem") {
                %tmpstr=%tmpstr SPC %obj.getName();
        }
     }
     for (%i=0; %i<getWordCount(%tmpstr); %i++) {
        %obj = getWord(%tmpstr,%i);
        if (isObject(%obj)) ItemListGroup.Add(%obj);
     }
     %tmpstr="";
     
   } else {
     error ("ItemListGroup still exists!!!!");
   }

   
  // Write Itemdefs here !!  
   %tmpfile=$MyPref::Cache::TmpPath @ "tmpItems.txt";
   ItemListGroup.save(%tmpfile); 
   %file = new FileObject();
   if(%file.openForRead(%tmpfile))  {
     while(!%file.isEOF())
      {
        %tmpline=%file.readLine();
        %tofile.writeLine(%tmpline);
      }
   } else {
     error ("CreateClientDBCache::ITEMS - cant open file " @ %tmpfile );
     return 0;
  }
  %file.close();
  %file.delete();
  
  //1.98 save quest here!!
  %tmpfile=$MyPref::Cache::TmpPath @ "tmpQuests.txt";
  Quest::load(true);
  QuestGroup.save(%tmpfile);
  %file = new FileObject();
  if(%file.openForRead(%tmpfile))  {
     while(!%file.isEOF())
      {
        %tmpline=%file.readLine();
        %tofile.writeLine(%tmpline);
      }
   } else {
     error ("CreateClientDBCache::QUEST - cant open file " @ %tmpfile );
     return 0;
  }
  %file.close();
  %file.delete();
  
  
   
  %tofile.close();
  %tofile.delete();
   
  compile(%filename);

 $Server::ClientDBCacheCRC=DBCache::getCrc();
 echo ("* DONE");
 return 1;
}
//-----------------------------------------------------------------------------
function ExportItemNames() {
   if (!$Server::Dedicated) {
    error("CreateClientDBCache::Server must be dedicated!");
    return;
   }
   %filename = $MyPref::Cache::TmpPath @ "itemlookup.txt";
   // Create Itemdefs:
      %tofile = new FileObject();
      %tofile.openforWrite(%filename);
   
     %tmpstr="";
     for (%i=0;%i<RootGroup.getcount();%i++) {
        %obj=RootGroup.getObject(%i);
        if (isObject(%obj) && %obj.getClassName() $= "SimObject" && %obj.classname $= "CustomItem") {
                %tofile.writeLine(%obj.getName());
        }
     }
     for (%i=0; %i<getWordCount(%tmpstr); %i++) {
        %obj = getWord(%tmpstr,%i);
        if (isObject(%obj)) ItemListGroup.Add(%obj);
     }
     %tmpstr="";
    
   %tofile.close();
   error("LIST SAVED TO" SPC %filename); 
}
//-----------------------------------------------------------------------------
function DBCache::getCrc() 
{
  return getFileCRC($MyPref::Cache::Path @ $MyPref::Cache::ClientDBFile @ ".dso");
}
//-----------------------------------------------------------------------------
function DBCache::Load() 
{
  return exec($MyPref::Cache::Path @ $MyPref::Cache::ClientDBFile);
}

