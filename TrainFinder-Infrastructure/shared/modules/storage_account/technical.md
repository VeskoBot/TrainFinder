# Technical Document

## Module Dependencies

None.

## Variables Declared

| Name | Description | Type | Default |
|------|-------------|------|---------|
| `rg_name` | Resource group name | `string` | |
| `location` | Azure region | `string` | `norwayeast` |
| `storage_account_name` | Storage account name | `string` | |
| `storage_account_sku` | Account tier | `string` | |
| `storage_account_replication_type` | Replication type | `string` | |
| `public_network_access_enabled` | Public network access | `bool` | |
| `tags` | Resource tags | `map(string)` | |

## Outputs Declared

| Name | Description |
|------|-------------|
| `storage_account_resource` | Full storage account resource object |

## Resources Invoked

- `azurerm_storage_account`

## Called By

- `management/main.tf` (TrainFinder Management Stack)
