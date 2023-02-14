using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System.Text.Json;

namespace Flattiverse.Connector
{
    /// <summary>
    /// Represents an universe within your universe group.
    /// </summary>
    public class Universe
    {
        /// <summary>
        /// The ID of the universe.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// The name of the universe.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The UniverseGroup this object belongs to.
        /// </summary>
        public readonly UniverseGroup Group;

        internal Universe(UniverseGroup group, JsonElement element)
        {
            Utils.Traverse(element, out ID, "id");
            Utils.Traverse(element, out Name, "name");

            Group = group;
        }

        /// <summary>
        /// Creates or updates a unit in the map of this universe group. The unit will be overwritten if the unit already
        /// exists (same name) and the colliding unit isn't a non editable unit like a player unit, shot or explosion.
        /// </summary>
        /// <param name="definition">The JSON formatted definition of the unit to create. Please refer to PROCOTOL.md for further information.</param>
        /// <remarks>This method is only accessible if you are an administrator.</remarks>
        public async Task SetUnit(string definition)
        {
            if (string.IsNullOrEmpty(definition))
                throw new GameException(0xB0);

            if (definition.Length > 2048)
                throw new GameException(0xB1);

            using (Query query = Group.connection.Query("unitSet"))
            {
                query.Write("universe", ID);
                query.Write("unit", definition);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Removes an unit from the universe.
        /// </summary>
        /// <param name="name">The name of the unit.</param>
        /// <returns>Nothing, or a GameException.</returns>
        /// <exception cref="GameException">Throws when trying to remove a non editable or non existing unit.</exception>
        public async Task RemoveUnit(string name)
        {
            if (!Utils.CheckName(name))
                throw new GameException(0x02);

            using (Query query = Group.connection.Query("unitRemove"))
            {
                query.Write("universe", ID);
                query.Write("unit", name);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves the json definition of all systems in the universegroup.
        /// </summary>
        /// <returns>All systems.</returns>
        public async Task<List<GameRegion>> GetRegions()
        {
            using (Query query = Group.connection.Query("regionList"))
            {
                query.Write("universe", ID);

                await query.Send().ConfigureAwait(false);

                List<GameRegion> regions = new List<GameRegion>();
                JsonElement element = await query.ReceiveJson().ConfigureAwait(false);
                if (element.ValueKind == JsonValueKind.Array)
                    foreach (JsonElement regionObject in element.EnumerateArray())
                    {
                        regions.Add(new GameRegion(regionObject));
                    }

                return regions;
            }
        }

        /// <summary>
        /// Creates or updates a region in the map of this universe group. The region will be overwritten if the region already
        /// exists (same id).
        /// </summary>
        /// <param name="definition">The JSON formatted definition of the region to create. Please refer to PROCOTOL.md for further information.</param>
        /// <remarks>This method is only accessible if you are an administrator.</remarks>
        public async Task SetRegion(int id, int teams, string name, double left, double top, double right, double bottom, bool startLocation, bool safeZone, bool slowRestore)
        {
            if (name is null)
            {
                using (Query query = Group.connection.Query("regionSetUnnamed"))
                {
                    query.Write("universe", ID);
                    query.Write("regionId", id);
                    query.Write("teams", teams);
                    query.Write("left", left);
                    query.Write("top", top);
                    query.Write("right", right);
                    query.Write("bottom", bottom);
                    query.Write("startLocation", startLocation);
                    query.Write("safeZone", safeZone);
                    query.Write("slowRestore", slowRestore);

                    await query.Send().ConfigureAwait(false);

                    await query.Wait().ConfigureAwait(false);
                }
            }
            else
            {
                using (Query query = Group.connection.Query("regionSet"))
                {
                    query.Write("universe", ID);
                    query.Write("regionId", id);
                    query.Write("teams", teams);
                    query.Write("name", name);
                    query.Write("left", left);
                    query.Write("top", top);
                    query.Write("right", right);
                    query.Write("bottom", bottom);
                    query.Write("startLocation", startLocation);
                    query.Write("safeZone", safeZone);
                    query.Write("slowRestore", slowRestore);

                    await query.Send().ConfigureAwait(false);

                    await query.Wait().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Removes an unit from the universe.
        /// </summary>
        /// <param name="name">The name of the unit.</param>
        /// <returns>Nothing, or a GameException.</returns>
        /// <exception cref="GameException">Throws when trying to remove a non editable or non existing unit.</exception>
        public async Task RemoveRegion(int id)
        {
            using (Query query = Group.connection.Query("regionRemove"))
            {
                query.Write("universe", ID);
                query.Write("regionId", id);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves the json definition for the map editor of a unit from the universe.
        /// </summary>
        /// <param name="name">The name of the unit.</param>
        /// <returns>The map editable json string.</returns>
        /// <exception cref="GameException">Throws when trying to access a non editable unit.</exception>
        public async Task<string> GetUnitMapEditJson(string name)
        {
            if (!Utils.CheckName(name))
                throw new GameException(0x02);

            using (Query query = Group.connection.Query("unitGet"))
            {
                query.Write("universe", ID);
                query.Write("unit", name);

                await query.Send().ConfigureAwait(false);

                return await query.ReceiveString().ConfigureAwait(false);
            }
        }
    }
}