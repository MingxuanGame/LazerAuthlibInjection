#!/usr/bin/env bash
echo "Running dotnet build..."
dotnet build -c Release -o ./bin/Release
if [ $? -ne 0 ]; then
    echo "Build failed"
    exit 1
fi

echo "Restoring tools..."
dotnet tool restore

echo "Running ILRepack..."
HarmonyPath="$HOME/.nuget/packages/lib.harmony/2.4.1/lib/net8.0/0Harmony.dll"
"$ILRepackPath" -out:./bin/Release/osu.Game.Rulesets.AuthlibInjection.dll \
    ./bin/Release/osu.Game.Rulesets.AuthlibInjection.dll \
    "$HarmonyPath" \
    -lib:./fakelib \
    /internalize
