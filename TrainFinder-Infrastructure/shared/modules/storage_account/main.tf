#################### Create storage account  ###################
resource "azurerm_storage_account" "storage_account" {
  resource_group_name           = var.rg_name
  location                      = var.location
  name                          = var.storage_account_name
  account_tier                  = var.storage_account_sku
  account_replication_type      = var.storage_account_replication_type
  tags                          = var.tags
  public_network_access_enabled = var.public_network_access_enabled
}