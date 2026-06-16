# Technical Document

## Module Dependencies

None. All resources are created inline (no sub-module calls).

## Variables Declared

| Name | Description | Type | Default |
|------|-------------|------|---------|
| `rg_name` | Resource group name | `string` | |
| `environment` | Environment (dev, qua, etc.) | `string` | |
| `location` | Azure region | `string` | |
| `vnet_name` | VNet base name | `string` | |
| `address_space` | VNet CIDR blocks | `list(string)` | |
| `subnets` | Subnet configuration map | `map(object)` | |
| `tags` | Resource tags | `map` | |
| `private_endpoint_policy` | Private endpoint network policy | `string` | `Disabled` |

## Outputs Declared

| Name | Description |
|------|-------------|
| `subnet_ids` | Map of subnet keys to IDs |
| `vnet_id` | VNet resource ID |
| `vnet_name` | VNet name |
| `environment` | Environment value |

## Resources Invoked

- `azurerm_virtual_network`
- `azurerm_subnet`
- `azurerm_network_security_group`
- `azurerm_network_security_rule`
- `azurerm_subnet_network_security_group_association`

## Called By

- `network/main.tf` (TrainFinder Network Stack)
