#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface IInventoryModifier
    {
        /// <summary>
        /// Change the size of all player inventory pages to the one configured in their most prioritized group
        /// </summary>
        /// <param name="player"> Player of whom to change the inventory size </param>
        void ModifyInventory(Player player);

        /// <summary>
        /// Change the size of a page
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="pageIndex"> Page to change the size of </param>
        /// <param name="width"> New width of the page </param>
        /// <param name="height"> New height of the page </param>
        /// <returns> A byte representing the index of the page that has been changed </returns>
        void ModifyPage(Player player, byte pageIndex, byte width, byte height);
        void ModifyPage(Player player, byte page);

        /// <summary>
        /// Reset all player inventory pages to their original game size
        /// </summary>
        /// <param name="player"> Player of whom to reset the inventory </param>
        void ResetInventorySize(Player player);
    }
}