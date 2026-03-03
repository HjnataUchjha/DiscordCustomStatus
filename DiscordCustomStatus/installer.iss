[Setup]
AppName=Discord Custom Status
AppVersion=0.0.2
DefaultDirName={pf}\DiscordCustomStatus
DefaultGroupName=Discord Custom Status
OutputDir=../Realises
OutputBaseFilename=Setup
Compression=lzma
SolidCompression=yes

[Files]
Source: "bin\Release\net8.0-windows\win-x64\publish\DiscordCustomStatus.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Discord Custom Status"; Filename: "{app}\DiscordCustomStatus.exe"
Name: "{group}\Uninstall Discord Custom Status"; Filename: "{uninstallexe}"
Name: "{autostartup}\Discord Custom Status"; Filename: "{app}\DiscordCustomStatus.exe"

[Run]
Filename: "{app}\DiscordCustomStatus.exe"; Description: "Запустить приложение"; Flags: nowait postinstall skipifsilent