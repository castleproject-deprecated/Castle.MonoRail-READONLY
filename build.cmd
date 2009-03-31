@echo off

REM ****************************************************************************
REM Copyright 2004-2009 Castle Project - http://www.castleproject.org/
REM Licensed under the Apache License, Version 2.0 (the "License");
REM you may not use this file except in compliance with the License.
REM You may obtain a copy of the License at
REM 
REM     http://www.apache.org/licenses/LICENSE-2.0
REM 
REM Unless required by applicable law or agreed to in writing, software
REM distributed under the License is distributed on an "AS IS" BASIS,
REM WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
REM See the License for the specific language governing permissions and
REM limitations under the License.
REM ****************************************************************************

IF "%~1"=="" goto quick

:normal
%~dp0SharedLibs\build\NAnt\bin\NAnt.exe -t:net-3.5 -D:nunit-console=%~dp0SharedLibs\build\NUnit\bin\nunit-console.exe -buildfile:default.build %*
goto end

:quick
%~dp0SharedLibs\build\NAnt\bin\NAnt.exe -t:net-3.5 -D:nunit-console=%~dp0SharedLibs\build\NUnit\bin\nunit-console.exe -buildfile:default.build quick release clean build

:end