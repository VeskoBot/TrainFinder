# Network Module

Creates and manages a virtual network with inline subnets, NSGs, and NSG rules.

## Usage

This module deploys:
- A VNet with environment-based naming (`{vnet_name}-{environment}-001`)
- Subnets with optional App Service delegation (`Microsoft.Web/serverFarms`)
- One NSG per subnet with configurable security rules
- NSG-to-subnet associations

## Example

```hcl
module "vnet-nonprod" {
  source        = "../modules/network"
  vnet_name     = "vnet-nonprod"
  address_space = ["10.1.0.0/22"]
  environment   = "dev"
  location      = "norwayeast"
  rg_name       = "rg-connectivity-noprod-001"
  tags          = { environment = "dev" }

  subnets = {
    app = {
      name              = "snet-app"
      address_prefixes  = ["10.1.0.0/26"]
      service_endpoints = []
      delegation        = true
      list_security_group = [{
        name                         = "Allow_HTTPS_Inbound"
        priority                     = 100
        direction                    = "Inbound"
        access                       = "Allow"
        protocol                     = "Tcp"
        source_port_ranges           = ["*"]
        destination_port_ranges      = ["443"]
        source_address_prefixes      = ["*"]
        destination_address_prefixes = ["10.1.0.0/26"]
      }]
    }
  }
}
```

## Inputs

| Name | Description | Type | Required |
|------|-------------|------|:--------:|
| `rg_name` | Resource group name | `string` | yes |
| `environment` | Environment name (dev, qua, etc.) | `string` | yes |
| `location` | Azure region | `string` | yes |
| `vnet_name` | VNet base name | `string` | yes |
| `address_space` | VNet address space | `list(string)` | yes |
| `subnets` | Map of subnet configs (name, prefixes, endpoints, delegation, NSG rules) | `map(object)` | yes |
| `tags` | Resource tags | `map` | yes |

## Outputs

| Name | Description |
|------|-------------|
| `subnet_ids` | Map of subnet keys to their IDs |
| `vnet_id` | VNet resource ID |
| `vnet_name` | VNet name (with environment suffix) |
| `environment` | Environment name |

## Resources Created

- `azurerm_virtual_network`
- `azurerm_subnet` (per subnet, with optional delegation)
- `azurerm_network_security_group` (per subnet)
- `azurerm_network_security_rule` (per rule in each subnet)
- `azurerm_subnet_network_security_group_association` (per subnet)
