# **CustomInventorySize** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin updates the inventory size of the players with a rocket group based configuration.

## Configuration
### RocketMod
```xml
<Configuration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- true : The plugin is enabled -->
  <!-- false : The plugin is disabled -->
  <Enabled>true</Enabled>

  <!-- List of inventory size by group -->
  <Groups>

    <!-- Name of the rocket group -->
    <Group GroupName="default">

      <!-- List of clothes and their new inventory size -->
      <Items>
        <ItemStorage Width="10" Height="20" Id="253" />
      </Items>

      <!-- List of pages and their new inventory size -->
      <Pages>
        <Page Width="5" Height="5" Page="2" />
      </Pages>

    </Group>

  </Groups>
</Configuration>
```

### OpenMod
```yaml
# true: plugin is enabled
# false: plugin is disabled
Enabled: true

# List of sizes by group
Groups:

  # Name of the openmod role
- GroupName: default

  # List of clothes and their new inventory size
  Items:
  - Id: 253
    Width: 10
    Height: 20

  # List of pages and their new inventory size
  # Available pages :
  # 2 : Hands
  # 3 : Backpack
  # 4 : Vest
  # 5 : Shirt
  # 6 : Pants
  # 7 : Storage
  Pages:
  - Page: 2
    Width: 5
    Height: 5
```

`Groups` : This plugin uses rocketmod groups / openmod roles. You can set different items/pages sizes for different groups. The plugin uses the priority of the groups to prioritize the items/pages sizes

`Items` : You can set a size specific to **clothes** that have an inventory. The size of the players' inventory will change when they'll equip this item. Items have priority before pages. It also works for storage items like **lockers** or **sentries**.

`Pages` : You can set a size for a page. It'll be the default page size for the players belonging to the group.

> *Pages* : 
> - `0` : <font color="ff150c">[Do not use] Primary weapon</font>
> - `1` : <font color="ff150c">[Do not use] Secondary weapon</font>
> - `2` : Player's hands
> - `3` : Player's backpack
> - `4` : Player's vest
> - `5` : Player's shirt
> - `6` : Player's pants
> - `7` : Opened storage
> - `8` : <font color="ff150c">[Do not use] Nearby items</font>

## Events

- `Player connected` : Check the player's group and update all clothes size accordingly to the configuration

- `Clothing equipped` : Check the player's group and update the equipped clothes size accordingly to the configuration

- `Player revived` : Check the player's group and update all clothes size accordingly to the configuration

- `Opened storage` : Check the player's group and update storage size accordingly to the configuration

## Notes

Reloading is supported. You can enable/disable the plugin while the server is running. The inventories will be updated when the plugin loads.

<font color="orange">Note that you cannot have more than 200 items in a storage slot. For example, you cannot have more than 200 items in your backpack</font>

### Attributions

Icon :
- [School bag icons created by Vitaly Gorbachev - Flaticon](https://www.flaticon.com/free-icons/school-bag)
- [Scale icons created by Freepik - Flaticon](https://www.flaticon.com/free-icons/scale)
- [Icon generator](https://romannurik.github.io/AndroidAssetStudio/icons-launcher.html)