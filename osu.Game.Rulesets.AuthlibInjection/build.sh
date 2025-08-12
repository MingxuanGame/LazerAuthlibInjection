#!/usr/bin/env bash
echo "Running dotnet build..."
dotnet build -c Release -o ./bin/Release
if [ $? -ne 0 ]; then
    echo "Build failed"
    exit 1
fi

echo "Running ILRepack..."
ILRepackPath="$HOME/.dotnet/tools/ilrepack"
if [ ! "$ILRepackPath" ]; then
    echo "ILRepack not found at $ILRepackPath. Please install it using 'dotnet tool install -g dotnet-ilrepack'."
    exit 1
fi
HarmonyPath="$HOME/.nuget/packages/lib.harmony/2.3.6/lib/net8.0/0Harmony.dll"
"$ILRepackPath" -out:./bin/Release/osu.Game.Rulesets.AuthlibInjection.dll \
    ./bin/Release/osu.Game.Rulesets.AuthlibInjection.dll \
    "$HarmonyPath" \
    -lib:./fakelib \
    /internalize
