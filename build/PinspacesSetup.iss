#define ApplicationExeName "Pinspaces.exe"
#define ApplicationName "Pinspaces"
#define ApplicationURL "https://github.com/CodePenguin/pinspaces/"
#define BuildVersion "0.0.0"
#define Publisher "CodePenguin"

[Setup]
AppId={{AC340095-83D1-456E-9F19-E58B91578772}
AppName={#ApplicationName}
AppVersion={#BuildVersion}
AppVerName={#ApplicationName} {#BuildVersion}
AppPublisher={#Publisher}
AppPublisherURL={#ApplicationURL}
AppSupportURL={#ApplicationURL}
AppUpdatesURL={#ApplicationURL}
DefaultDirName={pf}\{#ApplicationName}
DefaultGroupName={#ApplicationName}
SetupIconFile=..\Assets\Pinspaces.ico
AllowNoIcons=yes
LicenseFile=..\license.txt
OutputDir=..\bin
OutputBaseFilename=PinspacesSetup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\Pinspaces\bin\Release\publish\Pinspaces.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Pinspaces\bin\Release\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#ApplicationName}"; Filename: "{app}\{#ApplicationExeName}"
Name: "{commondesktop}\{#ApplicationName}"; Filename: "{app}\{#ApplicationExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#ApplicationExeName}"; Description: "{cm:LaunchProgram,{#StringChange(ApplicationName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
