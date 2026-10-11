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

; Acceso directo para configurar base de datos y migraciones (Modo Administrativo)
Name: "{autoprograms}\{#MyAppName}\Configurar Base de Datos"; Filename: "{app}\{#MyAppExeName}"; Parameters: "--config"; IconFilename: "{app}\Recurso_2logosisfac.ico"; Comment: "Asistente corporativo de conexión y actualización de base de datos"

; Acceso directo al desinstalador
Name: "{autoprograms}\{#MyAppName}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"; IconFilename: "{app}\Recurso_2logosisfac.ico"

; Acceso directo en el escritorio (opcional)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Recurso_2logosisfac.ico"; Tasks: desktopicon

[Run]
; Reglas de Firewall para SQL Server y SQL Browser en Modo Servidor (restringidas estrictamente a red privada)
Filename: "netsh.exe"; Parameters: "advfirewall firewall add rule name=""SQL Server (MiniSistemaFacturacion)"" dir=in action=allow protocol=TCP localport=1433 profile=private"; Check: ShouldConfigureServerFirewall; Flags: runhidden
Filename: "netsh.exe"; Parameters: "advfirewall firewall add rule name=""SQL Server Browser (MiniSistemaFacturacion)"" dir=in action=allow protocol=UDP localport=1434 profile=private"; Check: ShouldConfigureServerFirewall; Flags: runhidden

; Ejecutar la aplicación al finalizar
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
var
  InstallationModePage: TInputOptionWizardPage;
  ClientNetworkPage: TInputQueryWizardPage;
  IsServerModeSelected: Boolean;
  SqlRebootRequired: Boolean;

// ---------------------------------------------------------------------------------
// Verificación de actualización previa (database.config existente)
// ---------------------------------------------------------------------------------
function IsUpgradeInstallation: Boolean;
begin
  Result := FileExists(ExpandConstant('{commonappdata}\MiniSistemaFacturacion\database.config'));
end;

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
// Obtener el nombre de la instancia SQL Server instalada
// ---------------------------------------------------------------------------------
function GetInstalledSqlServerInstanceName: string;
var
  InstName: string;
begin
  Result := '';
  if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL', 'SQLEXPRESS', InstName) then
    Result := InstName
  else if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL', 'MSSQLSERVER', InstName) then
    Result := InstName;
end;

// ---------------------------------------------------------------------------------
// Verificación de firma digital oficial de Microsoft Corporation mediante PowerShell
// ---------------------------------------------------------------------------------
function VerifyMicrosoftDigitalSignature(const FilePath: string): Boolean;
var
  PowerShellCmd: string;
  ResultCode: Integer;
