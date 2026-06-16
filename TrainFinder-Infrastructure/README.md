# TrainFinder Infrastructure

Terraform infrastructure-as-code for the TrainFinder project deployed on Microsoft Azure.

## Repository Structure

```
├── shared/                  # Shared infrastructure (networking, management)
│   ├── network/             # VNet, subnets, NSGs, private DNS zones
│   ├── management/          # Shared storage account, Log Analytics workspace
│   ├── modules/             # Reusable Terraform modules for shared resources
│   └── pipelines/           # Azure DevOps pipelines for shared infra
│
├── infra/                   # Per-environment application infrastructure
│   ├── modules/             # Terraform modules for app resources
│   │   ├── app_service/     # Linux App Service Plan + Web App
│   │   ├── key_vault/       # Azure Key Vault (RBAC-enabled)
│   │   ├── sql_database/    # Azure SQL Server + Database
│   │   └── storage_account/ # Azure Storage Account
│   ├── env/                 # Environment-specific variable files
│   │   ├── dev.tfvars       # Development configuration
│   │   └── qa.tfvars        # QA configuration
│   └── pipelines/           # Azure DevOps pipelines for app infra
```

## Environments

| Environment | Purpose | Resource Group |
|-------------|---------|----------------|
| **dev** | Development | `rg-trainfinder-dev-001` |
| **qa** | Quality Assurance | `rg-trainfinder-qa-001` |

## Stacks

| Stack | Purpose | Resources |
|-------|---------|-----------|
| **shared/network** | Connectivity | VNet, subnets (app, PE, mgmt), NSGs, private DNS zones |
| **shared/management** | Shared services | Storage account (shell scripts), Log Analytics workspace |
| **infra** | Application (per-env) | SQL Server + DB, Key Vault, Storage Account, App Service |

## Deployment Order

1. `shared/network` - VNet and networking
2. `shared/management` - Shared management resources
3. `infra` - Application resources (select dev or qa)

## Configuration

- **Region:** Norway East (`norwayeast`)
- **Provider:** azurerm >= 4.0
- **State backend:** Azure Storage (Entra ID auth)
- **CI/CD:** Azure DevOps with TerraformTaskV4

## Prerequisites

1. Azure subscription with registered resource providers:
   - `Microsoft.Network`, `Microsoft.Storage`, `Microsoft.OperationalInsights`
   - `Microsoft.Sql`, `Microsoft.Web`, `Microsoft.KeyVault`
2. Azure DevOps variable groups:
   - **`landing-zone`** (shared/platform):
     - `service-connection` - ARM service connection name
     - `tfstate-rg` - Resource group for state storage
     - `tfstate-sa` - State storage account name
     - `tfstate-container` - Blob container name
     - `tfstate-file-net` - State file key for network stack
     - `tfstate-file-mgmt` - State file key for management stack
     - `tfstack-net` - Working directory for network stack
     - `tfstack-mgmt` - Working directory for management stack
   - **`trainfinder-infra`** (application/workload):
     - `tfstate-file-infra` - State file key prefix for infra stack
     - `tfstack-infra` - Working directory for infra stack
3. Service principal with:
   - **Contributor** on the subscription
   - **Storage Blob Data Contributor** on the state storage account
# TrainFinder Infrastructure

Terraform IaC for TrainFinder shared infrastructure on Azure. Manages the connectivity and management resource groups that underpin all application environments.

## Architecture

```
rg-management-001
├── Storage Account (sattrainfinder001cshell) — PowerShell / Cloud Shell
└── Log Analytics Workspace (la-trainfinder-mgmt-001) — Diagnostics

rg-connectivity-noprod-001
├── VNet: vnet-nonprod-dev-001 (10.1.0.0/22)
│   ├── snet-app   (10.1.0.0/26)   — App Service VNet integration (delegation)
│   ├── snet-pe    (10.1.0.64/26)  — Private endpoints (SQL, Storage, KV)
│   └── snet-mgmt  (10.1.0.128/27) — Management / ops
├── NSGs per subnet
└── Private DNS Zones (blob, SQL, Key Vault)
```

## Stacks

| Stack | Directory | State Key | Purpose |
|-------|-----------|-----------|---------|
| Network | `network/` | `tfstate-file-net` | VNet, subnets, NSGs, DNS zones |
| Management | `management/` | `tfstate-file-mgmt` | Shared storage account, Log Analytics |

## Modules

| Module | Used By | Description |
|--------|---------|-------------|
| `modules/network` | Network stack | VNet with inline subnets, NSGs, and NSG rules |
| `modules/private_dns_zone` | Network stack | Private DNS zones with VNet links and record support |
| `modules/storage_account` | Management stack | Azure Storage Account |
| `modules/log_analytics_workspace` | Management stack | Log Analytics workspace |

## Deployment

Deployed via Azure DevOps pipeline: `pipelines/network-deploy.yaml`

The pipeline runs security scanning (Terrascan), then deploys Network and Management stacks in sequence.

## Prerequisites

- Azure DevOps variable group `landing-zone` with:
  - `service-connection` — Azure service connection name
  - `tfstate-rg`, `tfstate-sa`, `tfstate-container` — Terraform state backend
  - `tfstate-file-net`, `tfstate-file-mgmt` — State file keys
  - `tfstack-net`, `tfstack-mgmt` — Working directories