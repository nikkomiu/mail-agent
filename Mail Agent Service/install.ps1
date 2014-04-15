$serviceName = "CMU Mail Agent"
$exePath = "C:\Program Files (x86)\CMU\MailAgent\cmuMailAgent.exe"
$username = "USERNAME"
$password = convertto-securestring -String "PASSWORD" -AsPlainText -Force  
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist $username, $password

$existingService = Get-WmiObject -Class Win32_Service -Filter "Name='$serviceName'"

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Copy-Item "$($scriptPath)\*" "C:\Program Files (x86)\CMU\MailAgent\" -recurse

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
