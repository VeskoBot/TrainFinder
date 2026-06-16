# TrainFinder Pipelines

## network-deploy.yaml

Main deployment pipeline. Runs on the `trainfinder-release` agent pool.

**Jobs:**
1. **Security Check** - Terrascan IaC scanning
2. **Plan/Deploy Network** - `network/` stack (VNet, subnets, NSGs, DNS zones)
3. **Plan/Deploy Management** - `management/` stack (storage account, Log Analytics)

**Mode parameter:** `plan` (dry-run) or `apply` (deploy).

**Required variable group:** `landing-zone`

## security.yaml

Standalone security scanning pipeline using Microsoft Security DevOps (Terrascan).
