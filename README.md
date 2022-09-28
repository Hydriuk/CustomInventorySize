# **CustomInventorySize** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin updates the inventory size of the players with a rocket group based configuration.

## Configuration

```xml
<Configuration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- true : The plugin is enabled -->
  <!-- false : The plugin is disabled -->
  <Enabled>true</Enabled>

  <!-- List of groups and the size of the inventory for the players belonging to the group -->
  <Groups>

    <!-- Name of the rocket group -->
    <Group GroupName="default"> <!-- Set size for the rocket default group -->

      <!-- List of sizes for given items -->
      <Items>
        <ItemStorage Width="20" Height="40" ItemId="253" /> <!-- Change the alicepack size to be 20x40 for the default group -->
      </Items>

      <!-- List of sizes for given pages -->
      <Pages>
        <Page Width="1" Height="1" Index="2" /> <!-- Change the hands size to be 1x1 for the default group-->
      </Pages>

    </Group>

  </Groups>
</Configuration>

```
`Group` : This plugin uses rocket groups. You can set different items/pages sizes for different groups. The plugin uses the priority of the rocket groups to prioritize the items/pages sizes

`Items` : You can set a size specific to an item. The size of the players' inventory will change when they'll equip this item. Items have priority before pages.

`Pages` : You can set a size for a page. It'll be the default page size for the players belonging to the group.

*Page indexes* : 
- `0`: Primary weapon
- `1`: Secondary weapon
- `2`: Player's hands
- `3`: Player's backpack
- `4`: Player's vest
- `5`: Player's shirt
- `6`: Player's pants
- `7`: <font color="darkred">[Do not use] Opened storage</font>
- `8`: <font color="darkred">[Do not use] Nearby items</font>

## Events

- `Player connected` : Check the player's group and update all clothes size accordingly to the configuration

- `Clothing equipped` : Check the player's group and update the equipped clothes size accordingly to the configuration

- `Player revived` : Check the player's group and update all clothes size accordingly to the configuration

## Notes

Reloading is supported. You can enable/disable the plugin while the server is running. The inventories will be updated when the plugin loads.

<font color="yellow">Note that you cannot have more than 200 items in a storage slot. For example, you cannot have more than 200 items in your backpack</font>