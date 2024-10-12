using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Media.Control;
using Windows.Storage.Streams;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

class Program
{
  static void Main(string[] args)
  {
    try
    {
      var builder = WebApplication.CreateBuilder(args);
      builder.WebHost.ConfigureKestrel(serverOptions =>
      {
        serverOptions.ListenLocalhost(5000);
      });

      var app = builder.Build();

      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.MapGet("/data", () => GetMediaPropertiesJson());
      app.Run();
    }
    catch (Exception e)
    {
      Console.Error.WriteLine(e.Message);
    }
  }

  static async Task<string> GetMediaPropertiesJson()
  {
    MediaInfo mediaInfo = await GetMediaProperties();
    return JsonSerializer.Serialize(mediaInfo, MediaInfoContext.Default.MediaInfo);
  }

  static async Task<MediaInfo> GetMediaProperties()
  {
    var sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
    var session = sessionManager.GetCurrentSession();
    if (session == null) return null;

    var mediaProperties = await session.TryGetMediaPropertiesAsync();
    if (mediaProperties == null) return null;

    return new MediaInfo
    {
      Title = mediaProperties.Title ?? "",
      Artist = mediaProperties.Artist ?? "",
      Thumbnail = await GetThumbnailInfoAsync(mediaProperties.Thumbnail),
    };
  }

  static async Task<ThumbnailInfo> GetThumbnailInfoAsync(IRandomAccessStreamReference thumbnailRef)
  {
    if (thumbnailRef == null)
    {
      return null;
    }

    using (var stream = await thumbnailRef.OpenReadAsync())
    {
      var reader = new DataReader(stream);
      await reader.LoadAsync((uint)stream.Size);
      byte[] bytes = new byte[stream.Size];
      reader.ReadBytes(bytes);

      return new ThumbnailInfo
      {
        Base64 = Convert.ToBase64String(bytes),
        MimeType = stream.ContentType,
      };
    }
  }
}

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(MediaInfo))]
internal partial class MediaInfoContext : JsonSerializerContext { }

public class MediaInfo
{
  [JsonPropertyName("title")]
  public string Title { get; set; }

  [JsonPropertyName("artist")]
  public string Artist { get; set; }

  [JsonPropertyName("thumbnail")]
  public ThumbnailInfo Thumbnail { get; set; }
}

public class ThumbnailInfo
{
  [JsonPropertyName("base64")]
  public string Base64 { get; set; }

  [JsonPropertyName("mimeType")]
  public string MimeType { get; set; }
}
