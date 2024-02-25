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

    public Universe()
    {
        BaseURI = "www.flattiverse.com";
        UseSSL = true;
    }

    public Universe(bool useSsl, string baseUri)
    {
        UseSSL = useSsl;
        BaseURI = baseUri;
    }

    public async Task Update()
    {
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
                        GalaxyInfo rGalaxyInfo = new GalaxyInfo(element);
                        lGalaxies.Add(rGalaxyInfo.Name, rGalaxyInfo);
                    }

                galaxies = lGalaxies;
            }
            else
                throw new GameException(0xF1);
        }
    }

    public ReadOnlyDictionary<string, GalaxyInfo> Galaxies => new ReadOnlyDictionary<string, GalaxyInfo>(galaxies);
    
    public async Task<Galaxy> Join(string uri, string auth, byte teamId)
    {
        Galaxy galaxy = new Galaxy(this);
        
        await galaxy.Connect(uri, auth, teamId);

        // We wait so that we are sure that we have all meta data.
        await galaxy.WaitLoginCompleted();
        
        return galaxy;
    }
}