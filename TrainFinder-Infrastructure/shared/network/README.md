# TrainFinder Network Stack

Deploys the nonprod connectivity resources:

- Single flat VNet (`vnet-nonprod`) in `rg-connectivity-noprod-001`
- Subnets: `snet-app` (App Service delegation), `snet-pe` (private endpoints), `snet-mgmt`
- NSGs per subnet
- Private DNS zones for blob, SQL, and Key Vault private link

## Usage

Deployed via `pipelines/network-deploy.yaml`. Backend state key: `tfstate-file-net`.
