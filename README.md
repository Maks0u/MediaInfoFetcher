# MediaWidget

This project provides a simple API to retrieve information about the currently playing media on a Windows system. It uses ASP.NET Core to create a local web server that serves media information, including title, artist, and thumbnail. The project also includes a static web page for displaying the media information.

## Features

- Retrieves current media information from Windows Media Transport Controls
- Provides a JSON API endpoint for easy integration
- Includes thumbnail data as base64-encoded string
- Runs as a local web server on port 5000
- Includes a static web page for displaying media information

## Requirements

- Windows 10 or later
- .NET 8.0 or later

## Installation

### Pre-built binary

1. Go to [release page](https://github.com/Maks0u/MediaWidget/releases/latest)
2. Download `MediaWidget.zip`
3. Extract zip file in a new folder
4. Run `MediaWidget.exe`

### Build from source

```
git clone https://github.com/Maks0u/MediaWidget.git
cd MediaWidget
dotnet build
dotnet run
```

## Usage

1. [Install](#installation) and run the application
2. The server will start on `http://localhost:5000`
3. Access the API endpoint at `http://localhost:5000/data`
4. In a web browser, access the static web page at `http://localhost:5000` or `http://localhost:5000/index.html`

## API Endpoint

### GET /data

Returns JSON data about the currently playing media.

Example response:

```json
{
  "title": "Song Title",
  "artist": "Artist Name",
  "thumbnail": {
    "base64": "base64encodedstring",
    "mimeType": "image/jpeg"
  }
}
```
