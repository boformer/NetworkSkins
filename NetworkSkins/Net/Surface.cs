namespace NetworkSkins.Skins.Modifiers
{
    public enum Surface
    {
        Unchanged = -1, // TODO remove? unused?
        /// <summary>
        /// None only works when the network does not clip terrain. The option should be hidden for network which do clip terrain
        /// </summary>
        None = 0,
        Pavement = 1,
        Gravel = 2,
        Ruined = 3,
        Count = 4
    }
}
