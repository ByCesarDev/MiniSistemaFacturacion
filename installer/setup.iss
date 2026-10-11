; =====================================================================
; Mini Sistema de Facturación - Script de Instalación Inno Setup 6
; Versión: 1.2.0
; Autor: Cesar Reyes (ByCesarDev)
; Arquitectura: Servidor Local (Standalone) / Terminal Cliente en Red
; =====================================================================

#define MyAppName "Mini Sistema de Facturación"
#define MyAppVersion "1.2.0"
#define MyAppPublisher "ByCesarDev"
#define MyAppURL "https://github.com/ByCesarDev/MiniSistemaFacturacion"
#define MyAppExeName "MiniSistemaFacturacion.exe"
#define MyAppId "{{B7E58C12-9A4B-4E7F-9A32-841D624A9B15}}"

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} v{#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\MiniSistemaFacturacion
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\bin\Installer
OutputBaseFilename=MiniSistemaFacturacion_Setup_v1.2.0
SetupIconFile=..\Recurso_2logosisfac.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName} v{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
WizardImageFile=assets\WizardImage.bmp
WizardSmallImageFile=assets\WizardSmallImage.bmp
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
CloseApplications=force
RestartApplications=no
MinVersion=6.1sp1

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Messages]
spanish.WelcomeLabel1=Bienvenido al Asistente de Instalación de [name]
spanish.WelcomeLabel2=Este programa instalará [name] versión {#MyAppVersion} en su equipo.%n%nSe configurarán los componentes esenciales, dependencias de seguridad y la estructura de base de datos.%n%nSe recomienda cerrar cualquier otra aplicación antes de continuar.

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Dirs]
; Crear directorio seguro de configuración en ProgramData con permisos restringidos
; authusers-readexec: Usuarios estándar solo lectura y ejecución
; admins-full: Administradores control total
; system-full: Sistema control total
Name: "{commonappdata}\MiniSistemaFacturacion"; Permissions: authusers-readexec system-full admins-full; Flags: uninsneveruninstall
Name: "{commonappdata}\MiniSistemaFacturacion\Backups"; Permissions: system-full admins-full; Flags: uninsneveruninstall

[Files]
; Archivos binarios de la aplicación compilada en Release
Source: "..\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Recurso_2logosisfac.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Acceso directo principal a la aplicación
Name: "{autoprograms}\{#MyAppName}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Recurso_2logosisfac.ico"

; Acceso directo para configurar base de datos y migraciones
Name: "{autoprograms}\{#MyAppName}\Configurar Base de Datos"; Filename: "{app}\{#MyAppExeName}"; Parameters: "--config"; IconFilename: "{app}\Recurso_2logosisfac.ico"; Comment: "Asistente corporativo de conexión y actualización de base de datos"

; Acceso directo al desinstalador
Name: "{autoprograms}\{#MyAppName}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"; IconFilename: "{app}\Recurso_2logosisfac.ico"

; Acceso directo en el escritorio (opcional)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Recurso_2logosisfac.ico"; Tasks: desktopicon

[Run]
; Ejecutar la aplicación al finalizar
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
var
  InstallationModePage: TInputOptionWizardPage;
  IsServerModeSelected: Boolean;

// ---------------------------------------------------------------------------------
// Verificación de .NET Framework 4.7.2 o superior
// Release DWORD >= 461808 (Windows 10 April 2018 Update / .NET 4.7.2)
// ---------------------------------------------------------------------------------
function IsDotNet472Installed: Boolean;
var
  ReleaseValue: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', ReleaseValue) then
  begin
    Result := (ReleaseValue >= 461808);
  end;
end;

