# XML to JSON Converter

This repository contains a .NET Core Web API and a React client application for uploading XML files, converting them to JSON, and saving them to a directory.

## Prerequisites

- .NET 6 SDK
- Node.js

## Libraries Used

This project uses several libraries to implement its functionality and tests.

### .NET Libraries

- **Moq:** A library for creating mock objects in unit tests.
- **Newtonsoft.Json:** A high-performance JSON framework for .NET, used for converting XML to JSON.
- **xUnit:** A unit testing tool for the .NET Framework.

### React Libraries

- **axios:** A promise-based HTTP client, used for making requests to the .NET API.
- **react-bootstrap:** A front-end framework for React, used for styling the client application.

## Running the .NET API

1. Navigate to the `XmltoJsonConverter` project directory.
2. Run `dotnet restore` to restore the necessary NuGet packages.
3. Run `dotnet run` to start the application.
4. The API will be running at `https://localhost:7097`.
5. Access the Swagger UI at `https://localhost:7097/swagger` to interact with the API without setting up the React client.

## Running the React Client

1. Navigate to the `xml-react` directory.
2. Run `npm install` to install the necessary npm packages.
3. Run `npm start` to start the application.
4. The client will be running at `http://localhost:3000`.

## Using the Application

1. Open your web browser and navigate to `http://localhost:3000`.
2. Select an XML file to upload and click the "Upload" button.
3. The converted JSON file will be saved in the `XmltoJsonConverter/SavedJsonFiles` directory.
4. You can view a list of converted files by making a GET request to the /fileconvert/list endpoint.
5. You can download a specific file by making a GET request to the /fileconvert/download/{fileName} endpoint, replacing {fileName} with the actual file name.

## Using Swagger UI

If you want to test the API without setting up the React client, you can use Swagger UI:

1. Start the .NET API as described above.
2. Open your web browser and navigate to `https://localhost:7097/swagger`.
3. Use the Swagger UI to send a POST request to the `/fileconvert` endpoint. You'll need to select an XML file to upload as part of the request(can select multiple files)
4. Use the Swagger UI to send a GET request to the /fileconvert/download/{fileName} endpoint to download a specific file, replacing {fileName} with the actual file name.

## Running the tests

The tests for the .NET API are written using the xUnit framework.

1. Navigate to the `XmltoJsonTests` project directory.
2. Run `dotnet restore` to restore the necessary NuGet packages.
3. Run `dotnet test` to execute the tests.

## Checking File Statuses

The application now provides a way to check the status of uploaded files. Here's how to use this functionality:

1. Upload a file as described in the "Using the Application" section above. The server will return a unique file ID for each uploaded file.
2. Use this file ID to get the status of the file. If you're using the React client, the file status will be automatically updated in the table on the right side of the screen. If you're using Swagger UI, you can send a GET request to the /fileconvert/{fileId} endpoint, replacing {fileId} with the actual file ID.

## Understanding File Statuses

The status of a file can be one of the following:

1. Processing: The file has been received and is currently being processed.
2. Completed: The file has been successfully converted to JSON and saved to the XmltoJsonConverter/SavedJsonFiles directory.
3. Error: {errorMessage}: An error occurred while processing the file. The error message provides more information about what went wrong. Possible errors include invalid XML format, file system errors, and server errors.

## Listing and Downloading Files

The application provides the ability to list all converted files and to download a specific file.

1. To list all the converted files, make a GET request to the /fileconvert/list endpoint. The response will be a list of file names.
2. To download a specific file, make a GET request to the /fileconvert/download/{fileName} endpoint, replacing {fileName} with the actual file name. The response will be the file itself.
