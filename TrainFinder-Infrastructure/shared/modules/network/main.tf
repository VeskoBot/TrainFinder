locals {
  vnet_name = var.environment == "shared" ? var.vnet_name : "${var.vnet_name}-${var.environment}-001"

  # Flatten NSG rules so we can create them with for_each
  nsg_rules = flatten([
    for subnet_key, subnet in var.subnets : [
      for rule in subnet.list_security_group : {
        subnet_key                   = subnet_key
        name                         = rule.name
        priority                     = rule.priority
        direction                    = rule.direction
        access                       = rule.access
        protocol                     = rule.protocol
        source_port_ranges           = rule.source_port_ranges
        destination_port_ranges      = rule.destination_port_ranges
        source_address_prefixes      = rule.source_address_prefixes
        destination_address_prefixes = rule.destination_address_prefixes
      }
    ]
  ])
}

resource "azurerm_virtual_network" "vnet" {
  name                = local.vnet_name
  address_space       = var.address_space
  location            = var.location
  resource_group_name = var.rg_name
  tags                = var.tags
}

resource "azurerm_subnet" "subnet" {
  for_each             = var.subnets
  name                 = each.value.name
  resource_group_name  = var.rg_name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = each.value.address_prefixes
  service_endpoints    = each.value.service_endpoints

  dynamic "delegation" {
    for_each = each.value.delegation ? [1] : []
    content {
      name = "webapp-delegation"
      service_delegation {
        name    = "Microsoft.Web/serverFarms"
        actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
      }
    }
  }
}

resource "azurerm_network_security_group" "nsg" {
  for_each            = var.subnets
  name                = "nsg-${each.value.name}-${var.environment}-001"
  location            = var.location
  resource_group_name = var.rg_name
  tags                = var.tags
}

resource "azurerm_network_security_rule" "rule" {
  for_each = {
    for rule in local.nsg_rules : "${rule.subnet_key}-${rule.name}" => rule
  }

  name      = each.value.name
  priority  = each.value.priority
  direction = each.value.direction
  access    = each.value.access
  protocol  = each.value.protocol

  source_port_range  = length(each.value.source_port_ranges) == 1 ? each.value.source_port_ranges[0] : null
  source_port_ranges = length(each.value.source_port_ranges) > 1 ? each.value.source_port_ranges : null

  destination_port_range  = length(each.value.destination_port_ranges) == 1 ? each.value.destination_port_ranges[0] : null
  destination_port_ranges = length(each.value.destination_port_ranges) > 1 ? each.value.destination_port_ranges : null

  source_address_prefix    = length(each.value.source_address_prefixes) == 1 ? each.value.source_address_prefixes[0] : null
  source_address_prefixes  = length(each.value.source_address_prefixes) > 1 ? each.value.source_address_prefixes : null

  destination_address_prefix    = length(each.value.destination_address_prefixes) == 1 ? each.value.destination_address_prefixes[0] : null
  destination_address_prefixes  = length(each.value.destination_address_prefixes) > 1 ? each.value.destination_address_prefixes : null

  resource_group_name         = var.rg_name
  network_security_group_name = azurerm_network_security_group.nsg[each.value.subnet_key].name
}

resource "azurerm_subnet_network_security_group_association" "nsg_assoc" {
  for_each                  = var.subnets
  subnet_id                 = azurerm_subnet.subnet[each.key].id
  network_security_group_id = azurerm_network_security_group.nsg[each.key].id
}
