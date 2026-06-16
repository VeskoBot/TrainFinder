# Log Analytics Workspace Module

Creates an Azure Log Analytics workspace for centralized diagnostics and monitoring.

## Usage

```hcl
module "log_analytics_workspace" {
  source                          = "../modules/log_analytics_workspace"
  rg_name                         = "rg-management-001"
  sku                             = "PerGB2018"
  log_analytics_workspace_name    = "la-trainfinder-mgmt-001"
  log_analytics_retention_in_days = 30
  tags                            = { environment = "management" }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| `rg_name` | Resource group name | `string` | | yes |
| `location` | Azure region | `string` | `norwayeast` | no |
| `sku` | Pricing tier (PerGB2018, etc.) | `string` | | yes |
| `log_analytics_workspace_name` | Workspace name | `string` | | yes |
| `log_analytics_retention_in_days` | Data retention in days | `number` | | yes |
| `tags` | Resource tags | `map(string)` | | yes |

## Outputs

| Name | Description |
|------|-------------|
| `workspace_resource` | The Log Analytics workspace resource |

## Resources Created

- `azurerm_log_analytics_workspace`
