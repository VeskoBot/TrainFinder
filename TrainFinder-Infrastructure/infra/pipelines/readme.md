# TrainFinder Pipelines

Azure DevOps pipeline definitions for deploying TrainFinder application infrastructure.

## Pipelines

| Pipeline | File | Purpose |
|----------|------|---------|
| Infra Deploy | `infra-deploy.yaml` | Plan/apply Terraform for dev or qa environment |

## Usage

The pipeline uses the `landing-zone` variable group and accepts two parameters:

- **mode** - `plan` or `apply`
- **environment** - `dev` or `qa`

Each environment uses its own tfvars file (`env/dev.tfvars`, `env/qa.tfvars`) and state key.
