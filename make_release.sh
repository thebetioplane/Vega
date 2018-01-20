#!/bin/bash

msbuild '-p:configuration=release;DebugSymbols=false' -nologo
rm -rf distro
mkdir distro
cp ./Vega/bin/Release/*.* ./distro
cd distro
rm -f *.xml *.config *.ini *.cache tmp *.swp *.dat
bundler_exe=../AssetBundler/bin/Release/AssetBundler.exe
chmod +x $bundler_exe
$bundler_exe ../AssetBundler/assets/img VegaImg.dat
$bundler_exe ../AssetBundler/assets/snd VegaSnd.dat
wine ../packages/peupdate.bin -rc Vega.exe
chmod +x Vega.exe
./Vega.exe make-index
rm -f *.log