// ---------------------------------------------------------------------------------
// Detección de instancia local de SQL Server (SQLEXPRESS o predeterminada)
// ---------------------------------------------------------------------------------
function IsSqlServerExpressInstalled: Boolean;
begin
  Result := RegKeyExists(HKLM, 'SYSTEM\CurrentControlSet\Services\MSSQL$SQLEXPRESS') or
            RegKeyExists(HKLM, 'SYSTEM\CurrentControlSet\Services\MSSQLSERVER') or
            RegKeyExists(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\SQLEXPRESS');
end;

// ---------------------------------------------------------------------------------
// Inicialización del Asistente: Comprobación de requisitos previos del sistema
// ---------------------------------------------------------------------------------
function InitializeSetup: Boolean;
begin
  Result := True;

  if not IsDotNet472Installed then
  begin
    MsgBox(
      'Mini Sistema de Facturación requiere Microsoft .NET Framework 4.7.2 o superior.' + #13#10#13#10 +
      'Por favor, instale .NET Framework 4.7.2 desde el sitio oficial de Microsoft e intente nuevamente.' + #13#10 +
      'https://dotnet.microsoft.com/download/dotnet-framework/net472',
      mbCriticalError, MB_OK);
    Result := False;
    Exit;
  end;
end;

// ---------------------------------------------------------------------------------
// Creación de la página personalizada de selección de Modo de Despliegue
// ---------------------------------------------------------------------------------
procedure InitializeWizard;
begin
  InstallationModePage := CreateInputOptionPage(
    wpSelectDir,
    'Modo de Despliegue de la Aplicación',
    'Seleccione la arquitectura de instalación para este equipo:',
    'Configure si este equipo funcionará como Servidor Principal o Terminal Cliente:',
    True, False);

  InstallationModePage.Add(
    'Modo Servidor / Caja Principal (Recomendado)' + #13#10 +
    '    • Instala el sistema para operar con base de datos SQL Server local.' + #13#10 +
    '    • Ideal para negocios con una sola computadora o el equipo central del local.');

  InstallationModePage.Add(
    'Modo Terminal Cliente (Estación de Trabajo / Red)' + #13#10 +
    '    • Instala la aplicación cliente para conectar a un servidor existente en la red.' + #13#10 +
    '    • Ideal para cajas secundarias, oficinas o puntos de facturación adicionales.');

  // Por defecto, seleccionar Modo Servidor
  InstallationModePage.SelectedValueIndex := 0;
end;

// ---------------------------------------------------------------------------------
// Validación de la página personalizada y advertencia de SQL Server si aplica
// ---------------------------------------------------------------------------------
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;

  if CurPageID = InstallationModePage.ID then
  begin
    IsServerModeSelected := (InstallationModePage.SelectedValueIndex = 0);

    if IsServerModeSelected then
    begin
      if not IsSqlServerExpressInstalled then
      begin
        MsgBox(
          'Información sobre Base de Datos:' + #13#10#13#10 +
          'No se detectó una instancia de SQL Server (SQLEXPRESS) instalada localmente.' + #13#10#13#10 +
          'El sistema se instalará correctamente. Al finalizar, el Asistente de Conexión le permitirá' + #13#10 +
          'conectar a una instancia de SQL Server existente o verificar la conexión con su servidor.' + #13#10#13#10 +
          'Si aún no tiene SQL Server Express, puede descargarlo gratuitamente desde:' + #13#10 +
          'https://www.microsoft.com/sql-server/sql-server-downloads',
          mbInformation, MB_OK);
      end;
    end;
  end;
end;

// ---------------------------------------------------------------------------------
// Protección absoluta de datos durante la desinstalación
// ---------------------------------------------------------------------------------
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    // Garantizar que la base de datos y la carpeta de backups nunca se eliminan
    MsgBox(
      'Mini Sistema de Facturación ha sido desinstalado de su equipo.' + #13#10#13#10 +
      'Nota de Seguridad de Datos:' + #13#10 +
      'Por seguridad comercial, sus bases de datos en SQL Server y copias de seguridad en' + #13#10 +
      'C:\ProgramData\MiniSistemaFacturacion\Backups NO han sido eliminadas y se encuentran intactas.',
      mbInformation, MB_OK);
  end;
end;
