 @ECHO OFF
 SET BuildCmd=%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\build\Buildage\scripts\Build.proj /toolsversion:4.0
 IF [%1]==[] (%BuildCmd%) ELSE (%BuildCmd% /t:%1)
 