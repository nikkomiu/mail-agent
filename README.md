## Table of Contents

- [Install](#installation)
- [Change Service User](#change-user-running-service)
- [Uninstall](#uninstallation)
- [Settings](#settings)
  - [General](#general)
  - [Log](#log)
  - [Mail](#mail)
  - [DefaultSavePath](#defaultsavepath)
  - [Export](#export)
  - [Profiles](#profiles)
    - [Profile Settings (Overview)](#profile-settings)
    - [Profile Keys](#profile-keys-overview)
      - [Description](#profile-key-description)
      - [Example](#example-keys)

## Compile

To compile this project you will need:

- Visual Studio 2010 or higher **- or -** NuGet Package Manager for Visual Studio
- [Microsoft Visual Studio Installer Projects](http://visualstudiogallery.msdn.microsoft.com/9abe329c-9bba-44a1-be59-0fbf6151054d)

NuGet should automatically resolve package dependencies at compile time

> If NuGet does not automatically resolve package dependencies at compile time you will need to:
> 
> 1. Open `Tools` -> `NuGet Package Manager` -> `Manage NuGet Packages for Solution...`
> 2. Click the **Restore** button at the top of the window

To build the solution for deployment:

1. Change the solution configuration from `Debug` to `Release`
2. Open `Build` -> `Clean Solution`
3. Open `Build` -> `Build Solution`
4. The compiled program should be located at `%project_root%\MailAgent.Service\bin\Release\` directory
5. Move the program to the deployment system and follow the [installation](#installation) instructions.

To build the installer:

1. Change the solution configuration from `Debug` to `Release`
2. Right Click on the `MailAgent.Setup` Project
3. Click `Rebuild`
4. The compiled program should be located at `%project_root%\MailAgent.Setup\bin\Release\` directory
5. Run the installation on the deployment system and follow the installation instructions.
6. Start the Mail Agent service in Services or **restart the deployment system** to start Mail Agent

## Installation

To perform a quick install of the service run the `MailAgent.exe` file **as an administrator** to install the service to the system.

You can also run the installer from the command prompt **as administrator**, navigate to the directory that contains the `MailAgent.exe` file, then run:

```
MailAgent.exe -install
```

After the service is installed you should restart the system for the service to start and all the changes to take effect.

## Change User Running Service

If you are going to specify a UNC path for the email content to be saved to, you will want/need to change the user that the service is running as to be a user that has write permission to the directory on the server.

1. Open **Control Panel**
    - If you are in the category view change the view format from Category to **Large Icons** or **Small Icons**
2. Open **Administrative Tools**
3. Open **Services**
4. Find the service **Mail Agent** listed in Services
5. **Right click** on the service and click **Properties**
6. Go to the **Log On tab** in the Properties dialog box
7. Change the Log on as to **This account**
8. Either enter the account username in the this account box or click Browse... to the right of the this account box and browse for a user on the local system or the domain
9. Enter the user's password in the password box and re-enter the user's password in the confirm password box
10. Click **Apply**
11. If the service was running before the change, you may need to restart the service for the changes to take effect.

## Uninstall

To uninstall the service run command prompt **as administrator**, navigate to the directory that contains the `MailAgent.exe` file, then run:

```
MailAgent.exe -uninstall
```

## Settings

There are multiple sections in the Settings XML file. The two primary sections are General and Profiles.

### General

The General section defines settings that are related to all of the profiles in Mail Agent

#### Log

The log section is used to define what gets logged and where logging gets done for Mail Agent

- `LocalLocation`: (True/False) Defines the location of the logging. This is defined by one of the following:
    - **Local Path** (in relation to the service executable) the local path is prefixed with the directory `logs` so the actual path is `APP_ROOT\logs\` on the disk where APP_ROOT is the directory where the app is stored.
    - **Absolute Path** with a drive letter and location on the drive to store the logs.
- `Location`: (String) The string path of the file that gets stored
- `Level`: (String) The level that logging is being run at. Each item inherits the ability to log any of the levels that are above it but not any levels that fall below the level.
    - `NONE`: Shows no log output in the log file.
    - `CRITICAL`: Shows only the critical errors in the application.
    - `ERROR`: Shows the critical errors as well as the errors.
    - `WARNING`: Shows all warnings as well as critical and error. Warnings encompass the start and stop details of the service
    - `INFO`: Shows all output above and including info. The info says when an email was processed and what was done with it.
    - `DEBUG`: Shows all of the output that is produced by Mail Agent.

#### Mail

The mail section defines all of the settings that are related to the actual Exchange Mailbox details.

- `Polling`: (Int) The interval for how often to check Exchange for new items in the Inbox
- `ExchangeVersion`: (String) The Exchange Version to use:
    - `2007 SP1`
    - `2010`
    - `2010 SP1`
    - `2010 SP2`
    - `2013`
    - `2013 SP1` 
- `Email`: (String) The email address that will be checked for new messages.
- `Password`: (String) (Optional: If the `UseWindowsCredentials` is set to True then you do not need to specify a password) The password for the email address that is going to be checked.
- `UseWindowsCredentials`: (True/False) Use if you are going to run the service as a system user and the system user has access to the email account that is going to be checked.
- `ErrorFolder`: (String) The Mailbox folder that is going to be used for errors in the process (Will be auto created if it does not exist).
- `SuccessFolder`: (String) The Mailbox folder that is going to be used for successfully imported emails (Will be auto created if it does not exist).
- `Url`: (String)
    - The Url of the Exchange API `https://mail.example.com/ews/Exchange.asmx`. You must specify `/ews/Exhange.asmx` at the end of your email server address if you are using this method.
    - `Auto`: You can set the Url property to auto if you can use AutoDiscovery for the email account that is being used.

#### DefaultSavePath

The default path that the files will be saved for the Mail Agent profiles. Can and should be overridden with the individual profiles.

#### Export

The export is where you can define the file that contains the 'keys' or attributes of the email message to save.

- `Filename`: The filename to use when exporting files. UNC Paths are allowed as long as the user running the Windows Service has access to the UNC Path
- `KeyDelimiter`: The character that is going to be used to separate the keys in the export file.

### Profiles

The profiles are where the individual actions take place based on email data. The profiles element contains a list of `Profile` elements.

**Note:** After adding a new profile to Mail Agent you must restart the Mail Agent service, any emails that have not yet been processed will still be waiting when the service restarts.

#### Profile Settings

- `Name`: (Unique String) The name of the profile
- `Alias`: (String) Add an email alias to the INMA account in Exchange and set this value accordingly if you would like all messages sent to the email alias (must be in the `To` field) to be handled by this profile
    - **Note:** The INMA account must hidden from the Global Address List in Exchange for this to work!
- `EmailSubject`: (String) The subject substring to match for the profile
    - **Recommendation:** Use a SHA1 value of the Name string to make sure that the emails have a unique value
      In Terminal: echo -n "Profile Name" | openssl sha1
- `EmailBody`: (String) The email body substring to match for the profile
    - `Save`: (True/False) Attribute for the EmailBody tag to allow saving the EmailBody
- `SaveAttachments`: (True/False) Defines if the attachments should be saved or not
- `SavePath`: (String) (Optional) Override for the globally defined save path
- `KeyDelimiter`: (String) (Optional) Override for the globally defined delimiter

You can set either EmailSubject or EmailBody to search for the search string or you can have both. The EmailSubject and EmailBody strings are used to make sure that the current profile is the correct profile to use for the email.

#### Profile Keys Overview

The profile keys are a list of the keys that you want to save in the Export file. You can name the XML elements whatever you want to name them they get stored in the order they are stored in the list.

*Key Examples:*

- `<Attr1 Type="Static">Test 1</Attr1>`: This will generate `Test 1` for the output file
- `<Attr2 Type="Dynamic">NULL</Attr2>`: This will generate an empty string for the output file
- `<Attr2 Type="Dynamic">DATETIME</Attr2>`: This will generate a datetime stamp for the output file
- `<Attr2 Type="Search">Some Value</Attr2>`: This will find the string 'Some Value' in the email body and set the value to the remainder of the line.

#### Profile Key Description

- `Types`: There are 3 different types of attributes that you can have for the keys:
    - `Static`: The static type will generate the text that you have inside of the element tag
    - `Dynamic`: The dynamic type will generate 'dynamic' content based on the value inside the element tag, possible element tag values are:
        - `DATETIME`: Creates a date/time stamp for the field **(reccomended)**
        - `DATE`: Creates a datestamp for the field
        - `TIME`: Creates a timestamp for the field
        - `INCREMENT`: Creates an increment of the individual files that are associated with an email **(reccomended)** __(to gaurantee unique indexes you will need this property)__
        - `NULL`: Makes the field black (used for an empty placeholder)
    - `Search`: Looks inside the email body for the designated text inside of the element tag and gets the value after the tag and before the newline

## Example Keys

Settings:

```xml
...
<EmailBody Save="True"></EmailBody>
<KeyDelimiter>,</KeyDelimiter>
<Keys>
  <Attr1 Type="Static">Static Text</Attr1>
  <Attr2 Type="Dynamic">NULL</Attr2>
  <Attr3 Type="Search">Name:</Attr3>
  <Attr4 Type="Search">Phone:</Attr4>
  <Attr5 Type="Dynamic">DATE</Attr5>
</Keys>
...
```

Email HTML Body:

```html
...
<p>Name: <b>Johnny</b></p>
<p>Phone: <b>123-456-7890</b></p>
...
```

Export File:

```
Static Text,,Johnny,123-456-7890,02/24/2014,output_20140224061014.html
```