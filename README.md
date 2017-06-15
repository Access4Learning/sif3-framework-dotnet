> Copyright 2017 Systemic Pty Ltd
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

### Contributing to this framework

See the [wiki associated with this repository](https://github.com/Access4Learning/sif3-framework-dotnet/wiki) for information on: 

* contributing to this framework 
* the coding style to be used and
* the structure of the SIF 3 Framework repositories

###Download Instructions

#####Option 1 - As a Zip.
Click on the button marked "Download ZIP" available from the Code tab.

#####Option 2 - Using a Git command-line client.
From the command-line type: `git clone https://github.com/nsip/sif3-framework-dotnet.git`

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

**Sep 01, 2014 - 0.13.1 Updated user documentation**

- Updated the Developer's Guide from user feedback.
- Updated the Demo Usage Guide to include instructions for implementing a Consumer.

**Sep 01, 2014 - 0.13.2 Updated user documentation**

- Updated the Demo Usage Guide to include instructions for running the demo over a LAN.

**Sep 11, 2014 - 0.14.0 Created a reference implementation of the SBP**

- Created a new Solution to contain a reference implementation of the SBP (only partially implemented).
- Upgraded Web API to version 5.2.2 on all appropriate projects.
- Added a SchoolInfo Consumer and Provider to the demo Solution.

**Oct 17, 2014 - 0.15.0 Added demo projects for the US locale**

- Upgraded NHibernate to version 4.0.1.4000 on all appropriate projects.
- Upgraded SQLite to version 1.0.94.0 on all appropriate projects.
- Added log4net to some projects.
- Added debug statements in some projects using log4net.
- Added error messages to the payload of GenericController response messages.
- Changed the existing demo Consumer and Provider projects to be AU locale specific.
- Made the AU Consumer and Provider projects simpler by removing the use of a database for retrieving sample student data from.
- Added new demo Consumer and Provider projects for the US locale.
- Updated the demo Setup project to cater for the AU and US locales.
- Added data models generated from the SIF US 3.2 XSDs to the Sif3Specification Solution.
- Updated the demo execution batch scripts to cater for the new demo projects.
- Updated the SharedLibs libraries.

**Oct 20, 2014 - 0.15.1 Added training exercise document**

- Added the Training Exercises (US) document.

**Nov 15, 2014 - 0.16.0 Handle payload-free POST requests for Environment**

- Implement the ability to handle payload-free POST requests for the EnvironmentsController (Simple SIF).
- Upgrade the GenericConsumer to take a solutionId.
- Improved logging of error messages.
- Added the Training Exercises (AU) document.

**Dec 22, 2014 - 0.16.1 Added documentation for URL postfix extensions**

- Added documentation to explain how to specify MIME Types using URL postfix extensions (Simple SIF).
- Enabled URL postfix extensions for MIME Types in the demo AU Provider.

**Jan 09, 2015 - 0.17.0 Improved exception handling and logging**

- Added exception classes to better manage exception handling and information.
- Implemented global error handling guidelines for Web API 2 - handlers and loggers.
- Added utility classes to help collate and extract error information from response messages.
- Updated the ConsumerApp to display more meaningful error details.
- Updated the EnvironmentsController to support better error messages.

**Jan 10, 2015 - 0.17.1 Simplified demo project to make it more intuitive**

- Based on feedback, deleted the SbpFramework Solution and instead incorporated its code into the Sif3FrameworkDemo Solution to reduce complexity and confusion.
- Based on feedback, removed shared code (projects) from the Sif3FrameworkDemo Solution to better reflect implementations where Consumers and Providers are developed by different vendors.

**Jan 26, 2015 - 0.17.2 Fixed urgent issue with demo AU Consumer**

- Fixed an issue introduced in version 0.17.0 whereby the demo AU Consumer referenced a non-existant file - SifFramework.brokered.config.
- Updated the demo AU Consumer and Provider to better reflect exception handling and logging enhancements in the SifFramework library.
- Updated the demo US Consumer and Provider to match the changes in the AU versions.

**Mar 29, 2015 - 0.18.0 Added ability to connect to a SIF Broker**

- Updated the GenericConsumer to allow registration to a SIF Broker, as well as (direct) to an Environment Provider.
- Updated the GenericController to allow creation of Service Providers that can connect to a SIF Broker, as well as run (directly) as an Environment/Service Provider.
- Added functionality for Consumers and Providers to store the session token (received after service registration) locally so that state can be maintained between Consumer and Provider sessions.
- Improved exception handling and logging in the SifFramework library.
- Updated the demo Consumers and Providers to reflect these changes.

**Mar 30, 2015 - 0.18.1 Updated documentation to reflect recent changes**

- Updated the US Provider project with configuration changes that should have been made in the last submission.
- Updated all documentation to reflect recent changes. Documentation on SIF Broker integration is still incomplete.

**May 03, 2015 - 0.19.0 Implement paging of retrieved data**

- Updated the service interface to facilitate paging of retrieved data.
- Updated the GenericConsumer to make paged retrievals by default.
- Updated the GenericController to handle (GET) requests for paged data.
- Updated the demo Consumers and Providers to reflect these changes.

**May 11, 2015 - 0.19.1 Update version of the SIF AU 1.3 data model**

- Updated the SIF AU 1.3 data model of the Sif3Specification Solution.

**May 17, 2015 - 0.20.0 Implement Query By Example**

- Updated the GenericConsumer to add a new Retrieve method that accepts an "example" object.
- Updated the Get method of the GenericController to handle a payload when a method override is requested.
- Updated the demo AU Provider configuration to manage redirection when a method override is requested.
- Updated the demo AU Consumer with an example call that uses the new Retrieve method.

**July 17, 2015 - 0.20.1 Added beta version of the SIF AU 1.4 data model**

- Added a beta version of the SIF AU 1.4 data model to the Sif3Specification Solution.

**Sept 23, 2015 - 0.21.0 Updated SIF AU data model and added HITS Consumer**

- Updated the SIF AU 1.3 and 1.4 data models of the Sif3Specification Solution.
- Updated all unit tests and (AU) demo projects to reference the updated 1.4 data models.
- Fixed an issue with a missing namespace on serialisation of data model collections.
- Enhanced the demo AU Consumer to demonstrate connection with HITS.

**Sept 28, 2015 - 0.22.0 Implement Service Paths**

- Updated the Framework service layer, Consumers and Providers to handle Service Paths.
- Updated (AU) demo projects to demonstrate Service Path usage.

**Jan 17, 2016 - 1.0.0 Implement multiple object operations**

- Redesigned Consumer implementation to handle multiple object operations.
- Redesigned Provider implementation to handle multiple object operations.
- Updated AU and US demo projects to reflect mutliple object operations.
- Updated documentation to reflect changes.
- Fixed issue with mustUseAdvisory implementation.

**Jan 29, 2016 - 1.1.0 Added Zone and Context using Matrix Parameters**

- Upgraded Web API to version 5.2.3 on all appropriate projects.
- Enhanced and configured the WebApi implementation to recognise Matrix Parameters.
- Updated Consumers to pass Zone and Context with all requests using Matrix Parameters.
- Updated Providers to handle receiving Zone and Context as Matrix Parameters.
- Updated AU and US demo projects to reflect the use of Matrix Parameters.

**Feb 03, 2016 - 1.1.1 Added SIF US 3.3 data model**

- Added the SIF US 3.3 data model to the Sif3Specification Solution.
- Updated US demo projects to use SIF US 3.3 model objects.
- Added a Service Path exercise to the AU and US training exercises.

**Feb 04, 2016 - 1.1.2 Resolve issue with Visual Studio 2013**

- Applied code change due to compiler error that occurs in VS 2013 but not VS 2015.

**May 24, 2016 - 2.0.0 Resolve issue with default zone implementation**

- Fix an issue regarding the definition of a default zone.
- Generate SIF AU 3.4 data model and incorporate into demo AU projects.
- Remove Visual Studio 2015 temporary files/directories from GitHub.
- Enchance AU demo projects to incorporate extended elements to StudentPersonal.
- Expose exception classes by making them public.
- Remove redundant local database files.
- Minor code updates.

**July 19, 2016 - 3.0.0 Added Functional Service support**

- Implemented function service providers
- Implemented function service consumers
- Implemented binding of SIF objects (e.g. jobs) to a specific consumer (curruntly only supported in functional services)
- Implemented support for multiple enviroment templates
- Generated SIF UK 2.0 data model and incorporated into UK demo
- Generated SIF 3.2 Infrastructure model and incorporated in UK/US/AU demos
- Added scripts to lauch projects from the command line
- Added scripts to build projects from the command line

**Jan 06, 2017 - 3.1.0 Implement payload compression and Changes Since**

- Added a new demo project specifically for connecting to HITS.
- Moved HITS specific Consumers from the demo.au.consumer project to the demo.hits.consumer project.
- Added documentation for creating a Consumer for HITS connection.
- Updated AU data models to SIF AU 3.4 in demo projects.
- Removed SharedLibs/Sif.Framework 1.2.0 as that version was never released.
- Fixed issue whereby SharedLibs/Sif.Framework 2.0.0 incorrectly contained version 3.0.0 of the framework.
- Implemented message payload compression.
- Implemented Changes Since mechanism.

**Apr 30, 2017 - 3.2.0 Updated to SIF Infrastructure 3.2.0**

- Fixed issue with incorrectly named matrix parameters (zone to zoneId, context to contextId).
- Completed and fixed SIF_HMACSHA256 authentication implementation.
- Updated the SIF Framework to use the latest version of the SIF Infrastructure (3.2.0).
- Fixed a namespace issue with the implementation of the SIF AU Data Model (3.4.0).
- Implemented the serviceType header (with a value of FUNCTIONAL) for Functional Service calls.
- Updated the SIF Framework to look for the SifFramework.config file in the current path first and the application folder second.

**May 03, 2017 - 3.2.1 Updated to SIF Infrastructure 3.2.1, AU Data Model 3.4.1**

- Updated the SIF Framework to use the latest version of the SIF Infrastructure (3.2.1).
- Updated the SIF Framework to use the latest version of the SIF AU Data Model (3.4.1).
- Implemented the Initialization object for Functional Service Jobs.

**June 10, 2017 - 3.2.1.1 Applied updates to AU Data Model 3.4.1**

- Updated the SIF Framework to use the officially approved version of the SIF AU Data Model (3.4.1).
- Fixed namespace issues with SIF AU Data Model usage in the demo projects.
- Updated the "Connecting Consumers to HITS" document to reflect recent updates to the HITS Dashboard and its usage.

**June 15, 2017 - 3.2.1.2 Updated to use strict version of AU Data Model 3.4.1**

- Updated the SIF Framework to use the "strict" version of the SIF AU Data Model (3.4.1).