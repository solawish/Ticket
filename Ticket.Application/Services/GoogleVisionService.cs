using Google.Cloud.Vision.V1;

namespace Ticket.Application.Services;

public class GoogleVisionService
{
    private readonly ImageAnnotatorClient _client;

    public GoogleVisionService(ImageAnnotatorClient client)
    {
        _client = client;
    }

    public async Task<string> GetTextFromImageAsync(Stream stream)
    {
        Image image = await Image.FromStreamAsync(stream);
        var imageAnnotator = await _client.DetectTextAsync(image);
        var text = imageAnnotator.FirstOrDefault()?.Description;
        return text;
    }
}