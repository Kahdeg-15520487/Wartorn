﻿; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Wartorn"
#define MyAppVersion "1.0"
#define MyAppPublisher "Nhóm 3 lớp IT008.H22"
#define MyAppURL "https://github.com/Kahdeg-15520487/Wartorn"
#define MyAppExeName "Wartorn.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D021FB05-CB59-44E7-A6FC-E4E26092E5B2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=G:\Workspace\c#\Wartorn\Build\license.txt
OutputDir=G:\Workspace\c#\Wartorn\Build\Installer
OutputBaseFilename=WartornSetup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Source: "..\Build\Wartorn.exe"; DestDir: "{app}"; Flags: ignoreversion
; Source: "..\Build\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Build\IronPython.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\IronPython.Modules.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\IronPython.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\IronPython.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Microsoft.Dynamic.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Microsoft.Scripting.AspNet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Microsoft.Scripting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\MonoGame.Framework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\NVorbis.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\OpenTK.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\SDL2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\soft_oal.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\StatsBalancer.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Wartorn.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Build\Content\*"; DestDir: "{app}\Content\"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\Build\data\*"; DestDir: "{app}\data\"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\Build\map\classic.map"; DestDir: "{app}\map\"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Dirs]
Name: "{app}\data\"
