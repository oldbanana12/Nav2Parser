@echo off
pushd "%~dp0"

powershell Compress-7Zip "bin\x64\Release" -ArchiveFileName "Nav2Parser.zip" -Format Zip

:exit
popd
@echo on
