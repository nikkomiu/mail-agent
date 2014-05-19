# CMU Mail Agent

## Details

This mail agent service is for Exchange Web Services and is compatible with `Exchange 2007 SP1` and newer.

## EWS 2.1 Installation

Download Microsoft Exchange Web Services Managed API 2.1 from the Microsoft Download Center [Link to Download](http://www.microsoft.com/en-us/download/details.aspx?id=42022)

Install the EWS API on the server that will be running the Mail Agent Service.

## Service Installation

To install Mail Agent run the PowerShell Script (**As Administrator**) that is in the directory to create the Windows Service:

    PS> .\install.ps1 -username=windows_user -password=user_password

#### Script Parameters

- (Required) `-username`: The Windows User that the service will run as (this can be changed later in Windows Services)
- (Required) `-password`: The password for the Windows User that the service will run as
- `-serviceName`: The name of the service to register in Windows Services (`Default`: CMU Mail Agent)
- `-exePath`: The path to the Windows Service Executable that is being installed (`Default`: <<current_dir>>\cmuMailAgent.exe)
- `-uninstall`: Used to uninstall the service (_does not require a value_)

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

#### Profile Settings

- `Name`: (Unique String) The name of the profile
- `EmailSubject`: (String) The subject substring to match for the profile
    - **Recommendation:** Use a SHA1 value of the Name string to make sure that the emails have a unique value
- `EmailBody`: (String) The email body substring to match for the profile
    - `Save`: (True/False) Attribute for the EmailBody tag to allow saving the EmailBody
- `SaveAttachments`: (True/False) Defines if the attachments should be saved or not
- `SavePath`: (String) (Optional) Override for the globally defined save path
- `KeyDelimiter`: (String) (Optional) Override for the globally defined delimiter
- `SavePath`: (String) (Optional) Override for the globally defined save path

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
        - `DATETIME`: Creates a date/time stamp for the field
        - `DATE`: Creates a datestamp for the field
        - `TIME`: Creates a timestamp for the field 
        - `NULL`: Makes the field black (used for an empty placeholder)
    - `Search`: Looks inside the email body for the designated text inside of the element tag and gets the value after the tag and before the newline

*Example:*

Settings:
```xml
...
<KeyDelimiter>,</KeyDelimiter>
<Keys>
  <Attr1 Type="Static">Static Text</Attr1>
  <Attr2 Type="Dynamic">NULL</Attr2>
  <Attr3 Type="Search">Name:</Attr3>
  <Attr4 Type="Search">Phone:</Attr4>
  <Attr5 Type="Dynamic">DATETIME</Attr5>
</Keys>
...
```

```html
...
<p>Name: <b>Johnny</b></p>
<p>Phone: <b>123-456-7890</b></p>
...
```

```

```






