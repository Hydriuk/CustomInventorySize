<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 28cm 58cm; }
        body { margin: 0.2cm; }
    }
</style>

# **CustomInventorySize** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin updates the inventory size of the players with permission based configuration.

1. [**Configuration Examples**](#configuration-examples)
   1. [OpenMod](#openmod)
   2. [RocketMod](#rocketmod)
2. [**Translation Examples**](#translation-examples)
   1. [OpenMod](#openmod-1)
   2. [RocketMod](#rocketmod-1)
3. [**Permissions**](#permissions)
   1. [Clothes](#clothes)
   2. [Storages](#storages)
   3. [Pages](#pages)
4. [**Notes**](#notes)


## **Configuration Examples**
### **OpenMod**
```yaml
# true: plugin is enabled
# false: plugin is disabled
Enabled: true
```

### **RocketMod**
```xml
<Configuration 
  xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
>
  <!-- true : The plugin is enabled -->
  <!-- false : The plugin is disabled -->
  <Enabled>true</Enabled>
</Configuration>
```

## **Translation Examples**
### **OpenMod**
```yaml
StorageItemDropped: Items from the storage were dropped because it was resized
InventoryItemDropped: Items from your inventory were dropped because it was resized
```
### **RocketMod**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Translations 
  xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
>
  <Translation Id="StorageItemDropped" Value="Items from the storage were dropped because it was resized" />
  <Translation Id="InventoryItemDropped" Value="Items from your inventory were dropped because it was resized" />
</Translations>
```


## **Permissions**

The inventory size of players is managed through permissions.  
RocketMod **group's** and OpenMod roles' **priorities** are used to **decide which permission should be used** when two permissions in two different groups set size of the same item / page.  
**Clothes** and **storages** permissions have **priority over** pages **permissions**.

### **Clothes**
The permission template to change a cloth inventory size is : **`inventorysize.item.<id>.<width>.<height>`**

In this template, replace :
- `<id>` with the id of the cloth
- `<width>` with the inventory width of the cloth
- `<heigth>` with the inventory height of the cloth

### **Storages**
The permission template to change a storage item inventory size is : **`inventorysize.item.<id>.<width>.<height>`**

In this template, replace :
- `<id>` with the id of the storage item
- `<width>` with the inventory width of the storage item
- `<heigth>` with the inventory height of the storage item

### **Pages**
The permission template to change a storage item inventory size is : **`inventorysize.page.<pageIndex>.<width>.<height>`**

In this template, replace :
- `<pageIndex>` with the index of the page. List of indexes below
- `<width>` with the inventory width of the page
- `<heigth>` with the inventory height of the page

> Pages : 
> - `2` : Player's hands
> - `3` : Player's backpack
> - `4` : Player's vest
> - `5` : Player's shirt
> - `6` : Player's pants

## **Notes**

Reloading is supported. You can enable/disable the plugin while the server is running. The inventories will be updated when the plugin loads.  
When reloading, storages will be updated, and items exceeding the new storages size will be dropped form the storage.

<font color="orange">Note that you cannot have more than 200 items in a storage slot. For example, you cannot have more than 200 items in your backpack</font>