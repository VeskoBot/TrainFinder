# Create a Log Analytics workspace
resource "azurerm_log_analytics_workspace" "main" {
  location            = var.location
  resource_group_name = var.rg_name
  sku                 = var.sku
  name                = var.log_analytics_workspace_name
  retention_in_days   = var.log_analytics_retention_in_days
  tags                = var.tags
}