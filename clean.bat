@setlocal
@call "D:\Programske datoteke (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86
@msbuild /t:clean /m
@cscript clean.vbs
@rd /S /Q output