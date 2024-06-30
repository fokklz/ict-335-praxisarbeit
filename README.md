# SaveUp App <!-- omit in toc -->

<!--TOC-->
- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Running the Backend](#running-the-backend)
  - [Running the App](#running-the-app)
<!--/TOC-->

## Overview

SaveUp is a mobile application designed to help users track their daily expenses and savings. The app allows users to log items they choose to forgo purchasing, along with a brief description and price, helping them visualize and manage their savings. The app features an intuitive and easy-to-use interface, optimized for both phones and tablets, ensuring a user-friendly experience.

## Prerequisites

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop)
- [MAUI Enabled Visual Studio 2022](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-8.0&tabs=vswin)

## Getting Started

Clone the repository from GitHub and navigate to the project directory.

```shell
git clone https://github.com/fokklz/ict-335-praxisarbeit.git
cd ict-335-praxisarbeit
```

### Running the Backend

For the app to work, the backend must be running. Since the backend is not hosted anywhere, you will need to run it locally. 
To run the backend, you can use the following command in the root directory of the project. (`ict-335-praxisarbeit`)

```shell
docker compose up -d
```
The backend will be available at [http://localhost:8000](http://localhost:8000/swagger)

To shut down the backend, you can run the following command in the same directory.

```shell
docker compose down
```

### Running the App

To run the app, you can open the `SaveUp.sln` file in Visual Studio 2022 and run the app with the green play button.
The app is developed to be compatible with various devices, including phones and tablets.

By default, the app will search for local connections. You can alter this behavior by defining `API:BaseURL` in the `appsettings.json` file - be cautious with emulators as they use special local IPs to access the host machine.

While the app was occasionally tested on WinUI to ensure functionality, further testing was not performed on MacOS/iOS or Linux due to resource constraints. The project kept all operating systems in mind when developing platform-specific code, even though not all were tested.
