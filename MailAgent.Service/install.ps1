Param([string]$serviceName, [string]$exePath, [string]$username, [string]$password, [string]$uninstall)

if (!$serviceName)
{
    $serviceName = "CMU Mail Agent"
}

if (!$exePath)
{
    $exePath = $MyInvocation.MyCommand.Definition.TrimEnd($MyInvocation.MyCommand.Name) + "MailAgent.exe"
}

if (!$username -or !$password)
{
    Write-Host "ERROR: Username and Password parameters are required." -ForegroundColor Red
    Write-Host "Use -username <some_username> -password <some_password> to specify the username and passowrd that the service should run as." -ForegroundColor Red -NoNewline
    return;
}

$existingService = Get-WmiObject -Class Win32_Service -Filter "Name='$serviceName'"

if ($uninstall)
{
	if ($existingService)
	{
		"'$serviceName' exists already. Stopping."
		Stop-Service $serviceName
		"Waiting 3 seconds to allow existing service to stop."
		Start-Sleep -s 3

		$existingService.Delete()
		"Waiting 5 seconds to allow service to be uninstalled."
		Start-Sleep -s 5
	
		"Uninstalled Service."
	}
	else
	{
		"Could not uninstall service."
	}
	
	return;
}

Write-Host "Creating Service: " $serviceName
Write-Host "  Path: " $exePath
Write-Host "  Username: " $username

$encPassword = convertto-securestring -String $password -AsPlainText -Force  
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist $username, $encPassword

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

if ($existingService)
{
  "'$serviceName' exists already. Stopping."
  Stop-Service $serviceName
  "Waiting 3 seconds to allow existing service to stop."
  Start-Sleep -s 3

  $existingService.Delete()
  "Waiting 5 seconds to allow service to be uninstalled."
  Start-Sleep -s 5
}

"Installing the service."
New-Service -BinaryPathName $exePath -Name $serviceName -Credential $cred -DisplayName $serviceName -StartupType Automatic 
"Installed the service."
