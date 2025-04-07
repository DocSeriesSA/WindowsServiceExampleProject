# doc.ecm Windows Service Example - Visual Studio C#

This is a simple example of a **Windows Service** designed to interact with the `doc.ecm` API.  
`doc.ecm` is a Document Electronic Management system that allows organizations to manage, modify, and integrate documents programmatically.

This project is aimed at advanced users and partners who want to automate tasks or integrate `doc.ecm` with external systems, such as:

- **SAP**
- **Digital signature providers**
- **Custom document workflows**
- **External databases or ERP systems**

## 🛠 What This Example Shows

- How to create and run a basic Windows Service in .NET
- How to authenticate and communicate with the `doc.ecm` API
- A simple structure to customize your own service logic

## 📁 Project Structure
* WindowsServiceExample is your windows service. Check in program.cs the start routine. Use Debug to code and always compile in Release (x64).
  
* Doc.ECM.ApiHelper.Static is our example API Helper. With this class you can query and use our API endpoints. Check the API swagger for more information about our endpoints.

## Limits
Be sure to use one token until the expiration date before asking for a new one. This project already handle this for you. There is a limit that we are going to introduce in the future.

Respect the call limit. Do not perform lots of API calls (1440 max per day). The only exception is the Object Save endpoint, that can be called as many times as you want, but only to perform real updates (that is, changing at least one value).

## 🚀 Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/DocSeriesSA/WindowsServiceExampleProject.git
2. Run the project once, then update the configuration file with your actual doc.ecm API credentials and endpoints.
3. Build and install the service using sc.exe or PowerShell.
4. Start the service and monitor the logs to see how it interacts with doc.ecm.

