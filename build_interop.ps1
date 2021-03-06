# Builds HandBrakeInterop from the HandBrake source tree and copying the DLLs
# to the VidCoder lib folder.

. ./build_common.ps1

# Change this to the folder containing HandBrakeInterop.sln
$HandBrakeInteropFolder = "..\HandBrakeSVN\win\CS\HandBrake.Interop"

$HandBrakeSolution = $HandBrakeInteropFolder + "\HandBrakeInterop.sln"

& $DevEnv11Exe $HandBrakeSolution /Rebuild "Release|x86"; ExitIfFailed
& $DevEnv11Exe $HandBrakeSolution /Rebuild "Release|x64"; ExitIfFailed
copy ($HandBrakeInteropFolder + "\HandBrakeInterop\bin\x86\Release\HandBrakeInterop.dll") Lib\x86 -force
copy ($HandBrakeInteropFolder + "\HandBrakeInterop\bin\x86\Release\HandBrakeInterop.pdb") Lib\x86 -force
copy ($HandBrakeInteropFolder + "\HandBrakeInterop\bin\x64\Release\HandBrakeInterop.dll") Lib\x64 -force
copy ($HandBrakeInteropFolder + "\HandBrakeInterop\bin\x64\Release\HandBrakeInterop.pdb") Lib\x64 -force
"File copy finished."

WriteSuccess

Write-Host
Pause