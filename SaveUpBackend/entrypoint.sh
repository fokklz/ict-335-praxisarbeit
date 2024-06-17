#!/bin/sh

# Start SurrealDB in the background
surreal start --log info --auth --user root --pass root file://opt/surreal &

# Start the .NET application
dotnet SaveUpBackend.dll
