resource "azurerm_service_plan" "main" {
  name                = var.plan_name
  location            = var.location
  resource_group_name = var.rg_name
  os_type             = "Windows"
  sku_name            = var.sku_name
  tags                = var.tags
}

resource "azurerm_windows_web_app" "main" {
  name                = var.app_name
  location            = var.location
  resource_group_name = var.rg_name
  service_plan_id     = azurerm_service_plan.main.id
  tags                = var.tags

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on = var.sku_name == "F1" ? false : true

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v8.0"
    }
  }

  app_settings = {
    "AzureAd__CallbackPath"                 = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/AzureAd--CallbackPath/)"
    "AzureAd__ClientId"                     = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/AzureAd--ClientId/)"
    "AzureAd__ClientSecret"                 = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/AzureAd--ClientSecret/)"
    "AzureAd__Instance"                     = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/AzureAd--Instance/)"
    "AzureAd__TenantId"                     = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/AzureAd--TenantId/)"
    "ConnectionStrings__TrainFinderDatabase" = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/ConnectionStrings--TrainFinderDatabase/)"
    "WEBJOBS_STOPPED"                        = "0"
    "AzureWebJobsStorage"                    = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/Storage--ConnectionString/)"
    "AzureWebJobsDashboard"                  = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/Storage--ConnectionString/)"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"    = "true"
    "Smtp__Host"                             = "smtp.gmail.com"
    "Smtp__Port"                             = "587"
    "Smtp__From"                             = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/Smtp--Username/)"
    "Smtp__EnableSsl"                        = "true"
    "Smtp__Username"                         = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/Smtp--Username/)"
    "Smtp__Password"                         = "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/Smtp--Password/)"
  }
}
