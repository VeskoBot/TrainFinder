# Terraform to deploy management resources (shared storage + log analytics)
provider "azurerm" {
  resource_provider_registrations = "none"
  features {}
}

module "storage_account" {
  source                           = "../modules/storage_account"
  for_each                         = var.storage_accounts
  rg_name                          = var.rg_name
  storage_account_name             = each.value.name
  storage_account_sku              = each.value.sku
  storage_account_replication_type = each.value.replication_type
  public_network_access_enabled    = each.value.public_network_access_enabled
  tags                             = var.tags
}

module "log_analytics_workspace" {
  source                          = "../modules/log_analytics_workspace"
  rg_name                         = var.rg_name
  sku                             = var.la_workspace.sku
  log_analytics_workspace_name    = var.la_workspace.name
  log_analytics_retention_in_days = var.la_workspace.retention_in_days
  tags                            = var.tags
}


