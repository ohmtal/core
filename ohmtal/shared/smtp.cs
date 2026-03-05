//-----------------------------------------------------------------------------
// SMTP Class
// RFC 821
// T.Huehn 2008 
//-----------------------------------------------------------------------------


/* EXAMPLE:
exec("common/shared/smtp.cs");
$pref::smtp::server = "mail.huehn.int:25"; $pref::smtp::hostname = "tom.huehn.int";  
sendmail("tom@huehn.int","tom@huehn.int","Some news you should know", "Someone nasty hacked your Server ! :P");
*/


/* Docu Variables:
$pref::smtp::server = ""; //set the smtpserver for use
$pref::smtp::hostname = ""; // Set a hostname for helo
*/
//------------------------------------------------------------------------------
$CRLF = "\r\n";
$SMTPDEBUG = false;
//------------------------------------------------------------------------------
function SendMail(%from,%to,%subject,%message) {

 if (isObject(SMTP)) { //mail just want to go out we schedule the next a bit
   schedule(10000,0,sendmail,%to,%subject,%message);
   return;
 }

 if ($pref::smtp::server $= "" || !$pref::smtp::hostname $="" ) {
  error("SendMail - Missing Configuration ! Cant send mail to " @ %to @ " Message:" SPC %message);
  return;
 }
 
 %msg= %message @ $CRLF @ "." @ $CRLF;
 
 %msg = 
        "From:" SPC %from @ $CRLF @
        "To:" SPC %to @ $CRLF @
        "Subject:" SPC %subject @ $CRLF @
        %message @ $CRLF @ ".";
 
 

 %con = new TCPObject(SMTP) {
   from  = %from;
   to    = %to;
   msg   = %msg;
   status = "connect";
 };
 %con.connect($pref::smtp::server);

  if ($SMTPDEBUG) echo ("SMTP::DEBUG::CON" SPC %con);

}
//------------------------------------------------------------------------------
// SMTP CLASS
//------------------------------------------------------------------------------
function SMTP::onConnected(%this)
{
  if ($SMTPDEBUG) echo ("SMTP::DEBUG::onConnected" SPC %line);  
}
//-----------------------------------------------------------------------------
function SMTP::onDisconnect(%this)
{
   // error("You have been disconnected.");
   %this.schedule(10,"delete");
}
//-----------------------------------------------------------------------------
function SMTP::onConnectFailed(%this)
{
   error("SMTP - Connection to server failed! SERVER:" SPC $pref::smtp::server);
   %this.schedule(10,"delete");
}
//-----------------------------------------------------------------------------
function SMTP::doSend(%this,%line) {
  if ($SMTPDEBUG) echo ("SMTP::DEBUG::SEND" SPC %line);   
  %this.send(%line @ $CRLF);
}
//-----------------------------------------------------------------------------
function SMTP::onLine(%this, %line)
{
   %line=trim(%line);
   %rescode=getWord(%line,0);

   if (  (%this.status !$= "data" && getSubstr(%rescode,0,1) !$= "2") 
       || (%this.status $= "data" && getSubstr(%rescode,0,1) !$= "3") ) {
     Error("SMTP - Got error: " SPC %line);
     %this.schedule(10,"delete");
   }
   
   if ($SMTPDEBUG) echo ("SMTP::DEBUG::RCV" SPC %line);   
   
   switch$ (%this.status ) {
    case "connect":
           %this.dosend("HELO" SPC $pref::smtp::hostname); 
           %this.status = "helo";
    case "helo":
           %this.dosend("MAIL FROM:<" @ %this.from @ ">");
           %this.status = "mail from";
    case "mail from":
           %this.dosend("RCPT TO:<" @ %this.to @ ">");
           %this.status = "rcpt to";
    case "rcpt to":
           %this.dosend("DATA");
           %this.status = "data";
    case "data":
           %this.dosend(%this.msg);
           %this.status = "done";
    case "done":
           %this.dosend("QUIT");
           %this.status = "quit";
    case "quit":
           //nothing to do here server should disconnect
   }
}
