namespace Flattiverse.Connector.Units;

/// <summary>
/// Describes the visual archetype of a meteoroid.
/// </summary>
public enum MeteoroidType : byte
{
    /// <summary>
    /// A familiar gray rock with speckles and fracture lines, the kind of space debris that looks exactly like "a rock in space."
    /// It tumbles with uncomplicated honesty and rarely surprises anyone-useful for calibrating expectations and lowering blood pressure.
    /// The universe produces these in bulk, like it got bored and started copy-pasting.
    /// </summary>
    StonyFragment,

    /// <summary>
    /// A compact, dark-metal lump that catches highlights in sharp flashes, like a coin flipping forever in slow motion.
    /// It looks small until you notice how stubbornly it holds its presence, dragging a faint wake of smaller grit behind it.
    /// Old-timers swear these are the "bones" of larger bodies, condensed into dense little problems that refuse to break politely.
    /// </summary>
    /// <remarks>
    /// Expected resources: Metal.
    /// </remarks>
    MetallicSlug,

    /// <summary>
    /// A soot-black chunk with a dull surface and crumbly edges, as if it is embarrassed to be seen under direct light.
    /// It sheds fine dust when warmed, leaving a smoky ribbon that makes it look dramatic in the worst possible way.
    /// Collectors like the romance of it-ancient, primitive, quietly strange-while practical crews just call it "the one that gets everywhere."
    /// </summary>
    /// <remarks>
    /// Expected resources: Carbon.
    /// </remarks>
    CarbonaceousChunk,

    /// <summary>
    /// A pale, translucent fragment that wears a faint halo when it drifts through warmer regions, like a ghost trying to remember solidity.
    /// Its surface is pocked and layered, alternating milky bands and clear glassy patches that sparkle then vanish as it rotates.
    /// It feels alive in a way rocks should not: always changing, always faintly shedding, always reminding you that "solid" is sometimes a temporary agreement.
    /// </summary>
    /// <remarks>
    /// Expected resources: Hydrogen.
    /// </remarks>
    CometaryIceFragment,

    /// <summary>
    /// A marbled shard of mixed textures-bright metallic veins threaded through darker stone-like someone poured molten metal into a cracked statue.
    /// It reflects light in inconsistent flashes, never quite symmetrical, always a little hostile to neat geometry.
    /// Every face looks like it belongs to a different parent body, and that patchwork quality gives it a reputation for being stubborn, unpredictable, and oddly charismatic.
    /// </summary>
    StonyIronShard,

    /// <summary>
    /// A glossy, obsidian-like body with smooth curves and mirror flashes that make it look briefly artificial at certain angles.
    /// It is the kind of object that triggers arguments: "Is that manufactured?" "No, it is just annoyingly pretty."
    /// Tiny bubbles and frozen ripples betray a violent birth, cooled too fast to be polite, leaving a sleek black relic that feels like a fragment of a broken window to nowhere.
    /// </summary>
    GlassyTektite,

    /// <summary>
    /// A scarred, dark shard whose surface looks sandblasted and heat-brushed, with shallow pits and sharp edges that catch harsh highlights.
    /// It carries the aesthetic of "been through something," even when no one agrees on what that something was.
    /// In motion it appears calmer than it is, turning slowly like a deliberate threat-an object that seems to have learned endurance the hard way, and kept the look as a souvenir.
    /// </summary>
    IrradiatedShard,

    /// <summary>
    /// A lump that refuses to look consistent: one side seems overly dark, the other oddly bright, as if the shadows cannot agree where to sit.
    /// Its silhouette reads different from different angles, and even careful observers find themselves second-guessing distance and size.
    /// It is the sort of thing crews nickname out of self-defense-The Joke, The Knot, The Bad Angle-because giving it a silly name feels safer than admitting it makes your eyes uncomfortable.
    /// </summary>
    AnomalousMass,

    /// <summary>
    /// Not a single body but a glittering haze of countless tiny flecks, like a strip of starlight someone spilled and forgot to clean up.
    /// From a distance it is beautiful-soft shimmer, gentle sparkle-up close it becomes an abrasive fog with the personality of a belt sander.
    /// The swarm's charm is purely aesthetic, which is fitting: in the Flattiverse, "beautiful but untouchable" is practically a design philosophy.
    /// </summary>
    MicroMeteorSwarm,

    /// <summary>
    /// A hollow-looking cinder with a pumice texture, full of tiny voids that make it seem lighter than it has any right to be.
    /// It catches light in a flattering way-soft highlights, gentle shading-like it was designed to look valuable.
    /// Up close it is mostly drama and very little substance, the kind of object that teaches crews to stop judging rocks by their sparkle.
    /// It is still popular, though, because everyone likes something that looks good in screenshots.
    /// </summary>
    HollowCinder,

    /// <summary>
    /// A ribbon of old slag and fused grit, stretched into a lopsided "comma" by ancient impacts and slow collisions.
    /// It has streaks and layers like geological sediment, except the colors do not quite make sense together.
    /// Some swear it is the remains of forgotten industry; others call it perfectly normal space trash with aspirations.
    /// Either way, it drifts like an accidental sculpture-proof that the belt produces art even when nobody asked for it.
    /// </summary>
    FusedSlagRibbon,

    /// <summary>
    /// A long, needle-like shard that looks less like a broken boulder and more like a snapped spear tip, thin enough to feel wrong.
    /// It rotates slowly, throwing tight flashes of light that make it seem to wink at passing ships.
    /// Nobody agrees on how such a shape survives intact for long, which is why it is always the subject of confident explanations and immediate skepticism.
    /// It is a belt curiosity: elegant, suspicious, and somehow always pointing at something important-until it is not.
    /// </summary>
    NeedleShard,

    /// <summary>
    /// A chalk-white flake with layered edges, like a piece of porcelain chipped off an antique plate and tossed into orbit.
    /// It looks delicate, even refined, and then you notice the jagged fractures and the faint dust cloud trailing behind it.
    /// It has the vibe of something that should be indoors, on a shelf, not tumbling through vacuum with a quiet sense of offended dignity.
    /// Crews like to photograph these because they look "too clean," which makes everyone else immediately distrust them.
    /// </summary>
    PorcelainFlake
}
