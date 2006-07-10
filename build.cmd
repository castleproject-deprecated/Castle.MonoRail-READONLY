SETLOCAL ENABLEEXTENSIONS
SET SVN=%1
SET NET_VER=%2

IF DEFINED SVN (
    IF "%SVN%"=="svn" "C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe" /command:update /path:".\" /notempfile /closeonend:1
    IF ERRORLEVEL 1 GOTO END
)

IF DEFINED NET_VER (
    IF "%NET_VER%"=="1.1" nant -t:net-1.1 -l:build.log -D:activerecord.test=false -D:ar.connection.connection_string.1="Data Source=.\sqlexpress;Initial Catalog=test;Integrated Security=SSPI;" -D:ar.connection.connection_string.2="Data Source=.\sqlexpress;Initial Catalog=test2;Integrated Security=SSPI;"
    IF "%NET_VER%"=="1.1d" nant -t:net-1.1 -l:build.log -D:debug="true" -D:optimize="false" -D:activerecord.test=false -D:ar.connection.connection_string.1="Data Source=.\sqlexpress;Initial Catalog=test;Integrated Security=SSPI;" -D:ar.connection.connection_string.2="Data Source=.\sqlexpress;Initial Catalog=test2;Integrated Security=SSPI;"
    IF "%NET_VER%"=="2.0" nant -t:net-2.0 -l:build.log -D:activerecord.test=false -D:brail.test=false -D:ar.connection.connection_string.1="Data Source=.\sqlexpress;Initial Catalog=test;Integrated Security=SSPI;" -D:ar.connection.connection_string.2="Data Source=.\sqlexpress;Initial Catalog=test2;Integrated Security=SSPI;"
    IF "%NET_VER%"=="2.0d" nant -t:net-2.0 -l:build.log -D:debug="true" -D:optimize="false" -D:activerecord.test=false -D:ar.connection.connection_string.1="Data Source=.\sqlexpress;Initial Catalog=test;Integrated Security=SSPI;" -D:ar.connection.connection_string.2="Data Source=.\sqlexpress;Initial Catalog=test2;Integrated Security=SSPI;"
)

:END