//-----------------------------------------------------------------------------
// HTTP Class
// RFC 2616
// T.Huehn 2009 
//-----------------------------------------------------------------------------

/* Docu
  Callback is called when it's finished. 
  You have not much time to work with the socketobject because it will be 
  deleted soon if $pref::http::DeleteOnDone = true.
  So maybe it's better to finish all work and set it to false.
  Because of a limitation of the TCPObject/Class it is only possible to have
  one object active. (limitation => Must name the object to get the callbacks)
  Possible fix would using a scriptobject instead of a simobject, which need
  a modification in TCP Object. But for my needs one at same time is ok.
  You can call multiple get/post but others will wait until the prior is 
  finished.
  
  Content:
  ========
      contentLineCount => count of lines
      contentX    => where X is the linenumber
      header      => full header of answer
      
  Status:
  ======
        connecting  - socket try to connect
        connected   - socket successfully connected - will send data
        done        - done - socket closed 
        failed      - failed - .. to connect or dns failed 
*/

/* EXAMPLE:
exec("common/shared/http.cs"); $HTTPDEBUG = true;

function MyHTTPCallBack(%sock) { if (!%sock.status $="failed") error("CONNECTION FAILED!"); else for (%i=0;%i<%sock.contentLineCount;%i++) echo (( %sock.content[%i] )); error ("Callback done"); }
HttpPost("www2.huehn.int:80","/testpost.php","data=Bla",MyHTTPCallBack);
HttpGet("www.huehn.int:80","/","",MyHTTPCallBack);
HttpGet("www.huehn.int:80","/","","");
*/

/* Docu Variables:
$pref::http::UserAgent = "Mozilla/3.0 (compatible)";
$pref::http::DeleteOnDone = true; //let the callback delete it or deleteit in class
*/


if ($pref::http::UserAgent $= "")
        $pref::http::UserAgent = "Mozilla/3.0 (compatible)"; 

if ($pref::http::DeleteOnDone $= "")
        $pref::http::DeleteOnDone = true; 
        

