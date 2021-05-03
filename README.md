**Distributed EHR**

*By Andrew Smith*

Due to the nature of the various components it will be difficult to set up and run this system in its entirety.
Since the server and database are hosted on live servers with DNS records pointed at them you will be able to interact
with the existing setup.  

To run the client application, on a windows pc
-Navigate to: DistributedEHR\EHRClient\bin\Release\netcoreapp3.1
-Run EHR.Client.exe

Logins
-Users: JD,     Turk,     Carla, Elliot, Danielle, Andrew
-Roles: Doctor, Doctor,   Nurse, Doctor, Doctor,   Admin

Credentials
Username: JD
Email: asmith1138+jd@gmail.com
Password: JD@Password3

Username: Turk
Email: asmith1138+turk@gmail.com
Password: ***

Username: Carla
Email: asmith1138+carla@gmail.com
Password: ***

Username: Elliot
Email: asmith1138+elliot@gmail.com
Password: ***

Username: Danielle
Email: asmith1138+danielle@gmail.com
Password: ***

Username: Andrew
Email: asmith1138@gmail.com
Password: ***


If you wish to build, you will need visual studio with .Net Core 3.1 and you just open the solution and build
Alternatively, with the dotnet CLI you can buil with 'dotnet build' from the folder of the solution/project you wish to build.
You will also need to publish to get the program to export to the publish folder above,
use the netcore3.1 publish profile or via the CLI 'dotnet publish'

To run the server:
This will be difficult but I will attempt to explain.  
1: Build via Visual Studio or CLI 'dotnet build'
2: In DistributedEHR\EHR.Server\appsettings.json change the connection string to point to your database
3: The database needs built, either run EHR.sql on you database called EHR
or through visual studio in the Package Manager console run 'Update-Database'
this may require 'Add-Migration Initial' first
4: In DistributedEHR\EHRClient\appsettings.json change the APIUrl and HubUrl to you localhost:\<port>\
5: Run the debugger in Visual studio, the server should start then the client app and it should be pointed at you local version
6: Note: the EHR.sql only contains users Andrew and Danielle and Andrew is in the role Nurse not Admin
