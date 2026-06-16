# TrainFinder application infrastructure

locals {
  key_vault_secrets = {
    "AzureAd--CallbackPath"                 = var.secret_azuread_callback_path
    "AzureAd--ClientId"                     = var.secret_azuread_client_id
    "AzureAd--ClientSecret"                 = var.secret_azuread_client_secret
    "AzureAd--Instance"                     = var.secret_azuread_instance
    "AzureAd--TenantId"                     = var.secret_azuread_tenant_id
    "ConnectionStrings--TrainFinderDatabase" = var.secret_connection_string
    "Storage--ConnectionString"             = var.secret_storage_connection_string
    "Smtp--Username"                         = var.secret_smtp_username
    "Smtp--Password"                         = var.secret_smtp_password
  }
}

resource "azurerm_resource_group" "main" {
  name     = var.rg_name
  location = var.location
  tags     = var.tags
}

module "sql_database" {
  source             = "./modules/sql_database"
  rg_name            = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  server_name        = var.sql.server_name
  db_name            = var.sql.db_name
  sku_name           = var.sql.sku_name
  admin_login        = var.sql.admin_login
  max_size_gb        = var.sql.max_size_gb
  geo_backup_enabled = var.sql.geo_backup_enabled
  tags               = var.tags
}

module "key_vault" {
  source         = "./modules/key_vault"
  rg_name        = azurerm_resource_group.main.name
  location       = azurerm_resource_group.main.location
  key_vault_name = var.key_vault_name
  tags           = var.tags

  secrets              = local.key_vault_secrets
  web_app_principal_id = module.app_service.principal_id
}

module "storage_account" {
  source               = "./modules/storage_account"
  rg_name              = azurerm_resource_group.main.name
  location             = azurerm_resource_group.main.location
  storage_account_name = var.storage_account_name
  tags                 = var.tags
}

module "app_service" {
  source        = "./modules/app_service"
  rg_name       = azurerm_resource_group.main.name
  location      = azurerm_resource_group.main.location
  plan_name     = var.app_service.plan_name
  app_name      = var.app_service.app_name
  sku_name      = var.app_service.sku_name
  key_vault_uri = module.key_vault.vault_uri
  tags          = var.tags
}
