# PrePurchasing

PrePurchasing is a web application designed to streamline the steps required for making campus acquisitions. It allows users to submit any type of order request, including KFS, MyTravel, DPO/DRO/PR, and other campus services. It also aids in departmental approval processes associated with order requests, and integrates seamlessly with DaFIS/KFS.

PrePurchasing was developed in the open by programmers in two colleges and one school, with further integration by the Kuali KFS team. Any programmer on campus has the opportunity to view the code as it's being written and contribute what is important to their clients, and the tools used (GitHub) make it easy to manage and accept these changes. Project management is handled by another tool (Trello), also accessible to anyone, allowing efficient collaboration between programmers on and off campus.

Even more important is the open collaboration between programmers and the clients of the system. From the creative and motivated group of programmers to the dedicated beta testers to the business-savvy steering committee, this is a genuine team effort. A built-in feedback system (UserVoice) allows instant collaboration between beta testers and programmers; a user can report a bug or vote on a feature suggestion right in the application itself, and the programmers can see how popular suggestions are and make changes to the system. Questions about using the system are efficiently routed to the community manager for resolution, and if a question has been asked before it becomes part of an automatic Frequently Asked Questions (FAQ) list.

Also of note, PrePurchasing is a cloud application, which means instead of being housed on servers in the campus data center, it runs on services on the Internet (Microsoft Azure). Like the electricity in your home, PrePurchasing can be adjusted to meet the demands placed on it. And like your utility bills, the campus only pays for how much of it is being used. This is much less expensive than buying an equivalently robust system and housing facility. And the use of cloud-based tools provides better access to everyone; all of the tools mentioned in developing PrePurchasing are also cloud-based.

As a result of this agile, cloud-based development process, PrePurchasing has steadily morphed into an intuitive time- and effort-saving system, and can continue to adapt to the changing business needs of the campus. The entire effort has taken less than a year from start to finish.

The people involved, from all over campus, are the real heroes in this story:

http://prepurchasing.ucdavis.edu/humans.txt

If you want to learn more about PrePurchasing, please see our website at:

http://ucdavis.github.com/Purchasing/index.html


# Development

This project uses devcontainers, so make sure you have docker setup and then install the devcontainer extension for VSCode if necessary.

## SSL certs (required one time setup)

You need to setup your dev certs the first time you run a local .net project. This is a one time setup for your machine, and you can use the same cert for all your projects.

The setup is different for Windows and Mac/Linux, so make sure to follow the instructions for your OS:

### Mac/Linux

```bash
# 1 Clean out any old dev-certs (optional)
dotnet dev-certs https --clean

# 2 Create and trust a fresh cert (adds it to macOS keychain)
dotnet dev-certs https --trust

# 3 Export that cert to PFX so Kestrel can load it
mkdir -p ~/.aspnet/https
dotnet dev-certs https \
  -ep ~/.aspnet/https/aspnetapp.pfx \
  -p CaesLocalDevCertPW
```

### Windows

```powershell
# 1 Clean old ones
dotnet dev-certs https --clean

# 2 Create + trust (adds to Windows Cert Store)
dotnet dev-certs https --trust

# 3 Export PFX
$certPath   = "$env:USERPROFILE\.aspnet\https"
New-Item -ItemType Directory $certPath -Force | Out-Null
dotnet dev-certs https `
  -ep "$certPath\aspnetapp.pfx" `
  -p CaesLocalDevCertPW
```

### Windows (WSL)

If you work entirely inside WSL 2, just run the mac/Linux commands inside your WSL shell.

### What is this doing?

We setup a dev cert on your local computer and store it in a specific place. Our `docker-compose.yml` file mounts that location into the dev container, and we set some env variables (in `devcontainer.json`) to tell the .net core web server to use that cert.

## Configuration

You'll want to update your appsettings.Development.json file with all of the config settings your need.  Look in 1pass for the settings.

## Running the app

Just hit F5 (or Ctrl+F5) in VSCode and it will build the project and start the app. 

Alternately, you can also navigate to the "Purchasing.Mvc" folder and run the following command:

```bash
dotnet watch
```

This will start the application on `https://localhost:44396` using your ssl cert.
