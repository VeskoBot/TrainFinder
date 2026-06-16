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