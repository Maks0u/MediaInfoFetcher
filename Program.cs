using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Media.Control;
using Windows.Storage.Streams;

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

class Program
{
  static async Task Main(string[] args)
  {
    try
    {
      var gsmtcsm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
      var currentSession = gsmtcsm.GetCurrentSession();
      if (currentSession == null)
      {
        Console.Error.WriteLine("No active media session found");
        return;
      }

      await PrintMediaProperties(currentSession);

      currentSession.MediaPropertiesChanged += async (sender, e) =>
      {
        await PrintMediaProperties(currentSession);
      };

      Console.ReadKey();
    }
    catch (Exception e)
    {
      Console.Error.WriteLine(e.Message);
    }
  }

  static async Task PrintMediaProperties(GlobalSystemMediaTransportControlsSession session)
  {
    var mediaProperties = await session.TryGetMediaPropertiesAsync();
    if (mediaProperties == null)
    {
      Console.Error.WriteLine("Unable to retrieve media properties");
      return;
    }

    var mediaInfo = new MediaInfo
    {
      Title = mediaProperties.Title ?? "",
      Artist = mediaProperties.Artist ?? "",
      Thumbnail = await GetThumbnailInfoAsync(mediaProperties.Thumbnail),
    };

    string jsonString = JsonSerializer.Serialize(mediaInfo, MediaInfoContext.Default.MediaInfo);
    Console.WriteLine(jsonString);
  }

  static async Task<ThumbnailInfo> GetThumbnailInfoAsync(
      IRandomAccessStreamReference thumbnailRef
  )
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
