# TrainFinder Management Stack

Deploys shared management resources into `rg-management-001`.

## Resources

| Resource | Name | Description |
|----------|------|-------------|
| Storage Account | `sattrainfinder001cshell` | Shared storage for PowerShell scripts / Cloud Shell |
| Log Analytics | `la-trainfinder-mgmt-001` | Centralized diagnostics and monitoring workspace |

## Modules Used

| Module | Source |
|--------|--------|
| `storage_account` | `../modules/storage_account` |
| `log_analytics_workspace` | `../modules/log_analytics_workspace` |

## Inputs

| Name | Description | Type | Default |
|------|-------------|------|---------|
| `storage_accounts` | Map of storage accounts to create | `map(object)` | n/a |
| `rg_name` | Resource group name | `string` | `rg-management-001` |
| `tags` | Tags for resources | `map(string)` | `{ environment = "management" }` |
| `la_workspace` | Log Analytics config (name, sku, retention) | `object` | n/a |

## Deployment

Deployed via `pipelines/network-deploy.yaml` (management job). Backend state key: `tfstate-file-mgmt`.
