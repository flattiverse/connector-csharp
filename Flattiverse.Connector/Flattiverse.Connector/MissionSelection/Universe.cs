using System.Collections.ObjectModel;
using Flattiverse.Connector.Hierarchy;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.MissionSelection;

public class Universe
{
    public readonly string BaseURI;
    public readonly bool UseSSL;

    private Dictionary<string, GalaxyInfo> galaxies = new Dictionary<string, GalaxyInfo>();

    private DateTime lastChecked;

    public Universe()
    {
        BaseURI = "www.flattiverse.com";
        UseSSL = true;
        
        Update().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Universe(bool useSsl, string baseUri)
    {
        UseSSL = useSsl;
        BaseURI = baseUri;
        
        Update().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Updates the data in the universe.
    /// </summary>
    /// <exception cref="GameException">Thrown, if you call Update() too often.</exception>
    /// <remarks>Refrain from calling this with a shorter interval than 15 seconds.</remarks>
    public async Task Update()
    {
        if (DateTime.UtcNow - lastChecked < new TimeSpan(0, 0, 0, 15))
            throw new GameException(0xEF);

        lastChecked = DateTime.UtcNow;
        
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(UseSSL ? $"https://{BaseURI}/api/galaxies/all" : $"http://{BaseURI}/api/galaxies/all");
            
            if (response.IsSuccessStatusCode)
            {
                await using Stream stream = await response.Content.ReadAsStreamAsync();
                using JsonDocument document = await JsonDocument.ParseAsync(stream);
                
                /*galaxies: [{
                  id, name, gameType, max*, allowSpectating,
                  teams: [{ id, name, red, green, blue }],
                  players: [{
                    id, name, datePlayedStart, hasAvatar (imageData is not DbNull),
                    globalStats: { statsKills, statsDeaths, statsColissions },
                    gameStats: { sessionKills, sessionDeaths, sessionCollisions },
                    sessionTeam }]
                }]*/
                
                if (document.RootElement.ValueKind != JsonValueKind.Array)
                    throw new GameException(0xF1);

                Dictionary<string, GalaxyInfo> lGalaxies = new Dictionary<string, GalaxyInfo>();
                
                foreach (JsonElement element in document.RootElement.EnumerateArray())
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        GalaxyInfo rGalaxyInfo = new GalaxyInfo(this, element);
                        lGalaxies.Add(rGalaxyInfo.Name, rGalaxyInfo);
                    }

                galaxies = lGalaxies;
            }
            else
                throw new GameException(0xF1);
        }
    }

    public ReadOnlyDictionary<string, GalaxyInfo> Galaxies => new ReadOnlyDictionary<string, GalaxyInfo>(galaxies);
}