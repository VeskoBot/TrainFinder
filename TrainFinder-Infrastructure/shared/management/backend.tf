terraform {
  backend "azurerm" {
    storage_account_name = "overwrite by azure DevOps"
    container_name       = "overwrite by azure DevOps"
    key                  = "overwrite by azure DevOps"
  }
}