begin
  Result := False;
  if not FileExists(FilePath) then
    Exit;

  PowerShellCmd := '-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command ' +
    '"$sig = Get-AuthenticodeSignature ''' + FilePath + '''; ' +
    'if ($sig.Status -eq ''Valid'' -and $sig.SignerCertificate.Subject -match ''Microsoft Corporation'') { exit 0 } else { exit 1 }"';

  if Exec('powershell.exe', PowerShellCmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Result := (ResultCode = 0);
    if Result then
      Log('Firma Digital: El instalador ''' + FilePath + ''' cuenta con firma oficial válida de Microsoft Corporation.')
    else
      Log('Firma Digital: Paquete rechazado, no posee firma auténtica de Microsoft Corporation.');
  end
  else
  begin
    Log('No se pudo ejecutar la comprobación Authenticode de PowerShell.');
  end;
end;

// ---------------------------------------------------------------------------------
// Búsqueda de paquete de instalación oficial de SQL Server en la carpeta fuente ({src})
// ---------------------------------------------------------------------------------
function FindSqlInstallerPackage(out PackagePath: string; out IsFullPackage: Boolean): Boolean;
var
  FindRec: TFindRec;
begin
  Result := False;
  PackagePath := '';
  IsFullPackage := False;

  // 1. Paquete offline completo (SQLEXPR_x64_*.exe)
  if FindFirst(ExpandConstant('{src}\SQLEXPR_x64_*.exe'), FindRec) then
  begin
    PackagePath := ExpandConstant('{src}\') + FindRec.Name;
    IsFullPackage := True;
    Result := True;
    FindClose(FindRec);
    Exit;
  end;

  // 2. Bootstrapper oficial (SQL2022-SSEI-Expr.exe o SQL2019-SSEI-Expr.exe)
  if FileExists(ExpandConstant('{src}\SQL2022-SSEI-Expr.exe')) then
  begin
    PackagePath := ExpandConstant('{src}\SQL2022-SSEI-Expr.exe');
    IsFullPackage := False;
    Result := True;
    Exit;
  end;
  if FileExists(ExpandConstant('{src}\SQL2019-SSEI-Expr.exe')) then
  begin
    PackagePath := ExpandConstant('{src}\SQL2019-SSEI-Expr.exe');
    IsFullPackage := False;
    Result := True;
    Exit;
  end;
end;

// ---------------------------------------------------------------------------------
// Configuración de TCP/IP estático en puerto 1433 en el registro para SQL Server
// ---------------------------------------------------------------------------------
procedure EnableSqlServerTcpIp;
var
  InstName: string;
  RegTcp: string;
begin
  InstName := GetInstalledSqlServerInstanceName;
  if InstName <> '' then
  begin
    RegTcp := 'SOFTWARE\Microsoft\Microsoft SQL Server\' + InstName + '\MSSQLServer\SuperSocketNetLib\Tcp';
    if RegKeyExists(HKLM, RegTcp) then
    begin
      RegWriteDWordValue(HKLM, RegTcp, 'Enabled', 1);
      RegWriteStringValue(HKLM, RegTcp + '\IPAll', 'TcpPort', '1433');
      RegWriteStringValue(HKLM, RegTcp + '\IPAll', 'TcpDynamicPorts', '');
      Log('Modo Servidor: Protocolo TCP/IP habilitado en registro para instancia ' + InstName + ' (Puerto 1433).');
    end;
  end;
end;

// ---------------------------------------------------------------------------------
// Comprobación de reinicio pendiente requerido por dependencias
// ---------------------------------------------------------------------------------
function NeedRestart(): Boolean;
begin
  Result := SqlRebootRequired;
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
// Creación de las páginas personalizadas: Modo de Despliegue y Conexión de Red
// ---------------------------------------------------------------------------------
procedure InitializeWizard;
begin
  SqlRebootRequired := False;

  // 1. Página de Selección de Modo (Servidor vs Cliente)
  InstallationModePage := CreateInputOptionPage(
    wpSelectDir,
    'Modo de Despliegue de la Aplicación',
    'Seleccione la arquitectura de instalación para este equipo:',
    'Configure si este equipo funcionará como Servidor Principal o Terminal Cliente:',
    True, False);

  InstallationModePage.Add(
    'Modo Servidor / Caja Principal (Recomendado)' + #13#10 +
    '    • Instala el sistema para operar con base de datos SQL Server local.' + #13#10 +
    '    • Permite conectar terminales clientes en red privada local.');

  InstallationModePage.Add(
    'Modo Terminal Cliente (Estación de Trabajo / Red Local)' + #13#10 +
    '    • Instala la aplicación para conectar al servidor de facturación en la red.' + #13#10 +
    '    • Ideal para cajas secundarias, puntos de venta o estaciones de consulta.');

  InstallationModePage.SelectedValueIndex := 0;

  // 2. Página de Conexión de Red para Terminal Cliente
  ClientNetworkPage := CreateInputQueryPage(
    InstallationModePage.ID,
    'Configuración de Red de la Terminal Cliente',
    'Indique los datos del servidor central de facturación:',
    'El sistema utilizará estos parámetros para enlazar con la base de datos central en la red local:');

  ClientNetworkPage.Add('Dirección IP o Nombre del Servidor:', False);
  ClientNetworkPage.Add('Nombre de la Instancia SQL (Predeterminado SQLEXPRESS):', False);
  ClientNetworkPage.Add('Nombre de la Base de Datos:', False);

  // Valores predeterminados para la red
  ClientNetworkPage.Values[0] := '192.168.1.100';
  ClientNetworkPage.Values[1] := 'SQLEXPRESS';
  ClientNetworkPage.Values[2] := 'MiniSistemaFacturacion';
end;

// ---------------------------------------------------------------------------------
// Control de visualización condicional de páginas
// ---------------------------------------------------------------------------------
function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := False;

  // Si es una actualización sobre instalación existente, conservar modo y configuración previa sin preguntar de nuevo
  if IsUpgradeInstallation then
  begin
    if (PageID = InstallationModePage.ID) or (PageID = ClientNetworkPage.ID) then
    begin
      Result := True;
      Exit;
    end;
  end;

  // En instalación limpia: Solo mostrar la página de conexión de red si se seleccionó Modo Cliente
  if PageID = ClientNetworkPage.ID then
  begin
    Result := IsServerModeSelected;
  end;
end;

// ---------------------------------------------------------------------------------
// Validación al avanzar entre páginas y manejo seguro de SQL Server
// ---------------------------------------------------------------------------------
function NextButtonClick(CurPageID: Integer): Boolean;
var
  ErrorCode: Integer;
  PkgPath: string;
  IsFullPkg: Boolean;
  InstallParams: string;
  ExecResult: Integer;
begin
  Result := True;

  if CurPageID = InstallationModePage.ID then
  begin
    IsServerModeSelected := (InstallationModePage.SelectedValueIndex = 0);

    if IsServerModeSelected then
    begin
      // Comprobar si ya existe instancia de SQL Server
      if IsSqlServerExpressInstalled then
      begin
        Log('Modo Servidor: Se detectó una instancia de SQL Server instalada. No se modificará la instalación existente.');
      end
      else
      begin
        // No hay SQL Server local. Comprobar si hay paquete oficial en la carpeta del instalador
        if FindSqlInstallerPackage(PkgPath, IsFullPkg) then
        begin
          // Verificar firma digital oficial de Microsoft Corporation
          if VerifyMicrosoftDigitalSignature(PkgPath) then
          begin
            if IsFullPkg then
            begin
              if MsgBox(
                'Paquete Oficial de SQL Server Detectado:' + #13#10#13#10 +
                'Se encontró el instalador completo oficial de Microsoft SQL Server Express:' + #13#10 +
                ExtractFileName(PkgPath) + #13#10#13#10 +
                '¿Desea instalar SQL Server Express en este equipo ahora?' + #13#10 +
                'La instalación se realizará de forma desatendida y segura mediante Autenticación de Windows.',
                mbConfirmation, MB_YESNO) = idYes then
              begin
                InstallParams := '/QUIET /IACCEPTSQLSERVERLICENSETERMS /ACTION=Install /FEATURES=SQLEngine /INSTANCENAME=SQLEXPRESS /SQLSVCACCOUNT="NT AUTHORITY\Network Service" /NPENABLED=0 /TCPENABLED=1';
                Log('Ejecutando instalación desatendida de SQL Server: ' + PkgPath);

                if Exec(PkgPath, InstallParams, '', SW_SHOW, ewWaitUntilTerminated, ExecResult) then
                begin
                  if ExecResult = 0 then
                  begin
                    MsgBox('Microsoft SQL Server Express ha sido instalado correctamente.', mbInformation, MB_OK);
                  end
                  else if ExecResult = 3010 then
                  begin
                    MsgBox('Microsoft SQL Server Express se instaló correctamente, pero el sistema requiere reiniciar para completar la configuración.', mbInformation, MB_OK);
                    SqlRebootRequired := True;
                  end
                  else
                  begin
                    MsgBox('La instalación de SQL Server finalizó con código de salida: ' + IntToStr(ExecResult) + '.' + #13#10 +
                           'Puede continuar la instalación y verificar la conexión posteriormente.', mbInformation, MB_OK);
                  end;
                end
                else
                begin
                  MsgBox('No se pudo ejecutar el instalador de SQL Server.', mbError, MB_OK);
                end;
              end;
            end
            else
            begin
              // Bootstrapper web oficial (SQL2022-SSEI-Expr.exe)
              // No asumir parámetros de instalación desatendida; ejecutar interactivamente
              if MsgBox(
                'Descargador Oficial de SQL Server Express Detectado:' + #13#10#13#10 +
                'Se detectó el asistente oficial de Microsoft SQL Server Express (' + ExtractFileName(PkgPath) + ').' + #13#10#13#10 +
                '¿Desea iniciar el asistente de Microsoft ahora para descargar e instalar la base de datos?',
                mbConfirmation, MB_YESNO) = idYes then
              begin
                Exec(PkgPath, '', '', SW_SHOW, ewWaitUntilTerminated, ExecResult);
              end;
            end;
          end
          else
          begin
            MsgBox('Advertencia de Seguridad:' + #13#10#13#10 +
                   'Se encontró un archivo instalador pero no cuenta con firma digital válida de Microsoft Corporation.' + #13#10 +
                   'Por seguridad, no se ejecutará.', mbError, MB_OK);
          end;
        end
        else
        begin
          // No se encontró paquete local: ofrecer abrir descarga oficial de Microsoft
          if MsgBox(
            'Información de Base de Datos (Modo Servidor):' + #13#10#13#10 +
            'No se detectó una instancia de SQL Server (SQLEXPRESS) instalada localmente.' + #13#10#13#10 +
            'Para operar como Servidor Principal de facturación, este equipo requiere Microsoft SQL Server Express 2022.' + #13#10#13#10 +
            '¿Desea abrir el sitio oficial de Microsoft para descargar e instalar SQL Server Express?' + #13#10#13#10 +
            '• Sí: Abre la página oficial de Microsoft (https://aka.ms/sqlserver2022express).' + #13#10 +
            '• No: Continúa la instalación (podrá instalar SQL Server luego o conectar a un servidor en red).',
            mbConfirmation, MB_YESNO) = idYes then
          begin
            ShellExec('open', 'https://aka.ms/sqlserver2022express', '', '', SW_SHOW, ewNoWait, ErrorCode);
          end;
        end;
      end;
    end;
  end
  else if CurPageID = ClientNetworkPage.ID then
  begin
    if Trim(ClientNetworkPage.Values[0]) = '' then
    begin
      MsgBox('Debe especificar la dirección IP o el nombre del servidor en la red.', mbError, MB_OK);
      Result := False;
      Exit;
    end;
    if Trim(ClientNetworkPage.Values[1]) = '' then
    begin
      ClientNetworkPage.Values[1] := 'SQLEXPRESS';
    end;
    if Trim(ClientNetworkPage.Values[2]) = '' then
    begin
      ClientNetworkPage.Values[2] := 'MiniSistemaFacturacion';
    end;
  end;
end;

// ---------------------------------------------------------------------------------
// Comprobación de regla de Firewall en Modo Servidor
// ---------------------------------------------------------------------------------
function ShouldConfigureServerFirewall: Boolean;
begin
  Result := IsServerModeSelected;
end;

// ---------------------------------------------------------------------------------
// Inicialización segura de la configuración en ProgramData
// ---------------------------------------------------------------------------------
procedure CurStepChanged(CurStep: TSetupStep);
var
  ConfigPath: string;
  Params: string;
  ResultCode: Integer;
  ServerHost: string;
  InstanceName: string;
  FullServer: string;
begin
  if CurStep = ssPostInstall then
  begin
    ConfigPath := ExpandConstant('{commonappdata}\MiniSistemaFacturacion\database.config');

    // Si ya existe configuración previa (actualización), conservarla intacta
    if FileExists(ConfigPath) then
    begin
      Log('Actualización: Archivo database.config existente detectado. Conservando configuración previa.');
    end
    else
    begin
      // Generar configuración inicial cifrada con DPAPI llamando al ejecutable elevado con --init-config
      if IsServerModeSelected then
      begin
        Params := '--init-config --server "localhost\SQLEXPRESS" --database "MiniSistemaFacturacion" --auth "windows"';
      end
      else
      begin
        ServerHost := Trim(ClientNetworkPage.Values[0]);
        InstanceName := Trim(ClientNetworkPage.Values[1]);
        if InstanceName <> '' then
          FullServer := ServerHost + '\' + InstanceName
        else
          FullServer := ServerHost;

        Params := '--init-config --server "' + FullServer + '" --database "' + Trim(ClientNetworkPage.Values[2]) + '" --auth "windows"';
      end;

      Log('Ejecutando inicialización segura de configuración: ' + Params);
      Exec(ExpandConstant('{app}\{#MyAppExeName}'), Params, '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    end;

    // Si es modo servidor, asegurar que el protocolo TCP/IP esté habilitado en el registro
    if IsServerModeSelected then
    begin
      EnableSqlServerTcpIp;
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
    // Notificación al usuario confirmando la integridad de sus datos contables
    MsgBox(
      'Mini Sistema de Facturación ha sido desinstalado de su equipo.' + #13#10#13#10 +
      'Conservación Absoluta de Datos:' + #13#10 +
      'Por seguridad comercial e integridad contable, sus bases de datos en SQL Server y copias' + #13#10 +
      'de seguridad en C:\ProgramData\MiniSistemaFacturacion\Backups NO han sido eliminadas.',
      mbInformation, MB_OK);
  end;
end;
