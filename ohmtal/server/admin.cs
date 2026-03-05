//-----------------------------------------------------------------------------
// Copyright (c) 2007/2023 Ohmtal Game Studio
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------
//
// ADMIN CONSOLE COMMANDS
//
//-----------------------------------------------------------------------------

function shutdown() {
  endmission();
  quit();
}
//-----------------------------------------------------------------------------

function saveAllAttribs() {

    if (!isMethod("DataManager","saveAttribs"))
    {
         error("No DataManager installed!!!");
         return;
    }

   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      DataManager::saveAttribs(%cl);
   }
}

//-----------------------------------------------------------------------------
function listClients() { 
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      echo (":: Clientid:" SPC %cl SPC "Clientname:" SPC %cl.nameBase SPC "Address:" SPC %cl.getAddress());  
   }
}
//-----------------------------------------------------------------------------
function listClientsToClient(%client) { 
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      MessageClient (%client,'MsgSys','Client: %1, %2',%cl,%cl.nameBase);  
   }
}

//-----------------------------------------------------------------------------
function getClientbyName(%name) { 
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      if (%cl.nameBase $= %name) {
        echo ("::" SPC %name SPC " have clientid:" SPC %cl);
        return %cl;
      }
        
   }
   return 0;
}
//-----------------------------------------------------------------------------
function KickbyName(%name) { 
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      if (%cl.nameBase $= %name) {
        echo ("::" SPC %name SPC " kicked.");
        kick(%cl);
        return 1;
      }
   }
   return 0;
}
//-----------------------------------------------------------------------------
function KickBanbyClient(%cl) { 
      SysLog("ALERT","KICKBAN","USER BANNED:" SPC %cl.namebase);
      if (%cl.isMethod("setattrib")) {
         %cl.setattrib("banned","1");
      }
      kick(%cl);
      return 1;
}
//-----------------------------------------------------------------------------
function KickBanbyName(%name) { 
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      if (%cl.nameBase $= %name) {
        return KickBanbyClient(%cl);
      }
   }
   return 0;
}
//-----------------------------------------------------------------------------
function kick(%client)
{
   %client.delete("You have been kicked from this server");
}


function kickWithReason(%client, %reason)
{
   %client.delete(%reason);
}

//-----------------------------------------------------------------------------
function ban(%client)
{
   %client.delete("You have been banned from this server");
}
//-----------------------------------------------------------------------------

