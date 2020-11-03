protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

XCOPY /Y Protocol.cs "../../../MMOClient/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../MMOServer/MMOServer/Packet"