> Copyright 2014 Systemic Pty Ltd
> 
> Licensed under the Apache License, Version 2.0 (the "License");
> you may not use this file except in compliance with the License.
> You may obtain a copy of the License at
> 
> [http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0 "Apache License, Version 2.0")
> 
> Unless required by applicable law or agreed to in writing, software
> distributed under the License is distributed on an "AS IS" BASIS,
> WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
> See the License for the specific language governing permissions and
> limitations under the License.

##Summary

The SIF3 Framework is a .NET framework that enables developers to efficiently implement SIF3 Services (Consumers and/or Providers). It fully encapsulates the low level SIF3 Infrastructure. It also provides a basic Environment Provider which is core to SIF3.

The framework includes a demo Solution that illustrates how to use it.

    NOTE: This current release is a Beta version that should only be used for inspection and prototyping. It is still
    a work in progress and not intended for commercial use as yet.

###Download Instructions

#####Option 1 - As a Zip.
Click on the button marked "Download ZIP" available from the Code tab.

#####Option 2 - Using a Git command-line client.
From the command-line type: `git clone git@github.com:nsip/Sif3Framework-dotNet.git`

If you want to use this option but don't have a client installed, one can be downloaded from [http://git-scm.com/download](http://git-scm.com/download "Git command-line client download").

###Getting started

To get started using this framework, read the *Sif3Framework .NET Developer's Guide.doc* and *Sif3Framework .NET Demo Usage Guide.doc* documents under the Documentation directory.

##Version control history

**Jan 28, 2014 - 0.1.0 Initial commit; Beta release to be used for collaborative review**

- Beta release to be used for collaborative review only and not intended for commercial use.

**Jan 29, 2014 - 0.2.0 Partial implementation of POST for Environment**

- Submitted a partial implementation of the POST action for the Environments Controller.

**Feb 03, 2014 - 0.3.0 Create sessionToken from POST of Environment**

- Update the POST action of the EnvironmentsController to authenticate a user and return an appropriately populated Environment response.

**Feb 06, 2014 - 0.4.0 Implemented authentication and GET of Environment**

- Completed implementation of the GET action for an environment.
- Fixed an issue with authentication based upon session token.

**Feb 15, 2014 - 0.5.0 Implemented DELETE of Environment**

- Tweaked the overall design of the core code to properly enable the implementation of the DELETE action for an Environment.

**Feb 20, 2014 - 0.6.0 Redesign of the persistence layer**

- Redesigned the persistence layer (repositories) to add the flexibility to inject a different SessionFactory.
- Prepared the code for a demo provider.

**Mar 12, 2014 - 0.7.0 Added a demo provider project**

- Added a demo provider project to help illustrate how the framework can be used to provide StudentPersonal data.
- Re-factored some code as a result of this work.

**Jul 16, 2014 - 0.7.1 Code documentation updates**

- Minor updates to documentation within the code.

**Aug 05, 2014 - 0.8.0 Extract demo projects into separate solution; Renamed the Sif.Framework.Core project**

- Extract demo projects into separate Solution.
- Extract data model and infrastructure projects into separate Solution.
- Fixed issue with BaseController.
- Changed constructors in the persistence, Service and Controller layers to accommodate Dependency Injection.
- Updated in-code documentation.
- Updated SessionFactorys to cater for use as an executable as well as for deployment to IIS.
- Updated version of Web API and SQLite.
- Added NHibernate configuration and DDLs for use with SQLite, SQL Server LocalDB, SQL Server, Oracle and MySQL.
- Added a project make setting up the demo database easier.
- Added demo databases for SQLite and SQL Server LocalDB.
- Renamed the Sif.Framework.Core project to Sif.Framework.

**Aug 06,2014 - 0.9.0 Added version numbers to project assemblies**

- Added assembly information (including version number) to each project assembly.
- Updated README.md to provide a more comprehensive version control history.
- Recompiled and re-referenced libraries in SharedLibs.
- Better organised the Scripts directory.

**Aug 07, 2014 - 0.10.0 Fixed issues with assembly references**

- Fixed issues with referencing of Sif.Specification.Infrastructure assembly.
- Renamed StudentPersonal.cfg.xml to Demo.cfg.xml to make the file name less specific.
- Fixed issue of incorrectly referenced Sif.Framework assembly in the Demo Provider.
- Added scripts to ease demo execution.

**Aug 19, 2014 - 0.10.1 Added user documentation**

- Added a draft version of the Developer's Guide.
- Added a draft version of the Demo Usage Guide.

**Aug 27, 2014 - 0.11.0 Implemented a Consumer framework**

- Added a generic Consumer to the framework.
- Added a utility class for HTTP operations.
- Created a new StudentPersonal demo Consumer.
- Updated .gitignore so that "x64" directories are no longer ignored (caused problems with SQLite DLLs).
- Re-ordered the projects listed in the VS Solutions to manage the default projects run when debugging.

**Aug 30, 2014 - 0.12.0 Upgraded to SIF Infrastructure 3.0.1**

- Upgraded the framework to use SIF Infrastructure 3.0.1.
- Updated the data models to the latest version of the SIF AU 1.3 Data Model.
- Re-designed the XML serialisation code to provide for better extensibility.
- Fixed an issue whereby the root element of collections returned by Controllers started with "ArrayOf".
- Made enhancements to the demo Setup.

**Aug 31, 2014 - 0.13.0 Fixed various issues**

- Fixed an issue with clean-up if a Consumer fails to register with the Environment Provider.
- Fixed an error in the returned Environment object on Consumer register.
- Added exception handling to the demo Consumer to ensure proper clean-up after an error.
