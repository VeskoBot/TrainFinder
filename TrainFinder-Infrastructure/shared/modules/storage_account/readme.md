# Storage Account Module

Creates an Azure Storage Account.

## Usage

```hcl
module "storage_account" {
  source                           = "../modules/storage_account"
  rg_name                          = "rg-management-001"
  storage_account_name             = "sattrainfinder001cshell"
  storage_account_sku              = "Standard"
  storage_account_replication_type = "LRS"
  public_network_access_enabled    = true
  tags                             = { environment = "management" }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| `rg_name` | Resource group name | `string` | | yes |
| `location` | Azure region | `string` | `norwayeast` | no |
| `storage_account_name` | Storage account name | `string` | | yes |
| `storage_account_sku` | Account tier (Standard/Premium) | `string` | | yes |
| `storage_account_replication_type` | Replication type (LRS, GRS, etc.) | `string` | | yes |
| `public_network_access_enabled` | Allow public network access | `bool` | | yes |
| `tags` | Resource tags | `map(string)` | | yes |

## Outputs

| Name | Description |
|------|-------------|
| `storage_account_resource` | The full Storage Account resource object |

## Resources Created

- `azurerm_storage_account`

## Cloud Provider Reference

[Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview)
