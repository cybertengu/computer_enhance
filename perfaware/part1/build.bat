@echo off
mkdir ..\build
pushd ..\build
cl /EHsc -FC -Zi /wd4700 ..\part1\part1.cpp user32.lib
popd
@echo off