//------------------------------------------------------------------------------
$CRLF = "\r\n";
$HTTPDEBUG = false;
//------------------------------------------------------------------------------
function HttpCreateSocket(%server,%path,%data,%callback)
{
  //server is like bla.huehn.int:25 so we split it internally, which we do need
  //for namebased servers!
  %posDelim = strpos(%server, ":");
  %len = strlen(%server);
  %hostname = getSubStr (%server, 0, %posDelim );
  %port     =  getSubStr (%server, %posDelim + 1, %len - %posDelim);
  

 %con = new TCPObject(HTTPSOCK) {
   hostname  = %hostname;
   port      = %port;
   path      = %path;
   postdata  = %data;
   callback  = %callback;
   status    = "connecting";
   buf       = ""; //here we keep all lines
   header    = ""; //header will be here
   contentLineCount   = 0; //content will be here
   headerdone = false; //helper to split header and content
 };
// %con.setFIFOMode();
 %con.connect(%server);
 
 return %con;

}
//------------------------------------------------------------------------------
function HttpGet(%server,%path,%data,%callback)
{

  if (isObject(HTTPSOCK)) { 
    schedule(10000,0,HttpGet,%server,%path,%data,%callback);
    return;
  }

  %con = HttpCreateSocket(%server,%path,%data,%callback);
  %con.mode = "get";
  if ($HTTPDEBUG) echo ("HTTP::DEBUG::GET::CON" SPC %con);
  return %con;
}
//------------------------------------------------------------------------------
function HttpPost(%server,%path,%data,%callback)
{
  if (isObject(HTTPSOCK)) { 
    schedule(10000,0,HttpPost,%server,%path,%data,%callback);
    return;
  }
  %con = HttpCreateSocket(%server,%path,%data,%callback);
  %con.mode = "post";
  if ($HTTPDEBUG) echo ("HTTP::DEBUG::POST::CON" SPC %con);
  return %con;
}
//------------------------------------------------------------------------------
// HTTPSOCK CLASS
//------------------------------------------------------------------------------
function HTTPSOCK::onConnected(%this)
{
  if ($HTTPDEBUG) echo ("HTTP::DEBUG::onConnected");
  %this.status = "connected";
  if (%this.mode $= "get")
        %this.SendGet();
  else
        %this.SendPost();
}
//-----------------------------------------------------------------------------
function HTTPSOCK::sendGet(%this)
{
  %header=         
             "GET " @ %this.path @ " HTTP/1.1" @ $CRLF
           @ "Host: " @ %this.hostname @ $CRLF
           @ "Content-type: application/x-www-form-urlencoded" @ $CRLF 
           @ "Connection: Close" @ $CRLF
           @ "User-Agent: " @ $pref::http::UserAgent @ $CRLF
           @ $CRLF
           ;

 %this.send(%header);
 if ($HTTPDEBUG) echo ("HTTP::DEBUG::GET::SEND\n" SPC %header);
           
}
//-----------------------------------------------------------------------------
function HTTPSOCK::sendPost(%this)
{
  if (%this.postdata $="")
        return;
  
  %header=         
             "POST " @ %this.path @ " HTTP/1.1" @ $CRLF
           @ "Host: " @ %this.hostname @ $CRLF
           @ "Content-type: application/x-www-form-urlencoded" @ $CRLF 
           @ "Connection: Close" @ $CRLF
           @ "User-Agent: " @ $pref::http::UserAgent @ $CRLF
           @ "Content-length: " @ strlen(%this.postdata) @ $CRLF
           @ $CRLF
           @ %this.postdata @ $CRLF
           ;

 %this.send(%header);
  if ($HTTPDEBUG) echo ("HTTP::DEBUG::POST::SEND" SPC %header);

}
//-----------------------------------------------------------------------------
function HTTPSOCK::onDisconnect(%this)
{
   // error("You have been disconnected.");
   %this.status = "done";
   if (%this.callback !$= "")
      schedule(0,0,%this.callback,%this);
      
   if ($HTTPDEBUG) {
      echo ("HTTP::DEBUG::POST::DISCONNECT");
      //%this.dump();
   }
      
   if ($pref::http::DeleteOnDone)
        %this.schedule(100,"delete");
   
}
//-----------------------------------------------------------------------------
function HTTPSOCK::onConnectFailed(%this)
{
   error("HTTP - Connection to server failed! SERVER:" SPC %this.hostname @ ":" @ %this.port);
   %this.status = "failed";
   if (%this.callback !$= "")
      schedule(0,0,%this.callback,%this);
   if ($HTTPDEBUG) {
      //%this.dump();
   }
   if ($pref::http::DeleteOnDone)
        %this.schedule(100,"delete");
}

function HTTPSOCK::onDNSFailed(%this)
{
  %this.onConnectFailed();
}

//-----------------------------------------------------------------------------
/* no need not finished!
function HTTPSOCK::onData(%this, %chunk)
{
error("CHUNK-------------------->"); echo (%chunk);
  if (!%this.headerDone)
  {
    %headerEnd = strpos(%chunk,$CRLF@$CRLF);
    if (%headerEnd < 0)
        %this.header = %this.header @ %chunk;
    } else {
        if (strpos(%chunk,$CRLF@$CRLF)
          ...
    }
    
  } else {
    %this.content = %this.content @ %chunk;
  }
}
*/
//-----------------------------------------------------------------------------
function HTTPSOCK::onLine(%this, %line)
{
  if ($HTTPDEBUG) echo ("HTTP::DEBUG::LINE:" SPC %line);
  if (!%this.headerdone)
  {
     if (%line $= "")
        %this.headerdone = true;
     else
        %this.header = %this.header @ %line @ $CRLF;
  } else {
        
        %this.content[%this.contentLineCount] = %line @ $CRLF;
        %this.contentLineCount++;        
  }
}

